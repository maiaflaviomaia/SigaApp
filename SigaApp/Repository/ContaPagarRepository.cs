using Microsoft.EntityFrameworkCore;
using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SigaApp.Repository
{
    public class ContaPagarRepository : IContaPagar
    {
        private readonly SigaContext _contexto;
        private readonly IUsuario _usuario;

        public ContaPagarRepository(SigaContext contexto, IUsuario usuario)
        {
            _contexto = contexto;
            _usuario = usuario;
        }

        public void Atualizar(ContaPagar objeto)
        {
            _contexto.ContasPagar.Update(objeto);
            Salvar();
        }

        public void Desativar(int id)
        {
            ContaPagar contas = _contexto.ContasPagar.Find(id);
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
            ContaPagar contas = _contexto.ContasPagar.Find(id);
            _contexto.ContasPagar.Remove(contas);
        }

        public void Inserir(ContaPagar objeto)
        {
            _contexto.ContasPagar.Add(objeto);
            Salvar();
        }

        public ContaPagar ObterPorId(int id)
        {
            return _contexto.ContasPagar
                .Include(o => o.Fornecedor)
                .Include(o => o.Categoria)
                .Include(o => o.SubCategoria)
                .Include(o => o.CentroDeCusto)
                .Include(o => o.Empresa)
                .AsNoTracking()
                .FirstOrDefault(c => c.ContasPagarID == id);
        }

        public IEnumerable<ContaPagar> ObterTodos()
        {
            int id = _usuario.ObterEmpresa();
            return _contexto.ContasPagar
                .Include(o => o.Fornecedor)
                .Include(o => o.Categoria)
                .Include(o => o.SubCategoria)
                .Include(o => o.CentroDeCusto)
                .Include(o => o.Empresa)
                .ToList().Where(c => c.FlagAtivo == true && c.DataExclusao is null && c.EmpresaID == id).OrderBy(c => c.DataVencimento);
        }

        public void Salvar()
        {
            _contexto.SaveChanges();
        }
    }
}
