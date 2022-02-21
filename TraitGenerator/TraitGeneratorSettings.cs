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
            PopulateGrid();
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
    
        private void PopulateGrid()
        {
            grdSentenceStructure.Columns.Add("Attribute", "Attribute");
            grdSentenceStructure.Columns.Add("FileName", "File Name");

            var fileOpenButton = new DataGridViewImageColumn();
            fileOpenButton.Name = "btnOpen";
            fileOpenButton.HeaderText = " ";
            fileOpenButton.Width = 30;
            
            fileOpenButton.Image = Image.FromFile("./Resources/folder.jpg");
            
            grdSentenceStructure.Columns.Add(fileOpenButton);

            var deleteButton = new DataGridViewImageColumn();
            deleteButton.Name = "btnDelete";                       
            deleteButton.HeaderText = " ";
            deleteButton.Width = 30;
            deleteButton.Image = Image.FromFile("./Resources/redX.png");

            grdSentenceStructure.Columns.Add(deleteButton);

            if (File.Exists(Constants.ATTRIBUTE_NAME_PATH))
            {
                try
                {
                    List<string> wholeSentenceList = File.ReadAllText(Constants.ATTRIBUTE_NAME_PATH).Split('|').Select(x => x.Trim()).Where(y => y.Length > 0).ToList<string>();

                    int rowIndex = 0;

                    foreach (string attributeFilePath in wholeSentenceList)
                    {
                        DataGridViewTextBoxCell textBox = new DataGridViewTextBoxCell();
                        DataGridViewTextBoxCell fileTextBox = new DataGridViewTextBoxCell();

                        textBox.Value = attributeFilePath.Split(';')[0];
                        fileTextBox.Value = attributeFilePath.Split(';')[1];
                        fileTextBox.ToolTipText = attributeFilePath.Split(';')[2];

                        grdSentenceStructure.Rows.Add(new DataGridViewRow());
                        grdSentenceStructure[0, rowIndex] = textBox;
                        grdSentenceStructure[1, rowIndex] = fileTextBox;

                        rowIndex++;
                    }
                }
                catch
                {
                    MessageBox.Show("Please select a new file.");
                }
            }
        }

        private bool ValidateGrid()
        {
            for(int i = 0; i < grdSentenceStructure.Rows.Count; i++)
            {
                var textBox = ((DataGridViewTextBoxCell)grdSentenceStructure.Rows[i].Cells[0]);
                if (textBox.Value == null)
                {
                    MessageBox.Show("Attribute needs a value.");

                    return false;
                }

                var fileTextBox = ((DataGridViewTextBoxCell)grdSentenceStructure.Rows[i].Cells[1]);
                if (fileTextBox.Value == null)
                {
                    MessageBox.Show("File Path needs a value.");

                    return false;
                }
            }

            return true;
        }

        private void SaveGridData()
        {
            if (grdSentenceStructure.Rows.Count > 0)
            {
                string attributeAndFileName = string.Empty;

                for (int i = 0; i < grdSentenceStructure.Rows.Count; i++)
                {
                    var textBox = ((DataGridViewTextBoxCell)grdSentenceStructure.Rows[i].Cells[0]);
                    var fileTextBox = ((DataGridViewTextBoxCell)grdSentenceStructure.Rows[i].Cells[1]);

                    attributeAndFileName += textBox.Value.ToString() + ";" + fileTextBox.Value.ToString() + ";" + fileTextBox.ToolTipText + "|";
                }

                attributeAndFileName = attributeAndFileName.Remove(attributeAndFileName.Length - 1);

                File.WriteAllText(Constants.ATTRIBUTE_NAME_PATH, attributeAndFileName);
            }
        }


        private void OpenFileDialogBox(DataGridViewTextBoxCell txtBox)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                string strFullPath = string.Empty;

                openFileDialog.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\Dictionary";
                openFileDialog.Filter = "Text Files (*.txt)|*.txt";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtBox.Value = openFileDialog.FileName.Split('\\').Last();
                    txtBox.ToolTipText = openFileDialog.FileName;
                }
            }
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {                   
            if (ValidateGrid())
            {
                SaveGridData();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
      
        private void btnAddRow_Click(object sender, EventArgs e)
        {
            grdSentenceStructure.Rows.Add(new DataGridViewRow());

            DataGridViewTextBoxCell textBox = new DataGridViewTextBoxCell();
            DataGridViewTextBoxCell fileTextBox = new DataGridViewTextBoxCell();
            DataGridViewImageCell buttonBox = new DataGridViewImageCell();
           
            grdSentenceStructure.Rows[grdSentenceStructure.Rows.Count - 1].Cells[0] = textBox;
            grdSentenceStructure.Rows[grdSentenceStructure.Rows.Count - 1].Cells[1] = fileTextBox;
            grdSentenceStructure.Rows[grdSentenceStructure.Rows.Count - 1].Cells[2] = buttonBox;

            ((DataGridViewTextBoxCell)grdSentenceStructure.Rows[grdSentenceStructure.Rows.Count - 1].Cells[1]).ReadOnly = true;
        }

        private void grdSentenceStructure_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (e.ColumnIndex == grdSentenceStructure.Columns["btnDelete"].Index)
            {
                grdSentenceStructure.Rows.RemoveAt(e.RowIndex);
            }
            else if (e.ColumnIndex == grdSentenceStructure.Columns["btnOpen"].Index)
            {
                OpenFileDialogBox((DataGridViewTextBoxCell)grdSentenceStructure.Rows[e.RowIndex].Cells[1]);
            }
        }

        #endregion

    }
}
