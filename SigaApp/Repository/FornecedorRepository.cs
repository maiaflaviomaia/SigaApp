using Microsoft.EntityFrameworkCore;
using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SigaApp.Repository
{
    public class FornecedorRepository : IFornecedor
    {
        private readonly SigaContext _contexto;
        private readonly IUsuario _usuario;

        public FornecedorRepository(SigaContext contexto, IUsuario usuario)
        {
            _contexto = contexto;
            _usuario = usuario;
        }

        public void Atualizar(Fornecedor objeto)
        {
            _contexto.Fornecedores.Update(objeto);
            Salvar();
        }

        public void Desativar(int id)
        {
            Fornecedor fornecedor = _contexto.Fornecedores.Find(id);
            fornecedor.DataExclusao = DateTime.Now;
            fornecedor.FlagAtivo = false;
            Salvar();
        }

        public void Dispose()
        {
            _contexto.Dispose();
        }

        public void Excluir(int id)
        {
            Fornecedor fornecedor = _contexto.Fornecedores.Find(id);
            _contexto.Fornecedores.Remove(fornecedor);
        }

        public void Inserir(Fornecedor objeto)
        {
            _contexto.Fornecedores.Add(objeto);
            Salvar();
        }

        public Fornecedor ObterPorId(int id)
        {
            return _contexto.Fornecedores
                .Include(s => s.Endereco)
                .Include(s => s.DadosBancarios).FirstOrDefault(c => c.FornecedorID == id);
        }

        public IEnumerable<Fornecedor> ObterTodos()
        {
            int id = _usuario.ObterEmpresa();
            return _contexto.Fornecedores
                .Include(s => s.Endereco)
                .Include(s => s.DadosBancarios)
                .ToList().Where(s => s.FlagAtivo = true && s.DataExclusao is null && s.EmpresaID == id).OrderBy(s => s.RazaoSocial);
        }

        public void Salvar()
        {
            _contexto.SaveChanges();
        }
    }
}
