using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using SigaApp.Servicos;
using SigaApp.Utils;

namespace SigaApp.Controllers
{
    [Authorize]
    public class FuncionariosController : Controller
    {
        private readonly IFuncionario _funcionario;
        private readonly ICargo _cargo;
        
        public FuncionariosController(IFuncionario funcionario, ICargo cargo)
        {
            _funcionario = funcionario;
            _cargo = cargo;
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

            var funcionario = from cs in _funcionario.ObterTodos() select cs;

            if (!String.IsNullOrEmpty(filtro))
            {
                funcionario = funcionario.Where(s => EF.Functions.Like(s.NomeCompleto, $"%{filtro}%")).ToList();
            }

            int pageSize = 10;
            return View(Paginacao<Funcionario>.Create(funcionario, pagina ?? 1, pageSize));
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var funcionario = _funcionario.ObterPorId(id);

            if (funcionario == null)
                return NotFound();

            return View(funcionario);
        }

        [HttpGet]
        public IActionResult Create()
        {
            CarregarCargos();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Funcionario funcionario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!String.IsNullOrEmpty(funcionario.CPF))
                    {
                        if (VerificarCPF(funcionario.CPF))
                            throw new ArgumentException("CPF já cadastrado");
                    }

                    if (!String.IsNullOrEmpty(funcionario.Email))
                    {
                        if (VerificarEmail(funcionario.Email))
                            throw new ArgumentException("E-mail já cadastrado");
                    }

                    funcionario.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);

                    FuncionarioService service = new FuncionarioService();
                    service.PreencherCampos(funcionario);

                    _funcionario.Inserir(funcionario);
                    return RedirectToAction(nameof(Index));
                }
                return View(funcionario);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(funcionario);
            }
            
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var funcionario = _funcionario.ObterPorId(id);
            
            if (funcionario == null)
                return NotFound();

            CarregarCargos();
            return View(funcionario);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Funcionario funcionario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    FuncionarioService service = new FuncionarioService();
                    service.ValidarCampos(funcionario);

                    _funcionario.Atualizar(funcionario);
                    return RedirectToAction(nameof(Index));
                }
                return View(funcionario);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(funcionario);
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var funcionario = _funcionario.ObterPorId(id);

            if (funcionario == null)
                return NotFound();

            return View(funcionario);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _funcionario.Desativar(id);
            return RedirectToAction(nameof(Index));
        }

        public IEnumerable<Cargo> CarregarCargos()
        {
           return ViewBag.ListaCargos = _cargo.ObterTodos(); 
        }

        public bool VerificarCPF(string cpf)
        {
            TratarValores tratarValores = new TratarValores();
            cpf = tratarValores.TratarCPF(cpf);

            var result = _funcionario.ObterTodos().Any(x => x.CPF == cpf);
            return result;
        }
                

        public bool VerificarEmail(string email)
        {
            var result = _funcionario.ObterTodos().Any(x => x.Email == email);
            return result;
        }
    }
}
