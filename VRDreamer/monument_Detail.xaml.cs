﻿using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VRDreamer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class monument_Detail : Page
    {
        private IMobileServiceTable<CustomVisionData> Table = App.MobileService.GetTable<CustomVisionData>();
        private MobileServiceCollection<CustomVisionData, CustomVisionData> items;
        public monument_Detail()
        {
            this.InitializeComponent();
        }
        googlePlaceApi gp;
        MediaPlayer _mediaPlayer;
        Monument_Detail_View m = new Monument_Detail_View();
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadingBar.Visibility = Visibility.Visible;
            LoadingBar.IsIndeterminate = true;
            // m = e.Parameter as string;
            string s = e.Parameter as string;
            try
            {
                items = await Table.Where(CustomVisionData
                            => CustomVisionData.Name == s).ToCollectionAsync();

                if(items.Count != 0)
                {
                    _mediaPlayer = new MediaPlayer();
                    name.Text = items[0].Name;
                    img.Source =  new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(this.BaseUri, items[0].Image_Url));
                    desc.Text = items[0].Desc;
                    video.Source = MediaSource.CreateFromUri(new Uri(items[0].Vidoe_Url));
                    _mediaPlayer = video.MediaPlayer;
                    _mediaPlayer.Play();
                }
    
               else
              { 
                LoadingBar.Visibility = Visibility.Collapsed;
                MessageDialog msgbox = new MessageDialog("No Match Found Sorry :(:(");
                await msgbox.ShowAsync();

               }

           // // string s = "Qutb complex";
           // HttpClient cl = new HttpClient();
           // s = Uri.EscapeDataString(s);
           // string url = "https://maps.googleapis.com/maps/api/place/textsearch/json?query=" + s + "&key=AIzaSyBMzL7mptHo33PNsuKmT9xKppNgkXotBOM";

           //// string url = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=28.661904,77.2232688&radius=50000&type=point_of_interest&keyword=" + s + "&key=AIzaSyBMzL7mptHo33PNsuKmT9xKppNgkXotBOM";
           // string picUrl = "https://maps.googleapis.com/maps/api/place/photo?maxwidth=300&photoreference=";
           // try
           // {
           //     s = await cl.GetStringAsync(url);
           //     gp = JsonConvert.DeserializeObject<googlePlaceApi>(s);
           //     picUrl += gp.results[0].photos[0].photo_reference + "&key=AIzaSyBMzL7mptHo33PNsuKmT9xKppNgkXotBOM";
           //     var stream = await cl.GetStreamAsync(picUrl);
           //     BitmapImage Image = new BitmapImage();
           //     using (var memStream = new MemoryStream())
           //     {
           //         await stream.CopyToAsync(memStream);
           //         memStream.Position = 0;
           //         Image.SetSource(memStream.AsRandomAccessStream());
           //     }
           //     img.Source = Image;
           //     name.Text = gp.results[0].name;
           //     //desc.Text = gp.results[0].vicinity;
           //     // parse the string in json and get the details
            }
            catch (Exception ex)
            {
                LoadingBar.Visibility = Visibility.Collapsed;
                MessageDialog msgbox = new MessageDialog("Sorry Can't Connect :(:(");
                await msgbox.ShowAsync();
            }
        }
        //private void Create_Diary_Botton_Click(object sender, RoutedEventArgs e)
        //{
        //    Frame.Navigate(typeof(Create_Diary_Tour));
        //}

        private void About_Button_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Dispose();
            Frame.Navigate(typeof(About));
        }

        private void Store_Button_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Dispose();
            Frame.Navigate(typeof(Store));
        }

        private void Scrap_Button_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Dispose();
            Frame.Navigate(typeof(NewScrape));
        }

        //private void Purchase_Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Frame.Navigate(typeof(MainPage));
        //}

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Dispose();
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private async void Navigate_Click(object sender, RoutedEventArgs e)
        {
            double lat = gp.results[0].geometry.location.lat;
            double lon = gp.results[0].geometry.location.lng;

            // Center on New York City
            var uriNewYork = new Uri(@"bingmaps:?rtp=pos." + m.MyLat.ToString() + "_" + m.MyLon.ToString() + "~pos."
                + lat.ToString() + "_" + lon.ToString() + "&trfc=1");

            // Launch the Windows Maps app
            var launcherOptions = new Windows.System.LauncherOptions();
            launcherOptions.TargetApplicationPackageFamilyName = "Microsoft.WindowsMaps_8wekyb3d8bbwe";
            var success = await Windows.System.Launcher.LaunchUriAsync(uriNewYork, launcherOptions);
            // navigate to navigation app
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Dispose();
            Frame.Navigate(typeof(Store), gp.results[0].name);
        }

    }
}
