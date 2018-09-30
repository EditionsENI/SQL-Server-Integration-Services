using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;

namespace SmtpPerso
{
    [DtsConnection(ConnectionType = "SMTPCusto",
        DisplayName = "SMTP non Exchange",
        Description = "Connection manager suportant l'authentification basique",
        UITypeName = "SmtpPerso.SmtpPersoUI, SmtpPerso, Version=1.0.0.0, Culture=neutral, PublicKeyToken=1d3d0d61bfe2e94b")

    ]
    public class SmtpPerso : ConnectionManagerBase
    {

        private string _serveur = string.Empty;
        private int _port = 25;
        private string _login = string.Empty;
        private string _password = string.Empty;

        public string Serveur { get => _serveur; set => _serveur = value; }
        public int Port { get => _port; set => _port = value; }
        public string Login { get => _login; set => _login = value; }
        public string Password { get => _password; set => _password = value; }

       public override DTSExecResult Validate(IDTSInfoEvents infoEvents)
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                
                try
                {
                    tcpClient.Connect(_serveur, _port);
                    return DTSExecResult.Success;

                }
                catch (Exception e)
                {
                    infoEvents.FireError(0, "Connexion SMTP", e.Message, "", 0);
                    return DTSExecResult.Success;
                }
            }
        }
        public override object AcquireConnection(object txn)
        {
            SmtpClient c = new SmtpClient(_serveur, _port);
            c.Credentials = new System.Net.NetworkCredential(_login, _password);
            c.EnableSsl = true;
            return c;
        }

        public override void ReleaseConnection(object connection)
        {
            if (connection != null)
            {
                SmtpClient c = (SmtpClient)connection;
                
                c.Dispose();
            }
        }

    }
}
