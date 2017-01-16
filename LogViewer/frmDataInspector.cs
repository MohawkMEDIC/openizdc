using LogViewer.Inspectors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogViewer
{
    public partial class frmDataInspector : Form
    {

        private String m_data = null;

        public frmDataInspector(String data)
        {
            InitializeComponent();
            this.m_data = data;
            foreach (var dataInspector in typeof(frmDataInspector).Assembly.GetTypes().Where(o => !o.IsAbstract && typeof(DataInspectorBase).IsAssignableFrom(o)))
                this.cbxViewer.Items.Add(Activator.CreateInstance(dataInspector));
            this.txtDecode.Text = new TextDataInspector().Inspect(data);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbxViewer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.txtDecode.Text = (cbxViewer.SelectedItem as DataInspectorBase).Inspect(this.m_data);
            }
            catch { }
        }
    }
}
