using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPattendance.Models;
using UWPattendance.Tasks;
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
    public sealed partial class People_Page : Page
    {
        string _dbPath = Database_Connection._dbpath;
        List<Person> Attendances;
        public People_Page()
        {
            this.InitializeComponent();
            var db = new SQLiteConnection(_dbPath);

            Attendances = db.Table<Person>().OrderBy(m => m.Id).ToList();

           

            foreach (var item in Attendances)
            {
                string a = item.ImagePath;
                string b = a.Replace(@"\\", "/");
                string c = a.Replace("\\", "/");

                Console.WriteLine(b);
                Console.WriteLine(a);
                item.ImagePath = c;
            }

            var count = Attendances.FirstOrDefault();

            Attendance_ListView.ItemsSource = Attendances;
        }

       

        private void Edit_Person_Button_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void Delete_Person_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Attendance_ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = Attendance_ListView.SelectedItem as Person;
            this.Frame.Navigate(typeof(PersonDetail), item.Id);
        }
    }
}
