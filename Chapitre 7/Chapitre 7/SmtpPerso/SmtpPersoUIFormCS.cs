using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;
namespace SmtpPerso
{
    public partial class SmtpPersoUIFormCS : Form
    {
        private ConnectionManager _connectionManager;
        private IServiceProvider _serviceProvider;
        public SmtpPersoUIFormCS()
        {
            InitializeComponent();
        }
        public void Initialize(ConnectionManager connectionManager, IServiceProvider serviceProvider)
        {
            _connectionManager = connectionManager;
            _serviceProvider = serviceProvider;
        }
        private void SmtpPersoUIFormCS_Load(object sender, EventArgs e)
        {
            try
            {
                this.textBox1.Text = _connectionManager.Properties["Serveur"].GetValue(_connectionManager).ToString();
                this.textBox2.Text = _connectionManager.Properties["Login"].GetValue(_connectionManager).ToString();
                this.textBox3.Text = _connectionManager.Properties["Password"].GetValue(_connectionManager).ToString();
                this.textBox4.Text = _connectionManager.Properties["Port"].GetValue(_connectionManager).ToString();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            { 
                _connectionManager.Properties["Serveur"].SetValue(_connectionManager,this.textBox1.Text);
                _connectionManager.Properties["Login"].SetValue(_connectionManager, this.textBox2.Text);
                _connectionManager.Properties["Password"].SetValue(_connectionManager, this.textBox3.Text);
                _connectionManager.Properties["Port"].SetValue(_connectionManager, int.Parse(this.textBox4.Text));
                this.DialogResult = DialogResult.OK;
                this.Close();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
