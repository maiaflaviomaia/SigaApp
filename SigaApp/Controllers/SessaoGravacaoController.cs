using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using SigaApp.Servicos;

namespace SigaApp.Controllers
{
    [Authorize]
    public class SessaoGravacaoController : Controller
    {
        private readonly ISessaoGravacao _sessao;
        private readonly IEstudio _estudio;
        private readonly ICliente _cliente;
        private readonly IFornecedor _fornecedor;
        private readonly IServicoPrestado _servico;
                
        public SessaoGravacaoController(ISessaoGravacao sessao, IEstudio estudio, ICliente cliente, IFornecedor fornecedor, IServicoPrestado servico)
        {
            _sessao = sessao;
            _estudio = estudio;
            _cliente = cliente;
            _fornecedor = fornecedor;
            _servico = servico;
        }

        [TempData]
        public string Mensagem { get; set; }

        
        [HttpGet]
        public ActionResult Index(int txtEstudio, int? pagina)
        {
            try
            {
                CarregarEstudios();
                var gravacao = _sessao.ObterTodos().Where(x => x.EstudioID == txtEstudio);

                ViewData["FiltroAtual"] = txtEstudio;

                int pageSize = 10;
                return View(Paginacao<SessaoGravacao>.Create(gravacao, pagina ?? 1, pageSize));
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var sessao = _sessao.ObterPorId(id);
            if(sessao == null)
                return NotFound();

            return View(sessao);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new SessaoGravacao();
            model.DataInicio = DateTime.Now;
            model.DataFim = DateTime.Now;

            CarregarCampos();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SessaoGravacao sessaoGravacao)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    sessaoGravacao.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);

                    SessaoGravacaoService service = new SessaoGravacaoService();
                    service.PreencherCampos(sessaoGravacao);

                    _sessao.Inserir(sessaoGravacao);
                    return RedirectToAction(nameof(Index));
                }

                return View(sessaoGravacao);

            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return RedirectToAction(nameof(Create));
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            CarregarCampos();
            var sessao = _sessao.ObterPorId(id);
            
            if (sessao == null)
                return NotFound();

