using Phoenix.Runtime;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Phoenix.Gui.Pages
{
    partial class AddDialog : Form
    {
        public AddDialog()
        {
            InitializeComponent();
        }

        [Category("Appearance")]
        public string Shortcut
        {
            get { return keyBox.Key; }
            set { keyBox.Key = value; ; }
        }
        public string Command
        {
            get { return commandBox.Text; }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void keyBox_KeyChanged(object sender, EventArgs e)
        {
            bool registered = RuntimeCore.Hotkeys.Contains(keyBox.Key);
            warningLabel.Visible = registered;
            okButton.Enabled = !registered && keyBox.Key != Keys.None.ToString() ;
        }
    }
}