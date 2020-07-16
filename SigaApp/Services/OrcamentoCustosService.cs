using SigaApp.Models.Entidades;
using System;

namespace SigaApp.Servicos
{
    public class OrcamentoCustosService
    {
        public void ValidarCampos(OrcamentoCustos custos)
        {
            try
            {
                if (custos == null)
                    throw new ArgumentException("Custos de Produção inválido");

                if (custos.EmpresaID.Equals(null))
                    throw new ArgumentException("Erro ao obter Empresa. Orçamento - Custos de Produção");

                if (custos.OrcamentoID.Equals(null))
                    throw new ArgumentException("Orçamento inválido");

                if (!custos.OrcamentoID.Equals(null) && custos.OrcamentoID <= 0)
                    throw new ArgumentException("Orçamento inválido");

                if (custos.Quantidade.Equals(null))
                    throw new ArgumentException("Campo Quantidade (Custos de Produção) é obrigatório");

                if (!custos.Quantidade.Equals(null) && custos.Quantidade <= 0)
                    throw new ArgumentException("Campo Quantidade (Custos de Produção) deve ser maior que zero");

                if (custos.ValorUnitario.Equals(null))
                    throw new ArgumentException("Campo Valor Unitário (Custos de Produção) é obrigatório");

                if (!custos.ValorUnitario.Equals(null) && custos.ValorUnitario <= 0)
                    throw new ArgumentException("Campo Valor Unitário (Custos de Produção) deve ser maior que zero");

                if (custos.ValorTotal.Equals(null))
                    throw new ArgumentException("Campo Valor Total (Custos de Produção) é obrigatório");

                if (!custos.ValorTotal.Equals(null) && custos.ValorTotal <= 0)
                    throw new ArgumentException("Campo Valor Total (Custos de Produção) deve ser maior que zero");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
