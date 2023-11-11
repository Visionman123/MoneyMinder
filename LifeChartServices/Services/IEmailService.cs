using LifeChartServices.Models;

namespace LifeChartServices.Services
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}