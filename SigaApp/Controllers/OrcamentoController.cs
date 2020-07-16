using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using SigaApp.Servicos;
using SigaApp.Utils;
using static SigaApp.Utils.Enums;

namespace SigaApp.Controllers
{
    [Authorize]
    public class OrcamentoController : Controller
    {
        private readonly IOrcamento _orcamento;
        private readonly ICliente _cliente;
        private readonly IServicoPrestado _servico;
        private readonly IOrcamentoServico _orcamentoServico;
        private readonly IFornecedor _fornecedor;
        private readonly IOrcamentoFornecedor _orcamentoFornecedor;
        private readonly IOrcamentoCusto _orcamentoCusto;
        private readonly IContaPagar _contaPagar;
        private readonly IContaReceber _contaReceber;
        

        public OrcamentoController(IOrcamento orcamento, ICliente cliente, IServicoPrestado servico, IOrcamentoServico orcamentoServico, IFornecedor fornecedor, IOrcamentoFornecedor orcamentoFornecedor, IOrcamentoCusto orcamentoCusto, IContaPagar contaPagar, IContaReceber contaReceber)
        {
            _orcamento = orcamento;
            _cliente = cliente;
            _servico = servico;
            _orcamentoServico = orcamentoServico;
            _fornecedor = fornecedor;
            _orcamentoFornecedor = orcamentoFornecedor;
            _orcamentoCusto = orcamentoCusto;
            _contaPagar = contaPagar;
            _contaReceber = contaReceber;
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

            var orcamento = from orc in _orcamento.ObterTodos() select orc;

            if (!String.IsNullOrEmpty(filtro))
            {
                orcamento = orcamento.Where(s => EF.Functions.Like(s.Cliente.RazaoSocial, $"%{filtro}%")).ToList();
            }

            int pageSize = 10;
            return View(Paginacao<Orcamento>.Create(orcamento, pagina ?? 1, pageSize));
        }


        [HttpGet]
        public ActionResult Details(int id)
        {
            var orcamento = _orcamento.ObterPorId(id);
            
            if (orcamento == null)
                return NotFound();

            return View(orcamento);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new Orcamento();
            model.DataOrcamento = DateTime.Now;
            model.DataValidade = DateTime.Now.AddMonths(1);

            CarregarCampos();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Orcamento orcamento)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    orcamento.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);

                    OrcamentoService service = new OrcamentoService();
                    service.PreencherCampos(orcamento);

                    _orcamento.Inserir(orcamento);

                    //Tratar o JSON da lista de serviços e transformar no objeto OrcamentoPublicidadeServicos
                    if (!String.IsNullOrEmpty(orcamento.ServicosJSON))
                    {
                        List<OrcamentoServico> ListaServicos = JsonConvert.DeserializeObject<List<OrcamentoServico>>(orcamento.ServicosJSON);
                        if (ListaServicos.Count >= 1)
                        {
                            for (int i = 0; i < ListaServicos.Count; i++)
                            {
                                var os = new OrcamentoServico();
                                os.EmpresaID = orcamento.EmpresaID;
                                os.OrcamentoID = orcamento.OrcamentoID;
                                os.ServicoPrestadoID = ListaServicos[i].ServicoPrestadoID;
                                os.Quantidade = ListaServicos[i].Quantidade;
                                os.ValorTotal = ListaServicos[i].ValorTotal;

                                OrcamentoServicoService oss = new OrcamentoServicoService();
                                oss.ValidarCampos(os);

                                _orcamentoServico.Inserir(os);
                            }
                        }
                    }


                    //Tratar o JSON da lista de Fornecedores e transformar no objeto OrcamentoFornecedores
                    if (!String.IsNullOrEmpty(orcamento.FornecedoresJSON))
                    {
                        List<OrcamentoFornecedor> ListaFornecedores = JsonConvert.DeserializeObject<List<OrcamentoFornecedor>>(orcamento.FornecedoresJSON);
                        if (ListaFornecedores.Count >= 1)
                        {
                            for (int i = 0; i < ListaFornecedores.Count; i++)
                            {
                                var of = new OrcamentoFornecedor();
                                of.EmpresaID = orcamento.EmpresaID;
                                of.OrcamentoID = orcamento.OrcamentoID;
                                of.FornecedorID = ListaFornecedores[i].FornecedorID;
                                of.Descricao = ListaFornecedores[i].Descricao;
                                of.Quantidade = ListaFornecedores[i].Quantidade;
                                of.ValorUnitario = ListaFornecedores[i].ValorUnitario;
                                of.ValorTotal = ListaFornecedores[i].ValorTotal;

                                OrcamentoFornecedorService opfs = new OrcamentoFornecedorService();
                                opfs.ValidarCampos(of);

                                _orcamentoFornecedor.Inserir(of);
                            }
                        }
                    }


