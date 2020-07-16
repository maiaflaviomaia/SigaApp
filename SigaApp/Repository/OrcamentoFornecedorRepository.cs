using Microsoft.EntityFrameworkCore;
using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SigaApp.Repository
{
    public class OrcamentoFornecedorRepository : IOrcamentoFornecedor
    {
        private readonly SigaContext _contexto;
        private readonly IUsuario _usuario;

        public OrcamentoFornecedorRepository(SigaContext contexto, IUsuario usuario)
        {
            _contexto = contexto;
            _usuario = usuario;
        }

        public void Atualizar(OrcamentoFornecedor objeto)
        {
            throw new NotImplementedException();
        }

        public void Desativar(int id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _contexto.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Excluir(int id)
        {
            throw new NotImplementedException();
        }

        public void Inserir(OrcamentoFornecedor objeto)
        {
            _contexto.OrcamentoFornecedores.Add(objeto);
            Salvar();
        }

        public OrcamentoFornecedor ObterPorId(int id)
        {
            return _contexto.OrcamentoFornecedores
                .Include(s => s.Orcamento)
                .FirstOrDefault(s => s.OrcamentoFornecedorID == id);
        }

        public IEnumerable<OrcamentoFornecedor> ObterTodos()
        {
            int id = _usuario.ObterEmpresa();
            return _contexto.OrcamentoFornecedores
                .Include(s => s.Orcamento)
                .Where(x => x.EmpresaID == id && x.OrcamentoID == x.Orcamento.OrcamentoID)
                .ToList();
        }

        public IEnumerable<OrcamentoFornecedor> ObterFornecedores(int orcamentoID)
        {
            int id = _usuario.ObterEmpresa();
            return _contexto.OrcamentoFornecedores
                .Include(x => x.Orcamento)
                .Where(x => x.EmpresaID == id && x.OrcamentoID == orcamentoID)
                .ToList();
        }

        public void Salvar()
        {
            _contexto.SaveChanges();
        }
    }
}
