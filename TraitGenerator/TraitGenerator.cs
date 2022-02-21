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
   
    public partial class TraitGenerator : Form
    {       
        List<KeyValuePair<string, List<string>>> MasterAttributeNamesList;
        Random RandomNumberGenerator = new Random();
      
        public TraitGenerator()
        {
            InitializeComponent();
            this.ActiveControl = txtNumberOfNames;
            this.Text = "Trait Generator v" + Application.ProductVersion;
        }

        #region Functions

        private bool PopulateLists()
        {           
            MasterAttributeNamesList = new List<KeyValuePair<string, List<string>>>();

            GenerateLists();

            return ValidateLists(MasterAttributeNamesList);
        }

        private void GenerateLists()
        {         
            List<string> lstAttibutesList = ReadDelineatedFile(Constants.ATTRIBUTE_NAME_PATH, '|');

            foreach (string attributeFilePath in lstAttibutesList)
            {
                KeyValuePair<string, List<string>> lstAttribute = new KeyValuePair<string, List<string>>(attributeFilePath.Split(';')[0], ReadDelineatedFile(attributeFilePath.Split(';')[2], '|'));

                MasterAttributeNamesList.Add(lstAttribute);
            }
        }

        private bool ValidateLists(List<KeyValuePair<string, List<string>>> p_lstSentenceParts)
        {
            string strErrorMessage = string.Empty;

            foreach (KeyValuePair<string, List<string>> sentencePart in p_lstSentenceParts)
            {
                if (sentencePart.Value.Count == 0)
                {
                    strErrorMessage += "Go into your settings to select " + DetermineIfVowel(sentencePart.Key.ToString()) + sentencePart.Key.ToString() + " list that is not empty.\n";
                }
            }

            if (strErrorMessage.Length > 0)
            {
                MessageBox.Show(strErrorMessage, "Error Generating Names");

                return false;
            }

            return true;
        }

        private string DetermineIfVowel(string p_strWord)
        {
            if (p_strWord.StartsWith("A") || p_strWord.StartsWith("E") || p_strWord.StartsWith("O") || p_strWord.StartsWith("I") || p_strWord.StartsWith("U") || p_strWord.StartsWith("Y"))
            {
                return "an ";
            }

            return "a ";
        }

        private List<KeyValuePair<string, string>> DetermineOrder()
        {
            int randomNumber;

            List<KeyValuePair<string, string>> traitSentence = new List<KeyValuePair<string, string>>();

            foreach (KeyValuePair<string, List<string>> sentence in MasterAttributeNamesList)
            {
                randomNumber = RandomNumberGenerator.Next(0, sentence.Value.Count);
                traitSentence.Add(new KeyValuePair<string, string>(sentence.Key, sentence.Value[randomNumber]));
            }

            return traitSentence;
        }

        private void PrintName(List<KeyValuePair<string, string>> traitList, int index)
        {
            txtNames.AppendText("Run " + (index + 1) + Environment.NewLine);
            foreach (KeyValuePair<string, string> sentence in traitList)
            {
                txtNames.AppendText(sentence.Key.ToString() + ": " + sentence.Value + Environment.NewLine);
            }

            txtNames.AppendText(Environment.NewLine);
        }

        private List<string> ReadDelineatedFile(string p_strPath, char p_chrDelimiter)
        {
            if (File.Exists(p_strPath))
            {
                string objFileContents = File.ReadAllText(p_strPath);

                if (objFileContents.Length != 0)
                {
                    return objFileContents.Split(p_chrDelimiter).Select(x => x.Trim()).ToList<string>().Where(y => y.Length != 0).ToList<string>();
                }
            }

            return new List<string>();
        }

        #endregion

        #region Events

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (PopulateLists())
            {
                txtNames.Clear();

                for (int i = 0; i < int.Parse(txtNumberOfNames.Text); i++)
                {
                    List<KeyValuePair<string, string>> traitList = DetermineOrder();
                    PrintName(traitList, i);
                }
            }
        }

        private void txtNumberOfNames_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            else if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                btnGenerate.PerformClick();
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            TraitGeneratorSettings form = new TraitGeneratorSettings();

            form.ShowDialog();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            About form = new About();

            form.ShowDialog();
        }

        #endregion


    }  
}
