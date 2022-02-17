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
        List<KeyValuePair<SentenceStructure, List<string>>> MasterNamesList;

        Random RandomNumberGenerator = new Random();
      
        public TraitGenerator()
        {
            InitializeComponent();
            this.ActiveControl = txtNumberOfNames;
        }

        #region Functions

        private bool PopulateLists()
        {
            List<string> PrototypeList = ReadCommaDelineatedFile(ConfigurationManager.AppSettings[SentenceStructure.Prototype.ToString()]);
            List<string> AgeList = ReadCommaDelineatedFile(ConfigurationManager.AppSettings[SentenceStructure.Age.ToString()]);
            List<string> WantList = ReadCommaDelineatedFile(ConfigurationManager.AppSettings[SentenceStructure.Want.ToString()]);
            List<string> FlawList = ReadCommaDelineatedFile(ConfigurationManager.AppSettings[SentenceStructure.Flaw.ToString()]);
            List<string> FearList = ReadCommaDelineatedFile(ConfigurationManager.AppSettings[SentenceStructure.Fear.ToString()]);

            MasterNamesList = new List<KeyValuePair<SentenceStructure, List<string>>>
            {
                new KeyValuePair<SentenceStructure, List<string>> (SentenceStructure.Prototype, PrototypeList),
                new KeyValuePair<SentenceStructure, List<string>> (SentenceStructure.Age, AgeList),
                new KeyValuePair<SentenceStructure, List<string>> (SentenceStructure.Want, WantList),
                new KeyValuePair<SentenceStructure, List<string>> (SentenceStructure.Flaw, FlawList),
                new KeyValuePair<SentenceStructure, List<string>> (SentenceStructure.Fear, FearList)
            };

            return ValidateLists(MasterNamesList);
        }

        private bool ValidateLists(List<KeyValuePair<SentenceStructure, List<string>>> p_lstSentenceParts)
        {
            string strErrorMessage = string.Empty;

            foreach (KeyValuePair<SentenceStructure, List<string>> sentencePart in p_lstSentenceParts)
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

        private List<KeyValuePair<SentenceStructure, string>> DetermineOrder()
        {

            int randomNumber;

            List<KeyValuePair<SentenceStructure, string>> traitSentence = new List<KeyValuePair<SentenceStructure, string>>();

            foreach (KeyValuePair<SentenceStructure, List<string>> sentence in MasterNamesList)
            {
                randomNumber = RandomNumberGenerator.Next(0, sentence.Value.Count);
                traitSentence.Add(new KeyValuePair<SentenceStructure, string>(sentence.Key, sentence.Value[randomNumber]));
            }

            return traitSentence;
        }

        private void PrintName(List<KeyValuePair<SentenceStructure, string>> traitList)
        {
            foreach (KeyValuePair<SentenceStructure, string> sentence in traitList)
            {
                txtNames.AppendText(sentence.Key.ToString() + ": " + sentence.Value + Environment.NewLine);
            }

            txtNames.AppendText(Environment.NewLine);
        }

        private List<string> ReadCommaDelineatedFile(string p_strPath)
        {
            if (File.Exists(p_strPath))
            {
                string objFileContents = File.ReadAllText(p_strPath);

                if (objFileContents.Length != 0)
                {
                    return objFileContents.Split(',').Select(x => x.Trim()).ToList<string>();
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
                    List<KeyValuePair<SentenceStructure, string>> traitList = DetermineOrder();
                    PrintName(traitList);
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

        #endregion
    }  
}
