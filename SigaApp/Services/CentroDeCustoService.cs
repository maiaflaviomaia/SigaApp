using SigaApp.Models.Entidades;
using System;

namespace SigaApp.Servicos
{
    public class CentroDeCustoService
    {
        public void PreencherCampos(CentroDeCusto centro)
        {
            try
            {
                if (centro == null)
                    throw new ArgumentException("Centro de Custo inválido");

                centro.FlagAtivo = true;
                centro.DataCadastro = DateTime.Now;
                centro.DataExclusao = null;

                ValidarCampos(centro);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void ValidarCampos(CentroDeCusto centro)
        {
            try
            {
                if (centro == null)
                    throw new ArgumentException("Centro de Custo inválido");

                if (String.IsNullOrEmpty(centro.Nome))
                    throw new ArgumentException("Campo Nome é obrigatório");

                if (!String.IsNullOrEmpty(centro.Nome) && centro.Nome.Length > 100)
                    throw new ArgumentException("Máximo de 100 caracteres - Campo Nome");

                if (String.IsNullOrEmpty(centro.Descricao))
                    throw new ArgumentException("Campo Descrição é obrigatório");

                if (!String.IsNullOrEmpty(centro.Descricao) && centro.Nome.Length > 150)
                    throw new ArgumentException("Máximo de 150 caracteres - Campo Descrição");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
