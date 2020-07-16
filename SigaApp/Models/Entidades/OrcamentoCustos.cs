using System.ComponentModel.DataAnnotations;

namespace SigaApp.Models.Entidades
{
    public class OrcamentoCustos
    {
        [Key]
        public int OrcamentoCustoID { get; set; }

        public int OrcamentoID { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; }

        [Display(Name = "Valor Unitário")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "R$ " + "{0:N2}")]
        public decimal ValorUnitario { get; set; }

        [Display(Name = "Unidade de Valor")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string UnidadeValor { get; set; }

        [Display(Name = "Valor Total")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "R$ " + "{0:N2}")]
        public decimal ValorTotal { get; set; }

        public Orcamento Orcamento { get; set; }

        public int EmpresaID { get; set; }
    }
}
