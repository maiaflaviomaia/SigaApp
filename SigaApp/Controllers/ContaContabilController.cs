using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using SigaApp.Servicos;

namespace SigaApp.Controllers
{
    [Authorize]
    public class ContaContabilController : Controller
    {
        private readonly IContaContabil _conta;
        
        public ContaContabilController(IContaContabil conta)
        {
            _conta = conta;
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

            var conta = from cs in _conta.ObterTodos() select cs;

            if (!String.IsNullOrEmpty(filtro))
            {
                conta = conta.Where(s => EF.Functions.Like(s.NomeConta, $"%{filtro}%")).ToList();
            }

            int pageSize = 10;
            return View(Paginacao<ContaContabil>.Create(conta, pagina ?? 1, pageSize));
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var contaContabil = _conta.ObterPorId(id);

            if(contaContabil == null)
                return NotFound();

            return View(contaContabil);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ContaContabil contaContabil)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!String.IsNullOrEmpty(contaContabil.NomeConta))
                    {
                        if (VerificarContaContabil(contaContabil.NomeConta))
                            throw new ArgumentException("Conta Contábil já cadastrada");
                    }

                    contaContabil.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);

                    ContaContabilService service = new ContaContabilService();
                    service.PreencherCampos(contaContabil);

                    _conta.Inserir(contaContabil);
                    return RedirectToAction(nameof(Index));
                }
                return View(contaContabil);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(contaContabil);
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var conta = _conta.ObterPorId(id);

            if (conta == null)
                return NotFound();

            return View(conta);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ContaContabil contaContabil)
        {
            if(contaContabil.ContaContabilID != id || contaContabil == null)
                return NotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    ContaContabilService service = new ContaContabilService();
                    service.ValidarCampos(contaContabil);

                    _conta.Atualizar(contaContabil);
                    return RedirectToAction(nameof(Index));
                }
                return View(contaContabil);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(contaContabil);
            }
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            var conta = _conta.ObterPorId(id);

            if (conta == null)
                return NotFound();

            return View(conta);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _conta.Desativar(id);
            return RedirectToAction(nameof(Index));
        }

        public bool VerificarContaContabil(string nomeConta)
        {
            var result = _conta.ObterTodos().Any(x => x.NomeConta.ToUpper() == nomeConta.ToUpper());
            return result;
        }
    }
}
