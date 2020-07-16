using System;
using System.ComponentModel.DataAnnotations;

namespace SigaApp.Models.Entidades
{
    public class Estudio
    {
        [Key]
        public int EstudioID { get; set; }

        [Display(Name = "Data de Cadastro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataCadastro { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Nome { get; set; }

        [Display(Name = "Descrição Detalhada")]
        public string DescricaoDetalhada { get; set; }

        [Display(Name = "Hora Início")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan HoraFuncionamentoInicio { get; set; }

        [Display(Name = "Hora Fim")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan HoraFuncionamentoFim { get; set; }

        [Display(Name = "Ativo?")]
        public bool FlagAtivo { get; set; }

        [Display(Name = "Data Excllusão")]
        public DateTime? DataExclusao { get; set; }

        public int EmpresaID { get; set; }
    }
}
