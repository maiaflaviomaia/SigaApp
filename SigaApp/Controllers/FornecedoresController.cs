using System;
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
    public class FornecedoresController : Controller
    {
        private readonly IFornecedor _fornecedor;
        
        public FornecedoresController(IFornecedor fornecedor)
        {
            _fornecedor = fornecedor;
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

            var fornecedor = from cs in _fornecedor.ObterTodos() select cs;

            if (!String.IsNullOrEmpty(filtro))
            {
                fornecedor = fornecedor.Where(s => EF.Functions.Like(s.RazaoSocial, $"%{filtro}%")).ToList();
            }

            int pageSize = 10;
            return View(Paginacao<Fornecedor>.Create(fornecedor, pagina ?? 1, pageSize));
        }


        public ActionResult Details(int id)
        {
            var fornecedor = _fornecedor.ObterPorId(id);
            if (fornecedor == null)
                return NotFound();

            return View(fornecedor);
        }

        public JsonResult AdicionarFornecedor(string txtFornecedor)
        {
            if (!String.IsNullOrEmpty(txtFornecedor))
            {
                if(!_fornecedor.ObterTodos().Any(x => x.RazaoSocial.ToUpper() == txtFornecedor.ToUpper()))
                {
                    Fornecedor fornecedor = new Fornecedor();
                    fornecedor.RazaoSocial = txtFornecedor;
                    Create(fornecedor);
                    return Json(true);
                }
            }
            return Json(false);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Fornecedor fornecedor)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!String.IsNullOrEmpty(fornecedor.CPF))
                    {
                        if (VerificarCPF(fornecedor.CPF))
                            throw new ArgumentException("CPF já cadastrado");
                    }

                    if (!String.IsNullOrEmpty(fornecedor.CNPJ))
                    {
                        if (VerificarCNPJ(fornecedor.CNPJ))
                            throw new ArgumentException("CNPJ já cadastrado");
                    }

                    if (!String.IsNullOrEmpty(fornecedor.Email))
                    {
                        if (VerificarEmail(fornecedor.Email))
                            throw new ArgumentException("E-mail já cadastrado");
                    }

                    fornecedor.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);

                    FornecedorService service = new FornecedorService();
                    service.PreencherCampos(fornecedor);

                    _fornecedor.Inserir(fornecedor);
                    return RedirectToAction(nameof(Index));
                }
                return View(fornecedor);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(fornecedor);
            }
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var fornecedor = _fornecedor.ObterPorId(id);

            if (fornecedor == null)
                return NotFound();

            return View(fornecedor);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Fornecedor fornecedor)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    FornecedorService service = new FornecedorService();
                    service.ValidarCampos(fornecedor);

                    _fornecedor.Atualizar(fornecedor);
                    return RedirectToAction(nameof(Index));
                }
                return View(fornecedor);

            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(fornecedor);
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var fornecedor = _fornecedor.ObterPorId(id);

            if (fornecedor == null)
                return NotFound();

            return View(fornecedor);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _fornecedor.Desativar(id);
            return RedirectToAction(nameof(Index));
        }

        public bool VerificarCPF(string cpf)
        {
            TratarValores tratarValores = new TratarValores();
            cpf = tratarValores.TratarCPF(cpf);

            var result = _fornecedor.ObterTodos().Any(x => x.CPF == cpf);
            return result;
        }

        public bool VerificarCNPJ(string cnpj)
        {
            TratarValores tratarValores = new TratarValores();
            cnpj = tratarValores.TratarCNPJ(cnpj);

            var result = _fornecedor.ObterTodos().Any(x => x.CNPJ == cnpj);
            return result;

        }

        public bool VerificarEmail(string email)
        {
            var result = _fornecedor.ObterTodos().Any(x => x.Email == email);
            return result;
        }
    }
}
