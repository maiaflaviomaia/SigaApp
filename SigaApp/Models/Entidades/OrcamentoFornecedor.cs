using System.ComponentModel.DataAnnotations;

namespace SigaApp.Models.Entidades
{
    public class OrcamentoFornecedor
    {
        [Key]
        public int OrcamentoFornecedorID { get; set; }

        public int OrcamentoID { get; set; }

        [Display(Name = "Fornecedor")]
        public int FornecedorID { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; }

        [Display(Name = "Valor Unitário")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "R$ " + "{0:N2}")]
        public decimal ValorUnitario { get; set; }

        [Display(Name = "Valor Total")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "R$ " + "{0:N2}")]
        public decimal ValorTotal { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        public int EmpresaID { get; set; }

        public Orcamento Orcamento { get; set; }
        public Fornecedor Fornecedor { get; set; }
    }
}
