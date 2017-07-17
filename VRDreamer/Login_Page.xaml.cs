using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
    public sealed partial class Login_Page : Page
    {
        private IMobileServiceTable<User> Table = App.MobileService.GetTable<User>();
        private MobileServiceCollection<User, User> items;
        public Login_Page()
        {
            this.InitializeComponent();
        }


        async Task MakePredictionRequest()
        {
            var client = new HttpClient();

            // Request headers - replace this example key with your valid subscription key.
            client.DefaultRequestHeaders.Add("Prediction-Key", "2765249eb75041abb68e1dd99a8d917b");

            // Prediction URL - replace this example URL with your valid prediction URL.
            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/3ad1f885-103f-46a8-84a6-d197c140970d/url?iterationId=6f9f234f-cc0f-4124-ab10-140dc54578d0";
            string img = "http://d2dzjyo4yc2sta.cloudfront.net/?url=images.pitchero.com%2Fui%2F1867965%2F1435918920_4727.jpg";
            HttpResponseMessage response;
            ASCIIEncoding encoding = new ASCIIEncoding();
            // Request body. Try this sample with a locally stored image.
            string imgUrl = "url:" + img;
            byte[] byteData = Encoding.ASCII.GetBytes(imgUrl);


            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Url", img)
            });



                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(url, content);
        }



        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await MakePredictionRequest();
            LoadingBar.Visibility = Visibility.Visible;
            LoadingBar.IsIndeterminate = true;
            try
            {
                items = await Table.Where(User
                                => User.UserName == UserName.Text).ToCollectionAsync();
                if (items.Count == 1)
                {
                    if (Password.Password == items[0].Password)
                    {
                        LoadingBar.Visibility = Visibility.Collapsed;
                        MessageDialog msgbox = new MessageDialog("Welcome " + UserName.Text);
                        await msgbox.ShowAsync();
                        App.userId = items[0].Id;
                        App.u = items[0];
                        Frame.Navigate(typeof(MainPage));
                    }
                }
                else
                {
                    LoadingBar.Visibility = Visibility.Collapsed;
                    MessageDialog msgbox = new MessageDialog("Password or username incorrect");
                    await msgbox.ShowAsync();
                }
            }
            catch (Exception)
            {
                LoadingBar.Visibility = Visibility.Collapsed;
                MessageDialog msgbox = new MessageDialog("Sorry Can't connect");
                await msgbox.ShowAsync();
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SignUp_Page));
        }
    }
}
