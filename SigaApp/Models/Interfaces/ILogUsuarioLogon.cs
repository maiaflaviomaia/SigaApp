using SigaApp.Models.Entidades;
using System;

namespace SigaApp.Models.Interfaces
{
    public interface ILogUsuarioLogon : IDisposable
    {
        void Inserir(LogUsuarioLogon log);
        void Salvar();
    }
}
