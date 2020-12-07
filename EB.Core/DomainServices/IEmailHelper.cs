using EB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.DomainServices
{
    public interface IEmailHelper
    {
       void SendVerificationEmail(Order order);
       void SendConfirmationEmail(Order order);
    }
}
