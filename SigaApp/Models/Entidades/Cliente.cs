﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SigaApp.Models.Entidades
{
    public class Cliente
    {
        [Key]
        public int ClienteID { get; set; }

        [Display(Name = "Data de Cadastro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataCadastro { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Display(Name = "Razão Social")]
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
        [StringLength(14)]
        public string TelefoneFixo { get; set; }

        [Display(Name = "Telefone Celular")]
        [StringLength(15)]
        public string TelefoneCelular { get; set; }

        [Display(Name = "Responsável")]
        public string Responsavel { get; set; }

        [Display(Name = "Ativo")]
        public bool FlagAtivo { get; set; }

        [Display(Name = "Data de Exclusão")]
        public DateTime? DataExclusao { get; set; }

        [Display(Name = "Observações")]
        [StringLength(500, ErrorMessage = "Máximo de 500 caracteres")]
        public string Observacoes { get; set; }

        [Display(Name = "Empresa")]
        public int EmpresaID { get; set; }

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

        [Display(Name = "Nome do Banco")]
        public string NomeBanco { get; set; }

        [Display(Name = "Número da Agência")]
        public string NumeroAgencia { get; set; }

        [Display(Name = "Número da Conta")]
        public string NumeroConta { get; set; }

        [Display(Name = "Operação")]
        public string Operacao { get; set; }

        [NotMapped]
        public string Endereco => Logradouro + ", " + Numero + ". " + Bairro + " - " + Cidade + "/" + UF + ". " + CEP;

        [NotMapped]
        public string DadosBancarios => NomeBanco + ". Agência: " +  NumeroAgencia + ". Conta: " + NumeroConta;
    }
}
