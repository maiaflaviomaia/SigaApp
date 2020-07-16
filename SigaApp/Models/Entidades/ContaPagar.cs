using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SigaApp.Utils.Enums;

namespace SigaApp.Models.Entidades
{
    public class ContaPagar
    {
        [Key]
        public int ContasPagarID { get; set; }

        [Display(Name = "Data de Cadastro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataCadastro { get; set; }

        [Display(Name = "Data de Vencimento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime DataVencimento { get; set; }

        [Display(Name = "Data de Pagamento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? DataPagamento { get; set; }

        [Display(Name = "Competência")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime Competencia { get; set; }

        [Display(Name = "Valor")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "R$ " + "{0:N2}")]
        public decimal Valor { get; set; }

        [Display(Name = "Fornecedor")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public int? FornecedorID { get; set; }

        [Display(Name = "Categoria")]
        public int? CategoriaID { get; set; }

        [Display(Name = "Sub-Categoria")]
        public int? SubCategoriaID { get; set; }

        [Display(Name = "Centro de Custos")]
        public int? CentroDeCustoID { get; set; }

        [Display(Name = "Orçamento")]
        public int? OrcamentoID { get; set; }

        [Display(Name = "Conta Contábil")]
        public int? ContaContabilID { get; set; }

        [Display(Name = "Status")]
        public StatusContaPagar? Status { get; set; }

        [Display(Name = "Forma de Pagamento")]
        public FormaPagamento? FormaPagamento { get; set; }

        [Display(Name = "Número do documento")]
        public int? NumeroDocumento { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Tipo de documento")]
        public TipoDocumento TipoDocumento { get; set; }

        [Display(Name = "Observações")]
        public string Observacoes { get; set; }

        [Display(Name = "Ativo?")]
        public bool FlagAtivo { get; set; }

        [Display(Name = "Data de Exclusão")]
        public DateTime? DataExclusao { get; set; }

        [Display(Name = "Desconto")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "R$ " + "{0:N2}")]
        public decimal? Desconto { get; set; }

        [Display(Name = "Juros")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "R$ " + "{0:N2}")]
        public decimal? Juros { get; set; }

        [Display(Name = "Multa")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "R$ " + "{0:N2}")]
        public decimal? Multa { get; set; }

        [Display(Name = "Valor Pago")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "R$ " + "{0:N2}")]
        public decimal? ValorPago { get; set; }

        [Display(Name = "Recorrente")]
        public bool Recorrente { get; set; }

        [Display(Name = "Empresa")]
        public int EmpresaID { get; set; }

        [NotMapped]
        public string ValorPorExtenso { get; set; }

        public Fornecedor Fornecedor { get; set; }
        public Categoria Categoria { get; set; }
        public Orcamento Orcamento { get; set; }
        public Categoria SubCategoria { get; set; }
        public CentroDeCusto CentroDeCusto { get; set; }
        public ContaContabil ContaContabil { get; set; }
        public Empresa Empresa { get; set; }
    }
}
