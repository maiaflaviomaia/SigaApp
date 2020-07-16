using System;
using System.ComponentModel.DataAnnotations;

namespace SigaApp.Models.Entidades
{
    public class ContaContabil
    {
        [Key]
        public int ContaContabilID { get; set; }

        [Display(Name = "Data de Cadastro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataCadastro { get; set; }

        [Display(Name = "Data de Abertura")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime DataAbertura { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public string NomeConta { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Tipo de Conta")]
        public string TipoConta { get; set; }

        [Display(Name = "Saldo Inicial")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "R$ " + "{0:N2}")]
        public decimal SaldoInicial { get; set; }

        [Display(Name = "Banco")]
        public string NomeBanco { get; set; }

        [Display(Name = "Agência")]
        public string NumeroAgencia { get; set; }

        [Display(Name = "Conta")]
        public string NumeroConta { get; set; }

        [Display(Name = "Ativo?")]
        public bool FlagAtivo { get; set; }

        [Display(Name = "Data Exclusão")]
        public DateTime? DataExclusao { get; set; }

        public int EmpresaID { get; set; }
    }
}
