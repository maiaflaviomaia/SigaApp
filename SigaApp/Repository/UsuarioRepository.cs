using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using SigaApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SigaApp.Repository
{
    public class UsuarioRepository : IUsuario
    {
        private readonly SigaContext _contexto;
        private readonly IHttpContextAccessor _accessor;

        public UsuarioRepository(SigaContext contexto, IHttpContextAccessor accessor)
        {
            _contexto = contexto;
            _accessor = accessor;
        }

        public void Inserir(Usuario usuario)
        {
            _contexto.Usuarios.Add(usuario);
            Salvar();
        }

        public void Atualizar(Usuario usuario)
        {
            _contexto.Usuarios.Update(usuario);
            Salvar();
        }

        public void Dispose()
        {
            _contexto.Dispose();
            GC.SuppressFinalize(this);
        }

        public Usuario ObterPorId(int id)
        {
            return _contexto.Usuarios
                .Include(x => x.Empresa)
                .AsNoTracking()
                .FirstOrDefault(x => x.UsuarioID == id);
        }

        public IEnumerable<Usuario> ObterUsuario(Usuario usuario)
        {
            try
            {
                Criptografia crip = new Criptografia(SHA512.Create());

                usuario.Senha = crip.CriptografarSenha(usuario.Senha);
                return _contexto.Usuarios.Where(x => x.Email == usuario.Email && x.Senha == usuario.Senha).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Salvar()
        {
            _contexto.SaveChanges();
        }

        public IEnumerable<Claim> ObterClaims()
        {
            return _accessor.HttpContext.User.Claims;
        }

        public int ObterEmpresa()
        {
            return Convert.ToInt32(_accessor.HttpContext.User.FindFirst(ClaimTypes.GroupSid)?.Value);
        }

        public IEnumerable<Usuario> ObterUsuarioPorEmail(string email)
        {
            return _contexto.Usuarios.Where(s => s.Email == email);
        }

        public IEnumerable<Usuario> ObterTodos()
        {
            return _contexto.Usuarios.Include(x => x.Empresa).ToList();
        }
    }
}
