using EB.Core.Entities;

namespace EB.Core.DomainServices
{
    public interface IEmailHelper
    {
       void SendVerificationEmail(Order order);
       void SendConfirmationEmail(Order order);
    }
}
