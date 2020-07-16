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
    public class ServicosPrestadosController : Controller
    {
        private readonly IServicoPrestado _servico;
        
        public ServicosPrestadosController(IServicoPrestado servico)
        {
            _servico = servico;
        }

        [TempData]
        public string Mensagem { get; set; }

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

            var servico = from cs in _servico.ObterTodos() select cs;

            if (!String.IsNullOrEmpty(filtro))
            {
                servico = servico.Where(s => EF.Functions.Like(s.Descricao, $"%{filtro}%")).ToList();
            }

            int pageSize = 10;
            return View(Paginacao<ServicoPrestado>.Create(servico, pagina ?? 1, pageSize));
        }


        public ActionResult Details(int id)
        {
            var servico = _servico.ObterPorId(id);

            if (servico == null)
                return NotFound();

            return View(servico);
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ServicoPrestado servicoPrestado)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!String.IsNullOrEmpty(servicoPrestado.Descricao))
                    {
                        if (VerificarServico(servicoPrestado.Descricao))
                            throw new ArgumentException("Serviço já cadastrado");
                    }

                    servicoPrestado.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);

                    ServicoPrestadoService service = new ServicoPrestadoService();
                    service.PreencherCampos(servicoPrestado);

                    _servico.Inserir(servicoPrestado);
                    return RedirectToAction(nameof(Index));
                }
                return View(servicoPrestado);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(servicoPrestado);
            }
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var servico = _servico.ObterPorId(id);
            if (servico == null)
                return NotFound();

            return View(servico);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ServicoPrestado servicoPrestado)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ServicoPrestadoService service = new ServicoPrestadoService();
                    service.ValidarCampos(servicoPrestado);

                    _servico.Atualizar(servicoPrestado);
                    return RedirectToAction(nameof(Index));
                }
                return View(servicoPrestado);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(servicoPrestado);
            }
        }

        
        public ActionResult Delete(int id)
        {
            var servico = _servico.ObterPorId(id);

            if (servico == null)
                return NotFound();

            return View(servico);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _servico.Desativar(id);
            return RedirectToAction(nameof(Index));
        }

        public bool VerificarServico(string descricao)
        {
            var result = _servico.ObterTodos().Any(x => x.Descricao.ToUpper() == descricao.ToUpper());
            return result;
        }
    }
}
