using System.ComponentModel.DataAnnotations;

namespace SigaApp.Models.Entidades
{
    public class DadosBancarios
    {
        [Key]
        public int DadosBancariosID { get; set; }

        [Display(Name = "Nome do Banco")]
        public string NomeBanco { get; set; }

        [Display(Name = "Número da Agência")]
        public string NumeroAgencia { get; set; }

        [Display(Name = "Número da Conta")]
        public string NumeroConta { get; set; }

        [Display(Name = "Operação")]
        public string Operacao { get; set; }

        [Display(Name = "Favorecido")]
        public string Favorecido { get; set; }

        public int? ClienteID { get; set; }

        public int? FornecedorID { get; set; }

        public int? FuncionarioID { get; set; }
    }
}
