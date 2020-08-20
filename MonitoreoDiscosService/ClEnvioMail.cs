using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Data;

namespace MonitoreoDiscosService
{
    class ClEnvioMail
    {
        public string Host = "mail.server.net";
        public int Puerto = 25;

        public bool EnvioMail(string x_FromSend, DataTable x_ToReceive, string x_Subject, string x_Body)
        {
            SmtpClient SmtpServer = new SmtpClient(Host, Puerto);
            MailMessage Correo = new MailMessage();
            Correo.From = new MailAddress(x_FromSend);
            for (int i = 0; i < x_ToReceive.Rows.Count; i++)
            {
                Correo.To.Add(x_ToReceive.Rows[i][0].ToString().Trim());
            }
            Correo.Subject = x_Subject;
            Correo.Body = x_Body;
            Correo.BodyEncoding = Encoding.GetEncoding("iso-8859-1");
            Correo.IsBodyHtml = false;
            Correo.Priority = MailPriority.High;
            try
            {
                SmtpServer.Send(Correo);
                Correo.Dispose();
                return true;
            }
            catch
            {
                Correo.Dispose();
                return false;
            }
        }
    }
}
