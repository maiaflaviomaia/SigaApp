using System;
using System.ComponentModel.DataAnnotations;

namespace SigaApp.Models.Entidades
{
    public class LogUsuarioLogon
    {
        [Key]
        public int LogUsuarioID { get; set; }
        public DateTime DataCadastro { get; set; }
        public string IP { get; set; }
        public int UsuarioID { get; set; }
        public int EmpresaID { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public string StackTrace { get; set; }
    }
}
