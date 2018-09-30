using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;

namespace ForeachWMI 
{
    public partial class ForeachWMIUI : ForEachEnumeratorUI
    {
        ForeachWMI _userEnumerator;
        public ForeachWMIUI()
        {
            InitializeComponent();
        }

        public override void Initialize(ForEachEnumeratorHost FEEHost, IServiceProvider serviceProvider, Connections connections, Variables variables)
        {
            _userEnumerator = (ForeachWMI)FEEHost.InnerObject;
            tbDomain.Text = _userEnumerator.Domain;

        }

        public override void SaveSettings()
        {
            _userEnumerator.Domain = tbDomain.Text;
        }

    }
}
