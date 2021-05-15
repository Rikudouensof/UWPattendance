using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPattendance
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Download : Page
    {
        public Download()
        {
            this.InitializeComponent();


        }
        public class ListItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
             var list = new List<ListItem>()
    {
        new ListItem(){Id = 1, Name = "Jerry"},
        new ListItem(){Id = 2, Name="George"},
        new ListItem(){Id = 3, Name="Kramer"},
        new ListItem(){Id = 4, Name = "Elaine"}
     };

    byte[] result;
    using (var memoryStream = new MemoryStream())
    {
        using (var streamWriter = new StreamWriter(memoryStream))
        {
            using (var csvWriter = new CsvWriter((ISerializer)streamWriter))
            {
                csvWriter.WriteRecords(list);
                streamWriter.Flush();
                result = memoryStream.ToArray();
            }
         }
     }

       // return new FileStreamResult(new MemoryStream(result), "text/csv") { FileDownloadName = "filename.csv" };
        }
    }
}
