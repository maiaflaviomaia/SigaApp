using System;
using System.Collections.Generic;

namespace SigaApp.Models.Interfaces
{
    public interface IRepository<T> : IDisposable where T : class
    {
        void Inserir(T objeto);
        void Atualizar(T objeto);
        void Desativar(int id);
        void Excluir(int id);
        void Salvar();
        IEnumerable<T> ObterTodos();
        T ObterPorId(int id);
    }
}
