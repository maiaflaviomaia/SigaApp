using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using SigaApp.Servicos;
using SigaApp.Utils;
using static SigaApp.Utils.Enums;

namespace SigaApp.Controllers
{
    [Authorize]
    public class ContasReceberController : Controller
    {
        private readonly IContaReceber _contas;
        private readonly ICliente _cliente;
        private readonly ICategoria _categoria;
        private readonly ICentroDeCusto _centroCusto;
        private readonly IContaContabil _contaContabil;
        private readonly ILancamento _lancamento;

        public ContasReceberController(IContaReceber contas, ICliente cliente, ICategoria categoria, ICentroDeCusto centroCusto, IContaContabil contaContabil, ILancamento lancamento)
        {
            _contas = contas;
            _cliente = cliente;
            _categoria = categoria;
            _centroCusto = centroCusto;
            _contaContabil = contaContabil;
            _lancamento = lancamento;
        }

        [TempData]
        public string Mensagem { get; set; }

        [HttpGet]
        public ActionResult Index(string filtroAtual, string filtro, int? pagina)
        {
            if (filtro != null)
            {
                pagina = 1;
            }
            else
            {
                filtro = filtroAtual;
            }

            ViewData["FiltroAtual"] = filtro;

            var conta = from cs in _contas.ObterTodos() select cs;

            if (!String.IsNullOrEmpty(filtro))
            {
                conta = conta.Where(s => EF.Functions.Like(s.Cliente.RazaoSocial, $"%{filtro}%")).ToList();
            }

            int pageSize = 10;
            return View(Paginacao<ContaReceber>.Create(conta, pagina ?? 1, pageSize));
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var conta = _contas.ObterPorId(id);

            if (conta == null)
                return NotFound();

            return View(conta);

        }

        public IActionResult Create()
        {
            var model = new ContaReceber();
            model.DataVencimento = DateTime.Now;
            model.Competencia = DateTime.Now;

            CarregarClientes();
            CarregarCategorias();
            CarregarCentroDeCusto();
            CarregarContaContabil();
            CarregarSubCategorias(0);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ContaReceber contaReceber)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    contaReceber.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);

                    ContaReceberService service = new ContaReceberService();
                    service.PreencherCampos(contaReceber);

