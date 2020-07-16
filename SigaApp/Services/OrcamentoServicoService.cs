using SigaApp.Models.Entidades;
using System;

namespace SigaApp.Servicos
{
    public class OrcamentoServicoService
    {
        public void ValidarCampos(OrcamentoServico oss)
        {
            try
            {
                if (oss == null)
                    throw new ArgumentException("Serviços do Orçamento inválidos");

                if (oss.EmpresaID.Equals(null))
                    throw new ArgumentException("Erro ao obter Empresa. Orçamento - Serviços");

                if (!oss.Quantidade.Equals(null) && oss.Quantidade <= 0)
                    throw new ArgumentException("O campo Quantidade deve ser maior que 0");

                if (!oss.OrcamentoID.Equals(null) && oss.OrcamentoID <= 0)
                    throw new ArgumentException("Orçamento inválido");

                if (!oss.ServicoPrestadoID.Equals(null) && oss.ServicoPrestadoID <= 0)
                    throw new ArgumentException("Serviço Pestado inválido");

                if (!oss.ValorTotal.Equals(null) && oss.ValorTotal <= 0)
                    throw new ArgumentException("Valor Total Serviços inválido");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
