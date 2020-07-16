using Microsoft.EntityFrameworkCore;
using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SigaApp.Repository
{
    public class ContaReceberRepository : IContaReceber
    {
        private readonly SigaContext _contexto;
        private readonly IUsuario _usuario;

        public ContaReceberRepository(SigaContext contexto, IUsuario usuario)
        {
            _contexto = contexto;
            _usuario = usuario;
        }

        public void Atualizar(ContaReceber objeto)
        {
            _contexto.ContasRebecer.Update(objeto);
            Salvar();
        }

        public void Desativar(int id)
        {
            ContaReceber contas = _contexto.ContasRebecer.Find(id);
            contas.FlagAtivo = false;
            contas.DataExclusao = DateTime.Now;
            Salvar();
        }

        public void Dispose()
        {
            _contexto.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Excluir(int id)
        {
            ContaReceber contas = _contexto.ContasRebecer.Find(id);
            _contexto.ContasRebecer.Remove(contas);
        }

        public void Inserir(ContaReceber objeto)
        {
            _contexto.ContasRebecer.Add(objeto);
            Salvar();
        }

        public ContaReceber ObterPorId(int id)
        {
            return _contexto.ContasRebecer
                .Include(o => o.Cliente)
                .Include(o => o.Categoria)
                .Include(o => o.SubCategoria)
                .Include(o => o.CentroDeCusto)
                .Include(o => o.ContaContabil)
                .Include(o => o.Empresa)
                .AsNoTracking()
                .FirstOrDefault(c => c.ContaReceberID == id);
        }

        public IEnumerable<ContaReceber> ObterTodos()
        {
            int id = _usuario.ObterEmpresa();
            return _contexto.ContasRebecer
                .Include(o => o.Cliente)
                .Include(o => o.Categoria)
                .Include(o => o.SubCategoria)
                .Include(o => o.CentroDeCusto)
                .Include(o => o.ContaContabil)
                .Include(o => o.Empresa)
                .ToList().Where(c => c.FlagAtivo == true && c.DataExclusao is null && c.EmpresaID == id).OrderBy(c => c.DataVencimento);
        }

        public void Salvar()
        {
            _contexto.SaveChanges();
        }
    }
}
