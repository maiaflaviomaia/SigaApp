using SigaApp.Models.Entidades;
using System;

namespace SigaApp.Servicos
{
    public class SessaoGravacaoService
    {
        public void PreencherCampos(SessaoGravacao sessao)
        {
            try
            {
                if (sessao == null)
                    throw new ArgumentException("Sessão inválida");

                sessao.FlagAtivo = true;
                sessao.DataCadastro = DateTime.Now;
                sessao.DataExclusao = null;

                ValidarCampos(sessao);
            }
            catch (Exception)
            {
                throw;
            }
        }
                

        public void ValidarCampos(SessaoGravacao sessao)
        {
            try
            {
                if (sessao == null)
                    throw new ArgumentException("Sessão inválida");

                if (sessao.DataFim.Date < sessao.DataInicio.Date)
                    throw new ArgumentException("A Data Fim não pode ser menor que a Data Inicio");

                if (sessao.HoraFim < sessao.HoraInicio)
                    throw new ArgumentException("A Hora Fim não pode ser menor que a Hora Inicio");

                if (sessao.DataInicio.Date > DateTime.Now.Date)
                    throw new ArgumentException("A Data Inicio não pode ser maior que a data atual");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
