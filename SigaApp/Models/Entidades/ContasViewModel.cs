using System.Collections.Generic;

namespace SigaApp.Models.Entidades
{
    public class ContasViewModel
    {
        public IEnumerable<ContaPagar> ContasPagar { get; set; }
        public IEnumerable<ContaReceber> ContasReceber { get; set; }
    }
}
