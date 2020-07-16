using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using SigaApp.Servicos;

namespace SigaApp.Controllers
{
    [Authorize]
    public class CategoriaController : Controller
    {
        private readonly ICategoria _categoria;
        
        public CategoriaController(ICategoria categoria)
        {
            _categoria = categoria;
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

            var categoria = from cs in _categoria.ObterTodos() select cs;

            if (!String.IsNullOrEmpty(filtro))
            {
                categoria = categoria.Where(s => EF.Functions.Like(s.Nome, $"%{filtro}%")).ToList();
            }

            int pageSize = 10;
            return View(Paginacao<Categoria>.Create(categoria, pagina ?? 1, pageSize));
        }

        public ActionResult MontarTreeView()
        {
            List<TreeViewNode> nodes = new List<TreeViewNode>();

            var categorias = _categoria.ObterCategoriaPai();
            foreach (Categoria categoria in categorias)
            {
                nodes.Add(new TreeViewNode { id = categoria.CategoriaID.ToString(), parent = "#", text = categoria.Nome });
            }

            var subCategorias = _categoria.ObterTodos().Where(x => x.CategoriaPai != null);
            foreach (Categoria subcategoria in subCategorias)
            {
                nodes.Add(new TreeViewNode {id = subcategoria.CategoriaPai.ToString() + "-" + subcategoria.CategoriaID.ToString(), parent = subcategoria.CategoriaPai.ToString(), text = subcategoria.Nome });
            }

            ViewBag.Json = JsonConvert.SerializeObject(nodes);
            return View();
        }


        [HttpGet]
        public ActionResult Details(int id)
        {
            var categoria = _categoria.ObterPorId(id);

            if (categoria == null)
                return NotFound();

            return View(categoria);
        }


        [HttpGet]
        public IActionResult Create()
        {
            CarregarCategoriaPai();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Categoria categoria)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!String.IsNullOrEmpty(categoria.Nome))
                    {
                        if (VerificarCategoria(categoria.Nome))
                            throw new ArgumentException("Categoria já cadastrada");
                    }

                    categoria.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);

                    CategoriaService service = new CategoriaService();
                    service.PreencherCampos(categoria);

                    _categoria.Inserir(categoria);
                    return RedirectToAction(nameof(Index));
                }
                return View(categoria);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(categoria);
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            CarregarCategoriaPai();

            var categoria = _categoria.ObterPorId(id);
            
            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Categoria categoria)
        {
            if (categoria == null || id != categoria.CategoriaID)
                return NotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    CategoriaService service = new CategoriaService();
                    service.ValidarCampos(categoria);

                    _categoria.Atualizar(categoria);
                    return RedirectToAction(nameof(Index));
                }
                return View(categoria);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(categoria);
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var categoria = _categoria.ObterPorId(id);

            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _categoria.Desativar(id);
            return RedirectToAction(nameof(Index));
        }

        public IEnumerable<Categoria> CarregarCategoriaPai()
        {
            return ViewBag.ListaCategoriaPai = _categoria.ObterCategoriaPai();
        }

        public bool VerificarCategoria(string nome)
        {
            var result = _categoria.ObterTodos().Any(x => x.Nome.ToUpper() == nome.ToUpper());
            return result;
        }
    }
}
