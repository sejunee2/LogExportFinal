using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogExport
{
    public partial class Form2 : Form
    {
        public Form2(List<string> logList)
        {
            InitializeComponent();
            logListBox.Items.Clear();
            try
            {
                foreach (string log in logList)
                {
                    logListBox.Items.Add(log);
                }
            }
            catch 
            { 
            }
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
        }
    }
}
