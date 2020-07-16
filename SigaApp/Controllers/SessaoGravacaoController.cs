using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using SigaApp.Servicos;

namespace SigaApp.Controllers
{
    [Authorize]
    public class SessaoGravacaoController : Controller
    {
        private readonly ISessaoGravacao _sessao;
        private readonly IEstudio _estudio;
        private readonly ICliente _cliente;
        private readonly IFornecedor _fornecedor;
        private readonly IServicoPrestado _servico;
                
        public SessaoGravacaoController(ISessaoGravacao sessao, IEstudio estudio, ICliente cliente, IFornecedor fornecedor, IServicoPrestado servico)
        {
            _sessao = sessao;
            _estudio = estudio;
            _cliente = cliente;
            _fornecedor = fornecedor;
            _servico = servico;
        }

        [TempData]
        public string Mensagem { get; set; }

        
        [HttpGet]
        public ActionResult Index(int txtEstudio, int? pagina)
        {
            try
            {
                CarregarEstudios();
                var gravacao = _sessao.ObterTodos().Where(x => x.EstudioID == txtEstudio);

                ViewData["FiltroAtual"] = txtEstudio;

                int pageSize = 10;
                return View(Paginacao<SessaoGravacao>.Create(gravacao, pagina ?? 1, pageSize));
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var sessao = _sessao.ObterPorId(id);
            if(sessao == null)
                return NotFound();

            return View(sessao);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new SessaoGravacao();
            model.DataInicio = DateTime.Now;
            model.DataFim = DateTime.Now;

            CarregarCampos();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SessaoGravacao sessaoGravacao)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    sessaoGravacao.EmpresaID = Convert.ToInt32(User.FindFirst(ClaimTypes.GroupSid).Value);

                    SessaoGravacaoService service = new SessaoGravacaoService();
                    service.PreencherCampos(sessaoGravacao);

                    _sessao.Inserir(sessaoGravacao);
                    return RedirectToAction(nameof(Index));
                }

                return View(sessaoGravacao);

            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return RedirectToAction(nameof(Create));
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            CarregarCampos();
            var sessao = _sessao.ObterPorId(id);
            
            if (sessao == null)
                return NotFound();

            return View(sessao);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, SessaoGravacao sessaoGravacao)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SessaoGravacaoService service = new SessaoGravacaoService();
                    service.ValidarCampos(sessaoGravacao);

                    _sessao.Atualizar(sessaoGravacao);
                    return RedirectToAction(nameof(Index));
                }

                return View(sessaoGravacao);

            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return RedirectToAction(nameof(Edit));
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var sessao = _sessao.ObterPorId(id);

            if (sessao == null)
                return NotFound();

            return View(sessao);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _sessao.Desativar(id);
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

        public IEnumerable<Fornecedor> CarregarFornecedores()
        {
            return ViewBag.ListaFornecedores = _fornecedor.ObterTodos();
        }

        public IEnumerable<ServicoPrestado> CarregarServicos()
        {
            return ViewBag.ListaServicos = _servico.ObterTodos();
        }

        public IEnumerable<SessaoGravacao> CarregarLista()
        {
            return _sessao.ObterTodos();
        }

        private void CarregarCampos()
        {
            CarregarEstudios();
            CarregarClientes();
            CarregarFornecedores();
            CarregarServicos();
        }
    }
}
