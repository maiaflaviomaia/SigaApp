using SigaApp.Models.Entidades;
using System;

namespace SigaApp.Servicos
{
    public class LancamentoService
    {
        public void PreencherCampos(Lancamento lancamento)
        {
            if (lancamento == null)
                throw new ArgumentException("Lançamento inválido");

            lancamento.DataCadastro = DateTime.Now;
            lancamento.FlagAtivo = true;
            lancamento.isContaPagarReceber = false;
            
            if (lancamento.NumeroDocumento == null)
                lancamento.NumeroDocumento = new Random().Next(1000000000);
                        
            ValidarCampos(lancamento);
        }
                

        public void ValidarCampos(Lancamento lancamento)
        {
            try
            {
                if (lancamento == null)
                    throw new ArgumentException("Lançamento inválido");

                if (String.IsNullOrEmpty(lancamento.Descricao))
                    throw new ArgumentException("Campo Descrição é obrigatório");

                if (!String.IsNullOrEmpty(lancamento.Descricao) && lancamento.Descricao.Length > 300)
                    throw new ArgumentException("Máximo de 300 caracteres - Campo Descrição");

                if (!String.IsNullOrEmpty(lancamento.Observacoes) && lancamento.Observacoes.Length > 500)
                    throw new ArgumentException("Máximo de 500 caracteres - Campo Observações");

                if (lancamento.Valor <= 0)
                    throw new ArgumentException("Valor inválido");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ValidarTransferencia(DateTime txtData, int txtOrigem, int txtDestino, decimal txtValor, string txtDescricao, string txtObservacao)
        {
            try
            {
                if (txtOrigem == txtDestino)
                    throw new ArgumentException("Transferência apenas entre contas distintas");

                if (txtData == null || txtOrigem <= 0 || txtDestino <= 0 || txtValor == 0 || String.IsNullOrEmpty(txtDescricao))
                    throw new ArgumentException("Todos os campos são de preenchimento obrigatórios");

                if (txtValor <= 0)
                    throw new ArgumentException("Não é permitido transferir valor menor ou igual que zero");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
