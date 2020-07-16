using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SigaApp.Models.Entidades
{
    public class ServicoPrestado
    {
        [Key]
        public int ServicoPrestadoID { get; set; }

        [Display(Name = "Data Cadastro")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataCadastro { get; set; }

        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public string Descricao { get; set; }

        [Display(Name = "Valor Unitário")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        [DisplayFormat(DataFormatString = "R$ " + "{0:N2}")]
        public decimal ValorUnitario { get; set; }

        [Display(Name = "Unidade de Valor")]
        public string UnidadeValor { get; set; }

        [Display(Name = "Ativo")]
        public bool FlagAtivo { get; set; }

        [Display(Name = "Data de Exclusão")]
        public DateTime? DataExclusao { get; set; }

        public int EmpresaID { get; set; }

        public ICollection<OrcamentoServico> OrcamentoServicos { get; set; }
    }
}
