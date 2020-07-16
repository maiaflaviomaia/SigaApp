using SigaApp.Models.Entidades;
using System;

namespace SigaApp.Servicos
{
    public class ContaContabilService
    {
        public void PreencherCampos(ContaContabil contaContabil)
        {
            try
            {
                if (contaContabil == null)
                    throw new ArgumentException("Conta Contábil inválida");

                contaContabil.FlagAtivo = true;
                contaContabil.DataCadastro = DateTime.Now;
                contaContabil.DataExclusao = null;

                ValidarCampos(contaContabil);
            }
            catch (Exception)
            {
                throw;
            }
        }

        
        public void ValidarCampos(ContaContabil contaContabil)
        {
            try
            {
                if (contaContabil == null)
                    throw new ArgumentException("Conta Contábil inválida");

                if (String.IsNullOrEmpty(contaContabil.NomeConta))
                    throw new ArgumentException("O campo Nome é obrigatório");

                if (String.IsNullOrEmpty(contaContabil.Descricao))
                    throw new ArgumentException("O campo Descrição é obrigatório");

                if (String.IsNullOrEmpty(contaContabil.TipoConta))
                    throw new ArgumentException("O campo Tipo Conta é obrigatório");

                if (contaContabil.SaldoInicial < 0)
                    throw new ArgumentException("O Saldo Inicial não pode ser menor que zero");

                if (contaContabil.DataAbertura.Date > DateTime.Now.Date)
                    throw new ArgumentException("A Data de Abertura não pode ser maior que a data atual");

                if (contaContabil.TipoConta == "Bancária" || contaContabil.TipoConta == "Investimento" && (String.IsNullOrEmpty(contaContabil.NomeBanco)))
                    throw new ArgumentException("Em contas do tipo Bancária ou Investimento os dados do Banco devem ser preenchidos");

                if (contaContabil.TipoConta == "Bancária" || contaContabil.TipoConta == "Investimento" && (String.IsNullOrEmpty(contaContabil.NumeroAgencia)))
                    throw new ArgumentException("Em contas do tipo Bancária ou Investimento os dados do Banco devem ser preenchidos");

                if (contaContabil.TipoConta == "Bancária" || contaContabil.TipoConta == "Investimento" && (String.IsNullOrEmpty(contaContabil.NumeroConta)))
                    throw new ArgumentException("Em contas do tipo Bancária ou Investimento os dados do Banco devem ser preenchidos");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
