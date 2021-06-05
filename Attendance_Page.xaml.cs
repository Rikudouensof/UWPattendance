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
  public sealed partial class Attendance_Page : Page
  {
    string _dbPath = Database_Connection._dbpath;
    List<Attendance> Attendances;

    public Attendance_Page()
    {
      this.InitializeComponent();
      var db = new SQLiteConnection(_dbPath);

      Attendances = db.Table<Attendance>().OrderBy(m => m.Id).ToList();

      //Attendances = AttendanceList.GetAttendance();

      Attendance_ListView.ItemsSource = Attendances.Where(m => m.Date_Signed_In_Date_and_Time == DateTime.Today);
    }

    private void Attemdamce_Date_Picker_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
    {
      var attendance = Attendances.Where(m => m.Date_Signed_In_Date_and_Time.Date.ToString("dd/MM/yyyy") == sender.SelectedDate.Value.ToString("dd/MM/yyyy"));
      Attendance_ListView.ItemsSource = attendance;
    }

    private void Attendance_ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {


      var item = Attendance_ListView.SelectedItem as Attendance;
      this.Frame.Navigate(typeof(PersonDetail), item.User_ID);
    }
  }
}
