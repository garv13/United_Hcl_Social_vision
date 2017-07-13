﻿using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace VRDreamer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public string userid = App.userId;
        private IMobileServiceTable<User> Table = App.MobileService.GetTable<User>();
        private MobileServiceCollection<User, User> items;
        private IMobileServiceTable<Scrap> Table1 = App.MobileService.GetTable<Scrap>();
        private MobileServiceCollection<Scrap, Scrap> items1;
        private IMobileServiceTable<Tour> Table2 = App.MobileService.GetTable<Tour>();
        private MobileServiceCollection<Tour, Tour> items2;
        private IMobileServiceTable<Diary> Table3 = App.MobileService.GetTable<Diary>();
        private MobileServiceCollection<Diary, Diary> items3;
        private IMobileServiceTable<Pointer> Table4 = App.MobileService.GetTable<Pointer>();
        private MobileServiceCollection<Pointer,Pointer> items4;
        Purchsed m = new Purchsed();
        User u = new User();
        string purchases = " ";
        List<string> Scrap_Purchases = new List<string>();
        List<string> Tour_Purchases = new List<string>();
        List<string> Diary_Purchases = new List<string>();
        List<Purchsed> Slist = new List<Purchsed>();
        List<Purchsed> Tlist = new List<Purchsed>();
        List<Purchsed> Dlist = new List<Purchsed>();

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        //private async void data_Update()
        //{
            
        //    items1 = await Table1.ToCollectionAsync();

        //    foreach(Scrap s in items1)
        //    {
        //        string[] po = s.Point_List.Split(',');
        //        items4 = await Table4.Where(Pointer
        //            => Pointer.Id == po[0]).ToCollectionAsync();
        //        s.lat = items4[0].lat;
        //        s.lon = items4[0].lon;
        //        await Table1.UpdateAsync(s);
        //    }
        //}
        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadingBar2.Visibility = Visibility.Visible;
                LoadingBar2.IsIndeterminate = true;

                items = await Table.Where(User
                            => User.Id == userid).ToCollectionAsync(); // get the username from login page
                if (items[0].ScrapePurchase != null)
                    Scrap_Purchases = items[0].ScrapePurchase.Split(',').ToList<string>();
                if (items[0].TourPurchases != null)
                    Tour_Purchases = items[0].TourPurchases.Split(',').ToList<string>();
                if (items[0].DiaryPurchases != null)
                    Diary_Purchases = items[0].DiaryPurchases.Split(',').ToList<string>();
                foreach (string id in Scrap_Purchases)
                {
                    if (id != "")
                    {
                        m = new Purchsed();
                        items1 = await Table1.Where(Scrap
                               => Scrap.Id == id).ToCollectionAsync();

                        m.Id = items1[0].Id;
                        m.Title = items1[0].Title;
                        m.Image = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(this.BaseUri, "Assets/augmented-reality-for-blog.jpg")); // some static iage for scrap
                        m.Type = "S";
                        Slist.Add(m);
                    }
                }

                foreach (string id in Tour_Purchases)
                {
                    if (id != "")
                    {
                        m = new Purchsed();
                        items2 = await Table2.Where(Tour
                            => Tour.Id == id).ToCollectionAsync();
                        m.Id = items2[0].Id;
                        m.Title = items2[0].Title;
                        m.Image = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(items2[0].Cover_Url));
                        m.Type = "T";
                        Tlist.Add(m);
                    }
                }
                foreach (string id in Diary_Purchases)
                {
                    if (id != "")
                    {
                        m = new Purchsed();
                        items3 = await Table3.Where(Diary
                               => Diary.Id == id).ToCollectionAsync();

                        m.Id = items3[0].Id;
                        m.Title = items3[0].Title;
                        m.Image = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(items3[0].Cover_Url));
                        m.Type = "D";
                        Dlist.Add(m);
                    }
                }

                items1 = await Table1.Where(Scrap
                           => Scrap.UserId == userid).ToCollectionAsync();

                foreach (Scrap sr in items1)
                {
                    m = new Purchsed();
                    m.Id = sr.Id;
                    m.Title = sr.Title;
                    m.Image = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(this.BaseUri, "Assets/augmented-reality-for-blog.jpg")); // some static iage for scrap
                    m.Type = "S";
                    Slist.Add(m);
                }

                items2 = await Table2.Where(Tour
                   => Tour.UserId == userid).ToCollectionAsync();
                foreach (Tour sr in items2)
                {
                    m = new Purchsed();

                    m.Id = sr.Id;
                    m.Title = sr.Title;
                    m.Image = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(sr.Cover_Url)); // some static iage for scrap
                    m.Type = "T";
                    Tlist.Add(m);
                }

                items3 = await Table3.Where(Diary
                => Diary.UserId == userid).ToCollectionAsync();
                foreach (Diary sr in items3)
                {
                    m = new Purchsed();
                    m.Id = sr.Id;
                    m.Title = sr.Title;
                    m.Image = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(sr.Cover_Url)); // some static iage for scrap
                    m.Type = "D";
                    Dlist.Add(m);
                }

                LoadingBar2.Visibility = Visibility.Collapsed;
                DiaryView.DataContext = Dlist;
                TourView.DataContext = Tlist;
                ScarpeView.DataContext = Slist;
            }
            catch (Exception)
            {

                MessageDialog msgbox = new MessageDialog("Sorry can't update now");
                await msgbox.ShowAsync();
                LoadingBar2.Visibility = Visibility.Collapsed;
            }
        }

        private void NextBar_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NewScrape));
        }

        private void ColorSelect_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Store));
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Grid g = new Grid();
            g = sender as Grid;
            string type = null;
            string id = null;
            foreach (FrameworkElement child in g.Children)
            {
                if ((Grid.GetRow(child) == 2) && (Grid.GetColumn(child) == 1))
                {
                    TextBlock b = child as TextBlock;


                    id = b.Text;
                }


                if ((Grid.GetRow(child) == 1) && (Grid.GetColumn(child) == 0))
                {
                    TextBlock b = child as TextBlock;

                    type = b.Text;
                }
            }

            if (type == "S")
                Frame.Navigate(typeof(ViewScrape), id);
            else if (type == "D")
                Frame.Navigate(typeof(DiaryViewer_Page), id);
            else if (type == "T")
                Frame.Navigate(typeof(TouristLens), id);
        }

        private void Create_Diary_Botton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Create_Diary_Tour));
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

        private void Purchase_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void TouristLink_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TouristToolkit));
        }
    }
}
