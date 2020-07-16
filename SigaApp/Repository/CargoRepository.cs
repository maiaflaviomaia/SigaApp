using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SigaApp.Repository
{
    public class CargoRepository : ICargo
    {
        private readonly SigaContext _contexto;
        private readonly IUsuario _usuario;

        public CargoRepository(SigaContext contexto, IUsuario usuario)
        {
            _contexto = contexto;
            _usuario = usuario;
        }

        public void Atualizar(Cargo objeto)
        {
            _contexto.Cargos.Update(objeto);
            Salvar();
        }

        public void Desativar(int id)
        {
            Cargo cs = _contexto.Cargos.Find(id);
            cs.DataExclusao = DateTime.Now;
            cs.FlagAtivo = false;
            Salvar();
        }

        public void Dispose()
        {
            _contexto.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Excluir(int id)
        {
            Cargo cs = _contexto.Cargos.Find(id);
            _contexto.Cargos.Remove(cs);
        }

        public void Inserir(Cargo objeto)
        {
            _contexto.Cargos.Add(objeto);
            Salvar();
        }

        public Cargo ObterPorId(int id)
        {
            return _contexto.Cargos.Find(id);
        }

        public IEnumerable<Cargo> ObterTodos()
        {
            var id = _usuario.ObterEmpresa();
            return _contexto.Cargos.ToList()
                .Where(s => s.FlagAtivo = true && s.DataExclusao is null && s.EmpresaID == id)
                .OrderBy(s => s.DescricaoSumaria);
        }

        public void Salvar()
        {
            _contexto.SaveChanges();
        }
    }
}
