using Microsoft.EntityFrameworkCore;
using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SigaApp.Repository
{
    public class LancamentoRepository : ILancamento
    {
        private readonly SigaContext _contexto;
        private readonly IUsuario _usuario;

        public LancamentoRepository(SigaContext contexto, IUsuario usuario)
        {
            _contexto = contexto;
            _usuario = usuario;
        }

        public void Atualizar(Lancamento objeto)
        {
            _contexto.Lancamentos.Update(objeto);
            Salvar();
        }

        public void Desativar(int id)
        {
            Lancamento lancamento = _contexto.Lancamentos.Find(id);
            lancamento.FlagAtivo = false;
            lancamento.DataExclusao = DateTime.Now;
            Atualizar(lancamento);
        }

        public void Dispose()
        {
            _contexto.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Excluir(int id)
        {
            Lancamento lancamento = _contexto.Lancamentos.Find(id);
            _contexto.Lancamentos.Remove(lancamento);
        }

        public void Inserir(Lancamento objeto)
        {
            _contexto.Lancamentos.Add(objeto);
            Salvar();
        }

        public Lancamento ObterPorId(int id)
        {
            return _contexto.Lancamentos
                .Include(o => o.Categoria)
                .Include(o => o.SubCategoria)
                .Include(o => o.CentroCusto)
                .Include(o => o.Fornecedor)
                .Include(o => o.Cliente)
                .Include(o => o.ContaContabil)
                .AsNoTracking()
                .FirstOrDefault(o => o.LancamentoID == id);
        }

        public IEnumerable<Lancamento> ObterTodos()
        {
            int id = _usuario.ObterEmpresa();
            return _contexto.Lancamentos
                .Include(o => o.Categoria)
                .Include(o => o.SubCategoria)
                .Include(o => o.CentroCusto)
                .Include(o => o.Fornecedor)
                .Include(o => o.Cliente)
                .Include(o => o.ContaContabil)
                .OrderBy(o => o.DataLancamento)
                .ToList().Where(o => o.FlagAtivo == true && o.DataExclusao is null && o.EmpresaID == id);
        }

        public void Salvar()
        {
            _contexto.SaveChanges();
        }
    }
}
