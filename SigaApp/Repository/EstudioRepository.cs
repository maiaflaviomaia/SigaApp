using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SigaApp.Repository
{
    public class EstudioRepository : IEstudio
    {
        private readonly SigaContext _contexto;
        private readonly IUsuario _usuario;

        public EstudioRepository(SigaContext contexto, IUsuario usuario)
        {
            _contexto = contexto;
            _usuario = usuario;
        }

        public void Atualizar(Estudio objeto)
        {
            _contexto.Estudios.Update(objeto);
            Salvar();
        }

        public void Desativar(int id)
        {
            Estudio estudio = _contexto.Estudios.Find(id);
            estudio.DataExclusao = DateTime.Now;
            estudio.FlagAtivo = false;
            Salvar();
        }

        public void Dispose()
        {
            _contexto.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Excluir(int id)
        {
            Estudio estudio = _contexto.Estudios.Find(id);
            _contexto.Estudios.Remove(estudio);
        }

        public void Inserir(Estudio objeto)
        {
            _contexto.Estudios.Add(objeto);
            Salvar();
        }

        public Estudio ObterPorId(int id)
        {
            return _contexto.Estudios.Find(id);
        }

        public IEnumerable<Estudio> ObterTodos()
        {
            int id = _usuario.ObterEmpresa();
            return _contexto.Estudios.ToList().Where(s => s.FlagAtivo = true && s.DataExclusao is null && s.EmpresaID == id).OrderBy(s => s.Nome);
        }

        public void Salvar()
        {
            _contexto.SaveChanges();
        }
    }
}
