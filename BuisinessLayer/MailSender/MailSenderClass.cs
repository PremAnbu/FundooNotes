using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.MailSender
{
    public static class MailSenderClass
    {
        public static void sendMail(String ToMail, String otp)
        {
            System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
            try
            {
                mailMessage.From = new System.Net.Mail.MailAddress("m_raghu@outlook.com", "FUNDOO NOTES");
                mailMessage.To.Add(ToMail);
                mailMessage.Subject = "Change password for Fundoo Notes";
                mailMessage.Body = "This is your otp please enter to change password "+otp;
                mailMessage.IsBodyHtml = true;
                System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient("smtp-mail.outlook.com");

                // Specifies how email messages are delivered. Here Email is sent through the network to an SMTP server.
                smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                // Set the port for Outlook's SMTP server
                smtpClient.Port = 587; // Outlook SMTP port for TLS/STARTTLS

                // Enable SSL/TLS
                smtpClient.EnableSsl = true;

                string loginName = "m_raghu@outlook.com";
                string loginPassword = "R@ghu2k01";

                System.Net.NetworkCredential networkCredential = new System.Net.NetworkCredential(loginName, loginPassword);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = networkCredential;

                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught: " + ex.Message);
            }
            finally
            {
                mailMessage.Dispose();
            }
        }
    }
    }
