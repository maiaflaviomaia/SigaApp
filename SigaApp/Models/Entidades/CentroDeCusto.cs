using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SigaApp.Models.Entidades
{
    public class CentroDeCusto
    {
        [Key]
        public int CentroCustoID { get; set; }

        [Display(Name = "Data de Cadastro")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataCadastro { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(100, ErrorMessage = "Máximo de 100 caracteres")]
        public string Nome { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(150, ErrorMessage = "Máximo de 150 caracteres")]
        public string Descricao { get; set; }

        [Display(Name = "Ativo?")]
        public bool FlagAtivo { get; set; }

        [Display(Name = "Data Exclusão")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataExclusao { get; set; }

        public int EmpresaID { get; set; }
    }
}
