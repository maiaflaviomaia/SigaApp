using System.Security.Cryptography;
using System.Text;

namespace SigaApp.Utils
{
    public class Criptografia
    {
        public HashAlgorithm _algoritmo;

        public Criptografia(HashAlgorithm algoritmo)
        {
            _algoritmo = algoritmo;
        }

        public string CriptografarSenha(string senha)
        {
            var encodedValue = Encoding.UTF8.GetBytes(senha);
            var senhaCriptografada = _algoritmo.ComputeHash(encodedValue);

            var sb = new StringBuilder();

            foreach (var caracter in senhaCriptografada)
            {
                sb.Append(caracter.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
