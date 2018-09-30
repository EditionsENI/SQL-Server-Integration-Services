using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Dts.Runtime;
using System.Management;

namespace ForeachWMI
{
    [DtsForEachEnumerator(DisplayName = "Enumerateur WMI",
      Description = "A managed enumerator",
      UITypeName = "ForeachWMI.ForeachWMIUI, ForeachWMI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=1d3d0d61bfe2e94b",
      ForEachEnumeratorContact = "Charles-Henri Sauget - chsauget@scop-it.com")]
    public class ForeachWMI : ForEachEnumerator
    {
        private string _domain = string.Empty;

        public string Domain { get => _domain; set => _domain = value; }


        public override object GetEnumerator(Connections connections, VariableDispenser variableDispenser, IDTSInfoEvents events, IDTSLogging log)
        {
            ArrayList users = new ArrayList();
            SelectQuery userQuery = new SelectQuery("Win32_UserAccount", string.Format("Domain='{0}'", _domain));
            try
            {
                ManagementObjectSearcher userSearcher = new ManagementObjectSearcher(userQuery);
                foreach (ManagementObject mo in userSearcher.Get())
                    users.Add(mo["Name"].ToString());

            }
            catch (Exception ex)
            {
                events.FireError(0, "ForEachUserEnumerator", ex.Message, ex.HelpLink, 0);
            }
            return users;
        }


    }
}
