using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;

namespace SmtpPerso
{
    class SmtpPersoUI : IDtsConnectionManagerUI
    {
        ConnectionManager _connectionManager;
        IServiceProvider _serviceProvider;


        public void Delete(IWin32Window parentWindow)
        {
            throw new NotImplementedException();
        }

        public bool Edit(IWin32Window parentWindow, Connections connections, ConnectionManagerUIArgs connectionUIArg)
        {
            SmtpPersoUIFormCS smtpPersoUIForm = new SmtpPersoUIFormCS();

            smtpPersoUIForm.Initialize(_connectionManager, _serviceProvider);
            if (smtpPersoUIForm.ShowDialog(parentWindow) == DialogResult.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Initialize(ConnectionManager connectionManager, IServiceProvider serviceProvider)
        {
           
            _connectionManager = connectionManager;
            _serviceProvider = serviceProvider;
        }

        public bool New(IWin32Window parentWindow, Connections connections, ConnectionManagerUIArgs connectionUIArg)
        {
            return this.Edit(parentWindow, connections, connectionUIArg);
        }
    }
}
