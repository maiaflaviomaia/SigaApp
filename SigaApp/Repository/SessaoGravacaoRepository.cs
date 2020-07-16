using Microsoft.EntityFrameworkCore;
using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SigaApp.Repository
{
    public class SessaoGravacaoRepository : ISessaoGravacao
    {
        private readonly SigaContext _contexto;
        private readonly IUsuario _usuario;

        public SessaoGravacaoRepository(SigaContext contexto, IUsuario usuario)
        {
            _contexto = contexto;
            _usuario = usuario;
        }

        public void Atualizar(SessaoGravacao objeto)
        {
            _contexto.SessoesGravacoes.Update(objeto);
            Salvar();
        }

        public void Desativar(int id)
        {
            SessaoGravacao sg = _contexto.SessoesGravacoes.Find(id);
            sg.DataExclusao = DateTime.Now;
            sg.FlagAtivo = false;
            Salvar();
        }

        public void Dispose()
        {
            _contexto.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Excluir(int id)
        {
            SessaoGravacao sg = _contexto.SessoesGravacoes.Find(id);
            _contexto.SessoesGravacoes.Remove(sg);
        }

        public void Inserir(SessaoGravacao objeto)
        {
            _contexto.SessoesGravacoes.Add(objeto);
            Salvar();
        }

        public SessaoGravacao ObterPorId(int id)
        {
            return _contexto.SessoesGravacoes
                .Include(s => s.Cliente)
                .Include(s => s.Fornecedor)
                .Include(s => s.servicoPrestado)
                .Include(s => s.Estudio)
                .AsNoTracking()
                .FirstOrDefault(o => o.SessaoID == id);
        }

        public IEnumerable<SessaoGravacao> ObterTodos()
        {
            int id = _usuario.ObterEmpresa();
            return _contexto.SessoesGravacoes
                .Include(s => s.Cliente)
                .Include(s => s.Fornecedor)
                .Include(s => s.servicoPrestado)
                .Include(s => s.Estudio)
                .ToList().Where(s => s.FlagAtivo = true && s.DataExclusao is null && s.EmpresaID == id).OrderBy(s => s.DataCadastro);
        }

        public void Salvar()
        {
            _contexto.SaveChanges();
        }
    }
}
