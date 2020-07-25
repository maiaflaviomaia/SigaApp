using System;
using System.ComponentModel.DataAnnotations;

namespace SigaApp.Models.Entidades
{
    public class MensagemSite
    {
        [Key]
        public int MensagemID { get; set; }
        public DateTime DataCadastro { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Mensagem { get; set; }
        public string IPUsuario { get; set; }
    }
}
