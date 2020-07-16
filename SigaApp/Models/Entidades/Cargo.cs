using System;
using System.ComponentModel.DataAnnotations;

namespace SigaApp.Models.Entidades
{
    public class Cargo
    {
        [Key]
        public int CargoID { get; set; }

        [Display(Name = "Data de Cadastro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataCadastro { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [Display(Name = "Descrição Sumária")]
        public string DescricaoSumaria { get; set; }

        [Display(Name = "Descrição Detalhada")]
        public string DescricaoDetalhada { get; set; }

        [Display(Name = "Salário Base")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "R$ " + "{0:N2}")]
        public decimal? SalarioBase { get; set; }

        [Display(Name = "Ativo?")]
        public bool FlagAtivo { get; set; }

        [Display(Name = "Data de Exclusão")]
        public DateTime? DataExclusao { get; set; }

        public int EmpresaID { get; set; }
    }
}
