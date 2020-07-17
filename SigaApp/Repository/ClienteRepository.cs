using Microsoft.EntityFrameworkCore;
using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SigaApp.Repository
{
    public class ClienteRepository : ICliente
    {
        private readonly SigaContext _contexto;
        private readonly IUsuario _usuario;

        public ClienteRepository(SigaContext contexto, IUsuario usuario)
        {
            _contexto = contexto;
            _usuario = usuario;
        }

        public void Atualizar(Cliente objeto)
        {
            _contexto.Clietes.Update(objeto);
            Salvar();
        }

        public void Desativar(int id)
        {
            Cliente cliente = _contexto.Clietes.Find(id);
            cliente.DataExclusao = DateTime.Now;
            cliente.FlagAtivo = false;
            Salvar();
        }

        public void Dispose()
        {
            _contexto.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Excluir(int id)
        {
            Cliente cliente = _contexto.Clietes.Find(id);
            _contexto.Clietes.Remove(cliente);
        }

        public void Inserir(Cliente objeto)
        {
            _contexto.Clietes.Add(objeto);
            Salvar();
        }

        public Cliente ObterPorId(int id)
        {
            return _contexto.Clietes.FirstOrDefault(c => c.ClienteID == id);
        }

        public IEnumerable<Cliente> ObterTodos()
        {
            int id = _usuario.ObterEmpresa();
            return _contexto.Clietes
                .ToList()
                .Where(s => s.FlagAtivo = true && s.DataExclusao is null && s.EmpresaID == id)
                .OrderBy(s => s.RazaoSocial);
        }

        public void Salvar()
        {
            _contexto.SaveChanges();
        }
    }
}
