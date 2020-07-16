using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using SigaApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SigaApp.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly IUsuario _usuario;
        private readonly IEmpresa _empresa;
        private readonly IEmail _email;
        
        public UsuarioController(IUsuario usuario, IEmpresa empresa, IEmail email, IHostingEnvironment env)
        {
            _usuario = usuario;
            _empresa = empresa;
            _email = email;
        }

        [TempData]
        public string MensagemLogin { get; set; }

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

            var usuario = from cs in _usuario.ObterTodos() select cs;

            if (!String.IsNullOrEmpty(filtro))
            {
                usuario = usuario.Where(s => EF.Functions.Like(s.Nome, $"%{filtro}%")).ToList();
            }

            int pageSize = 10;
            return View(Paginacao<Usuario>.Create(usuario, pagina ?? 1, pageSize));
        }

        [HttpGet]
        public ActionResult AlterarSenha(int id)
        {
            var usuario = _usuario.ObterPorId(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AlterarSenha(int id, Usuario usuario)
        {
            try
            {
                if (id != usuario.UsuarioID)
                    return NotFound();

                if (ModelState.IsValid)
                {
                    Criptografia crip = new Criptografia(SHA512.Create());

                    usuario.Senha = crip.CriptografarSenha(usuario.Senha);
                    _usuario.Atualizar(usuario);
                    return Redirect("/Home/Index");
                }
                return View(usuario);
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return View(usuario);
            }
        }

        [HttpGet]
        public ActionResult UsuarioLogado(int id, Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentException("Usuário inválido");

            if(id == 0)
            {
                return View(usuario);
            }
            else
            {
                var user = _usuario.ObterPorId(usuario.UsuarioID);
                //user.IsLogado = false;
                _usuario.Atualizar(user);
                HttpContext.SignOutAsync();
                return RedirectToAction(nameof(Login));
            }
        }
                

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/Home/Index");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(Usuario usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IEnumerable<Usuario> user = _usuario.ObterUsuario(usuario);
                    
                    if (user.Count() == 1)
                    {
                        usuario.UsuarioID = user.ElementAt(0).UsuarioID;
                        usuario.DataCadastro = user.ElementAt(0).DataCadastro;
                        usuario.Nome = user.ElementAt(0).Nome;
                        usuario.Email = user.ElementAt(0).Email;
                        usuario.Senha = user.ElementAt(0).Senha;
                        usuario.Perfil = user.ElementAt(0).Perfil;
                        usuario.EmpresaID = user.ElementAt(0).EmpresaID;
                        usuario.FlagAtivo = user.ElementAt(0).FlagAtivo;
                        usuario.DataExclusao = user.ElementAt(0).DataExclusao;
                        //usuario.IsLogado = user.ElementAt(0).IsLogado;

                        //if (usuario.IsLogado)
                        //    return RedirectToAction("UsuarioLogado", usuario);

                        //var usuarioAtualizar = _usuario.ObterPorId(usuario.UsuarioID);
                        //usuarioAtualizar.IsLogado = true;
                        //_usuario.Atualizar(usuarioAtualizar);

                        Autorizar(usuario);

                        return Redirect("/Home/Index");
                    }
                    else
                    {
                        MensagemLogin = "E-mail ou senha inválidos";
                        return RedirectToAction(nameof(Login));
                    }
                }
                else
                {
                    return View();
                }
            }
            catch (Exception)
            {
                MensagemLogin = "Erro ao tentar realizar o login.";
                return RedirectToAction(nameof(Login));
            }
        }

        private async void Autorizar(Usuario usuario)
        {
            var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Email, usuario.Email),
                        new Claim(ClaimTypes.Name, usuario.Nome),
                        new Claim(ClaimTypes.Role, usuario.Perfil),
                        new Claim(ClaimTypes.GroupSid, usuario.EmpresaID.ToString()),
                        new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString())
                    };

            var usuarioIdentidade = new ClaimsIdentity(claims, "login");

            ClaimsPrincipal principal = new ClaimsPrincipal(usuarioIdentidade);

            var propriedadesDeAutenticacao = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTime.Now.ToLocalTime().AddMinutes(30),
                IsPersistent = false
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, propriedadesDeAutenticacao);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var usuario = _usuario.ObterPorId(Convert.ToInt32(userId));
            //usuario.IsLogado = false;
            _usuario.Atualizar(usuario);
                        
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Authorize (Roles = "Master")]
        public ActionResult Create()
        {
            CarregarEmpresas();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Master")]
        public ActionResult Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                Criptografia crip = new Criptografia(SHA512.Create());

                usuario.DataCadastro = DateTime.Now;
                usuario.FlagAtivo = true;
                usuario.Senha = crip.CriptografarSenha(usuario.Senha);
                _usuario.Inserir(usuario);
                return Redirect("/Home/Index");
            }
            return View(usuario);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            id = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var usuario = _usuario.ObterPorId(id);

            if (usuario == null)
                return NotFound();
            
            return View(usuario);
        }

        [HttpGet]
        [Authorize(Roles = "Master")]
        public ActionResult Edit(int id)
        {
            var usuario = _usuario.ObterPorId(id);
            
            if (usuario == null)
                return NotFound();
            
            return View(usuario);
        }


        [HttpPost]
        [Authorize(Roles = "Master")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Usuario usuario)
        {
            if (id != usuario.UsuarioID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Criptografia crip = new Criptografia(SHA512.Create());
                
                usuario.Senha = crip.CriptografarSenha(usuario.Senha);
                _usuario.Atualizar(usuario);
                return Redirect("/Home/Index");
            }
            return View(usuario);
        }

        public IEnumerable<Empresa> CarregarEmpresas()
        {
            return ViewBag.ListaEmpresas = _empresa.ObterTodos();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult EsqueceuSenha()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult EsqueceuSenha(string txtEmail)
        {
            try
            {
                if (String.IsNullOrEmpty(txtEmail))
                    throw new ArgumentException("Informe o e-mail");

                var user = _usuario.ObterUsuarioPorEmail(txtEmail);
                var usuario = _usuario.ObterPorId(user.ElementAt(0).UsuarioID);

                if(user.Count() == 1)
                {
                    string novaSenhaGerada = Guid.NewGuid().ToString().Replace("-", "");

                    Criptografia crip = new Criptografia(SHA512.Create());

                    usuario.Senha = crip.CriptografarSenha(novaSenhaGerada);
                    _usuario.Atualizar(usuario);

                    EmailModel model = new EmailModel();
                    model.Destino = usuario.Email;
                    model.Assunto = "Siga - Redefinição de senha";
                    model.Mensagem = "Sua senha provisória é " + novaSenhaGerada + " Acesse seu perfil para redefinir sua senha";

                    EnvioDeEmail(model.Destino, model.Assunto, model.Mensagem).GetAwaiter();
                    return RedirectToAction(nameof(EnviadoComSucesso));
                }
                else
                {
                    MensagemLogin = "Usuário não encontrado";
                    return RedirectToAction(nameof(EsqueceuSenha));
                }
            }
            catch (Exception ex)
            {
                Mensagem = ex.Message.ToString();
                ModelState.AddModelError(String.Empty, Mensagem);
                return RedirectToAction(nameof(EsqueceuSenha));
            }
        }

        public async Task EnvioDeEmail(string email, string assunto, string mensagem)
        {
            try
            {
                await _email.EnviarEmailAsync(email, assunto, mensagem);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [AllowAnonymous]
        public ActionResult EnviadoComSucesso()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult FalhaAoEnviar()
        {
            return View();
        }
    }
}