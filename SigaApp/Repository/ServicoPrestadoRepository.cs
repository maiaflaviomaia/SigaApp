using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SigaApp.Repository
{
    public class ServicoPrestadoRepository : IServicoPrestado
    {
        private readonly SigaContext _contexto;
        private readonly IUsuario _usuario;

        public ServicoPrestadoRepository(SigaContext contexto, IUsuario usuario)
        {
            _contexto = contexto;
            _usuario = usuario;
        }

        public void Atualizar(ServicoPrestado objeto)
        {
            _contexto.ServicosPrestados.Update(objeto);
            Salvar();
        }

        public void Desativar(int id)
        {
            ServicoPrestado sp = _contexto.ServicosPrestados.Find(id);
            sp.DataExclusao = DateTime.Now;
            sp.FlagAtivo = false;
            Salvar();
        }

        public void Dispose()
        {
            _contexto.Dispose();
        }

        public void Excluir(int id)
        {
            ServicoPrestado sp = _contexto.ServicosPrestados.Find(id);
            _contexto.ServicosPrestados.Remove(sp);
        }

        public void Inserir(ServicoPrestado objeto)
        {
            _contexto.ServicosPrestados.Add(objeto);
            Salvar();
        }

        public ServicoPrestado ObterPorId(int id)
        {
            return _contexto.ServicosPrestados.Find(id);
        }

        public IEnumerable<ServicoPrestado> ObterTodos()
        {
            int id = _usuario.ObterEmpresa();
            return _contexto.ServicosPrestados.ToList().Where(s => s.FlagAtivo = true && s.DataExclusao is null && s.EmpresaID == id).OrderBy(s => s.Descricao);
        }

        public void Salvar()
        {
            _contexto.SaveChanges();
        }
    }
}
