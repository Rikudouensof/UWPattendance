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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPattendance
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Content_Frame.Navigate(typeof(Home_Page));
            Title_TextBlock.Text = "Home";
        }

        private void Hamburger_Button_Click(object sender, RoutedEventArgs e)
        {
            Menu_Split_View.IsPaneOpen = !Menu_Split_View.IsPaneOpen;
        }

        private void List_of_Menu_Listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Home_List_Box_Item.IsSelected)
            {
                Content_Frame.Navigate(typeof(Home_Page));
                Title_TextBlock.Text = "Home";
            }
            else if (People_List_Box_Item.IsSelected)
            {
                Content_Frame.Navigate(typeof(People_Page));
                Title_TextBlock.Text = "People";
            }
            else if (Add_People_List_Box_Item.IsSelected)
            {
                Content_Frame.Navigate(typeof(Add_People_Page));
                Title_TextBlock.Text = "Add People";
            }
            else if (Attendance_List_Box_Item.IsSelected)
            {
                Content_Frame.Navigate(typeof(Attendance_Page));
                Title_TextBlock.Text = "Attendance";
            }
            else if (Take_Attendance_List_Box_Item.IsSelected)
            {
                Content_Frame.Navigate(typeof(Take_Attendance_Page));
                Title_TextBlock.Text = "Take Attendance";
            }
        }

      
    }
}
