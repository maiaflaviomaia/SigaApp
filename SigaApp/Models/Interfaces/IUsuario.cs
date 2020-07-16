using SigaApp.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace SigaApp.Models.Interfaces
{
    public interface IUsuario : IDisposable
    {
        void Inserir(Usuario usuario);
        void Atualizar(Usuario usuario);
        void Salvar();
        IEnumerable<Usuario> ObterUsuario(Usuario usuario);
        IEnumerable<Usuario> ObterTodos();
        Usuario ObterPorId(int id);
        IEnumerable<Claim> ObterClaims();
        int ObterEmpresa();
        IEnumerable<Usuario> ObterUsuarioPorEmail(string email);
    }
}
