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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPattendance
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
      string _dbPath = Database_Connection._dbpath;
    List<Person> People;
    List<Attendance> Attendances;
    public MainPage()
        {

     
      this.InitializeComponent();

           
        }

      

      

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            Content_Frame.Navigate(typeof(Home_Page));
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
      var db = new SQLiteConnection(_dbPath);
      if (args.IsSettingsSelected)
            {

            }
            else
            {
                NavigationViewItem item = args.SelectedItem as NavigationViewItem;

                switch(item.Tag.ToString())
                {
                    case "Home":
                        Content_Frame.Navigate(typeof(Home_Page));
                        break;


                    case "People":
                      try
                      {
                        People = db.Table<Person>().OrderBy(m => m.Id).ToList();
                          Content_Frame.Navigate(typeof(People_Page));
                          break;
                      }
                      catch
                      {
              break;

                      }
           
                    

                    case "Attendance":
                      try
                      {
                        Attendances = db.Table<Attendance>().OrderBy(m => m.Id).ToList();
                        Content_Frame.Navigate(typeof(Attendance_Page));
                        break;
                      }
                      catch
                      {
              break;

            }
          

                    case "Add_Person":
                        Content_Frame.Navigate(typeof(Add_People_Page));
                        break;

                    case "Download":
                        Content_Frame.Navigate(typeof(Download));
                        break;

                    case "Add_Attendance":
                        Content_Frame.Navigate(typeof(Take_Attendance_Page));
                        break;
                }
            }


           
        }
    }
}
