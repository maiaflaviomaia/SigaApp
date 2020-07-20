using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SigaApp.Utils.Enums;

namespace SigaApp.Models.Entidades
{
    public class Orcamento
    {
        [Key]
        public int OrcamentoID { get; set; }

        [Display(Name = "Data de cadastro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataCadastro { get; set; }

        [Display(Name = "Data do orçamento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public DateTime DataOrcamento { get; set; }

        [Display(Name = "Data de validade")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public DateTime DataValidade { get; set; }

        [Display(Name = "Tipo")]
        public string TipoOrcamento { get; set; }

        [Display(Name = "Status")]
        public StatusOrcamento StatusOrcamento { get; set; }

        [Display(Name = "Título")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(200, ErrorMessage = "Máximo de 200 caracteres")]
        public string Titulo { get; set; }

        [Display(Name = "Total (R$)")]
        [DisplayFormat(DataFormatString = "R$ " + "{0:N2}")]
        public decimal TotalOrcamento { get; set; }

        [Display(Name = "Descontos (R$)")]
        [DisplayFormat(DataFormatString = "R$ " + "{0:N2}")]
        public decimal? Desconto { get; set; }

        [Display(Name = "Acréscimos (R$)")]
        [DisplayFormat(DataFormatString = "R$ " + "{0:N2}")]
        public decimal? Acrescimo { get; set; }

        [Display(Name = "Comissão (%)")]
        public decimal? TaxaComissao { get; set; }

        [Display(Name = "Impostos (%)")]
        public decimal? TaxaImposto { get; set; }

        [Display(Name = "Lucro desejado (%)")]
        public decimal? TaxaLucro { get; set; }

        [Display(Name = "Ativo?")]
        public bool FlagAtivo { get; set; }

        [Display(Name = "Data de exclusão")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataExclusao { get; set; }

        [Display(Name = "Cliente/Agência")]
        public int ClienteID { get; set; }

        public int EmpresaID { get; set; }

        [Display(Name = "Solicitante")]
        public string Solicitante { get; set; }

        [Display(Name = "Veiculação")]
        public string TipoVeiculacao { get; set; }

        [Display(Name = "Praça de veiculação")]
        [StringLength(100, ErrorMessage = "Máximo de 100 caracteres")]
        public string PracaVeiculacao { get; set; }

        [Display(Name = "Nome da Peça")]
        [StringLength(150, ErrorMessage = "Máximo de 150 caracteres")]
        public string NomePeca { get; set; }

        [Display(Name = "Duração")]
        public string DuracaoPeca { get; set; }

        [Display(Name = "Período de veiculação")]
        public string PeriodoVeiculacao { get; set; }

        [Display(Name = "Tipo de Mídia")]
        [StringLength(100, ErrorMessage = "Máximo de 100 caracteres")]
        public string TipoMidia { get; set; }

        [Display(Name = "BV (%)")]
        public decimal? BonificacaoVeiculacao { get; set; }

        [Display(Name = "Observações")]
        [StringLength(250, ErrorMessage = "Máximo de 250 caracteres")]
        public string Observacoes { get; set; }

        [NotMapped]
        public string ServicosJSON { get; set; }

        [NotMapped]
        public string FornecedoresJSON { get; set; }

        [NotMapped]
        public string CustoProducaoJSON { get; set; }

        [NotMapped]
        public string ValorPorExtenso { get; set; }

        [NotMapped]
        public bool GerarPagamentos { get; set; }

        public ICollection<OrcamentoServico> OrcamentoServicos { get; set; }
        public ICollection<OrcamentoFornecedor> OrcamentoFornecedores { get; set; }
        public ICollection<OrcamentoCustos> OrcamentoCustos { get; set; }
        public Cliente Cliente { get; set; }
        public Empresa Empresa { get; set; }
    }
}
