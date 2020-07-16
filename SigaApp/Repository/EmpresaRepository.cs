using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SigaApp.Repository
{
    public class EmpresaRepository : IEmpresa
    {
        private readonly SigaContext _contexto;

        public EmpresaRepository(SigaContext contexto)
        {
            _contexto = contexto;
        }

        public void Atualizar(Empresa objeto)
        {
            _contexto.Empresas.Update(objeto);
            Salvar();
        }

        public void Desativar(int id)
        {
            Empresa empresa = _contexto.Empresas.Find(id);
            empresa.FlagAtivo = false;
            empresa.DataExclusao = DateTime.Now;
            Salvar();
        }

        public void Excluir(int id)
        {
            Empresa empresa = _contexto.Empresas.Find(id);
            _contexto.Empresas.Remove(empresa);
        }

        public void Dispose()
        {
            _contexto.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Inserir(Empresa objeto)
        {
            _contexto.Empresas.Add(objeto);
            Salvar();
        }

        public Empresa ObterPorId(int id)
        {
            return _contexto.Empresas.Find(id);
        }

        public IEnumerable<Empresa> ObterPorNome(string nome)
        {
            return _contexto.Empresas.ToList().Where(e => e.RazaoSocial == nome);
        }

        public IEnumerable<Empresa> ObterTodos()
        {
            return _contexto.Empresas.ToList();
        }

        public void Salvar()
        {
            _contexto.SaveChanges();
        }
    }
}
