using Microsoft.EntityFrameworkCore;
using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SigaApp.Repository
{
    public class OrcamentoCustoRepository : IOrcamentoCusto
    {
        private readonly SigaContext _contexto;
        private readonly IUsuario _usuario;

        public OrcamentoCustoRepository(SigaContext contexto, IUsuario usuario)
        {
            _contexto = contexto;
            _usuario = usuario;
        }

        public void Atualizar(OrcamentoCustos objeto)
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

        public void Inserir(OrcamentoCustos objeto)
        {
            _contexto.OrcamentoCustos.Add(objeto);
            Salvar();
        }

        public OrcamentoCustos ObterPorId(int id)
        {
            return _contexto.OrcamentoCustos
                .Include(s => s.Orcamento)
                .FirstOrDefault(s => s.OrcamentoCustoID == id);
        }

        public IEnumerable<OrcamentoCustos> ObterTodos()
        {
            int id = _usuario.ObterEmpresa();
            return _contexto.OrcamentoCustos
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