                    //Tratar o JSON da lista de Custos de Produção e transformar no objeto OrcamentoCustos
                    if (!String.IsNullOrEmpty(orcamento.CustoProducaoJSON))
                    {
                        List<OrcamentoCustos> ListaCustos = JsonConvert.DeserializeObject<List<OrcamentoCustos>>(orcamento.CustoProducaoJSON);
                        if (ListaCustos.Count >= 1)
                        {
                            for (int i = 0; i < ListaCustos.Count; i++)
                            {
                                var oc = new OrcamentoCustos();
                                oc.EmpresaID = orcamento.EmpresaID;
                                oc.OrcamentoID = orcamento.OrcamentoID;
                                oc.Descricao = ListaCustos[i].Descricao;
                                oc.Quantidade = ListaCustos[i].Quantidade;
                                oc.ValorUnitario = ListaCustos[i].ValorUnitario;
                                oc.UnidadeValor = ListaCustos[i].UnidadeValor;
                                oc.ValorTotal = ListaCustos[i].ValorTotal;

                                OrcamentoCustosService opcs = new OrcamentoCustosService();
                                opcs.ValidarCampos(oc);

                                _orcamentoCusto.Inserir(oc);
                            }
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }

                CarregarCampos();
                return View(orcamento);

            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(orcamento);
            }
        }

        

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var orcamento = _orcamento.ObterPorId(id);

            if (orcamento == null)
                return NotFound();

            return View(orcamento);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _orcamento.Desativar(id);
            return RedirectToAction(nameof(Index));
        }

        public ActionResult AprovarOrcamento(int id)
        {
            var orcamento = _orcamento.ObterPorId(id);

            if(orcamento == null)
                return NotFound();

            OrcamentoService service = new OrcamentoService();
            service.AprovarOrcamento(orcamento);

            _orcamento.Atualizar(orcamento);

            return RedirectToAction(nameof(Index));
        }


        public ActionResult ReprovarOrcamento(int id)
        {
            var orcamento = _orcamento.ObterPorId(id);

            if (orcamento == null)
                return NotFound();

            OrcamentoService service = new OrcamentoService();
            service.ReprovarOrcamento(orcamento);

            _orcamento.Atualizar(orcamento);
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public ActionResult FaturarOrcamento(int id)
        {
            var orcamento = _orcamento.ObterPorId(id);

            if(orcamento == null)
                return NotFound();

            if (orcamento.StatusOrcamento != StatusOrcamento.Aprovado)
                return BadRequest();

            return View(orcamento);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FaturarOrcamento(int id, Orcamento orcamento)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    orcamento = _orcamento.ObterPorId(id);
                    TratarValores tratarValores = new TratarValores();

                    OrcamentoService service = new OrcamentoService();
                    service.FaturarOrcamento(orcamento);
                    _orcamento.Atualizar(orcamento);

                    #region GERANDO O CONTAS A RECEBER DO ORÇAMENTO
                    ContaReceber contaReceber = new ContaReceber();
                    contaReceber.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);
                    contaReceber.DataCadastro = DateTime.Now;
                    contaReceber.Competencia = DateTime.Now;
                    contaReceber.DataExclusao = null;
                    contaReceber.DataPagamento = null;
                    contaReceber.DataVencimento = DateTime.Now.AddMonths(1);
                    contaReceber.FlagAtivo = true;
                    contaReceber.ClienteID = orcamento.ClienteID;
                    contaReceber.CategoriaID = null;
                    contaReceber.SubCategoriaID = null;
                    contaReceber.CentroDeCustoID = null;
                    contaReceber.ContaContabilID = null;
                    contaReceber.OrcamentoID = orcamento.OrcamentoID;
                    contaReceber.Valor = orcamento.TotalOrcamento;
                    contaReceber.Recorrente = false;
                    contaReceber.TipoDocumento = TipoDocumento.NotaFiscal;
                    contaReceber.NumeroDocumento = Convert.ToInt32(orcamento.OrcamentoID.ToString() + tratarValores.TransformarHoraEmNumero(orcamento.DataCadastro));
                    contaReceber.Descricao = orcamento.Titulo;
                    contaReceber.FormaPagamento = FormaPagamento.Boleto;
                    contaReceber.Status = StatusContaReceber.Aberto;
                    contaReceber.Desconto = 0;
                    contaReceber.Juros = 0;
                    contaReceber.Multa = 0;
                    contaReceber.Observacoes = "Gerado automaticamente através do orçamento " + orcamento.OrcamentoID + " - " + orcamento.Cliente.RazaoSocial + " - " + orcamento.Titulo;

                    _contaReceber.Inserir(contaReceber);
                    #endregion

                    #region GERANDO O CONTAS A PAGAR DOS FORNECEDORES ENVOLVIDOS
                    if (orcamento.GerarPagamentos == true)
                    {
                        var listaFornecedor = _orcamentoFornecedor.ObterFornecedores(orcamento.OrcamentoID);

                        if (listaFornecedor != null)
                        {
                            foreach (var item in listaFornecedor)
                            {
                                ContaPagar contaPagar = new ContaPagar();
                                contaPagar.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);
                                contaPagar.DataCadastro = DateTime.Now;
                                contaPagar.Competencia = DateTime.Now;
                                contaPagar.DataExclusao = null;
                                contaPagar.DataPagamento = null;
                                contaPagar.DataVencimento = DateTime.Now.AddMonths(1);
                                contaPagar.FlagAtivo = true;
                                contaPagar.FornecedorID = item.FornecedorID;
                                contaPagar.CategoriaID = null;
                                contaPagar.SubCategoriaID = null;
                                contaPagar.CentroDeCustoID = null;
                                contaPagar.ContaContabilID = null;
                                contaPagar.OrcamentoID = orcamento.OrcamentoID;
                                contaPagar.Valor = item.ValorTotal;
                                contaPagar.Recorrente = false;
                                contaPagar.TipoDocumento = TipoDocumento.NotaFiscal;
                                contaPagar.NumeroDocumento = Convert.ToInt32(orcamento.OrcamentoID.ToString() + tratarValores.TransformarHoraEmNumero(orcamento.DataCadastro));
                                contaPagar.Descricao = item.Descricao;
                                contaPagar.FormaPagamento = FormaPagamento.Boleto;
                                contaPagar.Status = StatusContaPagar.Aberto;
                                contaPagar.Desconto = 0;
                                contaPagar.Juros = 0;
                                contaPagar.Multa = 0;
                                contaPagar.Observacoes = "Gerado automaticamente através do orçamento " + orcamento.OrcamentoID + " - " + orcamento.Cliente.RazaoSocial + " - " + orcamento.Titulo;

                                _contaPagar.Inserir(contaPagar);
                            }
                        }
                    }
                    #endregion

                    return RedirectToAction(nameof(Index));
                }

                return View(orcamento);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(orcamento);
            }
        }

        public ActionResult GerarPDFResumido(int id)
        {
            try
            {
                var result = _orcamento.ObterPorId(id);

                TratarValores tratarValores = new TratarValores();
                result.ValorPorExtenso = tratarValores.ExcreverValorPorExtenso(result.TotalOrcamento);

                var OrcamentoPDF = new ViewAsPdf(result);
                return OrcamentoPDF;
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return RedirectToAction(nameof(Index));
            }
            
        }

        public ActionResult GerarPDFDetalhado(int id)
        {
            try
            {
                var result = _orcamento.ObterPorId(id);

                TratarValores tratarValores = new TratarValores();
                result.ValorPorExtenso = tratarValores.ExcreverValorPorExtenso(result.TotalOrcamento);

                var OrcamentoPDF = new ViewAsPdf(result);
                return OrcamentoPDF;
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public ActionResult GerarRelatorio(int? pagina)
        {
            CarregarClientes();
            var relatorio = _orcamento.ObterTodos().Where(x => x.ClienteID == 0);

            int pageSize = 20;
            return View(Paginacao<Orcamento>.Create(relatorio, pagina ?? 1, pageSize));
        }

        [HttpPost]
        public ActionResult GerarRelatorio(int? txtCliente, DateTime txtDataInicio, DateTime txtDataFim, StatusOrcamento? txtStatus, string txtTipo, int? pagina)
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

                var relatorio = _orcamento.ObterTodos().Where(x => x.ClienteID == 0);

                if (txtCliente != null && txtDataInicio != null && txtDataFim != null && txtStatus == null && txtTipo == null)
                    relatorio = _orcamento.ObterTodos().Where(x => x.ClienteID == txtCliente && x.DataOrcamento >= txtDataInicio && x.DataOrcamento <= txtDataFim);

                if (txtCliente == null && txtDataInicio != null && txtDataFim != null && txtStatus == null && txtTipo == null)
                    relatorio = _orcamento.ObterTodos().Where(x => x.DataOrcamento >= txtDataInicio && x.DataOrcamento <= txtDataFim);

                if (txtCliente == null && txtDataInicio != null && txtDataFim != null && txtStatus != null && txtTipo == null)
                    relatorio = _orcamento.ObterTodos().Where(x => x.DataOrcamento >= txtDataInicio && x.DataOrcamento <= txtDataFim && x.StatusOrcamento == txtStatus);

                if (txtCliente == null && txtDataInicio != null && txtDataFim != null && txtStatus == null && txtTipo != null)
                    relatorio = _orcamento.ObterTodos().Where(x => x.DataOrcamento >= txtDataInicio && x.DataOrcamento <= txtDataFim && x.TipoOrcamento == txtTipo);

                if (txtCliente != null && txtDataInicio != null && txtDataFim != null && txtStatus != null && txtTipo != null)
                    relatorio = _orcamento.ObterTodos().Where(x => x.ClienteID == txtCliente && x.DataOrcamento >= txtDataInicio && x.DataOrcamento <= txtDataFim && x.StatusOrcamento == txtStatus && x.TipoOrcamento == txtTipo);

                ViewData["ValorTotal"] = relatorio.Sum(x => x.TotalOrcamento).ToString("C");

                TempData["txtCliente"] = txtCliente;
                TempData["txtDataInicio"] = txtDataInicio;
                TempData["txtDataFim"] = txtDataFim;
                TempData["txtStatus"] = txtStatus;
                TempData["txtTipo"] = txtTipo;

                int pageSize = 200000000;
                return View(Paginacao<Orcamento>.Create(relatorio, pagina ?? 1, pageSize));
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return RedirectToAction(nameof(GerarRelatorio));
            }
        }


        public ActionResult GerarExcel(int? txtCliente, DateTime txtDataInicio, DateTime txtDataFim, StatusOrcamento? txtStatus, string txtTipo)
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
                    txtStatus = (StatusOrcamento)TempData["txtStatus"];

                if (TempData["txtTipo"] != null)
                    txtTipo = TempData["txtTipo"].ToString();

                var relatorio = _orcamento.ObterTodos().Where(x => x.ClienteID == 0);

                if (txtCliente != null && txtDataInicio != null && txtDataFim != null && txtStatus == null && txtTipo == null)
                    relatorio = _orcamento.ObterTodos().Where(x => x.ClienteID == txtCliente && x.DataOrcamento >= txtDataInicio && x.DataOrcamento <= txtDataFim);

                if (txtCliente == null && txtDataInicio != null && txtDataFim != null && txtStatus == null && txtTipo == null)
                    relatorio = _orcamento.ObterTodos().Where(x => x.DataOrcamento >= txtDataInicio && x.DataOrcamento <= txtDataFim);

                if (txtCliente == null && txtDataInicio != null && txtDataFim != null && txtStatus != null && txtTipo == null)
                    relatorio = _orcamento.ObterTodos().Where(x => x.DataOrcamento >= txtDataInicio && x.DataOrcamento <= txtDataFim && x.StatusOrcamento == txtStatus);

                if (txtCliente == null && txtDataInicio != null && txtDataFim != null && txtStatus == null && txtTipo != null)
                    relatorio = _orcamento.ObterTodos().Where(x => x.DataOrcamento >= txtDataInicio && x.DataOrcamento <= txtDataFim && x.TipoOrcamento == txtTipo);

                if (txtCliente != null && txtDataInicio != null && txtDataFim != null && txtStatus != null && txtTipo != null)
                    relatorio = _orcamento.ObterTodos().Where(x => x.ClienteID == txtCliente && x.DataOrcamento >= txtDataInicio && x.DataOrcamento <= txtDataFim && x.StatusOrcamento == txtStatus && x.TipoOrcamento == txtTipo);


                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Orçamentos");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "Data de Cadastro";
                    worksheet.Cell(currentRow, 2).Value = "Data Orçamento";
                    worksheet.Cell(currentRow, 3).Value = "Cliente";
                    worksheet.Cell(currentRow, 4).Value = "Titulo";
                    worksheet.Cell(currentRow, 5).Value = "Status";
                    worksheet.Cell(currentRow, 6).Value = "Valor";

                    foreach (var rel in relatorio)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = rel.DataCadastro.ToString("dd/MM/yyyy");
                        worksheet.Cell(currentRow, 2).Value = rel.DataOrcamento.ToString("dd/MM/yyyy");
                        worksheet.Cell(currentRow, 3).Value = rel.Cliente.RazaoSocial;
                        worksheet.Cell(currentRow, 4).Value = rel.Titulo;
                        worksheet.Cell(currentRow, 5).Value = rel.StatusOrcamento;
                        worksheet.Cell(currentRow, 6).Value = rel.TotalOrcamento;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orcamentos.xlsx");
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

        public void CarregarCampos()
        {
            CarregarClientes();
            CarregarServicos();
            CarregarFornecedores();
        }

        public IEnumerable<Cliente> CarregarClientes()
        {
            return ViewBag.ListaClientes = _cliente.ObterTodos();
        }

        public IEnumerable<ServicoPrestado> CarregarServicos()
        {
            return ViewBag.ListaServicos = _servico.ObterTodos();
        }

        public IEnumerable<Fornecedor> CarregarFornecedores()
        {
            return ViewBag.ListaFornecedor = _fornecedor.ObterTodos();
        }
    }
}
