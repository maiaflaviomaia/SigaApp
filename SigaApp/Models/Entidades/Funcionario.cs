using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SigaApp.Models.Entidades
{
    public class Funcionario
    {
        [Key]
        public int FuncionarioID { get; set; }

        [Display(Name = "Data de Cadastro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataCadastro { get; set; }

        [Display(Name = "Nome Completo")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public string NomeCompleto { get; set; }

        [Display(Name = "Telefone Fixo")]
        public string TelefoneFixo { get; set; }

        [Display(Name = "Telefone Celular")]
        public string TelefoneCelular { get; set; }

        [Display(Name = "Estado Civil")]
        public string EstadoCivil { get; set; }

        [Display(Name = "Sexo")]
        public string Sexo { get; set; }

        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
        public string Email { get; set; }

        [Display(Name = "Data de Nascimento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataNascimento { get; set; }

        [Display(Name = "Data de Admissão")]
        public DateTime DataAdmissao { get; set; }

        [Display(Name = "Data de Demissão")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataDemissao { get; set; }

        [Display(Name = "Salário")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "R$ " + "{0:N2}")]
        public decimal Salario { get; set; }

        [Display(Name = "Cargo")]
        public int CargoID { get; set; }

        [Display(Name = "Regime de Trabalho")]
        public string RegimeTrabalho { get; set; }

        [Display(Name = "Jornada de Trabalho")]
        public string JornadaTrabalho { get; set; }

        [Display(Name = "CPF")]
        [StringLength(14)]
        public string CPF { get; set; }

        [Display(Name = "RG")]
        public string RG { get; set; }

        [Display(Name = "Carteira de Trabalho")]
        public string CTPS { get; set; }

        [Display(Name = "PIS/PASEP")]
        public string PisPasep { get; set; }

        [Display(Name = "Carteira de Habilitação - CNH")]
        public string CarteiraHabilitacao { get; set; }

        [Display(Name = "Título de Eleitor")]
        public string TituloEleitor { get; set; }

        [Display(Name = "Sessão Eleitoral")]
        public string SessaoEleitoral { get; set; }

        [Display(Name = "Zona Eleitoral")]
        public string ZonaEleitoral { get; set; }

        [Display(Name = "Ativo?")]
        public bool FlagAtivo { get; set; }

        [Display(Name = "Data de Exclusão")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataExclusao { get; set; }

        [Display(Name = "Observações")]
        [StringLength(500, ErrorMessage = "Máximo de 500 caracteres")]
        public string Observacoes { get; set; }

        public int EmpresaID { get; set; }

        [NotMapped]
        public string EnderecoCompleto { get { return Endereco.Logradouro + ", " + Endereco.Numero + " - " + Endereco.Bairro + ". " + Endereco.Cidade + "/" + Endereco.UF + ". " + Endereco.CEP + ". " + Endereco.Complemento; } }

        [NotMapped]
        public string DadosBancariosCompleto { get { return "Banco: " + DadosBancarios.NomeBanco + ". " + "Agência: " + DadosBancarios.NumeroAgencia + ". " + "Conta: " + DadosBancarios.NumeroConta; } }

        public Endereco Endereco { get; set; }
        public DadosBancarios DadosBancarios { get; set; }
        public Cargo Cargos { get; set; }
    }
}
