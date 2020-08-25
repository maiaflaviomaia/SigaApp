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
    public class ContasPagarController : Controller
    {
        private readonly IContaPagar _contas;
        private readonly IFornecedor _fornecedor;
        private readonly ICategoria _categoria;
        private readonly ICentroDeCusto _centroCusto;
        private readonly IContaContabil _contaContabil;
        private readonly ILancamento _lancamento;
        
        public ContasPagarController(IContaPagar contas, IFornecedor fornecedor, ICategoria categoria, ICentroDeCusto centroCusto, IContaContabil contaContabil, ILancamento lancamento)
        {
            _contas = contas;
            _fornecedor = fornecedor;
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
                conta = conta.Where(s => EF.Functions.Like(s.Fornecedor.RazaoSocial, $"%{filtro}%")).ToList();
            }

            int pageSize = 10;
            return View(Paginacao<ContaPagar>.Create(conta, pagina ?? 1, pageSize));
        }
               
        
        [HttpGet]
        public ActionResult Details(int id)
        {
            var conta = _contas.ObterPorId(id);

            if (conta == null)
                return NotFound();

            return View(conta);
        }


        [HttpGet]
        public ActionResult Create()
        {
            var model = new ContaPagar();
            model.DataVencimento = DateTime.Now;
            model.Competencia = DateTime.Now;

            CarregarFornecedores();
            CarregarCategorias();
            CarregarSubCategorias(0);
            CarregarCentroDeCusto();
            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ContaPagar contasPagar)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    contasPagar.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);

                    ContasPagarService service = new ContasPagarService();
                    service.PreencherCampos(contasPagar);

                    if(contasPagar.Recorrente == true)
                    {
                        int numParcela = 0;
                        for (int i = 1; i <= 12; i++)
                        {
                            numParcela++;
                            ContaPagar parcela = new ContaPagar();
                            parcela.CategoriaID = contasPagar.CategoriaID;
                            parcela.SubCategoriaID = contasPagar.SubCategoriaID;
                            parcela.CentroDeCustoID = contasPagar.CentroDeCustoID;
                            parcela.ContaContabilID = contasPagar.ContaContabilID;
                            parcela.DataCadastro = contasPagar.DataCadastro;
                            parcela.DataExclusao = contasPagar.DataExclusao;
                            parcela.DataPagamento = contasPagar.DataPagamento;

                            if (numParcela > 1)
                            {
                                parcela.DataVencimento = contasPagar.DataVencimento.AddMonths(numParcela - 1);
                                parcela.Competencia = contasPagar.Competencia.AddMonths(numParcela - 1);
                            }
                            else
                            {
                                parcela.DataVencimento = contasPagar.DataVencimento;
                                parcela.Competencia = contasPagar.Competencia;
                            }

                            parcela.Status = StatusContaPagar.Aberto;
                            parcela.Desconto = contasPagar.Desconto;
                            parcela.Descricao = contasPagar.Descricao;
                            parcela.EmpresaID = contasPagar.EmpresaID;
                            parcela.FlagAtivo = contasPagar.FlagAtivo;
                            parcela.FormaPagamento = contasPagar.FormaPagamento;
                            parcela.FornecedorID = contasPagar.FornecedorID;
                            parcela.Desconto = contasPagar.Desconto ?? 0;
                            parcela.Juros = contasPagar.Juros ?? 0;
                            parcela.Multa = contasPagar.Multa ?? 0;
                            parcela.Recorrente = contasPagar.Recorrente;
                            parcela.NumeroDocumento = contasPagar.NumeroDocumento + numParcela;
                            parcela.TipoDocumento = contasPagar.TipoDocumento;
                            parcela.Valor = contasPagar.Valor;
                            parcela.ValorPago = contasPagar.ValorPago;
                            parcela.Observacoes = "Gerado automaticamente através de cadastro de conta recorrente. Parcela: " + numParcela + "/12";

                            _contas.Inserir(parcela);
                        }
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        _contas.Inserir(contasPagar);
                        return RedirectToAction(nameof(Index));
                    }
                }

                CarregarFornecedores();
                CarregarCategorias();
                CarregarSubCategorias(0);
                CarregarCentroDeCusto();
                return View(contasPagar);
            }
            catch(Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                CarregarFornecedores();
                CarregarCategorias();
                CarregarSubCategorias(0);
                CarregarCentroDeCusto();
                return View(contasPagar);
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var contaPagar = _contas.ObterPorId(id);

            if (contaPagar == null)
                return NotFound();

            if (contaPagar.Status == StatusContaPagar.Pago)
                return BadRequest();

            CarregarFornecedores();
            CarregarCategorias();
            CarregarSubCategorias(Convert.ToInt32(contaPagar.CategoriaID));
            CarregarCentroDeCusto();

            return View(contaPagar);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ContaPagar contasPagar)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ContasPagarService service = new ContasPagarService();
                    service.ValidarCampos(contasPagar);

                    _contas.Atualizar(contasPagar);
                    return RedirectToAction(nameof(Index));
                }
                CarregarFornecedores();
                CarregarCategorias();
                CarregarSubCategorias(Convert.ToInt32(contasPagar.CategoriaID));
                CarregarCentroDeCusto();
                return View(contasPagar);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                CarregarFornecedores();
                CarregarCategorias();
                CarregarSubCategorias(Convert.ToInt32(contasPagar.CategoriaID));
                CarregarCentroDeCusto(); 
                return View(contasPagar);
            }
        }

        [HttpGet]
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
        public ActionResult Pagar(int id)
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
        public ActionResult Pagar(int id, ContaPagar contasPagar)
        {
            if(contasPagar.ContasPagarID != id || contasPagar == null)
                return NotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    ContasPagarService service = new ContasPagarService();
                    service.Pagar(contasPagar);
                    _contas.Atualizar(contasPagar);

                    var contasPagarAux = _contas.ObterPorId(id);
                    
                    Lancamento lancamento = new Lancamento();
                    service.GerarLancamento(lancamento, contasPagarAux);
                    _lancamento.Inserir(lancamento);
                    return RedirectToAction(nameof(Index));
                }

                CarregarContaContabil();
                return View(contasPagar);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                CarregarContaContabil();
                return View(contasPagar);
            }
        }


        [HttpGet]
        public ActionResult GerarRelatorio(int? pagina)
        {
                CarregarFornecedores();
                var relatorio = _contas.ObterTodos().Where(x => x.FornecedorID == 0);

                int pageSize = 20;
                return View(Paginacao<ContaPagar>.Create(relatorio, pagina ?? 1, pageSize));
        }


        [HttpPost]
        public ActionResult GerarRelatorio(int? txtFornecedor, DateTime txtDataInicio, DateTime txtDataFim, StatusContaPagar? txtStatus, int? pagina)
        {
            try
            {
                CarregarFornecedores();

                var dateDiff = txtDataFim.Date - txtDataInicio.Date;

                if (txtDataInicio.Date == DateTime.MinValue || txtDataFim.Date == DateTime.MinValue)
                    throw new ArgumentException("O intervalo de datas é obrigatório");

                if (dateDiff.Days > 90)
                    throw new ArgumentException("Intervalo máximo de 90 dias");

                if (txtDataFim.Date < txtDataInicio.Date)
                    throw new ArgumentException("A Data Fim não pode ser menor que a Data Inicio");

                var relatorio = _contas.ObterTodos().Where(x => x.FornecedorID == 0);

                if (txtFornecedor != null && txtDataFim != null && txtDataFim != null && txtStatus == null)
                    relatorio = _contas.ObterTodos().Where(x => x.FornecedorID == txtFornecedor && x.DataVencimento.Date >= txtDataInicio && x.DataVencimento.Date <= txtDataFim);

                if (txtFornecedor == null && txtDataInicio != null && txtDataFim != null && txtStatus == null)
                    relatorio = _contas.ObterTodos().Where(x => x.DataVencimento.Date >= txtDataInicio && x.DataVencimento.Date <= txtDataFim);

                if (txtFornecedor == null && txtDataInicio != null && txtDataFim != null && txtStatus != null)
                    relatorio = _contas.ObterTodos().Where(x => x.DataVencimento.Date >= txtDataInicio && x.DataVencimento.Date <= txtDataFim && x.Status == txtStatus);

                if (txtFornecedor != null && txtDataInicio != null && txtDataFim != null && txtStatus != null)
                    relatorio = _contas.ObterTodos().Where(x => x.FornecedorID == txtFornecedor && x.DataVencimento.Date >= txtDataInicio && x.DataVencimento.Date <= txtDataFim && x.Status == txtStatus);

                ViewData["ValorTotal"] = relatorio.Sum(x => x.Valor).ToString("C");

                TempData["txtFornecedor"] = txtFornecedor;
                TempData["txtDataInicio"] = txtDataInicio;
                TempData["txtDataFim"] = txtDataFim;
                TempData["txtStatus"] = txtStatus;

                int pageSize = 200000000;
                return View(Paginacao<ContaPagar>.Create(relatorio, pagina ?? 1, pageSize));
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                CarregarFornecedores();
                var relatorio = _contas.ObterTodos().Where(x => x.FornecedorID == 0);
                return View(Paginacao<ContaPagar>.Create(relatorio, pagina ?? 1, 20));
            }
        }

        public ActionResult GerarExcel(int? txtFornecedor, DateTime txtDataInicio, DateTime txtDataFim, StatusContaPagar? txtStatus)
        {
            try
            {
                if (TempData["txtFornecedor"] != null)
                    txtFornecedor = (int)TempData["txtFornecedor"];

                if (TempData["txtDataInicio"] != null)
                    txtDataInicio = (DateTime)TempData["txtDataInicio"];

                if (TempData["txtDataFim"] != null)
                    txtDataFim = (DateTime)TempData["txtDataFim"];

                if (TempData["txtStatus"] != null)
                    txtStatus = (StatusContaPagar)TempData["txtStatus"];

                var relatorio = _contas.ObterTodos().Where(x => x.FornecedorID == 0);

                if (txtFornecedor != null && txtDataFim != null && txtDataFim != null && txtStatus == null)
                    relatorio = _contas.ObterTodos().Where(x => x.FornecedorID == txtFornecedor && x.DataVencimento.Date >= txtDataInicio && x.DataVencimento.Date <= txtDataFim);

                if (txtFornecedor == null && txtDataInicio != null && txtDataFim != null && txtStatus == null)
                    relatorio = _contas.ObterTodos().Where(x => x.DataVencimento.Date >= txtDataInicio && x.DataVencimento.Date <= txtDataFim);

                if (txtFornecedor == null && txtDataInicio != null && txtDataFim != null && txtStatus != null)
                    relatorio = _contas.ObterTodos().Where(x => x.DataVencimento.Date >= txtDataInicio && x.DataVencimento.Date <= txtDataFim && x.Status == txtStatus);

                if (txtFornecedor != null && txtDataInicio != null && txtDataFim != null && txtStatus != null)
                    relatorio = _contas.ObterTodos().Where(x => x.FornecedorID == txtFornecedor && x.DataVencimento.Date >= txtDataInicio && x.DataVencimento.Date <= txtDataFim && x.Status == txtStatus);


                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("ContasPagar");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "DATA DE CADASTRO";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                    worksheet.Cell(currentRow, 2).Value = "DATA COMPETÊNCIA";
                    worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                    worksheet.Cell(currentRow, 3).Value = "DATA VENCIMENTO";
                    worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                    worksheet.Cell(currentRow, 4).Value = "DATA PAGAMENTO";
                    worksheet.Cell(currentRow, 4).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 4).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                    worksheet.Cell(currentRow, 5).Value = "CATEGORIA";
                    worksheet.Cell(currentRow, 5).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 5).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                    worksheet.Cell(currentRow, 6).Value = "SUB-CATEGORIA";
                    worksheet.Cell(currentRow, 6).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 6).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                    worksheet.Cell(currentRow, 7).Value = "CENTRO DE CUSTO";
                    worksheet.Cell(currentRow, 7).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 7).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                    worksheet.Cell(currentRow, 8).Value = "FORNECEDOR";
                    worksheet.Cell(currentRow, 8).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 8).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                    worksheet.Cell(currentRow, 9).Value = "DESCRIÇÃO";
                    worksheet.Cell(currentRow, 9).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 9).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                    worksheet.Cell(currentRow, 10).Value = "STATUS";
                    worksheet.Cell(currentRow, 10).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 10).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                    worksheet.Cell(currentRow, 11).Value = "VALOR";
                    worksheet.Cell(currentRow, 11).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 11).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                    worksheet.Cell(currentRow, 12).Value = "JUROS";
                    worksheet.Cell(currentRow, 12).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 12).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                    worksheet.Cell(currentRow, 13).Value = "MULTA";
                    worksheet.Cell(currentRow, 13).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 13).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                    worksheet.Cell(currentRow, 14).Value = "DESCONTOS";
                    worksheet.Cell(currentRow, 14).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 14).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                    worksheet.Cell(currentRow, 15).Value = "VALOR PAGO";
                    worksheet.Cell(currentRow, 15).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 15).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);

                    foreach (var rel in relatorio)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = rel.DataCadastro.ToString("dd/MM/yyyy");
                        worksheet.Cell(currentRow, 2).Value = rel.Competencia.ToString("MM/yyyy");
                        worksheet.Cell(currentRow, 3).Value = rel.DataVencimento.ToString("dd/MM/yyyy");
                        worksheet.Cell(currentRow, 4).Value = rel.DataPagamento;
                        worksheet.Cell(currentRow, 5).Value = rel.Categoria.Nome ?? " - ";
                        worksheet.Cell(currentRow, 6).Value = rel.SubCategoria.Nome ?? " - ";
                        worksheet.Cell(currentRow, 7).Value = rel.CentroDeCusto.Nome ?? " - ";
                        worksheet.Cell(currentRow, 8).Value = rel.Fornecedor.RazaoSocial ?? " - ";
                        worksheet.Cell(currentRow, 9).Value = rel.Descricao ?? " - ";
                        worksheet.Cell(currentRow, 10).Value = rel.Status.ToString() ?? " - ";
                        worksheet.Cell(currentRow, 11).Value = rel.Valor.ToString() ?? "R$ 0,00" ;
                        worksheet.Cell(currentRow, 12).Value = rel.Juros ?? 0;
                        worksheet.Cell(currentRow, 13).Value = rel.Multa ?? 0;
                        worksheet.Cell(currentRow, 14).Value = rel.Desconto ?? 0;
                        worksheet.Cell(currentRow, 15).Value = rel.ValorPago ?? 0;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ContasPagar.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View();
            }
        }

        public ActionResult GerarRecibo(int id)
        {
            try
            {
                var result = _contas.ObterPorId(id);

                TratarValores tratarValores = new TratarValores();
                result.ValorPorExtenso = tratarValores.ExcreverValorPorExtenso(result.ValorPago ?? 0);

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

        public IEnumerable<Fornecedor> CarregarFornecedores()
        {
            return ViewBag.ListaFornecedor = _fornecedor.ObterTodos();
        }

        public IEnumerable<Categoria> CarregarCategorias()
        {
            return ViewBag.ListaCategorias = _categoria.ObterTodasDespesas();
        }

        public IEnumerable<Categoria> CarregarSubCategorias(int id)
        {
            var categoria = _categoria.ObterPorId(id);

            if(categoria != null)
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


