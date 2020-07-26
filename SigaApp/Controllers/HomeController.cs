using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SigaApp.Models.Interfaces;
using static SigaApp.Utils.Enums;
using SigaApp.Models.Entidades;
using System.Net;

namespace SigaApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IContaReceber _contaReceber;
        private readonly IContaPagar _contaPagar;
        private readonly IOrcamento _orcamento;
        private readonly ILancamento _lancamento;
        private readonly IMensagemSite _mensagem;

        public HomeController(IContaReceber contaReceber, IContaPagar contaPagar, IOrcamento orcamento, ILancamento lancamento, IMensagemSite mensagem)
        {
            _contaReceber = contaReceber;
            _contaPagar = contaPagar;
            _orcamento = orcamento;
            _lancamento = lancamento;
            _mensagem = mensagem;
        }

        public IActionResult Index()
        {
            ContasViewModel contas = new ContasViewModel();
            contas.ContasPagar = _contaPagar.ObterTodos().Where(x => x.Status != StatusContaPagar.Pago).OrderBy(x => x.DataVencimento).Take(5);
            contas.ContasReceber = _contaReceber.ObterTodos().Where(x => x.Status != StatusContaReceber.Pago).OrderBy(x => x.DataVencimento).Take(5);

            var orcTotal = _orcamento.ObterTodos().Count();
            var orcAprovados = _orcamento.ObterTodos().Where(x => x.StatusOrcamento == StatusOrcamento.Aprovado).Count();
            var orcFaturados = _orcamento.ObterTodos().Where(x => x.StatusOrcamento == StatusOrcamento.Faturado).Count();
            var orcReprovados = _orcamento.ObterTodos().Where(x => x.StatusOrcamento == StatusOrcamento.Reprovado).Count();
            var orcCancelados = _orcamento.ObterTodos().Where(x => x.StatusOrcamento == StatusOrcamento.Cancelado).Count();
            var orcAberto = _orcamento.ObterTodos().Where(x => x.StatusOrcamento == StatusOrcamento.Aberto).Count();
            var orcAbertoValor = _orcamento.ObterTodos().Where(x => x.StatusOrcamento == StatusOrcamento.Aberto).Select(x => x.TotalOrcamento).Sum().ToString("C");


            ViewData["TotalContasPagar"] = contas.ContasPagar.Sum(x => x.Valor).ToString("C");
            ViewData["TotalContasReceber"] = contas.ContasReceber.Sum(x => x.Valor).ToString("C");
            ViewData["QtdTotalOrcamentos"] = orcTotal;
            ViewData["QtdOrcamentoAprovados"] = orcAprovados + orcFaturados;
            ViewData["QtdOrcamentoReprovados"] = orcReprovados + orcCancelados;
            ViewData["QtdOrcamentoAbertos"] = orcAberto;
            ViewData["ValOrcamentoAbertos"] = orcAbertoValor;

            return View(contas);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public JsonResult GerarGraficoAcumulado()
        {
            GraficoAcumuladoViewModel model = new GraficoAcumuladoViewModel();
            //model.TotalReceita = _contaReceber.ObterTodos().Select(x => x.Valor).Sum();
            //model.TotalDespesa = _contaPagar.ObterTodos().Select(x => x.Valor).Sum();

            model.TotalReceita = _lancamento.ObterTodos().Where(x => x.TipoLancamento == TipoLancamento.Credito && x.isTransferencia == false).Select(x => x.Valor).Sum();
            model.TotalDespesa = _lancamento.ObterTodos().Where(x => x.TipoLancamento == TipoLancamento.Debito && x.isTransferencia == false).Select(x => x.Valor).Sum();

            return Json(model);
        }

        public JsonResult GerarValoresReceitas()
        {
            List<decimal> listaSoma = new List<decimal>();
            int ano = DateTime.Now.Year;
           
            for(int i = 1; i <= 12; i++)
            {
                //var pesquisa = _contaReceber.ObterTodos().Where(x => x.DataVencimento >= (new DateTime(ano, i, 1)) && x.DataVencimento <= (new DateTime(ano, i, DateTime.DaysInMonth(ano, i)))).Select(x => x.Valor).Sum();
                var valorMes = _lancamento.ObterTodos()
                    .Where(x => x.TipoLancamento == TipoLancamento.Credito && x.DataLancamento >= (new DateTime(ano, i, 1)) && x.DataLancamento <= (new DateTime(ano, i, DateTime.DaysInMonth(ano, i))) && x.isTransferencia == false)
                    .Select(x => x.Valor).Sum();
                listaSoma.Add(valorMes);
            }

            return Json(listaSoma);
        }

        public JsonResult GerarValoresDespesas()
        {
            List<decimal> listaSoma = new List<decimal>();
            int ano = DateTime.Now.Year;

            for (int i = 1; i <= 12; i++)
            {
                //var valorMes = _contaPagar.ObterTodos().Where(x => x.DataVencimento >= (new DateTime(ano, i, 1)) && x.DataVencimento <= (new DateTime(ano, i, DateTime.DaysInMonth(ano, i)))).Select(x => x.Valor).Sum();
                var valorMes = _lancamento.ObterTodos()
                    .Where(x => x.TipoLancamento == TipoLancamento.Debito && x.DataLancamento >= (new DateTime(ano, i, 1)) && x.DataLancamento <= (new DateTime(ano, i, DateTime.DaysInMonth(ano, i))) && x.isTransferencia == false)
                    .Select(x => x.Valor).Sum();
                listaSoma.Add(valorMes);
            }

            return Json(listaSoma);
        }

        public JsonResult GerarGraficoMensal()
        {
            GraficoMensalViewModel model = new GraficoMensalViewModel();

            model.SomaReceitasMes = GerarValoresReceitas();
            model.SomaDespesasMes = GerarValoresDespesas();

            return Json(model);
        }
    }
}
