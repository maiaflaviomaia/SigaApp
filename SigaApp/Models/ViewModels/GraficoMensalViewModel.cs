using Microsoft.AspNetCore.Mvc;

namespace SigaApp.Models.Entidades
{
    public class GraficoMensalViewModel
    {
        public JsonResult SomaReceitasMes { get; set; }
        public JsonResult SomaDespesasMes { get; set; }
    }
}
