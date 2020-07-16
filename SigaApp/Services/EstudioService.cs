using SigaApp.Models.Entidades;
using System;

namespace SigaApp.Servicos
{
    public class EstudioService
    {
        public void PreencherCampos(Estudio estudio)
        {
            try
            {
                if (estudio == null)
                    throw new ArgumentException("Estúdio inválido");

                estudio.FlagAtivo = true;
                estudio.DataCadastro = DateTime.Now;
                estudio.DataExclusao = null;

                ValidarCampos(estudio);
            }
            catch (Exception)
            {
                throw;
            }
        }
         

        public void ValidarCampos(Estudio estudio)
        {
            try
            {
                if (String.IsNullOrEmpty(estudio.Nome))
                    throw new ArgumentException("O campo Nome é obrigatório");

                if (estudio.HoraFuncionamentoInicio == null)
                    throw new ArgumentException("O campo Hora Inicio deve ser preenchido");

                if (estudio.HoraFuncionamentoFim == null)
                    throw new ArgumentException("O campo Hora Fim deve ser preenchido");

                if (estudio.HoraFuncionamentoInicio > estudio.HoraFuncionamentoFim)
                    throw new ArgumentException("A Hora Inicio não pode ser maior que a Hora Fim");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
