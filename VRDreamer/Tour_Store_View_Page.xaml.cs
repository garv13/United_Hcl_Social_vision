using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VRDreamer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Tour_Store_View_Page : Page
    {

        List<StoreListing> sl = new List<StoreListing>();

        private StoreListing rec, recM;
        // private Tour n;

        private string price;
        public Tour_Store_View_Page()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rec = new StoreListing();
            LoadingBar.Visibility = Visibility.Visible;
            LoadingBar.IsIndeterminate = true;
            rec = e.Parameter as StoreListing;
            Title.Text = rec.Title;
            Cover.Source = rec.Image;
            FullCost.Text = "Tour " + rec.Price;
            Desc.Text = rec.MyId;
            LoadingBar.Visibility = Visibility.Collapsed;

        }
        //private void Create_Diary_Botton_Click(object sender, RoutedEventArgs e)
        //{
        //    Frame.Navigate(typeof(Create_Diary_Tour));
        //}

        private void About_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(About));
        }

        private void Store_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Store));
        }

        private void Scrap_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NewScrape));
        }

        //private void Purchase_Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Frame.Navigate(typeof(MainPage));
        //}

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void AR_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TouristToolkit));
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {   
           LoadingBar.Visibility = Visibility.Collapsed;
           MessageDialog mess = new MessageDialog("Purchased Can't be done now");
           await mess.ShowAsync();

        }
    }
}
