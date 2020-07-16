using SigaApp.Models.Entidades;
using System;
using SigaApp.Utils;

namespace SigaApp.Servicos
{
    public class FornecedorService
    {
        private readonly TratarValores tratarValores = new TratarValores();

        public void PreencherCampos(Fornecedor fornecedor)
        {
            try
            {
                if (fornecedor == null)
                    throw new ArgumentException("Fornecedor inválido");

                fornecedor.FlagAtivo = true;
                fornecedor.DataCadastro = DateTime.Now;
                fornecedor.DataExclusao = null;

                if (fornecedor.CPF != null)
                    fornecedor.CPF = tratarValores.TratarCPF(fornecedor.CPF);

                if (fornecedor.CNPJ != null)
                    fornecedor.CNPJ = tratarValores.TratarCNPJ(fornecedor.CNPJ);

                ValidarCampos(fornecedor);
            }
            catch (Exception)
            {
                throw;
            }
        }

        

        public void ValidarCampos(Fornecedor fornecedor)
        {
            try
            {
                if (fornecedor == null)
                    throw new ArgumentException("Fornecedor inválido");

                if (String.IsNullOrEmpty(fornecedor.RazaoSocial))
                    throw new ArgumentException("Campo Razão Social é obrigatório");

                if (fornecedor.RazaoSocial.Length > 250)
                    throw new ArgumentException("Campo Razão Social inválido. Máximo de 250 caracteres");

                if (fornecedor.RazaoSocial.Length < 2)
                    throw new ArgumentException("Campo Razão Social inválido. Mínimo de 02 caracteres");

                if (fornecedor.CPF != null && !tratarValores.ValidarCPF(fornecedor.CPF))
                    throw new ArgumentException("Número do CPF inválido");

                if (fornecedor.CPF != null && fornecedor.CPF.Length > 14)
                    throw new ArgumentException("Campo CPF inválido. Máximo de 11 caracteres");

                if (fornecedor.CNPJ != null && !tratarValores.ValidarCNPJ(fornecedor.CNPJ))
                    throw new ArgumentException("Número do CNPJ inválido. Máximo de 14 caracteres");

                if (fornecedor.CNPJ != null && fornecedor.CNPJ.Length > 18)
                    throw new ArgumentException("Campo CNPJ inválido");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

