using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using SigaApp.Servicos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SigaApp.Controllers
{
    [Authorize]
    public class AgendaController : Controller
    {
        private readonly IAgenda _agenda;
        private readonly IEstudio _estudio;
        private readonly ICliente _cliente;

        [TempData]
        public string Mensagem { get; set; }

        public AgendaController(IAgenda agenda, IEstudio estudio, ICliente cliente)
        {
            _agenda = agenda;
            _estudio = estudio;
            _cliente = cliente;
        }

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

            var agenda = from cs in _agenda.ObterTodos() select cs;

            if (!String.IsNullOrEmpty(filtro))
            {
                agenda = agenda.Where(s => EF.Functions.Like(s.Titulo, $"%{filtro}%")).ToList();
            }

            int pageSize = 10;
            return View(Paginacao<Agenda>.Create(agenda, pagina ?? 1, pageSize));
        }


        public ActionResult Details(int id)
        {
            var agenda = _agenda.ObterPorId(id);

            if (agenda == null)
                return NotFound();

            return View(agenda);
        }

        
        public ActionResult Create()
        {
            var model = new Agenda();
            model.DataEvento = DateTime.Now;

            CarregarClientes();
            CarregarEstudios();
            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Agenda agenda)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    agenda.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);

                    AgendaService service = new AgendaService();
                    service.PreencherCampos(agenda);

                    _agenda.Inserir(agenda);
                    return RedirectToAction(nameof(Index));
                }
                return View(agenda);
                
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(agenda);
            }
        }

        
        public ActionResult Edit(int id)
        {
            var agenda = _agenda.ObterPorId(id);

            if(agenda == null)
                return NotFound();

            CarregarClientes();
            CarregarEstudios();

            return View(agenda);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Agenda agenda)
        {
            if(id != agenda.AgendaID || agenda == null)
                return NotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    AgendaService service = new AgendaService();
                    service.ValidarCampos(agenda);

                    _agenda.Atualizar(agenda);
                    return RedirectToAction(nameof(Index));
                }
                return View(agenda);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(agenda);
            }
        }

        
        public ActionResult Delete(int id)
        {
            var agenda = _agenda.ObterPorId(id);

            if (agenda == null)
                return NotFound();

            return View(agenda);
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _agenda.Desativar(id);
            return RedirectToAction(nameof(Index));
        }

        public IEnumerable<Estudio> CarregarEstudios()
        {
            return ViewBag.ListaEstudios = _estudio.ObterTodos();
        }

        public IEnumerable<Cliente> CarregarClientes()
        {
            return ViewBag.ListaClientes = _cliente.ObterTodos();
        }
    }
}
