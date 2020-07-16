using System;
using System.ComponentModel.DataAnnotations;

namespace SigaApp.Models.Entidades
{
    public class Agenda
    {
        [Key]
        public int AgendaID { get; set; }

        [Display(Name = "Data de Cadastro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataCadastro { get; set; }

        [Display(Name = "Título")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(100, ErrorMessage = "Máximo de 100 caracteres")]
        public string Titulo { get; set; }

        [Display(Name = "Cliente")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public int? ClienteID { get; set; }

        [Display(Name = "Estúdio")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public int? EstudioID { get; set; }

        [Display(Name = "Data")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public DateTime DataEvento { get; set; }

        [Display(Name = "Hora")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public TimeSpan Hora { get; set; }

        [Display(Name = "Observações")]
        [StringLength(250, ErrorMessage = "Máximo de 250 caracteres")]
        public string Observacao { get; set; }

        [Display(Name = "Ativo?")]
        public bool FlagAtivo { get; set; }

        [Display(Name = "Data de Exclusão")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataExclusao { get; set; }

        [Display(Name = "Empresa")]
        public int EmpresaID { get; set; }

        public Cliente Cliente { get; set; }
        public Estudio Estudio { get; set; }
    }
}
