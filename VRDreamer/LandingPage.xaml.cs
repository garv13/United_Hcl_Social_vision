using Microsoft.WindowsAzure.MobileServices;
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

namespace VRDreamer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LandingPage : Page
    { 
         private IMobileServiceTable<User> Table = App.MobileService.GetTable<User>();
         private MobileServiceCollection<User, User> items;
        public LandingPage()
        {
            this.InitializeComponent();
            Loaded += LandingPage_Loaded;
        }

        private async void LandingPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Username Hardcoded
            items = await Table.Where(User
                           => User.UserName == "hcl").ToCollectionAsync();
            App.userId = items[0].Id;
            App.u = items[0];
        }

            private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TouristToolkit));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Store));
        }
    }
}
