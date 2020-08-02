using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;

namespace SigaApp.Repository
{
    public class LogUsuarioLogonRepository : ILogUsuarioLogon
    {
        private readonly SigaContext _contexto;
        private readonly IUsuario _usuario;

        public LogUsuarioLogonRepository(SigaContext contexto, IUsuario usuario)
        {
            _contexto = contexto;
            _usuario = usuario;
        }

        public void Dispose()
        {
            _contexto.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Inserir(LogUsuarioLogon log)
        {
            _contexto.LogUsuarios.Add(log);
            Salvar();
        }

        public void Salvar()
        {
            _contexto.SaveChanges();
        }
    }
}
