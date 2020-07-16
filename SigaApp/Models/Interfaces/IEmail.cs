using System.Threading.Tasks;

namespace SigaApp.Models.Interfaces
{
    public interface IEmail
    {
        Task EnviarEmailAsync(string email, string assunto, string mensagem);
    }
}
