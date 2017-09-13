using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Card_Sheet_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string dataSheetName;
        string htmlContent;
        string cardTemplate;
        string outputFileName = "testfile.html";
        string costImage = "cardCost.png";


        public MainWindow()
        {
            InitializeComponent();
            ConsoleClear();
        }

        private void ConsoleAdd(string TextToAdd)
        {
            rtb_Console.AppendText(TextToAdd + "\r");
        }

        private void ConsoleClear()
        {
            rtb_Console.Document.Blocks.Clear();
        }

        private void loadSourceFile()
        {
            //Reset Vars from previous run through
            cardTemplate = "";
            htmlContent = "";

            ConsoleAdd("Source file loading. Please wait...");

            //Read from source file: cardSheetHead.txt
            string line = "";
            System.IO.StreamReader file = new System.IO.StreamReader("cardSheetHead.txt");
            while ((line = file.ReadLine()) != null)
            {
                htmlContent += (line + "\n");
            }
            file.Close();

            //Reset line var
            line = "";

            //Read from source file: cardContent.txt
            file = new System.IO.StreamReader("cardContent.txt");
            while ((line = file.ReadLine()) != null)
            {
                cardTemplate += (line + "\n");
            }
            file.Close();
            ConsoleAdd("Source file load complete");
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            loadSourceFile();
            //// Create OpenFileDialog 
            // Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            //// Display OpenFileDialog by calling ShowDialog method 
            //  Nullable<bool> result = dlg.ShowDialog();

            //// Get the selected file name and display in a TextBox 
            //if (result == true)
            //{
            //    // Open document 
            //    dataSheetName = dlg.FileName;                        
            //}

            dataSheetName = "Card Library - Sheet1.tsv";

            //Deletes outputFileName if found
            if (System.IO.File.Exists(outputFileName))
            {
                System.IO.File.Delete(outputFileName);
                ConsoleAdd("Deleting old output file: " + outputFileName);
            }

            if (dataSheetName != null && dataSheetName != "")
            {
                ConsoleAdd("Generating output file: " + outputFileName);
                string line = "";
                string outputFileContent = "";
                bool numFail = false;
                outputFileContent += htmlContent;

                string templateCopy = "";

                int lineCount = 0;
                System.IO.StreamReader file = new System.IO.StreamReader(dataSheetName);
                while ((line = file.ReadLine()) != null)
                {
                    //skips first line
                    if (lineCount > 0)
                    {
                        if (numFail == false)
                        {
                            //Creates an array of Card details from the line
                            string[] cardDetails = line.Split('\t');

                            //Number of copies per card checker
                            int numCopies = 0;

                            //If ignoring the print column from the TSV
                            if (chkBox_overridePrintNum.IsChecked == true)
                            {
                                try
                                {
                                    numCopies = Int32.Parse(txtBox_overridePrintNum.Text);
                                }
                                catch
                                {
                                    numFail = true;
                                }
                            }

                            // Else create number of copies based on print column. (Default)
                            else
                            {
                                if (cardDetails[0] != "")
                                {
                                    try
                                    {
                                        numCopies = Int32.Parse(cardDetails[0]);
                                    }
                                    catch
                                    {
                                        numFail = true;
                                    }
                                }
                            }

                            //Test for reasonable number
                            if (numCopies > 10 || numCopies < 0)
                            {
                                numFail = true;
                            }

                            //Error test
                            if (numFail == true)
                            {
                                ConsoleAdd("Invalid number of copies requested: " + numCopies);
                                ConsoleAdd("Printing ZERO instead");
                                numCopies = 0;
                            }

                            //Image substitutions for CardDetails[5] (source)
                            string imageReplacementSource = "";

                            switch (cardDetails[5])
                            {
                                case "Bio":
                                    imageReplacementSource = "bio.png";
                                    break;
                                case "Tech":
                                    imageReplacementSource = "tech.png";
                                    break;
                                case "Mystical":
                                    imageReplacementSource = "mystical.png";
                                    break;
                                default:
                                    imageReplacementSource = "cardCost.png";
                                    break;                                    
                            }

                            //Main replacement loop to be run on each line (CARD)
                                for (int i = 0; i < numCopies; i++)
                            {
                                //Adds default template to the temporary copy
                                templateCopy = "";
                                templateCopy += cardTemplate;

                                //Replace all elements in HTML with parts from the file.
                                templateCopy = templateCopy.Replace("REPLACECOSTIMAGE", costImage);
                                templateCopy = templateCopy.Replace("REPLACECOST", cardDetails[1]);
                                templateCopy = templateCopy.Replace("REPLACENAME", cardDetails[3]);
                                templateCopy = templateCopy.Replace("REPLACETYPE", cardDetails[4]);
                               // templateCopy = templateCopy.Replace("REPLACESOURCE", cardDetails[5]);
                                templateCopy = templateCopy.Replace("REPLACEIMAGESOURCE", imageReplacementSource);
                                templateCopy = templateCopy.Replace("REPLACEABILITY1", cardDetails[6]);
                                templateCopy = templateCopy.Replace("REPLACEFACTION", cardDetails[7]);
                                templateCopy = templateCopy.Replace("REPLACECARDIMAGE", cardDetails[8]);
                                templateCopy = templateCopy.Replace("REPLACEOVERLAY", cardDetails[9]);

                               
                                


                                outputFileContent += templateCopy;
                            }
                        }
                    }
                    else
                    {
                        lineCount++;
                    }

                }
                file.Close();

                outputFileContent += "</body> </ html > ";

                System.IO.StreamWriter fileOut = new System.IO.StreamWriter(outputFileName);
                fileOut.Write(outputFileContent);
                fileOut.Close();
                ConsoleAdd("Generating output file complete.");
                ConsoleAdd("--------------------------------");

                if (numFail != true)
                {
                    System.Diagnostics.Process.Start(outputFileName);
                }
            }
        }

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // set the current caret position to the end
            rtb_Console.ScrollToEnd();
        }

        private void chkBox_ignorePrintNum_ToggleCheck(object sender, RoutedEventArgs e)
        {
            //Toggles the print # 
            if (lbl_overrideLabel.IsEnabled == true)
            {
                lbl_overrideLabel.IsEnabled = false;
                txtBox_overridePrintNum.IsEnabled = false;
            }
            else
            {
                lbl_overrideLabel.IsEnabled = true;
                txtBox_overridePrintNum.IsEnabled = true;
            }
        }


    }
}
