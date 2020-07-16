using SigaApp.Models.Entidades;
using SigaApp.Utils;
using System;

namespace SigaApp.Servicos
{
    public class ClienteService
    {
        private readonly TratarValores tratarValores = new TratarValores();

        public void PreencherCampos(Cliente cliente)
        {
            try
            {
                if (cliente == null)
                    throw new ArgumentException("Cliente inválido");

                cliente.FlagAtivo = true;
                cliente.DataCadastro = DateTime.Now;
                cliente.DataExclusao = null;

                if (cliente.CPF != null)
                    cliente.CPF = tratarValores.TratarCPF(cliente.CPF);

                if (cliente.CNPJ != null)
                    cliente.CNPJ = tratarValores.TratarCNPJ(cliente.CNPJ);

                ValidarCampos(cliente);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ValidarCampos(Cliente cliente)
        {
            try
            {
                if (cliente == null)
                    throw new ArgumentException("Cliente inválido");

                if (String.IsNullOrEmpty(cliente.RazaoSocial))
                    throw new ArgumentException("Campo Razão Social é obrigatório");

                if (!String.IsNullOrEmpty(cliente.RazaoSocial) && cliente.RazaoSocial.Length > 250)
                    throw new ArgumentException("Campo Razão Social - Máximo de 250 caracteres");

                if (!String.IsNullOrEmpty(cliente.RazaoSocial) && cliente.RazaoSocial.Length < 2)
                    throw new ArgumentException("Campo Razão Social - Mínimo de 02 caracteres");

                if (cliente.CPF != null && !tratarValores.ValidarCPF(cliente.CPF))
                    throw new ArgumentException("Número do CPF inválido");

                if (cliente.CPF != null && cliente.CPF.Length > 14)
                    throw new ArgumentException("Campo CPF inválido. Máximo de 11 caracteres");

                if (cliente.CNPJ != null && !tratarValores.ValidarCNPJ(cliente.CNPJ))
                    throw new ArgumentException("Número do CNPJ inválido. Máximo de 14 caracteres");

                if (cliente.CNPJ != null && cliente.CNPJ.Length > 18)
                    throw new ArgumentException("Campo CNPJ inválido");

                if (!String.IsNullOrEmpty(cliente.Observacoes) && cliente.Observacoes.Length > 500)
                    throw new ArgumentException("Máximo de 500 caracteres - Campo Observações");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
