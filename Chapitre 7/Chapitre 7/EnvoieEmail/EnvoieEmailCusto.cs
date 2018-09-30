using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Microsoft.SqlServer.Dts.Runtime;

namespace EnvoieEmail
{
    [DtsTask
    (
          DisplayName = "Tâche d’envoi de mail par SMTP non Exchange",
          IconResource = "EnvoieEmail.envelope.ico",
          TaskContact = "Sauget Charles-Henri - chsauget@scop-it.com"
    )]
    public class EnvoieEmailCusto : Task
    {
        private string _objet = string.Empty;
        public string Objet
        {
            get { return _objet; }
            set { _objet = value; }
        }

        private string _message = string.Empty;
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
        private Connections _connections;
        private string _customSmtpConnectionID = string.Empty;
        public string CustomSmtpConnectionID
        {
            get { return _customSmtpConnectionID; }
            set { _customSmtpConnectionID = value; }
        }
        private string _destinataire = string.Empty;
        public string Destinataire
        {
            get { return _destinataire; }
            set { _destinataire = value; }
        }
        private string _expediteur = string.Empty;
        public string Expediteur
        {
            get { return _expediteur; }
            set { _expediteur = value; }
        }
        public override DTSExecResult Validate(Connections connections, VariableDispenser variableDispenser, IDTSComponentEvents componentEvents, IDTSLogging log)
        {
            try
            {
                if (this.Message.Length == 0 || this.Objet.Length == 0 || this.Expediteur.Length == 0 || this.Destinataire.Length == 0)
                {
                    throw new Exception("Merci de remplir tous les champs");
                }

                return DTSExecResult.Success;
            }
            catch(Exception e)
            {
                componentEvents.FireError(0, "Tâche Evoie Email", e.Message, "", 0);
                return DTSExecResult.Failure;
            }

        }

        public override DTSExecResult Execute(Connections connections, VariableDispenser variableDispenser, IDTSComponentEvents componentEvents, IDTSLogging log, object transaction)
        {
            
            try
            {
                ConnectionManager cm = connections[_customSmtpConnectionID];
                SmtpClient c = (SmtpClient)cm.AcquireConnection(transaction);
                using (MailMessage m = new MailMessage(_expediteur, _destinataire, _objet, _message))
                {
                    c.SendMailAsync(m);
         
                }
                return DTSExecResult.Success;


            }
            catch (Exception e)
            {
                componentEvents.FireError(0, "Tâche Evoie Email", e.Message, "", 0);
                return DTSExecResult.Failure;
            }
           
        }

        public override void InitializeTask(Connections connections, VariableDispenser variableDispenser, IDTSInfoEvents events, IDTSLogging log, EventInfos eventInfos, LogEntryInfos logEntryInfos, ObjectReferenceTracker refTracker)
        {
            _connections = connections;
            base.InitializeTask(connections, variableDispenser, events, log, eventInfos, logEntryInfos, refTracker);
        }
    }
}
