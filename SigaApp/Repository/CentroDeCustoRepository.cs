using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SigaApp.Repository
{
    public class CentroDeCustoRepository : ICentroDeCusto
    {
        private readonly SigaContext _contexto;
        private readonly IUsuario _usuario;

        public CentroDeCustoRepository(SigaContext contexto, IUsuario usuario)
        {
            _contexto = contexto;
            _usuario = usuario;
        }

        public void Atualizar(CentroDeCusto objeto)
        {
            _contexto.CentroDeCustos.Update(objeto);
            Salvar();
        }

        public void Desativar(int id)
        {
            var centro = _contexto.CentroDeCustos.Find(id);
            centro.FlagAtivo = false;
            centro.DataExclusao = DateTime.Now;
            Atualizar(centro);
        }

        public void Dispose()
        {
            _contexto.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Excluir(int id)
        {
            var centro = _contexto.CentroDeCustos.Find(id);
            _contexto.CentroDeCustos.Remove(centro);
        }

        public void Inserir(CentroDeCusto objeto)
        {
            _contexto.CentroDeCustos.Add(objeto);
            Salvar();
        }

        public CentroDeCusto ObterPorId(int id)
        {
            return _contexto.CentroDeCustos.Find(id);
        }

        public IEnumerable<CentroDeCusto> ObterTodos()
        {
            var id = _usuario.ObterEmpresa();
            return _contexto.CentroDeCustos.ToList().Where(c => c.FlagAtivo == true && c.DataExclusao is null && c.EmpresaID == id);
        }

        public void Salvar()
        {
            _contexto.SaveChanges();
        }
    }
}
