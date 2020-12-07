using EB.Core.DomainServices;
using EB.Core.Entities;
using System;
using System.Net;
using System.Net.Mail;

namespace EB.Infrastructure.Email
{
    public class EmailHelper : IEmailHelper
    {
        const string Email = "esbjergbryghusforsendelse@gmail.com";

        public void SendVerificationEmail(Order order)
        {
            string productTable = "<table><tr><th>Produkt</th><th>Antal</th><th>Subtotal</th></tr>";
            foreach (OrderBeer ob in order.OrderBeers)
            {
                string item = "<tr><th>" + ob.Beer.Name + "</th><th>" + ob.Amount + "</th><th>" + Math.Round((ob.Amount * ob.Beer.Price), 2) + "</th></tr>";
                productTable += item;
            }
            productTable += "</table>";
            string bodyString = "<p>Tak for din ordre fra Esbjerg Bryghus. Du kan se status for din ordre ved <a href=\"http://localhost:4200/login\">at logge ind på din konto<a/>.<br>Hvis du har spørgsmål til ordren, kan du kontakte os på EsbjergBryghusForsendelse@gmail.com</p>" +
                "<h1>Din faktura for ordre #" + order.ID + 1000 + "</h1><br>" + productTable + "<br><h4>Total: " + order.AccumulatedPrice + "</h4>";

            var fromAddress = new MailAddress(Email, "Esbjerg Bryghus");
            var toAddress = new MailAddress(order.Customer.Email, order.Customer.FirstName + " " + order.Customer.LastName);
            const string fromPassword = "aulzazywqewtvzix";
            string subject = "Bestilling af ordre " + (order.ID + 1000);
            string body = bodyString;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                IsBodyHtml = true,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }

        public void SendConfirmationEmail(Order order)
        {
            string productTable = "<table><tr><th>Produkt</th><th>Antal</th><th>Subtotal</th></tr>";
            foreach (OrderBeer ob in order.OrderBeers)
            {
                string item = "<tr><th>" + ob.Beer.Name + "</th><th>" + ob.Amount + "</th><th>" + Math.Round((ob.Amount * ob.Beer.Price), 2) + "</th></tr>";
                productTable += item;
            }
            productTable += "</table>";
            string bodyString = "<p>Tak for din ordre fra Esbjerg Bryghus. Du kan se status for din ordre ved <a href=\"http://localhost:4200/login\">at logge ind på din konto<a/>.<br>Hvis du har spørgsmål til ordren, kan du kontakte os på EsbjergBryghusForsendelse@gmail.com</p>" +
                "<h1>Din faktura for ordre #" + order.ID + 1000 + "</h1><br>" + productTable + "<br><h4>Total: " + order.AccumulatedPrice + "</h4>";

            var fromAddress = new MailAddress(Email, "Esbjerg Bryghus");
            var toAddress = new MailAddress(order.Customer.Email, order.Customer.FirstName + " " + order.Customer.LastName);
            const string fromPassword = "aulzazywqewtvzix";
            string subject = "Bekræftelse af ordre " + (order.ID + 1000);
            string body = bodyString;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                IsBodyHtml = true,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}
