using Microsoft.EntityFrameworkCore;
using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SigaApp.Repository
{
    public class OrcamentoServicoRepository : IOrcamentoServico
    {
        private readonly SigaContext _contexto;
        private readonly IUsuario _usuario;

        public OrcamentoServicoRepository(SigaContext contexto, IUsuario usuario)
        {
            _contexto = contexto;
            _usuario = usuario;
        }

        public void Atualizar(OrcamentoServico objeto)
        {
            _contexto.OrcamentoServicos.Update(objeto);
            Salvar();
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

        public void Inserir(OrcamentoServico objeto)
        {
            //string query = "INSERT INTO ORCAMENTO_PUBLICIDADE_SERVICO (EmpresaID, OrcamentoID, ServicoPrestadoID, Quantidade, ValorTotal) VALUES ({0}, {1}, {2}, {3}, {4})";
            //_contexto.Database.ExecuteSqlCommand(query, objeto.EmpresaID, objeto.OrcamentoID, objeto.ServicoPrestadoID, objeto.Quantidade, objeto.ValorTotal);
            _contexto.OrcamentoServicos.Add(objeto);
            Salvar();
        }

        public OrcamentoServico ObterPorId(int id)
        {
            return _contexto.OrcamentoServicos
                .Include(s => s.Orcamento)
                .FirstOrDefault(s => s.OrcamentoServicoID == id);
        }

        public IEnumerable<OrcamentoServico> ObterTodos()
        {
            int id = _usuario.ObterEmpresa();
            return _contexto.OrcamentoServicos
                .Include(s => s.Orcamento)
                .Where(s => s.EmpresaID == id && s.OrcamentoID == s.Orcamento.OrcamentoID)
                .ToList();
        }

        public void Salvar()
        {
            _contexto.SaveChanges();
        }
    }
}
