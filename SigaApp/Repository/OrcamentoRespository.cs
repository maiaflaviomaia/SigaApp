using Microsoft.EntityFrameworkCore;
using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using static SigaApp.Utils.Enums;

namespace SigaApp.Repository
{
    public class OrcamentoRepository : IOrcamento
    {
        private readonly SigaContext _contexto;
        private readonly IUsuario _usuario;

        public OrcamentoRepository(SigaContext contexto, IUsuario usuario)
        {
            _contexto = contexto;
            _usuario = usuario;
        }

        public void Atualizar(Orcamento objeto)
        {
            _contexto.Orcamentos.Update(objeto);
            Salvar();
        }

        public void Desativar(int id)
        {
            Orcamento orc = _contexto.Orcamentos.Find(id);
            orc.StatusOrcamento = StatusOrcamento.Cancelado;
            Salvar();
        }

        public void Dispose()
        {
            _contexto.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Excluir(int id)
        {
            Orcamento orc = _contexto.Orcamentos.Find(id);
            _contexto.Orcamentos.Remove(orc);
        }

        public void Inserir(Orcamento objeto)
        {
            _contexto.Orcamentos.Add(objeto);
            Salvar();
        }

        public Orcamento ObterPorId(int id)
        {
            return _contexto.Orcamentos
                .Include(o => o.Cliente)
                .Include(o => o.OrcamentoServicos).ThenInclude(s => s.ServicoPrestado)
                .Include(o => o.OrcamentoFornecedores).ThenInclude(s => s.Fornecedor)
                .Include(o => o.OrcamentoCustos)
                .Include(o => o.Empresa)
                .AsNoTracking()
                .FirstOrDefault(o => o.OrcamentoID == id);
        }

        public IEnumerable<Orcamento> ObterTodos()
        {
            int id = _usuario.ObterEmpresa();
            return _contexto.Orcamentos
                .Include(o => o.Cliente)
                .Include(o => o.Empresa)
                .ToList().Where(o => o.FlagAtivo == true && o.DataExclusao is null && o.EmpresaID == id).OrderBy(o => o.DataCadastro);
        }

        public void Salvar()
        {
            _contexto.SaveChanges();
        }
    }
}
