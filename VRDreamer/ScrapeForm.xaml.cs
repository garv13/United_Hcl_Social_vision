﻿using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
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
    public sealed partial class ScrapeForm : Page
    {
        List<PointerView> li;
        List<Pointer> list;
        public ScrapeForm()
        {
            this.InitializeComponent();
            li = new List<PointerView>();

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            list = e.Parameter as List<Pointer>;
            int i = 0;
            foreach (Pointer p in list)
            {
                PointerView po = new PointerView();
                po.Serial = i.ToString();
                li.Add(po);
                i++;
            }
            ListPointer.ItemsSource = li;
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var lol = sender as Button;
            var lol2 = lol.Parent as Grid;
            var lol3 = lol2.Children[4] as TextBlock;
            int i = int.Parse(lol3.Text);


            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            openPicker.FileTypeFilter.Add(".png");
            StorageFile file = await openPicker.PickSingleFileAsync();
            li[i].File = file;
        }

        private async void NextBar_Click(object sender, RoutedEventArgs e)
        {
            LoadingBar.Visibility = Visibility.Visible;
            LoadingBar.IsIndeterminate = true;

            int i = 0;
            Scrap s = new Scrap();
            s.Title = scrapeName.Text;
            s.UserId = App.userId;
            s.store = true;
            s.Point_List = "";
            try
            {
                foreach (Pointer p in list)
                {
                    double lat = p.lat;
                    double lon = p.lon;
                    p.Desc = li[i].Desc;
                    p.Media_Type = "Image";
                    var credentials = new StorageCredentials("vrdreamer", "lTD5XmjEhvfUsC/vVTLsl01+8pJOlMdF/ri7W1cNOydXwSdb8KQpDbiveVciOqdIbuDu6gJW8g44YtVjuBzFkQ==");
                    var client = new CloudBlobClient(new Uri("https://vrdreamer.blob.core.windows.net/"), credentials);
                    var container = client.GetContainerReference("first");
                    //  await container.CreateIfNotExistsAsync();

                    //var perm = new BlobContainerPermissions();
                    //perm.PublicAccess = BlobContainerPublicAccessType.Blob;
                    //await container.SetPermissionsAsync(perm);
                    var blockBlob = container.GetBlockBlobReference(Guid.NewGuid().ToString() + ".png");


                    //await blockBlob.UploadFromStreamAsync(fileStream);
                    await blockBlob.UploadFromFileAsync(li[i].File);

                    p.Media_Url = blockBlob.StorageUri.PrimaryUri.ToString();
                    p.UserId = "ccb753d1-3bdb-4cb6-a375-3635237a9de7";
                    p.Title = li[i].Title;
                    await App.MobileService.GetTable<Pointer>().InsertAsync(p);
                    s.Point_List += p.Id + ",";
                    i++;
                    s.lon = lon;
                    s.lat = lat;
                }
                s.Point_List = s.Point_List.Substring(0, s.Point_List.Length - 1);
                await App.MobileService.GetTable<Scrap>().InsertAsync(s);
                LoadingBar.Visibility = Visibility.Collapsed;
                Frame.Navigate(typeof(TouristToolkit));
            }
            catch (Exception)
            {
                MessageDialog msgbox = new MessageDialog("Sorry can't upload now");
                await msgbox.ShowAsync();
                LoadingBar.Visibility = Visibility.Collapsed;
            }
        }

        

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

        
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void AR_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TouristToolkit));
        }
    }
}
