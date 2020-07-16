using SigaApp.Models.Entidades;
using System;

namespace SigaApp.Servicos
{
    public class CategoriaService
    {
        public void PreencherCampos(Categoria categoria)
        {
            try
            {
                if (categoria == null)
                    throw new ArgumentException("Categoria inválida");

                categoria.DataCadastro = DateTime.Now;
                categoria.FlagAtivo = true;
                categoria.DataExclusao = null;

                ValidarCampos(categoria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Desativar(Categoria categoria)
        {
            categoria.FlagAtivo = false;
            categoria.DataExclusao = DateTime.Now;
        }

        public void ValidarCampos(Categoria categoria)
        {
            try
            {
                if (categoria == null)
                    throw new ArgumentException("Categoria inválida");

                if (String.IsNullOrEmpty(categoria.Nome))
                    throw new ArgumentException("Campo Nome é obrigatório");

                if (String.IsNullOrEmpty(categoria.Tipo))
                    throw new ArgumentException("Campo Tipo é obrigatório");

                if (!String.IsNullOrEmpty(categoria.Nome) && categoria.Nome.Length > 100)
                    throw new ArgumentException("Máximo de 100 caracteres - Campo Nome");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
