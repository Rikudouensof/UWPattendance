﻿using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPattendance.Models;
using UWPattendance.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
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
    public sealed partial class Home_Page : Page
    {
        string _dbPath = Database_Connection._dbpath;
        public Home_Page()
        {
            
            this.InitializeComponent();
            var db = new SQLiteConnection(_dbPath);
      DateTextblock.Text = DateTime.UtcNow.ToString("dd/MMM/yyyy");

      try
      {
        var numbers = db.Table<Person>().Count();


        Numbers_TextBlock.Text = numbers.ToString();
      }
      catch
      {



        Numbers_TextBlock.Text = 0.ToString();
      }
           

        }

       

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);
            StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
        }
    }
}
