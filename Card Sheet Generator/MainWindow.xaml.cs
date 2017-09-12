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

        string dataSheetName = "";
        string outputFileName = "testfile.html";
        string costImage = "cardCost.png";
        string htmlContent;

        string cardTemplate;

        public MainWindow()
        {
            InitializeComponent();
            rtb_Console.AppendText("Program Started");
        }

        private void ConsoleMakeText(string TextToMake)
        {
            rtb_Console.Document.Blocks.Clear();
            rtb_Console.AppendText(TextToMake);
        }

        private void ConsoleAddText(string TextToAdd)
        {
            rtb_Console.AppendText("\n" + TextToAdd);
        }

        private void loadSourceFile()
        {
            ConsoleMakeText("Source file loading. Please wait...");
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader("cardSheetHead.txt");
            while ((line = file.ReadLine()) != null)
            {
                htmlContent += (line + "\n");
            }
            file.Close();

            line = "";
            file = new System.IO.StreamReader("cardContent.txt");
            while ((line = file.ReadLine()) != null)
            {
                cardTemplate += (line + "\n");
            }
            file.Close();
            ConsoleAddText("Source file load complete");
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
                ConsoleAddText("Deleting old output file.");
            }



            if (dataSheetName != null && dataSheetName != "")
            {
                ConsoleAddText("Generating output file. Please wait...");
                string line = "";
                string outputFileContent = "";
                outputFileContent += htmlContent;

                string templateCopy = "";

                int lineCount = 0;
                System.IO.StreamReader file = new System.IO.StreamReader(dataSheetName);
                while ((line = file.ReadLine()) != null)
                {
                    //skips first line
                    if (lineCount > 0)
                    {

                        //Creates an array of Card details from the line
                        string[] cardDetails = line.Split('\t');

                        //Creates number of copies based on print column.
                        //int numCopies = 0;
                        //if (cardDetails[0] != "")
                        //{
                        //    numCopies = Int32.Parse(cardDetails[0]);
                        //}

                        //for (int i = 0; i < numCopies; i++)
                        //{
                            //Adds default template to the temporary copy
                            templateCopy = "";
                            templateCopy += cardTemplate;

                            //Replace all elements in HTML with parts from the file.
                            templateCopy = templateCopy.Replace("REPLACECOSTIMAGE", costImage);
                            templateCopy = templateCopy.Replace("REPLACECOST", cardDetails[1]);
                            templateCopy = templateCopy.Replace("REPLACENAME", cardDetails[3]);
                            templateCopy = templateCopy.Replace("REPLACETYPE", cardDetails[4]);
                            templateCopy = templateCopy.Replace("REPLACESOURCE", cardDetails[5]);
                            templateCopy = templateCopy.Replace("REPLACEABILITY1", cardDetails[6]);
                            templateCopy = templateCopy.Replace("REPLACEFACTION", cardDetails[7]);
                            templateCopy = templateCopy.Replace("REPLACECARDIMAGE", cardDetails[8]);
                            templateCopy = templateCopy.Replace("REPLACEOVERLAY", cardDetails[9]);

                            outputFileContent += templateCopy;
                        //}
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
                file.Close();
                ConsoleAddText("Generating output file complete.");
            }
        }
    }
}
