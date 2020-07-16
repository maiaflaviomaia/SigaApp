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
    public class CentroDeCustosController : Controller
    {
        private readonly ICentroDeCusto _centroCusto;

        public CentroDeCustosController(ICentroDeCusto centroCusto)
        {
            _centroCusto = centroCusto;
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

            var centroCusto = from cs in _centroCusto.ObterTodos() select cs;

            if (!String.IsNullOrEmpty(filtro))
            {
                centroCusto = centroCusto.Where(s => EF.Functions.Like(s.Nome, $"%{filtro}%")).ToList();
            }

            int pageSize = 10;
            return View(Paginacao<CentroDeCusto>.Create(centroCusto, pagina ?? 1, pageSize));
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var centroCusto = _centroCusto.ObterPorId(id);

            if (centroCusto == null)
                return NotFound();

            return View(centroCusto);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CentroDeCusto centroDeCusto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!String.IsNullOrEmpty(centroDeCusto.Nome))
                    {
                        if (VerificarCentroCusto(centroDeCusto.Nome))
                            throw new ArgumentException("Centro de Custo já cadastrado");
                    }

                    centroDeCusto.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);

                    CentroDeCustoService service = new CentroDeCustoService();
                    service.PreencherCampos(centroDeCusto);

                    _centroCusto.Inserir(centroDeCusto);
                    return RedirectToAction(nameof(Index));
                }
                return View(centroDeCusto);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(centroDeCusto);
            }

            
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var centro = _centroCusto.ObterPorId(id);

            if (centro == null)
                return NotFound();

            return View(centro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CentroDeCusto centroDeCusto)
        {
            if(centroDeCusto.CentroCustoID != id || centroDeCusto == null)
                return NotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    CentroDeCustoService service = new CentroDeCustoService();
                    service.ValidarCampos(centroDeCusto);

                    _centroCusto.Atualizar(centroDeCusto);
                    return RedirectToAction(nameof(Index));
                }
                return View(centroDeCusto);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(centroDeCusto);
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var centroDeCusto = _centroCusto.ObterPorId(id);

            if (centroDeCusto == null)
                return NotFound();

            return View(centroDeCusto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _centroCusto.Desativar(id);
            return RedirectToAction(nameof(Index));
        }

        public bool VerificarCentroCusto(string nome)
        {
            var result = _centroCusto.ObterTodos().Any(x => x.Nome.ToUpper() == nome.ToUpper());
            return result;
        }
    }
}
