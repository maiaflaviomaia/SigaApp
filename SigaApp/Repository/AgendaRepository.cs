using Microsoft.EntityFrameworkCore;
using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SigaApp.Repository
{
    public class AgendaRepository : IAgenda
    {
        private readonly SigaContext _contexto;
        private readonly IUsuario _usuario;

        public AgendaRepository(SigaContext contexto, IUsuario usuario)
        {
            _contexto = contexto;
            _usuario = usuario;
        }

        public void Atualizar(Agenda objeto)
        {
            _contexto.Agendas.Update(objeto);
            Salvar();
        }

        public void Desativar(int id)
        {
            var agenda = _contexto.Agendas.Find(id);
            agenda.FlagAtivo = false;
            agenda.DataExclusao = DateTime.Now;
            Atualizar(agenda);
        }

        public void Dispose()
        {
            _contexto.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Excluir(int id)
        {
            var agenda = _contexto.Agendas.Find(id);
            _contexto.Agendas.Remove(agenda);
        }

        public void Inserir(Agenda objeto)
        {
            _contexto.Agendas.Add(objeto);
            Salvar();
        }

        public Agenda ObterPorId(int id)
        {
            return _contexto.Agendas
                .Include(x => x.Cliente)
                .Include(x => x.Estudio)
                .FirstOrDefault(x => x.AgendaID == id);
        }

        public IEnumerable<Agenda> ObterTodos()
        {
            var id = _usuario.ObterEmpresa();
            return _contexto.Agendas
                .Include(x => x.Cliente)
                .Include(x => x.Estudio)
                .Where(x => x.FlagAtivo == true && x.DataExclusao == null && x.EmpresaID == id).ToList();
        }

        public void Salvar()
        {
            _contexto.SaveChanges();
        }
    }
}
