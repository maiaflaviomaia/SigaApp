using SigaApp.Context;
using SigaApp.Models.Entidades;
using SigaApp.Models.Interfaces;
using System;

namespace SigaApp.Repository
{
    public class MensagemSiteRepository : IMensagemSite
    {
        private readonly SigaContext _contexto;

        public MensagemSiteRepository(SigaContext contexto)
        {
            _contexto = contexto;
        }

        public void Inserir(MensagemSite objeto)
        {
            try
            {
                _contexto.MensagensSite.Add(objeto);
                _contexto.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
