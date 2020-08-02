using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SigaApp.Utils.Enums;

namespace SigaApp.Models.Entidades
{
    public class Lancamento
    {
        [Key]
        public int LancamentoID { get; set; }

        [Display(Name = "Data de Cadastro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataCadastro { get; set; }

        [Display(Name = "Data de Lançamento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataLancamento { get; set; }

        [Display(Name = "Empresa")]
        public int EmpresaID { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(300, ErrorMessage = "Máximo de 300 caracteres")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Descricao { get; set; }

        [Display(Name = "Observações")]
        [StringLength(500, ErrorMessage = "Máximo de 500 caracteres")]
        public string Observacoes { get; set; }

        [Display(Name = "Cliente/Fornecedor")]
        public string Nome { get; set; }

        [Display(Name = "Tipo de Lançamento")]
        public TipoLancamento TipoLancamento { get; set; }

        [Display(Name = "Número Documento")]
        public int? NumeroDocumento { get; set; }

        [Display(Name = "Conta Contábil")]
        public int? ContaContabilID { get; set; }

        [Display(Name = "Categoria")]
        public int? CategoriaID { get; set; }

        [Display(Name = "Sub-Categoria")]
        public int? SubCategoriaID { get; set; }

        [Display(Name = "Fornecedor")]
        public int? FornecedorID { get; set; }

        [Display(Name = "Cliente")]
        public int? ClienteID { get; set; }

        [Display(Name = "Centro de Custo")]
        public int? CentroCustoID { get; set; }

        [Display(Name = "Valor")]
        [DataType(DataType.Currency)]
        public decimal Valor { get; set; }

        [Display(Name = "Ativo?")]
        public bool FlagAtivo { get; set; }

        public bool isContaPagarReceber { get; set; }

        public bool isTransferencia { get; set; }

        [NotMapped]
        public string MesAno { get { return DataLancamento.ToString("MM/yyyy"); } }

        [Display(Name = "Data de Exclusão")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataExclusao { get; set; }

        public ContaContabil ContaContabil { get; set; }
        public Categoria Categoria { get; set; }
        public Categoria SubCategoria { get; set; }
        public Fornecedor Fornecedor { get; set; }
        public Cliente Cliente { get; set; }
        public CentroDeCusto CentroCusto { get; set; }
    }
}
