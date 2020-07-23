using SigaApp.Models.Entidades;
using System;
using SigaApp.Utils;

namespace SigaApp.Servicos
{
    public class FuncionarioService
    {
        TratarValores tratarValores = new TratarValores();

        public void PreencherCampos(Funcionario funcionario)
        {
            try
            {
                if (funcionario == null)
                    throw new ArgumentException("Funcionário inválido");

                funcionario.DataCadastro = DateTime.Now;
                funcionario.FlagAtivo = true;
                funcionario.DataExclusao = null;

                if (funcionario.CPF != null)
                    funcionario.CPF = tratarValores.TratarCPF(funcionario.CPF);

                ValidarCampos(funcionario);
            }
            catch (Exception)
            {
                throw;
            }
        }

        
        public void ValidarCampos(Funcionario funcionario)
        {
            try
            {
                if (String.IsNullOrEmpty(funcionario.NomeCompleto))
                    throw new ArgumentException("O campo Nome Completo é obrigatório");

                if (funcionario.DataAdmissao.Date != null && funcionario.DataAdmissao.Date > DateTime.Now.Date)
                    throw new ArgumentException("A data de admissão não pode ser maior que a data atual");

                if (funcionario.DataDemissao < funcionario.DataAdmissao)
                    throw new ArgumentException("A data de demissão não pode ser menor que a data de admissão");

                if (funcionario.Salario <= 0)
                    throw new ArgumentException("Salário inválido");

                if (funcionario.CPF != null && !tratarValores.ValidarCPF(funcionario.CPF))
                    throw new ArgumentException("Número do CPF inválido");

                if (funcionario.CPF != null && funcionario.CPF.Length > 14)
                    throw new ArgumentException("Campo CPF inválido. Máximo de 11 caracteres");

                if (funcionario.DataNascimento != null && funcionario.DataNascimento > DateTime.Now.Date)
                    throw new ArgumentException("Data de Nascimento inválida");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
