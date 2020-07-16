using System;
using System.ComponentModel.DataAnnotations;

namespace SigaApp.Models.Entidades
{
    public class SessaoGravacao
    {
        [Key]
        public int SessaoID { get; set; }

        [Display(Name = "Data de Cadastro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataCadastro { get; set; }

        [Display(Name = "Data Início")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public DateTime DataInicio { get; set; }

        [Display(Name = "Data Fim")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public DateTime DataFim { get; set; }

        [Display(Name = "Hora Início")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public TimeSpan HoraInicio { get; set; }

        [Display(Name = "Hora Fim")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public TimeSpan HoraFim { get; set; }

        [Display(Name = "Estúdio")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public int EstudioID { get; set; }

        [Display(Name = "Cliente")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public int ClienteID { get; set; }

        [Display(Name = "Fornecedor")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public int FornecedorID { get; set; }

        [Display(Name = "Serviço Prestado")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public int ServicoPrestadoID { get; set; }

        [Display(Name = "Observações")]
        public string Observacoes { get; set; }

        [Display(Name = "Ativo?")]
        public bool FlagAtivo { get; set; }

        [Display(Name = "Data de Exclusão")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataExclusao { get; set; }

        public int EmpresaID { get; set; }

        public Cliente Cliente { get; set; }
        public Fornecedor Fornecedor { get; set; }
        public ServicoPrestado servicoPrestado { get; set; }
        public Estudio Estudio { get; set; }
    }
}
