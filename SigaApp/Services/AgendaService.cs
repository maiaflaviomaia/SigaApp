using SigaApp.Models.Entidades;
using System;

namespace SigaApp.Servicos
{
    public class AgendaService
    {
        public void PreencherCampos(Agenda agenda)
        {
            if (agenda == null)
                throw new ArgumentException("Agenda inválida");

            agenda.DataCadastro = DateTime.Now;
            agenda.FlagAtivo = true;
            agenda.DataExclusao = null;

            ValidarCampos(agenda);
        }

        public void ValidarCampos(Agenda agenda)
        {
            if (agenda == null)
                throw new ArgumentException("Agenda inválida");

            if (agenda.Hora.TotalMinutes < 0)
                throw new ArgumentException("Hora inválida");

            if  (agenda.DataEvento.Date < DateTime.Now.Date)
                throw new ArgumentException("Não é permitido criar um agendamento para dias anteriores à hoje");
        }
    }
}
