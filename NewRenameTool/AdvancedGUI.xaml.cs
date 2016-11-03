using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NewRenameTool.GUI
{
    /// <summary>
    /// Interaktionslogik für AdvancedGUI.xaml
    /// </summary>
    public partial class AdvancedGUI : UserControl
    {
        char[] seperator = new char[] { ' ', '-', '_' };

        // stores original index and display index
        Dictionary<int, int> columnIndex = new Dictionary<int, int>();
        bool dropped = false;

        public AdvancedGUI()
        {
            InitializeComponent();
            this.DataContext = this;

            dataGrid.CanUserAddRows = false;
            dataGrid.ColumnDisplayIndexChanged += dataGrid_ColumnDisplayIndexChanged;
        }

        #region Properties
        DataTable _dt = new DataTable();
        DataTable dt
        {
            get { return _dt; }
            set { _dt = value; }
        }

        /* List<List<string>> _Items = new List<List<string>>();
         List<List<string>> Items
         {
             get { return _Items; }
             set { Items = value; }
         }*/

        private ObservableCollection<FileInfo> _files = new ObservableCollection<FileInfo>();
        public ObservableCollection<FileInfo> Files
        {
            get { return _files; }
        }



        #endregion Properties

        #region Table

        private void ParseName(string fileName)
        {
            List<string> fileNameParts = fileName.SplitAndKeep(seperator).ToList<string>();
            DataRow dr = dt.NewRow();

            // remove empty strings. Whitespace is kept.
            fileNameParts.RemoveAll(item => item.Equals(""));
            for (int i = 0; i < fileNameParts.Count; i++)
            {
                dt.Columns.Add();
                dr[i] = fileNameParts[i];
            }

            //Items.Add(fileNameParts);
            int j = 0;
            var columnsCount = dataGrid.Columns.Count;
            for (j = 0; j < fileNameParts.Count; j++)
            {
                var index = j + columnsCount;
                var col = new DataColumn();
                // col.Header = index;
                //col.Binding = new Binding(string.Format("[{0}]", j));
                //dataGrid.Columns.Add(col);
                dt.Columns.Add(col);
                // here, original index and match obviously
                columnIndex.Add(index, index);
            }
            dt.Rows.Add(dr);
            dataGrid.ItemsSource = dt.DefaultView; // Items;
        }

        #endregion Table


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
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                _files.Clear();

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string filePath in files)
                {
                    _files.Add(new FileInfo(filePath));
                }
            }

            string fileName = Path.GetFileNameWithoutExtension(_files[0].Name);
            if (!dropped)
                ParseName(fileName);

            var listbox = sender as ListBox;
            listbox.Background = new SolidColorBrush(Color.FromRgb(226, 226, 226));
            dropped = true;
            e.Handled = true;
        }


        private void dataGrid_ColumnDisplayIndexChanged(object sender, DataGridColumnEventArgs e)
        {
            // Set the new display index, so we can acess it later
            // e.Column.Header is the original index, as the columns are simply named "0", "1" etc.
            columnIndex[(int)e.Column.Header] = e.Column.DisplayIndex;
        }


        #endregion Events

        #region Buttons

        private void ButtonRename_Click(object sender, RoutedEventArgs e)
        {
            int paddingLength = 0;
            double highestNumber = 0;
            double currentNumber = 0;

            foreach (FileInfo file in _files)
            {
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


            foreach (FileInfo file in _files)
            {

                string name = Path.GetFileNameWithoutExtension(file.Name);
                string[] fileName = name.SplitAndKeep(seperator).ToArray<string>();
                string[] newFileName = new string[dataGrid.Columns.Count];

                for (int i = 0; i < dataGrid.Columns.Count; i++)
                {

                    string text = fileName[i];
                    int number;
                    if (int.TryParse(text, out number))
                    {
                        text = text.PadLeft(paddingLength, char.Parse("0"));
                    }
                    newFileName[columnIndex[i]] = text;
                }

                string fileNameString = Path.GetDirectoryName(file.FullName) + "/";
                fileNameString += string.Join("", newFileName);
                fileNameString += file.Extension;
                file.MoveTo(fileNameString);
                //MessageBox.Show(fileNameString);
            }

        }
    }

        #endregion
}
