namespace SigaApp.Utils
{
    public class Enums
    {
        public enum StatusOrcamento
        {
            Aberto = 0,
            Aprovado = 1,
            Reprovado = 2,
            Cancelado = 3,
            Encerrado = 4,
            Faturado = 5
        }

        public enum StatusContaPagar
        {
            Aberto = 0,
            Vencido = 1,
            Pago = 2,
            Atrasado = 3,
            Protestado = 4,
            Cancelado = 5
        }

        public enum StatusContaReceber
        {
            Aberto = 0,
            Vencido = 1,
            Pago = 2,
            Atrasado = 3,
            Protestado = 4,
            Cancelado = 5
        }

        public enum FormaPagamento
        {
            Dinheiro = 0,
            Cheque = 1,
            DebitoEmConta = 2,
            Boleto = 3,
            TransferenciaBancaria = 4,
            CartaoCredito = 5,
            CartaoDebito = 6
        }

        public enum TipoDocumento
        {
            NotaFiscal = 0,
            CupomFiscal = 1,
            DocumentoFiscal = 2,
            Recibo = 3,
            Comprovante = 4
        }

        public enum TipoLancamento
        {
            Credito = 1,
            Debito = 2
        }
    }
}
