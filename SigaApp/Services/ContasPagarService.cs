using SigaApp.Models.Entidades;
using System;
using static SigaApp.Utils.Enums;

namespace SigaApp.Servicos
{
    public class ContasReceberService
    {
        public void PreencherCampos(ContaPagar contasPagar)
        {
            try
            {
                if (contasPagar == null)
                    throw new ArgumentException("Conta a Pagar inválida");

                contasPagar.FlagAtivo = true;
                contasPagar.DataCadastro = DateTime.Now;
                contasPagar.DataExclusao = null;
                contasPagar.Status = StatusContaPagar.Aberto;
                contasPagar.Juros = contasPagar.Juros ?? 0;
                contasPagar.Desconto = contasPagar.Desconto ?? 0;
                contasPagar.Multa = contasPagar.Multa ?? 0;

                if (contasPagar.NumeroDocumento == null)
                    contasPagar.NumeroDocumento = new Random().Next(1000000000);

                ValidarCampos(contasPagar);
            }
            catch (Exception)
            {
                throw;
            }
        }
                    

        public void Pagar(ContaPagar conta)
        {
            try
            {
                if (conta == null)
                    throw new ArgumentException("Conta a Pagar inválida");

                if (conta.ContaContabilID == null || conta.ContaContabilID <= 0)
                    throw new ArgumentException("Informe a Conta Contábil");
                
                conta.Status = StatusContaPagar.Pago;
                conta.ValorPago = (conta.Valor + conta.Juros + conta.Multa) - conta.Desconto;
                conta.DataPagamento = DateTime.Now;

                ValidarCampos(conta);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void GerarLancamento(Lancamento lancamento, ContaPagar contasPagar)
        {
            try
            {
                lancamento.DataCadastro = contasPagar.DataCadastro;
                lancamento.DataExclusao = null;
                lancamento.FlagAtivo = true;
                lancamento.DataLancamento = DateTime.Now;
                lancamento.CategoriaID = contasPagar.CategoriaID;
                lancamento.SubCategoriaID = contasPagar.SubCategoriaID;
                lancamento.CentroCustoID = contasPagar.CentroDeCustoID;
                lancamento.ContaContabilID = contasPagar.ContaContabilID;
                lancamento.Descricao = contasPagar.Descricao;
                lancamento.EmpresaID = contasPagar.EmpresaID;
                lancamento.FornecedorID = contasPagar.FornecedorID;
                lancamento.ClienteID = null;
                lancamento.NumeroDocumento = contasPagar.NumeroDocumento;
                lancamento.TipoLancamento = TipoLancamento.Debito;
                lancamento.Nome = contasPagar.Fornecedor.RazaoSocial;
                lancamento.Valor = contasPagar.ValorPago ?? 0;
                lancamento.isContaPagarReceber = true;
                lancamento.isTransferencia = false;
                lancamento.Observacoes = "Gerado automaticamente pela Conta a Pagar " + contasPagar.NumeroDocumento + " - " + contasPagar.Fornecedor.RazaoSocial + " - " + contasPagar.Descricao;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Erro ao gerar lançamentos " + ex.Message.ToString());
            }
        }

        public void ValidarCampos(ContaPagar conta)
        {
            try
            {
                if (conta == null)
                    throw new ArgumentException("Conta a pagar inválida");

                if (conta.DataVencimento == null)
                    throw new ArgumentException("Data de vencimento inválida");

                if (conta.FornecedorID == null)
                    throw new ArgumentException("Fornecedor inválido");

                if (conta.CategoriaID == null)
                    throw new ArgumentException("Categoria inválida");

                if (conta.SubCategoriaID == null)
                    throw new ArgumentException("Sub-Categoria inválida");

                if (String.IsNullOrEmpty(conta.Descricao))
                    throw new ArgumentException("Campo Descrição é obrigatório");

                if (conta.Valor == 0 || conta.Valor < 0)
                    throw new ArgumentException("Campo Valor é obrigatório");

                if (conta.ValorPago != null && conta.ValorPago < 0)
                    throw new ArgumentException("Valor pago não pode ser menor que zero");

                if (conta.Multa != null && conta.Multa < 0)
                    throw new ArgumentException("Multa não pode ser menor que zero");

                if (conta.Juros != null && conta.Juros < 0)
                    throw new ArgumentException("Juros não pode ser menor que zero");

                if (conta.Desconto != null && conta.Desconto < 0)
                    throw new ArgumentException("Desconto não pode ser menor que zero");
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
