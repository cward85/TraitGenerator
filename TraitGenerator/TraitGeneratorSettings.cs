using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TraitGenerator
{
    public partial class TraitGeneratorSettings : Form
    {
        public TraitGeneratorSettings()
        {
            InitializeComponent();          
            PopulateTextBoxes();            
        }
     
        #region Functions

        private void OpenFileDialogBox(string p_strConfigSection, TextBox txtBox)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                string strFullPath = string.Empty;

                openFileDialog.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\Dictionary";
                openFileDialog.Filter = "Text Files (*.txt)|*.txt";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    strFullPath = openFileDialog.FileName;
                    txtBox.Text = openFileDialog.FileName.Split('\\').Last();
                    var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    var settings = config.AppSettings.Settings;
                    settings[p_strConfigSection].Value = strFullPath;
                    config.Save();

                    ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
                }
            }
        }

        private void PopulateTextBoxes()
        {
            txtPrototype.Text = ConfigurationManager.AppSettings[SentenceStructure.Prototype.ToString()].Split('\\').Last();
            txtAge.Text = ConfigurationManager.AppSettings[SentenceStructure.Age.ToString()].Split('\\').Last();            
            txtWant.Text = ConfigurationManager.AppSettings[SentenceStructure.Want.ToString()].Split('\\').Last();
            txtFlaw.Text = ConfigurationManager.AppSettings[SentenceStructure.Flaw.ToString()].Split('\\').Last();
            txtFear.Text = ConfigurationManager.AppSettings[SentenceStructure.Fear.ToString()].Split('\\').Last();
            
        }     
      
        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {                   
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        private void btnPrototypeFolder_Click(object sender, EventArgs e)
        {
            OpenFileDialogBox(SentenceStructure.Prototype.ToString(), txtPrototype);
        }

        private void btnAgeFolder_Click(object sender, EventArgs e)
        {
            OpenFileDialogBox(SentenceStructure.Age.ToString(), txtAge);
        }

        private void btnWantFolder_Click(object sender, EventArgs e)
        {
            OpenFileDialogBox(SentenceStructure.Want.ToString(), txtWant);
        }

        private void btnFlawFolder_Click(object sender, EventArgs e)
        {
            OpenFileDialogBox(SentenceStructure.Flaw.ToString(), txtFlaw);
        }

        private void btnFearFolder_Click(object sender, EventArgs e)
        {
            OpenFileDialogBox(SentenceStructure.Fear.ToString(), txtFear);
        }
    }
}
