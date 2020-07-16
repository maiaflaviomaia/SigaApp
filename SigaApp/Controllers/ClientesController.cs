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
    public class ClientesController : Controller
    {
        private readonly ICliente _cliente;
        
        public ClientesController(ICliente cliente)
        {
            _cliente = cliente;
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

            var cliente = from cs in _cliente.ObterTodos() select cs;

            if (!String.IsNullOrEmpty(filtro))
            {
                cliente = cliente.Where(s => EF.Functions.Like(s.RazaoSocial, $"%{filtro}%")).ToList();
            }

            int pageSize = 10;
            return View(Paginacao<Cliente>.Create(cliente, pagina ?? 1, pageSize));
        }


        public ActionResult Details(int id)
        {
            var cliente = _cliente.ObterPorId(id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        public JsonResult AdicionarCliente(string txtCliente)
        {
            if (!String.IsNullOrEmpty(txtCliente))
            {
                if (!_cliente.ObterTodos().Any(x => x.RazaoSocial.ToUpper() == txtCliente.ToUpper()))
                {
                    Cliente cliente = new Cliente();
                    cliente.RazaoSocial = txtCliente;
                    Create(cliente);
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
        public ActionResult Create(Cliente cliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if(!String.IsNullOrEmpty(cliente.CPF))
                    {
                        if (VerificarCPF(cliente.CPF))
                            throw new ArgumentException("CPF já está cadastrado");
                    }

                    if (!String.IsNullOrEmpty(cliente.CNPJ))
                    {
                        if (VerificarCNPJ(cliente.CNPJ))
                            throw new ArgumentException("CNPJ já cadastrado");
                    }

                    if (!String.IsNullOrEmpty(cliente.Email))
                    {
                        if (VerificarEmail(cliente.Email))
                            throw new ArgumentException("E-mail já cadastrado");
                    }

                    cliente.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);

                    ClienteService service = new ClienteService();
                    service.PreencherCampos(cliente);

                    _cliente.Inserir(cliente);
                    return RedirectToAction(nameof(Index));
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(cliente);
            }
            
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var cliente = _cliente.ObterPorId(id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Cliente cliente)
        {
            if(cliente == null || cliente.ClienteID != id)
                return NotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    ClienteService service = new ClienteService();
                    service.ValidarCampos(cliente);

                    _cliente.Atualizar(cliente);
                    return RedirectToAction(nameof(Index));
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(cliente);
            }
        }


        public ActionResult Delete(int id)
        {
            var cliente = _cliente.ObterPorId(id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _cliente.Desativar(id);
            return RedirectToAction(nameof(Index));
        }
        
        public bool VerificarCPF(string cpf)
        {
            TratarValores tratarValores = new TratarValores();
            cpf = tratarValores.TratarCPF(cpf);
            
            var result = _cliente.ObterTodos().Any(x => x.CPF == cpf);
            return result;
        }

        public bool VerificarCNPJ(string cnpj)
        {
            TratarValores tratarValores = new TratarValores();
            cnpj = tratarValores.TratarCNPJ(cnpj);
            
            var result = _cliente.ObterTodos().Any(x => x.CNPJ == cnpj);
            return result;
            
        }

        public bool VerificarEmail(string email)
        {
            var result = _cliente.ObterTodos().Any(x => x.Email == email);
            return result;
        }
    }
}
