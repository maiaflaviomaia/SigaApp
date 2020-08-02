using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SigaApp.Models.Entidades
{
    public class Categoria
    {
        public Categoria()
        {
            SomatorioMensal = new List<SomatorioMensal>();
        }

        [Key]
        public int CategoriaID { get; set; }

        [Display(Name = "Data de Cadastro")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataCadastro { get; set; }

        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Tipo { get; set; }

        [Display(Name = "Categoria Pai")]
        public int? CategoriaPai { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(100, ErrorMessage = "Máximo de 100 caracteres")]
        public string Nome { get; set; }

        [Display(Name = "Ativo?")]
        public bool FlagAtivo { get; set; }

        [Display(Name = "Data Exclusão")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataExclusao { get; set; }

        public int EmpresaID { get; set; }

        public ICollection<Categoria> SubCategoria { get; set; }

        public List<SomatorioMensal> SomatorioMensal { get; set; }
    }

    public class SomatorioMensal
    {
        [Key]
        public string MesAno { get; set; }
        public decimal Total { get; set; }
    }

    /*Criada para vizualizar as categorias em estilo TreeView*/
    public class TreeViewNode
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
    }
}

