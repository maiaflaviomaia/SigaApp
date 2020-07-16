using SigaApp.Models.Entidades;
using System.Collections.Generic;

namespace SigaApp.Models.Interfaces
{
    public interface IOrcamentoFornecedor : IRepository<OrcamentoFornecedor>
    {
        IEnumerable<OrcamentoFornecedor> ObterFornecedores(int id);
    }
}
