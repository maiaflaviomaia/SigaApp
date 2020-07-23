using SigaApp.Models.Entidades;
using System;

namespace SigaApp.Servicos
{
    public class OrcamentoFornecedorService
    {
        public void ValidarCampos(OrcamentoFornecedor fornecedor)
        {
            try
            {
                if (fornecedor == null)
                    throw new ArgumentException("Fornecedor inválido");

                if (fornecedor.EmpresaID.Equals(null))
                    throw new ArgumentException("Erro ao obter Empresa.");

                if (fornecedor.OrcamentoID.Equals(null))
                    throw new ArgumentException("Orçamento inválido");

                if (!fornecedor.OrcamentoID.Equals(null) && fornecedor.OrcamentoID <= 0)
                    throw new ArgumentException("Orçamento inválido");

                if (String.IsNullOrEmpty(fornecedor.Descricao))
                    throw new ArgumentException("Campo Descrição (Fornecedor) é obrigatório");

                if (fornecedor.FornecedorID.Equals(null))
                    throw new ArgumentException("Fornecedor inválido");

                if (!fornecedor.FornecedorID.Equals(null) && fornecedor.FornecedorID <= 0)
                    throw new ArgumentException("Fornecedor inválido");

                if (fornecedor.Quantidade.Equals(null))
                    throw new ArgumentException("Campo Quantidade (Fornecedor) inválido");

                if (fornecedor.Quantidade <= 0)
                    throw new ArgumentException("O campo Quantidade (Fornecedor) deve ser maior que zero.");

                if (!fornecedor.ValorUnitario.Equals(null) && fornecedor.ValorUnitario <= 0)
                    throw new ArgumentException("Campo Valor Unitário (Fornecedor) é obrigatório");

                if (!fornecedor.ValorTotal.Equals(null) && fornecedor.ValorTotal <= 0)
                    throw new ArgumentException("Campo Valor Total (Fornecedor) é obrigatório");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
