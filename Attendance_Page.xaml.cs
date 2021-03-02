﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPattendance.Models;
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
        List<Attendance> Attendances;

        public Attendance_Page()
        {
            this.InitializeComponent();
            Attendances = AttendanceList.GetAttendance();
            Attendance_ListView.ItemsSource = Attendances;
        }

        private void Attemdamce_Date_Picker_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
        {
            Attendances = AttendanceList.GetAttendance();
            var attendance = Attendances.Where(m => m.Date_Signed_In_Date_and_Time.Date.ToString("dd/MM/yyyy") == sender.SelectedDate.Value.ToString("dd/MM/yyyy"));
            Attendance_ListView.ItemsSource = attendance;
        }
    }
}