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
using SigaApp.Models.ViewModels;
using SigaApp.Servicos;
using static SigaApp.Utils.Enums;

namespace SigaApp.Controllers
{
    [Authorize]
    public class LancamentosController : Controller
    {
        private readonly ILancamento _lancamento;
        private readonly IFornecedor _fornecedor;
        private readonly ICliente _cliente;
        private readonly ICategoria _categoria;
        private readonly ICentroDeCusto _centroCusto;
        private readonly IContaContabil _contaContabil;

        public LancamentosController(ILancamento lancamento, IFornecedor fornecedor, ICliente cliente, ICategoria categoria, ICentroDeCusto centroCusto, IContaContabil contaContabil)
        {
            _lancamento = lancamento;
            _fornecedor = fornecedor;
            _cliente = cliente;
            _categoria = categoria;
            _centroCusto = centroCusto;
            _contaContabil = contaContabil;
        }

        [TempData]
        public string Mensagem { get; set; }

        
        [HttpGet]
        public ActionResult Index(int txtConta, int? pagina)
        {
            try
            {
                CarregarContaContabil();

                var lancamento = _lancamento.ObterTodos().Where(x => x.ContaContabilID == txtConta);

                var receitas = lancamento.Where(x => x.TipoLancamento == TipoLancamento.Credito).Select(x => x.Valor).Sum();
                var despesas = lancamento.Where(x => x.TipoLancamento == TipoLancamento.Debito).Select(x => x.Valor).Sum();

                decimal saldoInicial = 0;

                if(txtConta == 0)
                {
                   saldoInicial = 0;
                }
                else
                {
                    saldoInicial = _contaContabil.ObterPorId(txtConta).SaldoInicial;
                }

                ViewData["FiltroAtual"] = txtConta;
                ViewData["SaldoInicial"] = saldoInicial.ToString("C");
                ViewData["SaldoAtual"] = (saldoInicial + receitas - despesas).ToString("C");

                int pageSize = 10;
                return View(Paginacao<Lancamento>.Create(lancamento, pagina ?? 1, pageSize));
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var lancamento = _lancamento.ObterPorId(id);

            if (lancamento == null)
                return NotFound();

            return View(lancamento);
        }

        [HttpGet]
        public ActionResult TransferirValores()
        {
            CarregarContaContabil();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TransferirValores(DateTime txtData, int txtOrigem, int txtDestino, decimal txtValor, string txtDescricao, string txtObservacao)
        {
            try
            {
                var saldoContaOrigem = VerificarSaldoContaOrigem(txtOrigem);
                
                if (txtValor > saldoContaOrigem)
                    throw new ArgumentException("Saldo insuficiente. Seu saldo na conta origem é de " + saldoContaOrigem.ToString("C"));

                LancamentoService service = new LancamentoService();
                service.ValidarTransferencia(txtData, txtOrigem, txtDestino, txtValor, txtDescricao, txtObservacao);

                #region DEBITANDO A CONTA ORIGEM
                Lancamento lancamentoDebitar = new Lancamento();
                lancamentoDebitar.DataCadastro = DateTime.Now;
                lancamentoDebitar.DataLancamento = txtData;
                lancamentoDebitar.ContaContabilID = txtOrigem;
                lancamentoDebitar.TipoLancamento = TipoLancamento.Debito;
                lancamentoDebitar.Valor = txtValor;
                lancamentoDebitar.Descricao = txtDescricao;
                lancamentoDebitar.CategoriaID = null;
                lancamentoDebitar.CentroCustoID = null;
                lancamentoDebitar.ClienteID = null;
                lancamentoDebitar.DataExclusao = null;
                lancamentoDebitar.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);
                lancamentoDebitar.FlagAtivo = true;
                lancamentoDebitar.isContaPagarReceber = false;
                lancamentoDebitar.isTransferencia = true;
                lancamentoDebitar.FornecedorID = null;
                lancamentoDebitar.Nome = "Transferência entre contas";
                lancamentoDebitar.NumeroDocumento = new Random().Next(1000000000);
                lancamentoDebitar.Observacoes = txtObservacao;
                
                _lancamento.Inserir(lancamentoDebitar);
                #endregion

                #region CREDITANDO A CONTA DESTINO
                Lancamento lancamentoCreditar = new Lancamento();
                lancamentoCreditar.DataCadastro = DateTime.Now;
                lancamentoCreditar.DataLancamento = txtData;
                lancamentoCreditar.ContaContabilID = txtDestino;
                lancamentoCreditar.TipoLancamento = TipoLancamento.Credito;
                lancamentoCreditar.Valor = txtValor;
                lancamentoCreditar.Descricao = txtDescricao;
                lancamentoCreditar.CategoriaID = null;
                lancamentoCreditar.CentroCustoID = null;
                lancamentoCreditar.ClienteID = null;
                lancamentoCreditar.DataExclusao = null;
                lancamentoCreditar.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);
                lancamentoCreditar.FlagAtivo = true;
                lancamentoCreditar.isContaPagarReceber = false;
                lancamentoDebitar.isTransferencia = true;
                lancamentoCreditar.FornecedorID = null;
                lancamentoCreditar.Nome = "Transferência entre contas";
                lancamentoCreditar.NumeroDocumento = new Random().Next(1000000000);
                lancamentoCreditar.Observacoes = txtObservacao;

                _lancamento.Inserir(lancamentoCreditar);
                #endregion

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                CarregarContaContabil();
                return View();
            }
        }

