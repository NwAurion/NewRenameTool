using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NewRenameTool.GUI
{
    /// <summary>
    /// Interaktionslogik für SimpleGUI.xaml
    /// </summary>
    public partial class SimpleGUI : UserControl
    {
        public SimpleGUI()
        {
            InitializeComponent();
            this.DataContext = this;

        }

        private ObservableCollection<FileInfo> _files = new ObservableCollection<FileInfo>();
        public ObservableCollection<FileInfo> Files
        {
            get { return _files; }
        }


        #region Events

        private void DropBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                var listbox = sender as ListBox;
                listbox.Background = new SolidColorBrush(Color.FromRgb(155, 155, 155));
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }


        private void DropBox_DragLeave(object sender, DragEventArgs e)
        {
            var listbox = sender as ListBox;
            listbox.Background = new SolidColorBrush(Color.FromRgb(226, 226, 226));
        }


        private void DropBox_Drop(object sender, DragEventArgs e)
        {
            // clear old data
            _files.Clear();

            // local list, needed to order files
            List<FileInfo> fileList = new List<FileInfo>();

            // get new data
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string filePath in files)
                {
                    // buffer the file infos
                    fileList.Add(new FileInfo(filePath));
                }
            }

            // order files
            IEnumerable<FileInfo> sorted = fileList.CustomSortFileInfo();

            // transform back to list so we can handle it more easily
            List<FileInfo> sortedList = sorted.ToList<FileInfo>();

            // add files to the collection
            sortedList.ForEach(file => _files.Add(file));

            (sender as ListBox).Background = new SolidColorBrush(Color.FromRgb(226, 226, 226));

            e.Handled = true;
        }

        #endregion events

        #region buttons

        private void ButtonRename_Click(object sender, RoutedEventArgs e)
        {
            int paddingLength = 0;
            double highestNumber = 0;
            double currentNumber = 0;

            // determine padding via filenames
            foreach (FileInfo file in _files)
            {
                // need to use only the base number, as we can ignore the decimal digits when dealing with the padding
                var regex = new Regex(@"[0-9]+");
                string name = file.Name.Substring(0, file.Name.LastIndexOf("."));

                Match m = regex.Match(name);
                int index = m.Index;
                double numbering = double.Parse(m.ToString());

                // find longest number of all the files so we know how much we have to pad later
                if ((currentNumber = numbering) > highestNumber)
                {
                    highestNumber = currentNumber;
                }

                paddingLength = highestNumber.ToString().Length;
            }

            // the whole name as in "Tower of God 20.cbz"
            string text = textbox.Text;

            // the actual name, e.g. "Tower of God"
            string namePart;

            if (text.Contains("{"))
            {
                namePart = text.Substring(0, text.IndexOf("{"));
            }
            else
            {
                namePart = text;
            }

            // the chapter number, e.g. 20


            // @"\{num.pad\([0-9]*\)\}
            // explicit set padding via the TextBox
            string paddingRegex = @"\([0-9]*\)";
            Match paddingMatch = Regex.Match(text, paddingRegex);
            if (paddingMatch.Length > 0)
            {
                string paddingCountRegex = @"[0-9]+";
                Match match = Regex.Match(paddingMatch.Value, paddingCountRegex);

                if (match.Length > 0)
                {
                    paddingLength = int.Parse(match.Value);
                }
            }

            foreach (FileInfo file in _files)
            {



                Regex chapterRegex = new Regex(@"[0-9]+(.[0-9])?");
                Match chapterMatch = chapterRegex.Match(file.Name);

                string name;
                if (namePart != "")
                {
                    name = namePart;
                }
                else
                {
                    name = file.Name.Substring(0, chapterMatch.Index - 1);
                }


                // need cultureInfo once here so it parses the dot, so 7.5 stays 7.5 instead of getting changed to 75
                double num = double.Parse(chapterMatch.Value, CultureInfo.GetCultureInfo("en-US"));

                // need cultureInfo a second time to make the dot stay, otherwise the string conversion changes it to the local cultures decimal seperator, so a comma in Germany e.g.
                string numString = num.ToString(CultureInfo.GetCultureInfo("en-US"));

                //#error padding not working correctly..
                string paddingString = "";
                for (int i = 1; i < paddingLength; i++)
                {
                    paddingString += "0";
                }
                string numPaddedString = num.ToString(paddingString + "0.0", CultureInfo.InvariantCulture);


                string newFileName = file.Directory.FullName + "/" + namePart + " " + numPaddedString + file.Extension;

                // finally, we can move (rename) the file
                file.MoveTo(newFileName);
            }
        }

        #endregion buttons
    }
}
