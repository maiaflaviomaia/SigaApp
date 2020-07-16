using Microsoft.EntityFrameworkCore;
using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SigaApp.Repository
{
    public class CategoriaRepository : ICategoria
    {
        private readonly SigaContext _context;
        private readonly IUsuario _usuario;

        public CategoriaRepository(SigaContext contexto, IUsuario usuario)
        {
            _context = contexto;
            _usuario = usuario;
        }

        public void Atualizar(Categoria objeto)
        {
            _context.Categorias.Update(objeto);
            Salvar();
        }

        public void Desativar(int id)
        {
            Categoria categoria = _context.Categorias.Find(id);
            categoria.FlagAtivo = false;
            categoria.DataExclusao = DateTime.Now;
            Salvar();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Excluir(int id)
        {
            Categoria cf = _context.Categorias.Find(id);
            _context.Categorias.Remove(cf);
        }

        public void Inserir(Categoria objeto)
        {
            _context.Categorias.Add(objeto);
            Salvar();
        }

        public Categoria ObterPorId(int id)
        {
            return _context.Categorias
                .Include(s => s.SubCategoria)
                .FirstOrDefault(s => s.CategoriaID == id);
        }

        public IEnumerable<Categoria> ObterTodos()
        {
            int id = _usuario.ObterEmpresa();
            return _context.Categorias
                .Include(s => s.SubCategoria)
                .AsNoTracking()
                .ToList().Where(s => s.FlagAtivo = true && s.DataExclusao is null && s.EmpresaID == id).OrderBy(s => s.Nome);
        }

        public IEnumerable<Categoria> ObterCategoriaPai()
        {
            int id = _usuario.ObterEmpresa();
            return _context.Categorias
                .Include(s => s.SubCategoria)
                .AsNoTracking()
                .ToList().Where(s => s.FlagAtivo = true && s.DataExclusao is null && s.EmpresaID == id && s.CategoriaPai is null).OrderBy(s => s.Nome);
        }

        public IEnumerable<Categoria> ObterTodasDespesas()
        {
            int id = _usuario.ObterEmpresa();
            return _context.Categorias
                .Include(s => s.SubCategoria)
                .AsNoTracking()
                .ToList().Where(s => s.FlagAtivo = true && s.DataExclusao is null && s.EmpresaID == id && s.Tipo == "Despesas" && s.CategoriaPai is null).OrderBy(s => s.Nome);
        }

        public IEnumerable<Categoria> ObterTodasReceitas()
        {
            int id = _usuario.ObterEmpresa();
            return _context.Categorias
                .Include(s => s.SubCategoria)
                .AsNoTracking()
                .ToList().Where(s => s.FlagAtivo = true && s.DataExclusao is null && s.EmpresaID == id && s.Tipo == "Receitas" && s.CategoriaPai is null).OrderBy(s => s.Nome);
        }

        public IEnumerable<Categoria> ObterSubCategorias(int id)
        {
            int idUser = _usuario.ObterEmpresa();
            var lista = _context.Categorias
            .Include(s => s.SubCategoria)
            .AsNoTracking()
            .ToList().Where(s => s.CategoriaPai == id && s.EmpresaID == idUser).OrderBy(s => s.Nome);
            return lista;
        }

        public void Salvar()
        {
            _context.SaveChanges();
        }
    }
}
