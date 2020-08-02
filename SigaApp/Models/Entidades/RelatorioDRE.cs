using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigaApp.Models.Entidades
{
    public class RelatorioDRE
    {
        public RelatorioDRE()
        {
            ListaSubCategorias = new List<Categoria>();
        }

        public int CategoriaPaiID { get; set; }
        public string DescricaoCategoria { get; set; }
        public List<Categoria> ListaSubCategorias { get; set; }
    }
}
