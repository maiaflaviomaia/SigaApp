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
    public class CargoController : Controller
    {
        private readonly ICargo _cargo;
        
        public CargoController(ICargo cargo)
        {
            _cargo = cargo;
        }

        [TempData]
        public string Mensagem { get; set; }

        [HttpGet]
        public ActionResult Index(string filtroAtual, string filtro, int? pagina)
        {  
            if(filtro != null)
            {
                pagina = 1;
            }
            else
            {
                filtro = filtroAtual;
            }

            ViewData["FiltroAtual"] = filtro;

            var cargo = from cs in _cargo.ObterTodos() select cs;

            if (!String.IsNullOrEmpty(filtro))
            {
                cargo = cargo.Where(s => EF.Functions.Like(s.DescricaoSumaria, $"%{filtro}%")).ToList();
            }

            int pageSize = 10;
            return View(Paginacao<Cargo>.Create(cargo, pagina ?? 1, pageSize));
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var cargo = _cargo.ObterPorId(id);
                
            if (cargo == null)
                return NotFound();

            return View(cargo);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cargo cargo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!String.IsNullOrEmpty(cargo.DescricaoSumaria))
                    {
                        if (VerificarCargo(cargo.DescricaoSumaria))
                            throw new ArgumentException("Cargo já cadastrado");
                    }

                    cargo.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);

                    CargoService service = new CargoService();
                    service.PreencherCampos(cargo);
                    
                    _cargo.Inserir(cargo);
                    return RedirectToAction(nameof(Index));
                }
                return View(cargo);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(cargo);
            }
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var cargo = _cargo.ObterPorId(id);

            if (cargo == null)
                return NotFound();

            return View(cargo);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Cargo cargo)
        {
            if (id != cargo.CargoID || cargo == null)
                return NotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    CargoService service = new CargoService();
                    service.ValidarCampos(cargo);

                    _cargo.Atualizar(cargo);
                    return RedirectToAction(nameof(Index));
                }
                return View(cargo);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(cargo);
            }

        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            var cargo = _cargo.ObterPorId(id);

            if (cargo == null)
                return NotFound();

            return View(cargo);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _cargo.Desativar(id);
            return RedirectToAction(nameof(Index));
        }

        public bool VerificarCargo(string descricaoSumaria)
        {
                var result = _cargo.ObterTodos().Any(x => x.DescricaoSumaria.ToUpper() == descricaoSumaria.ToUpper());
                return result;
        }
    }
}
