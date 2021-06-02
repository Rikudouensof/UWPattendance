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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPattendance
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonDetail : Page
    {
        string _dbPath = Database_Connection._dbpath;
    int userId = 0;
        public PersonDetail()
        {
            this.InitializeComponent();
      
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var user_Id = (int)e.Parameter;
             userId = user_Id;
            var db = new SQLiteConnection(_dbPath);
            var selected_person = db.Table<Person>().Where(m => m.Id == user_Id).FirstOrDefault();
            var user_attendance = db.Table<Attendance>().Where(m => m.User_ID == user_Id).OrderByDescending(m => m.Date_Signed_In_Date_and_Time).Select(m => m.Date_Signed_In_Date_and_Time).FirstOrDefault();
      BitmapImage imagesource = new BitmapImage(new Uri(selected_person.ImagePath));
      UserImage.Source = imagesource;

      var attendance_of_person = db.Table<Attendance>().Where(m => m.User_ID == user_Id).OrderByDescending(m => m.Date_Signed_In_Date_and_Time);
      Last_Login_TextBlock.Text = user_attendance.ToString("ddd dd MMM yyyy  HH:mm");

            Last_Name_TextBlock.Text = selected_person.LastName;
            FirstName_Name_TextBlock.Text = selected_person.FirstName;
            Registration_Date_TextBlock.Text = selected_person.DateRegistered.ToString("ddd dd MMM yyyy  HH:mm");
      Attendance_ListView.ItemsSource = attendance_of_person;

            // parameters.Name
            // parameters.Text
            // ...
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
      var db = new SQLiteConnection(_dbPath);
      var selected_person = db.Table<Person>().Where(m => m.Id == userId).FirstOrDefault();
      db.Delete(selected_person);
      this.Frame.Navigate(typeof(People_Page));
    }
  }
}
