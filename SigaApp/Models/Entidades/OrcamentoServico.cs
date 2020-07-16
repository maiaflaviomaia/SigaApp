using System.ComponentModel.DataAnnotations;

namespace SigaApp.Models.Entidades
{
    public class OrcamentoServico
    {
        [Key]
        public int OrcamentoServicoID { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        public int OrcamentoID { get; set; }

        [Display(Name = "Serviço Prestado")]
        public int ServicoPrestadoID { get; set; }

        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; }

        [Display(Name = "Valor Total")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "R$ " + "{0:N2}")]
        public decimal ValorTotal { get; set; }

        public int EmpresaID { get; set; }

        public Orcamento Orcamento { get; set; }
        public ServicoPrestado ServicoPrestado { get; set; }
    }
}