                    if (contaReceber.Recorrente == true)
                    {
                        int numParcela = 0;
                        for (int i = 1; i <= 12; i++)
                        {
                            numParcela++;
                            ContaReceber parcela = new ContaReceber();
                            parcela.CategoriaID = contaReceber.CategoriaID;
                            parcela.SubCategoriaID = contaReceber.SubCategoriaID;
                            parcela.CentroDeCustoID = contaReceber.CentroDeCustoID;
                            parcela.ContaContabilID = contaReceber.ContaContabilID;
                            parcela.DataCadastro = contaReceber.DataCadastro;
                            parcela.DataExclusao = contaReceber.DataExclusao;
                            parcela.DataPagamento = contaReceber.DataPagamento;

                            if (numParcela > 1)
                            {
                                parcela.DataVencimento = contaReceber.DataVencimento.AddMonths(numParcela - 1);
                                parcela.Competencia = contaReceber.Competencia.AddMonths(numParcela - 1);
                            }
                            else
                            {
                                parcela.DataVencimento = contaReceber.DataVencimento;
                                parcela.Competencia = contaReceber.Competencia;
                            }

                            parcela.Status = StatusContaReceber.Aberto;
                            parcela.Desconto = contaReceber.Desconto;
                            parcela.Descricao = contaReceber.Descricao;
                            parcela.EmpresaID = contaReceber.EmpresaID;
                            parcela.FlagAtivo = contaReceber.FlagAtivo;
                            parcela.FormaPagamento = contaReceber.FormaPagamento;
                            parcela.ClienteID = contaReceber.ClienteID;
                            parcela.Desconto = contaReceber.Desconto ?? 0;
                            parcela.Juros = contaReceber.Juros ?? 0;
                            parcela.Multa = contaReceber.Multa ?? 0;
                            parcela.Recorrente = contaReceber.Recorrente;
                            parcela.NumeroDocumento = contaReceber.NumeroDocumento + numParcela;
                            parcela.TipoDocumento = contaReceber.TipoDocumento;
                            parcela.Valor = contaReceber.Valor;
                            parcela.ValorRecebido = contaReceber.ValorRecebido;
                            parcela.Observacoes = "Gerado automaticamente através de cadastro de conta recorrente. Parcela: " + numParcela + "/12";

                            _contas.Inserir(parcela);
                        }
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        _contas.Inserir(contaReceber);
                        return RedirectToAction(nameof(Index));
                    }
                }

                CarregarClientes();
                CarregarCategorias();
                CarregarSubCategorias(0);
                CarregarCentroDeCusto();
                return View(contaReceber);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                CarregarClientes();
                CarregarCategorias();
                CarregarSubCategorias(0);
                CarregarCentroDeCusto();
                return View(contaReceber);
            }
        }

        public IActionResult Edit(int id)
        {
            var contaReceber = _contas.ObterPorId(id);

            if (contaReceber == null)
                return NotFound();

            if (contaReceber.Status == StatusContaReceber.Pago)
                return BadRequest();

            CarregarClientes();
            CarregarCategorias();
            CarregarSubCategorias(Convert.ToInt32(contaReceber.CategoriaID));
            CarregarCentroDeCusto();

            return View(contaReceber);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ContaReceber contaReceber)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ContaReceberService service = new ContaReceberService();
                    service.ValidarCampos(contaReceber);

                    _contas.Atualizar(contaReceber);
                    return RedirectToAction(nameof(Index));
                }
                CarregarClientes();
                CarregarCategorias();
                CarregarSubCategorias(Convert.ToInt32(contaReceber.CategoriaID));
                CarregarCentroDeCusto();
                return View(contaReceber);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                CarregarClientes();
                CarregarCategorias();
                CarregarSubCategorias(Convert.ToInt32(contaReceber.CategoriaID));
                CarregarCentroDeCusto();
                return View(contaReceber);
            }
        }

        public ActionResult Delete(int id)
        {
            var conta = _contas.ObterPorId(id);

            if (conta == null)
                return NotFound();

            return View(conta);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _contas.Desativar(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ActionResult Receber(int id)
        {
            var conta = _contas.ObterPorId(id);

            if (conta == null)
                return NotFound();

            conta.DataPagamento = DateTime.Now;
            CarregarContaContabil();
            return View(conta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Receber(int id, ContaReceber contaReceber)
        {
            if(contaReceber.ContaReceberID != id || contaReceber == null)
                return NotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    ContaReceberService service = new ContaReceberService();
                    service.Receber(contaReceber);
                    _contas.Atualizar(contaReceber);

                    var contaRecberAux = _contas.ObterPorId(id);

                    Lancamento lancamento = new Lancamento();
                    service.GerarLancamento(lancamento, contaRecberAux);
                    _lancamento.Inserir(lancamento);

                    return RedirectToAction(nameof(Index));
                }
                CarregarContaContabil();
                return View(contaReceber);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                CarregarContaContabil();
                return View(contaReceber);
            }
        }


        [HttpGet]
        public ActionResult GerarRelatorio(int? pagina)
        {
            CarregarClientes();
            var relatorio = _contas.ObterTodos().Where(x => x.ClienteID == 0);

            int pageSize = 20;
            return View(Paginacao<ContaReceber>.Create(relatorio, pagina ?? 1, pageSize));
        }


        [HttpPost]
        public ActionResult GerarRelatorio(int? txtCliente, DateTime txtDataInicio, DateTime txtDataFim, StatusContaReceber? txtStatus, int? pagina)
        {
            try
            {
                CarregarClientes();

                var dateDiff = txtDataFim.Date - txtDataInicio.Date;

                if (txtDataInicio.Date == DateTime.MinValue || txtDataFim.Date == DateTime.MinValue)
                    throw new ArgumentException("O intervalo de datas é obrigatório");

                if (dateDiff.Days > 90)
                    throw new ArgumentException("Intervalo máximo de 90 dias");

                if (txtDataFim.Date < txtDataInicio.Date)
                    throw new ArgumentException("A Data Fim não pode ser menor que a Data Inicio");

                var relatorio = _contas.ObterTodos().Where(x => x.ClienteID == 0);

                if (txtCliente != null && txtDataFim != null && txtDataFim != null && txtStatus == null)
                    relatorio = _contas.ObterTodos().Where(x => x.ClienteID == txtCliente && x.DataVencimento.Date >= txtDataInicio && x.DataVencimento.Date <= txtDataFim);

                if (txtCliente == null && txtDataInicio != null && txtDataFim != null && txtStatus == null)
                    relatorio = _contas.ObterTodos().Where(x => x.DataVencimento.Date >= txtDataInicio && x.DataVencimento.Date <= txtDataFim);

                if (txtCliente == null && txtDataInicio != null && txtDataFim != null && txtStatus != null)
                    relatorio = _contas.ObterTodos().Where(x => x.DataVencimento.Date >= txtDataInicio && x.DataVencimento.Date <= txtDataFim && x.Status == txtStatus);

                if (txtCliente != null && txtDataInicio != null && txtDataFim != null && txtStatus != null)
                    relatorio = _contas.ObterTodos().Where(x => x.ClienteID == txtCliente && x.DataVencimento.Date >= txtDataInicio && x.DataVencimento.Date <= txtDataFim && x.Status == txtStatus);

                ViewData["ValorTotal"] = relatorio.Sum(x => x.Valor).ToString("C");

                TempData["txtCliente"] = txtCliente;
                TempData["txtDataInicio"] = txtDataInicio;
                TempData["txtDataFim"] = txtDataFim;
                TempData["txtStatus"] = txtStatus;

                int pageSize = 2000000000;
                return View(Paginacao<ContaReceber>.Create(relatorio, pagina ?? 1, pageSize));
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return RedirectToAction(nameof(GerarRelatorio));
            }
        }


        public ActionResult GerarExcel(int? txtCliente, DateTime txtDataInicio, DateTime txtDataFim, StatusContaReceber? txtStatus)
        {
            try
            {
                if (TempData["txtCliente"] != null)
                    txtCliente = (int)TempData["txtCliente"];

                if (TempData["txtDataInicio"] != null)
                    txtDataInicio = (DateTime)TempData["txtDataInicio"];

                if (TempData["txtDataFim"] != null)
                    txtDataFim = (DateTime)TempData["txtDataFim"];

                if (TempData["txtStatus"] != null)
                    txtStatus = (StatusContaReceber)TempData["txtStatus"];

                var relatorio = _contas.ObterTodos().Where(x => x.ClienteID == 0);

                if (txtCliente != null && txtDataFim != null && txtDataFim != null && txtStatus == null)
                    relatorio = _contas.ObterTodos().Where(x => x.ClienteID == txtCliente && x.DataVencimento.Date >= txtDataInicio && x.DataVencimento.Date <= txtDataFim);

                if (txtCliente == null && txtDataInicio != null && txtDataFim != null && txtStatus == null)
                    relatorio = _contas.ObterTodos().Where(x => x.DataVencimento.Date >= txtDataInicio && x.DataVencimento.Date <= txtDataFim);

                if (txtCliente == null && txtDataInicio != null && txtDataFim != null && txtStatus != null)
                    relatorio = _contas.ObterTodos().Where(x => x.DataVencimento.Date >= txtDataInicio && x.DataVencimento.Date <= txtDataFim && x.Status == txtStatus);

                if (txtCliente != null && txtDataInicio != null && txtDataFim != null && txtStatus != null)
                    relatorio = _contas.ObterTodos().Where(x => x.ClienteID == txtCliente && x.DataVencimento.Date >= txtDataInicio && x.DataVencimento.Date <= txtDataFim && x.Status == txtStatus);


                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("ContasReceber");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "Data de Cadastro";
                    worksheet.Cell(currentRow, 2).Value = "Data Competência";
                    worksheet.Cell(currentRow, 3).Value = "Data Vencimento";
                    worksheet.Cell(currentRow, 4).Value = "Data Pagamento";
                    worksheet.Cell(currentRow, 5).Value = "Categoria";
                    worksheet.Cell(currentRow, 6).Value = "Sub-categoria";
                    worksheet.Cell(currentRow, 7).Value = "Centro de Custo";
                    worksheet.Cell(currentRow, 8).Value = "Cliente";
                    worksheet.Cell(currentRow, 9).Value = "Descrição";
                    worksheet.Cell(currentRow, 10).Value = "Status";
                    worksheet.Cell(currentRow, 11).Value = "Valor";
                    worksheet.Cell(currentRow, 12).Value = "Juros";
                    worksheet.Cell(currentRow, 13).Value = "Multa";
                    worksheet.Cell(currentRow, 14).Value = "Descontos";
                    worksheet.Cell(currentRow, 15).Value = "Valor Recebido";

                    foreach (var rel in relatorio)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = rel.DataCadastro.ToString("dd/MM/yyyy");
                        worksheet.Cell(currentRow, 2).Value = rel.Competencia.ToString("MM/yyyy");
                        worksheet.Cell(currentRow, 3).Value = rel.DataVencimento.ToString("dd/MM/yyyy");
                        worksheet.Cell(currentRow, 4).Value = rel.DataPagamento;
                        worksheet.Cell(currentRow, 5).Value = rel.Categoria.Nome;
                        worksheet.Cell(currentRow, 6).Value = rel.SubCategoria.Nome;
                        worksheet.Cell(currentRow, 7).Value = rel.CentroDeCusto.Nome;
                        worksheet.Cell(currentRow, 8).Value = rel.Cliente.RazaoSocial;
                        worksheet.Cell(currentRow, 9).Value = rel.Descricao;
                        worksheet.Cell(currentRow, 10).Value = rel.Status;
                        worksheet.Cell(currentRow, 11).Value = rel.Valor;
                        worksheet.Cell(currentRow, 12).Value = rel.Juros;
                        worksheet.Cell(currentRow, 13).Value = rel.Multa;
                        worksheet.Cell(currentRow, 14).Value = rel.Desconto;
                        worksheet.Cell(currentRow, 15).Value = rel.ValorRecebido;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ContasReceber.xlsx");
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

        public ActionResult GerarRecibo(int id)
        {
            try
            {
                var result = _contas.ObterPorId(id);

                TratarValores tratarValores = new TratarValores();
                result.ValorPorExtenso = tratarValores.ExcreverValorPorExtenso(result.ValorRecebido ?? 0);

                var reciboPDF = new ViewAsPdf(result);
                return reciboPDF;
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return RedirectToAction(nameof(Index));
            }
        }

        public IEnumerable<Cliente> CarregarClientes()
        {
            return ViewBag.ListaClientes = _cliente.ObterTodos();
        }

        public IEnumerable<Categoria> CarregarCategorias()
        {
            return ViewBag.ListaCategorias = _categoria.ObterTodasReceitas();
        }

        public IEnumerable<Categoria> CarregarSubCategorias(int id)
        {
            var categoria = _categoria.ObterPorId(id);

            if (categoria != null)
            {
                return ViewBag.ListaSubCategorias = categoria.SubCategoria;
            }
            else
            {
                return ViewBag.ListaSubCategorias = _categoria.ObterSubCategorias(id);
            }
        }

        public IEnumerable<CentroDeCusto> CarregarCentroDeCusto()
        {
            return ViewBag.ListaCentroDeCusto = _centroCusto.ObterTodos();
        }

        public IEnumerable<ContaContabil> CarregarContaContabil()
        {
            return ViewBag.ListaContaContabil = _contaContabil.ObterTodos();
        }
    }
}
