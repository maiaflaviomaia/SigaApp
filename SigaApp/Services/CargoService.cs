using SigaApp.Models.Entidades;
using System;

namespace SigaApp.Servicos
{
    public class CargoService
    {
        public void PreencherCampos(Cargo cargo)
        {
            try
            {
                if (cargo == null)
                    throw new ArgumentException("Cargo inválido");

                cargo.DataCadastro = DateTime.Now;
                cargo.FlagAtivo = true;
                cargo.DataExclusao = null;

                ValidarCampos(cargo);
            }
            catch (Exception)
            {
                throw;
            }
        }
                

        public void ValidarCampos(Cargo cargo)
        {
            try
            {
                if (cargo == null)
                    throw new ArgumentException("Cargo inválido");

                if (String.IsNullOrEmpty(cargo.DescricaoSumaria))
                    throw new ArgumentException("Campo Descrição é obrigatório");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
