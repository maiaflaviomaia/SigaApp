using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using SigaApp.Utils;

namespace SigaApp.Controllers
{
    [Authorize]
    public class EmpresasController : Controller
    {
        private readonly IEmpresa _empresa;

        public EmpresasController(IEmpresa empresa)
        {
            _empresa = empresa;
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

            var empresa = from cs in _empresa.ObterTodos() select cs;

            if (!String.IsNullOrEmpty(filtro))
            {
                empresa = empresa.Where(s => EF.Functions.Like(s.RazaoSocial, $"%{filtro}%")).ToList();
            }

            int pageSize = 10;
            return View(Paginacao<Empresa>.Create(empresa, pagina ?? 1, pageSize));
        }

        
        public ActionResult Details(int id)
        {
            var empresa = _empresa.ObterPorId(id);
            
            if (empresa == null)
                return NotFound();

            return View(empresa);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Empresa empresa)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TratarValores tratarValores = new TratarValores();

                    empresa.DataCadastro = DateTime.Now;
                    empresa.FlagAtivo = true;
                    empresa.DataExclusao = null;
                    empresa.CNPJ = tratarValores.TratarCNPJ(empresa.CNPJ);

                    _empresa.Inserir(empresa);
                    return RedirectToAction(nameof(Index));
                }
                return View(empresa);
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao tentar criar empresa - " + ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(empresa);
            }
        }

        public ActionResult Edit(int id)
        {
            var empresa = _empresa.ObterPorId(id);

            if (empresa == null)
                return NotFound();
            
            return View(empresa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Empresa empresa)
        {
            if (empresa == null || id != empresa.EmpresaID)
                return NotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    _empresa.Atualizar(empresa);
                    return RedirectToAction(nameof(Index));
                }
                return View(empresa);
            }
            catch (Exception ex)
            {
                Mensagem = "Erro ao tentar editar empresa - " + ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(empresa);
            }
        }

        public ActionResult Delete(int id)
        {
            var empresa = _empresa.ObterPorId(id);

            if (empresa == null)
                return NotFound();

            return View(empresa);
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _empresa.Desativar(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
