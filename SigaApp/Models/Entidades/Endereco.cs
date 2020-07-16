using System.ComponentModel.DataAnnotations;

namespace SigaApp.Models.Entidades
{
    public class Endereco
    {
        [Key]
        public int EnderecoID { get; set; }

        [Display(Name = "Logradouro")]
        public string Logradouro { get; set; }

        [Display(Name = "Número")]
        public string Numero { get; set; }

        [Display(Name = "Bairro")]
        public string Bairro { get; set; }

        [Display(Name = "Cidade")]
        public string Cidade { get; set; }

        [Display(Name = "UF")]
        public string UF { get; set; }

        [Display(Name = "CEP")]
        [StringLength(9)]
        public string CEP { get; set; }

        [Display(Name = "Complemento")]
        public string Complemento { get; set; }

        public int? ClienteID { get; set; }

        public int? FornecedorID { get; set; }

        public int? FuncionarioID { get; set; }
    }
}
