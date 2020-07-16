using SigaApp.Models.Entidades;
using System.Collections.Generic;

namespace SigaApp.Models.Interfaces
{
    public interface ICategoria : IRepository<Categoria>
    {
        IEnumerable<Categoria> ObterTodasDespesas();
        IEnumerable<Categoria> ObterTodasReceitas();
        IEnumerable<Categoria> ObterCategoriaPai();
        IEnumerable<Categoria> ObterSubCategorias(int id);
    }
}
