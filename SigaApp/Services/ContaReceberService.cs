using SigaApp.Models.Entidades;
using System;
using static SigaApp.Utils.Enums;

namespace SigaApp.Servicos
{
    public class ContaReceberService
    {
        public void PreencherCampos(ContaReceber conta)
        {
            try
            {
                if (conta == null)
                    throw new ArgumentException("Conta a Receber inválida");

                conta.FlagAtivo = true;
                conta.DataCadastro = DateTime.Now;
                conta.DataExclusao = null;
                conta.Status = StatusContaReceber.Aberto;
                conta.Juros = conta.Juros ?? 0;
                conta.Desconto = conta.Desconto ?? 0;
                conta.Multa = conta.Multa ?? 0;

                if (conta.NumeroDocumento == null)
                    conta.NumeroDocumento = new Random().Next(1000000000);

                ValidarCampos(conta);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Receber(ContaReceber conta)
        {
            try
            {
                if (conta == null)
                    throw new ArgumentException("Conta a Receber inválida");

                if (conta.ContaContabilID == null || conta.ContaContabilID <= 0)
                    throw new ArgumentException("Informe a Conta Contábil");

                conta.DataPagamento = DateTime.Now;
                conta.Status = StatusContaReceber.Pago;
                conta.ValorRecebido = (conta.Valor + conta.Juros + conta.Multa) - conta.Desconto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void GerarLancamento(Lancamento lancamento, ContaReceber contaReceber)
        {
            if (contaReceber == null)
                throw new ArgumentException("Conta a Receber inválida");

            try
            {
                lancamento.DataCadastro = contaReceber.DataCadastro;
                lancamento.DataExclusao = null;
                lancamento.FlagAtivo = true;
                lancamento.isContaPagarReceber = true;
                lancamento.isTransferencia = false;
                lancamento.DataLancamento = DateTime.Now;
                lancamento.CategoriaID = contaReceber.CategoriaID;
                lancamento.CentroCustoID = contaReceber.CentroDeCustoID;
                lancamento.ContaContabilID = contaReceber.ContaContabilID;
                lancamento.Descricao = contaReceber.Descricao;
                lancamento.EmpresaID = contaReceber.EmpresaID;
                lancamento.ClienteID = contaReceber.ClienteID;
                lancamento.FornecedorID = null;
                lancamento.NumeroDocumento = contaReceber.NumeroDocumento;
                lancamento.Observacoes = "Gerado automaticamente pela Conta a Receber " + contaReceber.NumeroDocumento + " - " + contaReceber.Cliente.RazaoSocial + " - " + contaReceber.Descricao;
                lancamento.SubCategoriaID = contaReceber.SubCategoriaID;
                lancamento.TipoLancamento = TipoLancamento.Credito;
                lancamento.Nome = contaReceber.Cliente.RazaoSocial;
                lancamento.Valor = contaReceber.ValorRecebido ?? 0;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Erro ao gerar Lançamentos - " + ex.Message.ToString());
            }
            
        }

        public void ValidarCampos(ContaReceber conta)
        {
            try
            {
                if (conta == null)
                    throw new ArgumentException("Conta a Receber inválida");

                if (conta.DataVencimento == null)
                    throw new ArgumentException("Campo Data de Vencimento é obrigatório");

                if (conta.DataVencimento != null && conta.DataVencimento.Date < DateTime.Now.Date)
                    throw new ArgumentException("A data de vencimento não pode ser menor que a data atual");

                if (conta.FormaPagamento == null)
                    throw new ArgumentException("Campo Forma de Pagamento é obrigatório");

                if (conta.ClienteID == null || conta.ClienteID <= 0)
                    throw new ArgumentException("Cliente inválido. Campo obrigatório");

                if (conta.CategoriaID == null || conta.CategoriaID <= 0)
                    throw new ArgumentException("Categoria inválida. Campo obrigatório");

                if (conta.SubCategoriaID == null || conta.SubCategoriaID <= 0)
                    throw new ArgumentException("Sub-Categoria inválida. Campo obrigatório");
                                
                if (String.IsNullOrEmpty(conta.Descricao))
                    throw new ArgumentException("Campo Descrição é obrigatório");

                if (conta.Juros != null && conta.Juros < 0)
                    throw new ArgumentException("Juros não pode ser menor que zero");

                if (conta.Multa != null && conta.Multa < 0)
                    throw new ArgumentException("Multa não pode ser menor que zero");

                if (conta.Valor <= 0)
                    throw new ArgumentException("Campo Valor é obrigatório");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
