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
    public class EstudiosController : Controller
    {
        private readonly IEstudio _estudio;
        
        public EstudiosController(IEstudio estudio)
        {
            _estudio = estudio;
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

            var estudio = from cs in _estudio.ObterTodos() select cs;

            if (!String.IsNullOrEmpty(filtro))
            {
                estudio = estudio.Where(s => EF.Functions.Like(s.Nome, $"%{filtro}%")).ToList();
            }

            int pageSize = 10;
            return View(Paginacao<Estudio>.Create(estudio, pagina ?? 1, pageSize));
        }


        [HttpGet]
        public ActionResult Details(int id)
        {
            var estudio = _estudio.ObterPorId(id);

            if (estudio == null)
                return NotFound();

            return View(estudio);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Estudio estudio)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!String.IsNullOrEmpty(estudio.Nome))
                    {
                        if (VerificarEstudio(estudio.Nome))
                            throw new ArgumentException("Estúdio já cadastrado");
                    }

                    estudio.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);

                    EstudioService service = new EstudioService();
                    service.PreencherCampos(estudio);

                    _estudio.Inserir(estudio);
                    return RedirectToAction(nameof(Index));
                }
                return View(estudio);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(estudio);
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var estudio = _estudio.ObterPorId(id);

            if (estudio == null)
                return NotFound();

            return View(estudio);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Estudio estudio)
        {
            if(id != estudio.EstudioID || estudio == null)
                return NotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    EstudioService service = new EstudioService();
                    service.ValidarCampos(estudio);

                    _estudio.Atualizar(estudio);
                    return RedirectToAction(nameof(Index));
                }
                return View(estudio);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(estudio);
            }
        }

        
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var estudio = _estudio.ObterPorId(id);

            if (estudio == null)
                return NotFound();

            return View(estudio);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _estudio.Desativar(id);
            return RedirectToAction(nameof(Index));
        }

        public bool VerificarEstudio(string nome)
        {
            var result = _estudio.ObterTodos().Any(x => x.Nome.ToUpper() == nome.ToUpper());
            return result;
        }
    }
}
