using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SigaApp.Models.Entidades
{
    public class Fornecedor
    {
        [Key]
        public int FornecedorID { get; set; }

        [Display(Name = "Data de Cadastro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataCadastro { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Display(Name = "Razão Social")]
        [MaxLength(250, ErrorMessage = "Máximo de 250 caracteres")]
        [MinLength(2, ErrorMessage = "Mínimo de 02 caracteres")]
        public string RazaoSocial { get; set; }

        [Display(Name = "Nome Fantasia")]
        public string NomeFantasia { get; set; }

        [Display(Name = "Tipo Pessoa")]
        public string TipoPessoa { get; set; }

        [StringLength(14)]
        public string CPF { get; set; }

        [StringLength(18)]
        public string CNPJ { get; set; }

        [Display(Name = "Inscrição Estadual")]
        public string InscricaoEstadual { get; set; }

        [Display(Name = "Inscrição Municipal")]
        public string InscricaoMunicipal { get; set; }

        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
        public string Email { get; set; }

        [Display(Name = "Telefone Fixo")]
        public string TelefoneFixo { get; set; }

        [Display(Name = "Telefone Celular")]
        public string TelefoneCelular { get; set; }

        [Display(Name = "Responsável")]
        public string Responsavel { get; set; }

        [Display(Name = "Ativo?")]
        public bool FlagAtivo { get; set; }

        [Display(Name = "Data de exclusão")]
        public DateTime? DataExclusao { get; set; }

        [Display(Name = "Observações")]
        [StringLength(500, ErrorMessage = "Máximo de 500 caracteres")]
        public string Observacoes { get; set; }

        public int EmpresaID { get; set; }

        [NotMapped]
        public string EnderecoCompleto { get { return Endereco.Logradouro + ", " + Endereco.Numero + " - " + Endereco.Bairro + ". " + Endereco.Cidade + "/" + Endereco.UF + ". " + Endereco.CEP + ". " + Endereco.Complemento; } }

        [NotMapped]
        public string DadosBancariosCompleto { get { return "Banco: " + DadosBancarios.NomeBanco + ". " + "Agência: " + DadosBancarios.NumeroAgencia + ". " + "Conta: " + DadosBancarios.NumeroConta; } }

        public virtual Endereco Endereco { get; set; }
        public virtual DadosBancarios DadosBancarios { get; set; }
    }
}
