﻿using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class About : Page
    {
        public About()
        {
            this.InitializeComponent();
            Loaded += About_Loaded;

        }
        private IMobileServiceTable<CustomVisionData> Table = App.MobileService.GetTable<CustomVisionData>();
        private MobileServiceCollection<CustomVisionData, CustomVisionData> items;


        private async void About_Loaded(object sender, RoutedEventArgs e)
        {
          //  await Data_Entry_Code();
            await(new MessageDialog("Will be updated soon")).ShowAsync();
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


        // Data entry Code

        private async Task Data_Entry_Code()
        {
            CustomVisionData ob = new CustomVisionData();
            ob.Name = "EFL Cup";
            ob.Image_Url = "http://www.newstalk.com/content/000/images/000190/196833_54_news_hub_168750_656x500.jpg";
            ob.Desc = "The EFL Cup, currently known as the Carabao Cup for sponsorship reasons, is an annual knockout football competition in men's domestic English football.";
            ob.Vidoe_Url = "https://vrdreamer.blob.core.windows.net/match-videos/EFL%20Cup%20Final%202017%20Celebrations%20-%20Manchester%20United.mp4";
            await App.MobileService.GetTable<CustomVisionData>().InsertAsync(ob);

        }


    }
}