        public decimal VerificarSaldoContaOrigem(int txtOrigem)
        {
            var lancamento = _lancamento.ObterTodos().Where(x => x.ContaContabilID == txtOrigem);
            var saldoInicial = _contaContabil.ObterPorId(txtOrigem).SaldoInicial;

            var receitas = lancamento.Where(x => x.TipoLancamento == TipoLancamento.Credito).Select(x => x.Valor).Sum();
            var despesas = lancamento.Where(x => x.TipoLancamento == TipoLancamento.Debito).Select(x => x.Valor).Sum();
            var saldo = saldoInicial + receitas - despesas;
            return saldo;
        }
        
        [HttpGet]
        public ActionResult Create()
        {
            var model = new Lancamento();
            model.DataLancamento = DateTime.Now;

            CarregarFornecedores();
            CarregarClientes();
            CarregarCategorias();
            CarregarSubCategorias(0);
            CarregarCentroDeCusto();
            CarregarContaContabil();

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Lancamento lancamento)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    lancamento.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);

                    LancamentoService service = new LancamentoService();
                    service.PreencherCampos(lancamento);

                    _lancamento.Inserir(lancamento);
                    
                    return RedirectToAction(nameof(Index));
                }

                CarregarFornecedores();
                CarregarClientes();
                CarregarCategorias();
                CarregarSubCategorias(0);
                CarregarCentroDeCusto();
                CarregarContaContabil();
                return View(lancamento);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(lancamento);
            }
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var lancamento = _lancamento.ObterPorId(id);

            if (lancamento == null)
                return NotFound();

            CarregarFornecedores();
            CarregarClientes();

            if(lancamento.TipoLancamento == TipoLancamento.Credito)
            {
                CarregarCategoriasReceitas();
            }
            else
            {
                CarregarCategoriasDespesas();
            }
            
            CarregarSubCategorias(Convert.ToInt32(lancamento.CategoriaID));
            CarregarCentroDeCusto();
            CarregarContaContabil();
            return View(lancamento);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Lancamento lancamento)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    LancamentoService service = new LancamentoService();
                    service.ValidarCampos(lancamento);

                    _lancamento.Atualizar(lancamento);
                    return RedirectToAction(nameof(Index));
                }
                return View(lancamento);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(lancamento);
            }

        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            var lancamento = _lancamento.ObterPorId(id);

            if (lancamento == null)
                return NotFound();

            return View(lancamento);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _lancamento.Desativar(id);
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        public ActionResult GerarRelatorio(int? pagina)
        {
            CarregarContaContabil();
            var relatorio = _lancamento.ObterTodos().Where(x => x.ClienteID == 0);

            int pageSize = 20;
            return View(Paginacao<Lancamento>.Create(relatorio, pagina ?? 1, pageSize));
        }

        [HttpPost]
        public ActionResult GerarRelatorio(int? txtConta, DateTime txtDataInicio, DateTime txtDataFim, TipoLancamento? txtTipo, int? pagina)
        {
            try
            {
                CarregarContaContabil();

                var dateDiff = txtDataFim.Date - txtDataInicio.Date;

                if (txtDataInicio.Date == DateTime.MinValue || txtDataFim.Date == DateTime.MinValue)
                    throw new ArgumentException("O intervalo de datas é obrigatório");

                if (dateDiff.Days > 90)
                    throw new ArgumentException("Intervalo máximo de 90 dias");

                if (txtDataFim.Date < txtDataInicio.Date)
                    throw new ArgumentException("A Data Fim não pode ser menor que a Data Inicio");

                var relatorio = _lancamento.ObterTodos().Where(x => x.LancamentoID == 0);

                if (txtConta != null && txtDataInicio != null && txtDataFim != null && txtTipo == null)
                    relatorio = _lancamento.ObterTodos().Where(x => x.ContaContabilID == txtConta && x.DataLancamento >= txtDataInicio && x.DataLancamento <= txtDataFim);

                if (txtConta == null && txtDataInicio != null && txtDataFim != null && txtTipo != null)
                    relatorio = _lancamento.ObterTodos().Where(x => x.DataLancamento >= txtDataInicio && x.DataLancamento <= txtDataFim && x.TipoLancamento == txtTipo);

                if (txtConta == null && txtDataInicio != null && txtDataFim != null && txtTipo == null)
                    relatorio = _lancamento.ObterTodos().Where(x => x.DataLancamento >= txtDataInicio && x.DataLancamento <= txtDataFim);

                if (txtConta != null && txtDataInicio != null && txtDataFim != null && txtTipo != null)
                    relatorio = _lancamento.ObterTodos().Where(x => x.ContaContabilID == txtConta && x.DataLancamento >= txtDataInicio && x.DataLancamento <= txtDataFim && x.TipoLancamento == txtTipo);
                                
                ViewData["ValorCredito"] = relatorio.Where(x => x.TipoLancamento == TipoLancamento.Credito).Select(x => x.Valor).Sum().ToString("C");
                ViewData["ValorDebito"] = relatorio.Where(x => x.TipoLancamento == TipoLancamento.Debito).Select(x => x.Valor).Sum().ToString("C");

                TempData["txtConta"] = txtConta;
                TempData["txtDataInicio"] = txtDataInicio;
                TempData["txtDataFim"] = txtDataFim;
                TempData["txtTipo"] = txtTipo;

                int pageSize = 200000000;
                return View(Paginacao<Lancamento>.Create(relatorio, pagina ?? 1, pageSize));
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View();
            }
        }


        public ActionResult GerarExcel(int? txtConta, DateTime txtDataInicio, DateTime txtDataFim, TipoLancamento? txtTipo)
        {
            try
            {
                if (TempData["txtConta"] != null)
                    txtConta = (int)TempData["txtConta"];

                if (TempData["txtDataInicio"] != null)
                    txtDataInicio = (DateTime)TempData["txtDataInicio"];

                if (TempData["txtDataFim"] != null)
                    txtDataFim = (DateTime)TempData["txtDataFim"];

                if (TempData["txtTipo"] != null)
                    txtTipo = (TipoLancamento)TempData["txtTipo"];

                var relatorio = _lancamento.ObterTodos().Where(x => x.ClienteID == 0);

                if (txtConta != null && txtDataInicio != null && txtDataFim != null && txtTipo == null)
                    relatorio = _lancamento.ObterTodos().Where(x => x.ContaContabilID == txtConta && x.DataLancamento >= txtDataInicio && x.DataLancamento <= txtDataFim);

                if (txtConta == null && txtDataInicio != null && txtDataFim != null && txtTipo != null)
                    relatorio = _lancamento.ObterTodos().Where(x => x.DataLancamento >= txtDataInicio && x.DataLancamento <= txtDataFim && x.TipoLancamento == txtTipo);

                if (txtConta == null && txtDataInicio != null && txtDataFim != null && txtTipo == null)
                    relatorio = _lancamento.ObterTodos().Where(x => x.DataLancamento >= txtDataInicio && x.DataLancamento <= txtDataFim);

                if (txtConta != null && txtDataInicio != null && txtDataFim != null && txtTipo != null)
                    relatorio = _lancamento.ObterTodos().Where(x => x.ContaContabilID == txtConta && x.DataLancamento >= txtDataInicio && x.DataLancamento <= txtDataFim && x.TipoLancamento == txtTipo);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Lancamentos");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "Data de Cadastro";
                    worksheet.Cell(currentRow, 2).Value = "Data Lançamento";
                    worksheet.Cell(currentRow, 3).Value = "Descrição";
                    worksheet.Cell(currentRow, 4).Value = "Tipo Lançamento";
                    worksheet.Cell(currentRow, 5).Value = "Conta";
                    worksheet.Cell(currentRow, 6).Value = "Número Doc.";
                    worksheet.Cell(currentRow, 7).Value = "Cliente/Fornecedor";
                    worksheet.Cell(currentRow, 8).Value = "Categoria";
                    worksheet.Cell(currentRow, 9).Value = "Sub-Categoria";
                    worksheet.Cell(currentRow, 10).Value = "Centro de Custo";
                    worksheet.Cell(currentRow, 11).Value = "Conta COntábil";
                    worksheet.Cell(currentRow, 12).Value = "Valor";

                    foreach (var rel in relatorio)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = rel.DataCadastro.ToString("dd/MM/yyyy");
                        worksheet.Cell(currentRow, 2).Value = rel.DataLancamento.ToString("dd/MM/yyyy");
                        worksheet.Cell(currentRow, 3).Value = rel.Descricao ?? "-";
                        worksheet.Cell(currentRow, 4).Value = rel.TipoLancamento;
                        worksheet.Cell(currentRow, 5).Value = rel.ContaContabil.NomeConta ?? "-";
                        worksheet.Cell(currentRow, 6).Value = rel.NumeroDocumento ?? 0;
                        worksheet.Cell(currentRow, 7).Value = rel.Nome ?? "-";
                        worksheet.Cell(currentRow, 8).Value = rel.Categoria.Nome ?? "-";
                        worksheet.Cell(currentRow, 9).Value = rel.SubCategoria.Nome ?? "-";
                        worksheet.Cell(currentRow, 10).Value = rel.CentroCusto.Nome ?? "-";
                        worksheet.Cell(currentRow, 11).Value = rel.ContaContabil.NomeConta ?? "-";
                        worksheet.Cell(currentRow, 12).Value = rel.Valor.ToString("C") ?? "R$ 0.00";
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Lancamentos.xlsx");
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

        
        [HttpGet]
        public ActionResult GerarDRE() 
        {
            return View(new RelatorioDREViewModel { Relatorio = RelatorioDRE() });
        }

        private IEnumerable<RelatorioDRE> RelatorioDRE()
        {
            string ano = DateTime.Now.Year.ToString();
            string[] meses = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };

            var todosLancamentos = _lancamento.ObterTodos();
            var todasCategorias = _categoria.ObterTodos();
            
            for (int i = 0; i < meses.Length; i++)
            {
                foreach (var item in todasCategorias)
                {
                    string _mesAno = string.Format("{0}/{1}", meses[i], ano);
                    var lancamentos = todosLancamentos.Where(x => x.MesAno.Equals(_mesAno) && x.SubCategoriaID == item.CategoriaID).ToList();
                    decimal somatorioLancamentos = lancamentos != null && lancamentos.Count > 0 ? lancamentos.Sum(x => x.Valor) : 0;
                    item.SomatorioMensal.Add(new SomatorioMensal() { MesAno = _mesAno, Total = somatorioLancamentos });
                }
            }

            var categorias = from c in todasCategorias
                             where !c.CategoriaPai.HasValue
                             select new RelatorioDRE
                             {
                                 CategoriaPaiID = c.CategoriaID,
                                 DescricaoCategoria = c.Nome,
                                 ListaSubCategorias = (from nc in todasCategorias
                                                       where nc.CategoriaPai.HasValue && nc.CategoriaPai == c.CategoriaID
                                                       select nc).ToList()
                             };

            return categorias;
        }

        public IEnumerable<Fornecedor> CarregarFornecedores()
        {
            return ViewBag.ListaFornecedor = _fornecedor.ObterTodos();
        }

        public IEnumerable<Cliente> CarregarClientes()
        {
            return ViewBag.ListaClientes = _cliente.ObterTodos();
        }

        public IEnumerable<Categoria> CarregarCategorias()
        {
            return ViewBag.ListaCategorias = _categoria.ObterTodos();
        }

        public IEnumerable<Categoria> CarregarCategoriasDespesas()
        {
            return ViewBag.ListaCategorias = _categoria.ObterTodasDespesas();
        }

        public IEnumerable<Categoria> CarregarCategoriasReceitas()
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
