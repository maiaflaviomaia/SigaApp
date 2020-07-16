using SigaApp.Models.Entidades;
using System;

namespace SigaApp.Servicos
{
    public class ServicoPrestadoService
    {
        public void PreencherCampos(ServicoPrestado servico)
        {
            try
            {
                if (servico == null)
                    throw new ArgumentException("Serviço inválido");

                servico.DataCadastro = DateTime.Now;
                servico.FlagAtivo = true;
                servico.DataExclusao = null;

                ValidarCampos(servico);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void ValidarCampos(ServicoPrestado servico)
        {
            try
            {
                if (servico == null)
                    throw new ArgumentException("Serviço inválido");

                if (String.IsNullOrEmpty(servico.UnidadeValor))
                    throw new ArgumentException("Campo Unidade de Valor não pode ser nulo");

                if (String.IsNullOrEmpty(servico.Descricao))
                    throw new ArgumentException("Campo Descrição não pode ser nulo");

                if (!String.IsNullOrEmpty(servico.Descricao) && servico.Descricao.Length > 250)
                    throw new ArgumentException("Máximo de 250 caracteres - Campo Descrição");

                if (String.IsNullOrEmpty(servico.ValorUnitario.ToString()))
                    throw new ArgumentException("Campo Valor Unitário não pode ser nulo");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
