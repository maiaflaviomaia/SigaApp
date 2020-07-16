using Microsoft.EntityFrameworkCore;
using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SigaApp.Repository
{
    public class FuncionarioRepository : IFuncionario
    {
        private readonly SigaContext _contexto;
        private readonly IUsuario _usuario;

        public FuncionarioRepository(SigaContext contexto, IUsuario usuario)
        {
            _contexto = contexto;
            _usuario = usuario;
        }

        public void Atualizar(Funcionario objeto)
        {
            _contexto.Funcionarios.Update(objeto);
            Salvar();
        }

        public void Desativar(int id)
        {
            Funcionario funcionario = _contexto.Funcionarios.Find(id);
            funcionario.DataExclusao = DateTime.Now;
            funcionario.FlagAtivo = false;
            Salvar();
        }

        public void Dispose()
        {
            _contexto.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Excluir(int id)
        {
            Funcionario funcionario = _contexto.Funcionarios.Find(id);
            _contexto.Funcionarios.Remove(funcionario);
        }

        public void Inserir(Funcionario objeto)
        {
            _contexto.Funcionarios.Add(objeto);
            Salvar();
        }

        public Funcionario ObterPorId(int id)
        {
            return _contexto.Funcionarios
                .Include(s => s.Endereco)
                .Include(s => s.DadosBancarios)
                .Include(s => s.Cargos)
                .FirstOrDefault(c => c.FuncionarioID == id);
        }

        public IEnumerable<Funcionario> ObterTodos()
        {
            int id = _usuario.ObterEmpresa();
            return _contexto.Funcionarios
                .Include(s => s.Endereco)
                .Include(s => s.DadosBancarios)
                .Include(s => s.Cargos)
                .ToList()
                .Where(s => s.FlagAtivo = true && s.DataExclusao is null && s.EmpresaID == id)
                .OrderBy(s => s.NomeCompleto);
        }

        public void Salvar()
        {
            _contexto.SaveChanges();
        }
    }
}