            return View(sessao);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, SessaoGravacao sessaoGravacao)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SessaoGravacaoService service = new SessaoGravacaoService();
                    service.ValidarCampos(sessaoGravacao);

                    _sessao.Atualizar(sessaoGravacao);
                    return RedirectToAction(nameof(Index));
                }

                return View(sessaoGravacao);

            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return RedirectToAction(nameof(Edit));
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var sessao = _sessao.ObterPorId(id);

            if (sessao == null)
                return NotFound();

            return View(sessao);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _sessao.Desativar(id);
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public ActionResult GerarRelatorio(int? pagina)
        {
            CarregarClientes();
            CarregarEstudios();
            CarregarServicos();
            var relatorio = _sessao.ObterTodos().Where(x => x.SessaoID == 0);

            int pageSize = 20;
            return View(Paginacao<SessaoGravacao>.Create(relatorio, pagina ?? 1, pageSize));
        }

        [HttpPost]
        public ActionResult GerarRelatorio(int? txtEstudio, int? txtCliente, DateTime txtDataInicio, DateTime txtDataFim, int? txtServico, int? pagina)
        {
            try
            {
                CarregarClientes();
                CarregarEstudios();
                CarregarServicos();

                var dateDiff = txtDataFim.Date - txtDataInicio.Date;

                if (txtDataInicio.Date == DateTime.MinValue || txtDataFim.Date == DateTime.MinValue)
                    throw new ArgumentException("O intervalo de datas é obrigatório");

                if (dateDiff.Days > 90)
                    throw new ArgumentException("Intervalo máximo de 90 dias");

                if (txtDataFim.Date < txtDataInicio.Date)
                    throw new ArgumentException("A Data Fim não pode ser menor que a Data Inicio");

                var relatorio = _sessao.ObterTodos().Where(x => x.SessaoID == 0);

                if (txtEstudio != null && txtCliente != null && txtDataInicio != null && txtDataFim != null && txtServico != null)
                    relatorio = _sessao.ObterTodos().Where(x => x.EstudioID == txtEstudio && x.ClienteID ==  txtCliente && x.DataInicio >= txtDataInicio && x.DataFim <= txtDataFim && x.ServicoPrestadoID == txtServico);

                if (txtEstudio == null && txtCliente == null && txtDataInicio != null && txtDataFim != null && txtServico == null)
                    relatorio = _sessao.ObterTodos().Where(x => x.DataInicio >= txtDataInicio && x.DataFim <= txtDataFim);

                if (txtEstudio != null && txtCliente == null && txtDataInicio != null && txtDataFim != null && txtServico == null)
                    relatorio = _sessao.ObterTodos().Where(x => x.EstudioID == txtEstudio && x.DataInicio >= txtDataInicio && x.DataFim <= txtDataFim);

                if (txtEstudio != null && txtCliente != null && txtDataInicio != null && txtDataFim != null && txtServico == null)
                    relatorio = _sessao.ObterTodos().Where(x => x.EstudioID == txtEstudio && x.ClienteID == txtCliente && x.DataInicio >= txtDataInicio && x.DataFim <= txtDataFim);

                if (txtEstudio == null && txtCliente != null && txtDataInicio != null && txtDataFim != null && txtServico == null)
                    relatorio = _sessao.ObterTodos().Where(x => x.ClienteID == txtCliente && x.DataInicio >= txtDataInicio && x.DataFim <= txtDataFim);

                if (txtEstudio == null && txtCliente == null && txtDataInicio != null && txtDataFim != null && txtServico != null)
                    relatorio = _sessao.ObterTodos().Where(x => x.DataInicio >= txtDataInicio && x.DataFim <= txtDataFim && x.ServicoPrestadoID == txtServico);

                TempData["txtEstudio"] = txtEstudio;
                TempData["txtCliente"] = txtCliente;
                TempData["txtDataInicio"] = txtDataInicio;
                TempData["txtDataFim"] = txtDataFim;
                TempData["txtServico"] = txtServico;

                int pageSize = 200000000;
                return View(Paginacao<SessaoGravacao>.Create(relatorio, pagina ?? 1, pageSize));
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return RedirectToAction(nameof(GerarRelatorio));
            }
        }


        public ActionResult GerarExcel(int? txtEstudio, int? txtCliente, DateTime txtDataInicio, DateTime txtDataFim, int? txtServico, int? pagina)
        {
            try
            {
                if (TempData["txtEstudio"] != null)
                    txtEstudio = (int)TempData["txtEstudio"];

                if (TempData["txtCliente"] != null)
                    txtCliente = (int)TempData["txtCliente"];

                if (TempData["txtDataInicio"] != null)
                    txtDataInicio = (DateTime)TempData["txtDataInicio"];

                if (TempData["txtDataFim"] != null)
                    txtDataFim = (DateTime)TempData["txtDataFim"];

                if (TempData["txtServico"] != null)
                    txtServico = (int)TempData["txtServico"];

                var relatorio = _sessao.ObterTodos().Where(x => x.SessaoID == 0);

                if (txtEstudio != null && txtCliente != null && txtDataInicio != null && txtDataFim != null && txtServico != null)
                    relatorio = _sessao.ObterTodos().Where(x => x.EstudioID == txtEstudio && x.ClienteID == txtCliente && x.DataInicio >= txtDataInicio && x.DataFim <= txtDataFim && x.ServicoPrestadoID == txtServico);

                if (txtEstudio == null && txtCliente == null && txtDataInicio != null && txtDataFim != null && txtServico == null)
                    relatorio = _sessao.ObterTodos().Where(x => x.DataInicio >= txtDataInicio && x.DataFim <= txtDataFim);

                if (txtEstudio != null && txtCliente == null && txtDataInicio != null && txtDataFim != null && txtServico == null)
                    relatorio = _sessao.ObterTodos().Where(x => x.EstudioID == txtEstudio && x.DataInicio >= txtDataInicio && x.DataFim <= txtDataFim);

                if (txtEstudio != null && txtCliente != null && txtDataInicio != null && txtDataFim != null && txtServico == null)
                    relatorio = _sessao.ObterTodos().Where(x => x.EstudioID == txtEstudio && x.ClienteID == txtCliente && x.DataInicio >= txtDataInicio && x.DataFim <= txtDataFim);

                if (txtEstudio == null && txtCliente != null && txtDataInicio != null && txtDataFim != null && txtServico == null)
                    relatorio = _sessao.ObterTodos().Where(x => x.ClienteID == txtCliente && x.DataInicio >= txtDataInicio && x.DataFim <= txtDataFim);

                if (txtEstudio == null && txtCliente == null && txtDataInicio != null && txtDataFim != null && txtServico != null)
                    relatorio = _sessao.ObterTodos().Where(x => x.DataInicio >= txtDataInicio && x.DataFim <= txtDataFim && x.ServicoPrestadoID == txtServico);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Gravações");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "Data de Cadastro";
                    worksheet.Cell(currentRow, 2).Value = "Data Início";
                    worksheet.Cell(currentRow, 3).Value = "Data Fim";
                    worksheet.Cell(currentRow, 4).Value = "Hora Início";
                    worksheet.Cell(currentRow, 5).Value = "Hora Fim";
                    worksheet.Cell(currentRow, 6).Value = "Estúdio";
                    worksheet.Cell(currentRow, 7).Value = "Cliente";
                    worksheet.Cell(currentRow, 8).Value = "Fornecedor";
                    worksheet.Cell(currentRow, 9).Value = "Serviço Prestado";

                    foreach (var rel in relatorio)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = rel.DataCadastro.ToString("dd/MM/yyyy");
                        worksheet.Cell(currentRow, 2).Value = rel.DataInicio.ToString("dd/MM/yyyy");
                        worksheet.Cell(currentRow, 3).Value = rel.DataFim.ToString("dd/MM/yyyy");
                        worksheet.Cell(currentRow, 4).Value = rel.HoraInicio.ToString();
                        worksheet.Cell(currentRow, 5).Value = rel.HoraFim.ToString();
                        worksheet.Cell(currentRow, 6).Value = rel.Estudio.Nome;
                        worksheet.Cell(currentRow, 7).Value = rel.Cliente.RazaoSocial;
                        worksheet.Cell(currentRow, 8).Value = rel.Fornecedor.RazaoSocial;
                        worksheet.Cell(currentRow, 9).Value = rel.ServicoPrestado.Descricao;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Gravacoes.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return RedirectToAction(nameof(GerarRelatorio));
            }
        }

        public IEnumerable<Estudio> CarregarEstudios()
        {
            return ViewBag.ListaEstudios = _estudio.ObterTodos();
        }

        public IEnumerable<Cliente> CarregarClientes()
        {
            return ViewBag.ListaClientes = _cliente.ObterTodos();
        }

        public IEnumerable<Fornecedor> CarregarFornecedores()
        {
            return ViewBag.ListaFornecedores = _fornecedor.ObterTodos();
        }

        public IEnumerable<ServicoPrestado> CarregarServicos()
        {
            return ViewBag.ListaServicos = _servico.ObterTodos();
        }

        private void CarregarCampos()
        {
            CarregarEstudios();
            CarregarClientes();
            CarregarFornecedores();
            CarregarServicos();
        }
    }
}
