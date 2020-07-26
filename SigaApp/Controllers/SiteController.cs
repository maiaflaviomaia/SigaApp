using System;
using System.Net;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Razor;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using SigaApp.Utils;

namespace SigaApp.Controllers
{
    public class SiteController : Controller
    {
        private readonly IMensagemSite _mensagem;
        private readonly IUsuario _usuario;
        private readonly IEmpresa _empresa;

        public SiteController(IMensagemSite mensagem, IUsuario usuario, IEmpresa empresa)
        {
            _mensagem = mensagem;
            _usuario = usuario;
            _empresa = empresa;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult PaginaInicial()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult EnviarMensagem(string txtNome, string txtEmail, string txtMensagem)
        {
            if (!String.IsNullOrEmpty(txtNome) && !String.IsNullOrEmpty(txtEmail) && !String.IsNullOrEmpty(txtMensagem))
            {
                IPAddress ip = Request.HttpContext.Connection.RemoteIpAddress;

                MensagemSite mensagem = new MensagemSite();
                mensagem.DataCadastro = DateTime.Now;
                mensagem.Nome = txtNome;
                mensagem.Email = txtEmail;
                mensagem.Mensagem = txtMensagem;
                mensagem.IPUsuario = ip.ToString();

                _mensagem.Inserir(mensagem);

                return RedirectToAction(nameof(EnviarMensagem));
            }
            return RedirectToAction(nameof(EnviarMensagem));
        }

        [HttpGet]
        public ActionResult Funcionalidades()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Contrate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contrate(Usuario usuario, string txtEmpresa, string txtDocumento, string txtTelefone)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Empresa empresa = new Empresa();
                    empresa.DataCadastro = DateTime.Now;
                    empresa.DataExclusao = null;
                    empresa.FlagAtivo = true;
                    empresa.RazaoSocial = txtEmpresa;
                    empresa.NomeFantasia = txtEmpresa;
                    empresa.CNPJ = txtDocumento;
                    empresa.TelefoneFixo = txtTelefone;
                    empresa.TelefoneCelular = null;
                    empresa.Responsavel = usuario.Nome;
                    empresa.Email = usuario.Email;
                    empresa.Endereco = null;
                    empresa.DadosBancarios = null;

                    _empresa.Inserir(empresa);

                    Criptografia crip = new Criptografia(SHA512.Create());
                    usuario.DataCadastro = DateTime.Now;
                    usuario.DataExclusao = null;
                    usuario.DataLimiteTeste = DateTime.Now.AddDays(15);
                    usuario.FlagAtivo = true;
                    usuario.Perfil = "Admin";
                    usuario.EmpresaID = empresa.EmpresaID;
                    usuario.Senha = crip.CriptografarSenha(usuario.Senha);
                    
                    _usuario.Inserir(usuario);

                    return RedirectToAction(nameof(PaginaInicial));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(String.Empty, ex.Message.ToString());
                    return View();
                }
            }
            return View();
        }
    }
}