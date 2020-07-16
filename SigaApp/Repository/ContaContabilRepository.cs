using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SigaApp.Repository
{
    public class ContaContabilRepository : IContaContabil
    {
        private readonly SigaContext _contexto;
        private readonly IUsuario _usuario;

        public ContaContabilRepository(SigaContext contexto, IUsuario usuario)
        {
            _contexto = contexto;
            _usuario = usuario;
        }

        public void Atualizar(ContaContabil objeto)
        {
            _contexto.ContasContabeis.Update(objeto);
            Salvar();
        }

        public void Desativar(int id)
        {
            ContaContabil cc = _contexto.ContasContabeis.Find(id);
            cc.FlagAtivo = false;
            cc.DataExclusao = DateTime.Now;
            Salvar();
        }

        public void Dispose()
        {
            _contexto.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Excluir(int id)
        {
            ContaContabil cc = _contexto.ContasContabeis.Find(id);
            _contexto.ContasContabeis.Remove(cc);
        }

        public void Inserir(ContaContabil objeto)
        {
            _contexto.ContasContabeis.Add(objeto);
            Salvar();
        }

        public ContaContabil ObterPorId(int id)
        {
            return _contexto.ContasContabeis.Find(id);
        }

        public IEnumerable<ContaContabil> ObterTodos()
        {
            int id = _usuario.ObterEmpresa();
            return _contexto.ContasContabeis.ToList().Where(c => c.FlagAtivo == true && c.DataExclusao is null && c.EmpresaID == id).OrderBy(s => s.DataCadastro);
        }

        public void Salvar()
        {
            _contexto.SaveChanges();
        }
    }
}
