using System.Windows;

namespace NewRenameTool
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

              // original rename method, way simpler, but can only deal with simple files names like "somename 3.txt", and not "some 3 name.txt"
        /* private void ButtonRename_Click(object sender, RoutedEventArgs e){
        double highestNumber = 0;
           double currentNumber = 0;
           var regex = new Regex(@"[0-9]\d*\.?[0-9]*$");

           Dictionary<string, string> tmp = new Dictionary<string, string>();

           foreach (FileInfo file in _files)
           {
               string fileName = file.FullName.Substring(0, file.FullName.LastIndexOf("."));

               Match m = regex.Match(fileName);
               int index = m.Index;
               double numbering = double.Parse(fileName.Substring(index), CultureInfo.InvariantCulture);

               // find longest number of all the files so we know how much we have to pad later
               if ((currentNumber = numbering) > highestNumber)
               {
                   highestNumber = currentNumber;
               }
           }

           int paddingLength = highestNumber.ToString().Length;

           foreach (FileInfo file in _files)
           {
               // find out where the number is
               string fileName = file.FullName.Substring(0, file.FullName.LastIndexOf("."));
               Match m = regex.Match(fileName);
               int index = m.Index;

               // the original number
               string numbering = fileName.Substring(index);
               string newNumbering;

               if (numbering.Contains("."))
               {
                   string[] number = numbering.Split(char.Parse("."));
                   newNumbering = number[0].PadLeft(paddingLength, char.Parse("0")) + "." + number[1];
               }
               else
               {
                   newNumbering = numbering.PadLeft(paddingLength, char.Parse("0"));
               }

               string actualFileName = fileName.Substring(0, index);

               int length = highestNumber.ToString().Length;
               // build new file name with the padded number
               string newFileName = actualFileName + newNumbering + file.Extension;

               // only rename if padding was added, otherwise it will throw any error. It's also unnecessary in the first place
               if (!newFileName.Equals(file.FullName))
               {
                   // rename file
                   file.MoveTo(newFileName);
               }
           }
    }*/
   
    }
}
