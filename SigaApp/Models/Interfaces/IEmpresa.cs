using SigaApp.Models.Entidades;
using System;
using System.Collections.Generic;

namespace SigaApp.Models.Interfaces
{
    public interface IEmpresa : IDisposable
    {
        void Inserir(Empresa objeto);
        void Atualizar(Empresa objeto);
        void Desativar(int id);
        void Excluir(int id);
        void Salvar();
        IEnumerable<Empresa> ObterTodos();
        Empresa ObterPorId(int id);
        IEnumerable<Empresa> ObterPorNome(string nome);
    }
}
