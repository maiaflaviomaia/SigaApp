using SigaApp.Models.Entidades;
using System;
using static SigaApp.Utils.Enums;

namespace SigaApp.Servicos
{
    public class OrcamentoService
    {
        public void PreencherCampos(Orcamento orcamento)
        {
            if (orcamento == null)
                throw new ArgumentException("Orçamento inválido");

            orcamento.DataCadastro = DateTime.Now;
            orcamento.FlagAtivo = true;
            orcamento.DataExclusao = null;
            orcamento.StatusOrcamento = StatusOrcamento.Aberto;

            ValidarCampos(orcamento);
        }

        public void AprovarOrcamento(Orcamento orcamento)
        {
            if (orcamento == null)
                throw new ArgumentException("Orçamento inválido");

            orcamento.StatusOrcamento = StatusOrcamento.Aprovado;
        }

        public void ReprovarOrcamento(Orcamento orcamento)
        {
            if (orcamento == null)
                throw new ArgumentException("Orçamento inválido");

            orcamento.StatusOrcamento = StatusOrcamento.Reprovado;
        }

        public void FaturarOrcamento(Orcamento orcamento)
        {
            if (orcamento == null)
                throw new ArgumentException("Orçamento inválido");

            orcamento.StatusOrcamento = StatusOrcamento.Faturado;
        }

        public void ValidarCampos(Orcamento orcamento)
        {
            try
            {
                if (orcamento == null)
                    throw new ArgumentException("Orçamento inválido");

                if (orcamento.DataOrcamento == null)
                    throw new ArgumentException("O campo Data do Orçamento é obrigatório");

                if (orcamento.DataValidade == null)
                    throw new ArgumentException("O campo Data de Validade é obrigatório");

                if (orcamento.DataOrcamento > orcamento.DataValidade)
                    throw new ArgumentException("A data do orçamento não pode ser maior que a data de validade");

                if (String.IsNullOrEmpty(orcamento.TipoOrcamento))
                    throw new ArgumentException("O campo Tipo é obrigatório");

                if (String.IsNullOrEmpty(orcamento.Titulo))
                    throw new ArgumentException("O campo Título é obrigatório");

                if (!String.IsNullOrEmpty(orcamento.Titulo) && orcamento.Titulo.Length > 200)
                    throw new ArgumentException("Máximo de 200 caracteres - Campo Título");

                if (String.IsNullOrEmpty(orcamento.Solicitante))
                    throw new ArgumentException("Campo Solicitante é obrigatório");

                if (!String.IsNullOrEmpty(orcamento.Solicitante) && orcamento.Solicitante.Length > 150)
                    throw new ArgumentException("Máximo de 150 caracteres - Campo Solicitante");

                if (!String.IsNullOrEmpty(orcamento.TipoVeiculacao) && orcamento.TipoVeiculacao.Length > 50)
                    throw new ArgumentException("Máximo de 50 caracteres - Campo Tipo de Veiculação");

                if (!String.IsNullOrEmpty(orcamento.PracaVeiculacao) && orcamento.PracaVeiculacao.Length > 100)
                    throw new ArgumentException("Máximo de 100 caracteres - Campo Praça de Veiculação");

                if (!String.IsNullOrEmpty(orcamento.NomePeca) && orcamento.NomePeca.Length > 150)
                    throw new ArgumentException("Máximo de 150 caracteres - Campo Nome da Peça");

                if (!String.IsNullOrEmpty(orcamento.DuracaoPeca) && orcamento.DuracaoPeca.Length > 50)
                    throw new ArgumentException("Máximo de 50 caracteres - Campo Duração");

                if (!String.IsNullOrEmpty(orcamento.PeriodoVeiculacao) && orcamento.PeriodoVeiculacao.Length > 20)
                    throw new ArgumentException("Máximo de 20 caracteres - Campo Período de Veiculação");

                if (!String.IsNullOrEmpty(orcamento.TipoMidia) && orcamento.TipoMidia.Length > 100)
                    throw new ArgumentException("Máximo de 100 caracteres - Campo Tipo de Mídia");

                if (!String.IsNullOrEmpty(orcamento.Observacoes) && orcamento.Observacoes.Length > 250)
                    throw new ArgumentException("Máximo de 250 caracteres - Campo Observações");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
