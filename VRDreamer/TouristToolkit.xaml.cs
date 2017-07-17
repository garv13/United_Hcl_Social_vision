﻿using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI.Core;
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
    public sealed partial class TouristToolkit : Page
    {
        string blobUrl;
        bool _isFocused = false;
        private IMobileServiceTable<Pointer> Table2 = App.MobileService.GetTable<Pointer>();
        private MobileServiceCollection<Pointer, Pointer> items2;
        MediaCapture _mediaCapture;
        bool _isPreviewing;
        DisplayRequest _displayRequest;
        string s;
        OrientationSensor or;
        Compass c;
        List<Scrap> li;
        List<string> id;
        List<Pointer> li3;
        List<PointerViewAR> li2;
        Geoposition pos;
        bool myBool, first;

        double yaw;
        double wid, h, stepW, stepH;
        double pitch;
        int i;
        private IMobileServiceTable<Scrap> Table3 = App.MobileService.GetTable<Scrap>();
        private MobileServiceCollection<Scrap, Scrap> items3;
        private IMobileServiceTable<Tour> Table4 = App.MobileService.GetTable<Tour>();
        private MobileServiceCollection<Tour,Tour> items4;


   
        public TouristToolkit()
        {
            blobUrl = "";
            this.InitializeComponent();
            i = 0;
            li = new List<Scrap>();
            li2 = new List<PointerViewAR>();
            li3 = new List<Pointer>();
            id = new List<string>();
            or = OrientationSensor.GetDefault();
            myBool = false;
            c = Compass.GetDefault();
            try
            {
                c.ReportInterval = 4;
                c.ReadingChanged += C_ReadingChanged;
                or.ReportInterval = 4;
                or.ReadingChanged += Or_ReadingChanged;
            }
            catch
            { }
            this.InitializeComponent();
            PreviewControl.Loaded += PreviewControl_Loaded;

            Application.Current.Suspending += Application_Suspending;
        }
        private async void Or_ReadingChanged(OrientationSensor sender, OrientationSensorReadingChangedEventArgs args)
        {
            if (myBool)
            {
                OrientationSensorReading reading = args.Reading;

                // Quaternion values
                SensorQuaternion q = reading.Quaternion;
                double y = args.Reading.Quaternion.Y;
                if (y < 0)
                    y = y + 2;
                pitch = y;
                //    pitch = pitch * 180 / Math.PI;
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    for (; lol.Children.Count > 6;)
                    {
                        lol.Children.RemoveAt(6);
                    }

                    foreach (PointerViewAR n in li2)
                    {
                        Image img = new Image();
                        img.Name = n.Id;
                        img.Width = 250;
                        img.Height = 250;
                        img.Source = n.Media;// img.Stretch = Stretch.UniformToFill;
                        TranslateTransform t = new TranslateTransform();
                        double dis = getDistance(n.lat, n.lon, pos.Coordinate.Latitude, pos.Coordinate.Longitude);
                        if (dis<50)
                        //TODO: in prod write distance<20
                        {
                            // double ang = getangle(n.lat, n.lon, pos.Coordinate.Latitude, pos.Coordinate.Longitude);
                            t.X = angleDiff(n.Yaw, yaw) * stepW * 2;
                            t.Y = (n.Pitch - pitch) * stepH * 2;
                            img.RenderTransform = t;
                            img.IsTapEnabled = true;
                            img.Tapped += Img_Tapped;
                            lol.Children.Add(img);
                            Grid.SetRow(img, 1);
                        }
                    }


                });

            }


        }
        private void Img_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image img = sender as Image;
            string s = img.Name;
            Frame.Navigate(typeof(Image_Tapped_Detail), s);
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            button.IsEnabled = false;
            button2.IsEnabled = false;
            var accessStatus = await Geolocator.RequestAccessAsync();
            await StartPreviewAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    // _rootPage.NotifyUser("Waiting for update...", NotifyType.StatusMessage);

                    // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 2, MovementThreshold = 6 };

                    // Subscribe to the StatusChanged event to get updates of location status changes.


                    // Carry out the operation.
                    pos = await geolocator.GetGeopositionAsync();
                    button.IsEnabled = true;
                    button2.IsEnabled = true;
                    textBox.Text = "Ready";
                    myBool = false;
                    first = true;
                    geolocator.PositionChanged += Geolocator_PositionChanged;
                    break;
            }

        }
        private void C_ReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
        {
            CompassReading reading = args.Reading;
            yaw = reading.HeadingTrueNorth.Value;

        }
        private double angleDiff(double yaw, double yaw2)
        {
            double diff = 0d;
            double diff2 = 0d;
            diff = yaw - yaw2;
            if (yaw > yaw2)
                diff2 = yaw - (yaw2 + 360);
            else
                diff2 = yaw2 - (yaw + 360);

            if (Math.Abs(diff) < Math.Abs(diff2))
                return diff;
            else
                return diff2;
        }
        private async void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            if (myBool || first)
            {
                pos = args.Position;
                try
                {
                    string pur = App.u.TourPurchases;
                    string[] purchases = pur.Split(',');

                    items4 = await Table4.Where(t =>
                    purchases.Contains(t.Id)).ToCollectionAsync();

                    List<string> scrapeId = new List<string>();
                    foreach (Tour t in items4)
                    {
                        string[] temp = t.Scrap_List.Split(',');
                        for (int i = 0; i < temp.Length; i++)
                        {
                            scrapeId.Add(temp[i]);
                        }
                    }
                    myBool = false;
                    first = false;

                    items3 = await Table3.Where(t =>
                    (t.lat - pos.Coordinate.Latitude < 0.0018018018
                    && t.lat - pos.Coordinate.Latitude > -0.0018018018
                    && t.lon - pos.Coordinate.Longitude < 0.0018018020
                    && t.lon - pos.Coordinate.Longitude > -0.0018018020
                    && (t.UserId == App.userId || scrapeId.Contains(t.Id))
                    )).ToCollectionAsync();

                    myBool = true;
                    first = true;
                    //TODO : add logic to get only those he has bought or created
                    // query updated , testing left
                    if (items3.Count != 0)
                    {
                        first = false;
                        myBool = false;
                        foreach (Scrap s in items3)
                        {
                            if (id.Contains(s.Id))
                            { }
                            else
                            {
                                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                {

                                    textBox.Text = "Loading new Scrapes";
                                });
                                id.Add(s.Id);
                                li.Add(s);
                                string[] str = s.Point_List.Split(',');
                                for (int i = 0; i < str.Length; i++)
                                {
                                    string po = str[i];
                                    items2 = await Table2.Where(t => t.Id == po).ToCollectionAsync();
                                    Pointer p = new Pointer();
                                    if (items2.Count > 0)
                                    {
                                        p = items2[0];
                                        li3.Add(p);
                                    }
                                }//all pointers loaded for one scrap

                                for (int i = 0; i < li3.Count; i++)
                                {
                                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                    {
                                        PointerViewAR p = new PointerViewAR();
                                        p.Id = li3[i].Id;
                                        p.lat = li3[i].lat;
                                        p.lon = li3[i].lon;
                                        p.Pitch = li3[i].Pitch;
                                        p.Title = li3[i].Title;
                                        p.Yaw = li3[i].Yaw;
                                        p.Desc = li3[i].Desc;
                                        p.Media = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(li3[i].Media_Url));
                                        li2.Add(p);
                                    });
                                }
                                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                {

                                    textBox.Text = "Done";
                                });
                            }
                        }
                        foreach (Scrap s in li)
                        {
                            if (items3.Contains(s))
                            { }
                            else
                            {
                                myBool = false;
                                li.Remove(s);
                                id.Remove(s.Id);
                                string[] str = s.Point_List.Split(',');
                                for (int i = 0; i < str.Length; i++)
                                {
                                    string po = str[i];
                                    //items2 = await Table2.Where(t => t.Id == po).ToCollectionAsync();
                                    List<Pointer> p = new List<Pointer>();

                                    p = li3.Where(tr => tr.Id == str[i]).ToList<Pointer>();
                                    li3.Remove(p[0]);
                                    List<PointerViewAR> pt = new List<PointerViewAR>();
                                    pt = li2.Where(tr => tr.Id == str[i]).ToList<PointerViewAR>();
                                    li2.Remove(pt[0]);


                                }//all pointers removed for one scrap

                            }
                        }
                        myBool = true;

                    }


                }
                catch (Exception ex)
                {
                    myBool = true;

                }
            }

        }
        private void PreviewControl_Loaded(object sender, RoutedEventArgs e)
        {
            wid = PreviewControl.ActualWidth;
            h = PreviewControl.ActualHeight;
            stepH = h / 0.5;
            stepW = wid / 90;
        }
        private async void button_Click(object sender, RoutedEventArgs e)
        {
            button.IsEnabled = false;
            button2.IsEnabled = false;
            PB.Visibility = Visibility.Visible;
            using (var captureStream = new InMemoryRandomAccessStream())
            {

                await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), captureStream);
                using (var s = captureStream.AsStream())
                {

                    //await _mediaCapture.CapturePhotoToStorageFileAsync(ImageEncodingProperties.CreatePng(), file);
                    var credentials = new StorageCredentials("vrdreamer", "lTD5XmjEhvfUsC/vVTLsl01+8pJOlMdF/ri7W1cNOydXwSdb8KQpDbiveVciOqdIbuDu6gJW8g44YtVjuBzFkQ==");
                    var client = new CloudBlobClient(new Uri("https://vrdreamer.blob.core.windows.net/"), credentials);
                    var container = client.GetContainerReference("datasetimages");

                    var blockBlob = container.GetBlockBlobReference(Guid.NewGuid().ToString() + ".jpeg");
                    s.Position = 0;

                    await blockBlob.UploadFromStreamAsync(s);
                    ////await blockBlob.UploadFromFileAsync(captureStream);
                    blobUrl = blockBlob.StorageUri.PrimaryUri.ToString();
                    var add = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36";
                    var httpRequestMessage = new Windows.Web.Http.HttpRequestMessage(Windows.Web.Http.HttpMethod.Get, new Uri("http://www.google.com/searchbyimage?site=search&sa=X&image_url=" + blockBlob.StorageUri.PrimaryUri.ToString()));
                    httpRequestMessage.Headers.Add("User-Agent", add);
                    //items2 = await Table2.ToCollectionAsync();
                    web.NavigateWithHttpRequestMessage(httpRequestMessage);
                    web.DOMContentLoaded += Web_DOMContentLoaded;
                }
            }


        }


        static async Task MakePredictionRequest(string imageFilePath)
        {
            var client = new HttpClient();

            // Request headers - replace this example key with your valid subscription key.
            client.DefaultRequestHeaders.Add("Prediction-Key", "2765249eb75041abb68e1dd99a8d917b");

            // Prediction URL - replace this example URL with your valid prediction URL.
            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/3ad1f885-103f-46a8-84a6-d197c140970d/url?iterationId=6f9f234f-cc0f-4124-ab10-140dc54578d0";
            string img = "";
            HttpResponseMessage response;
            ASCIIEncoding encoding = new ASCIIEncoding();
            // Request body. Try this sample with a locally stored image.
            string imgUrl = "Url:" + img;
            byte[] byteData = encoding.GetBytes(imgUrl);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(url, content);
            }
        }



        private async void Web_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {

            Monument_Detail_View m = new Monument_Detail_View();
            m.MyLat = pos.Coordinate.Latitude;
            m.MyLon = pos.Coordinate.Longitude;
            bool con = false;
            s = await web.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });
            try
            {
                int i = s.IndexOf("<a class=\"_gUb");
                int j = s.IndexOf(">", i);
                i = s.IndexOf("<", j);
                s = s.Substring(j + 1, i - j - 1);
                s = Check_File(s);
                if (s != null)
                    con = true;
                //load file and get exact name and put it in s
            }
            catch
            { }
            //if match found
            if (con)
            {
                m.Title = s;
                button.IsEnabled = true;
                button2.IsEnabled = true;
                PB.Visibility = Visibility.Collapsed;

                Frame.Navigate(typeof(monument_Detail), m);
                
            }
            else
            {
                VisionServiceClient cl = new VisionServiceClient("db82ef68dc95459fad7b46d7a50bb944");
                VisualFeature[] vf = new VisualFeature[] { VisualFeature.Tags, VisualFeature.Categories };
                AnalysisResult ar = await cl.AnalyzeImageAsync(blobUrl, vf);
                Tag[] f = ar.Tags;
                Category[] c = ar.Categories;
                int i = 0;
                bool tex = false;
                foreach (Tag t in f)
                {
                    if (t.Name.Contains("text"))
                    {
                        tex = true;
                        break;
                    }
                }
                foreach (Category ct in c)
                {
                    if (ct.Name.Contains("text"))
                    {
                        tex = true;
                        break;
                    }
                }
                if (tex)
                {
                    try

                    {
                        string text = "";
                        cl = new VisionServiceClient("db82ef68dc95459fad7b46d7a50bb944");
                        OcrResults ocr = await cl.RecognizeTextAsync(blobUrl);
                        foreach (Region r in ocr.Regions)
                        {
                            foreach (Line l in r.Lines)
                            {
                                foreach (Word w in l.Words)
                                {
                                    text = text + " " + w.Text;
                                }
                            }
                        }
                        button.IsEnabled = true;
                        button2.IsEnabled = true;
                        PB.Visibility = Visibility.Collapsed;

                        Frame.Navigate(typeof(Ocr_Detail), text);
                    }
                    catch (Exception ex)
                    { }
                }
                else
                {
                    button.IsEnabled = true;
                    button2.IsEnabled = true;
                    PB.Visibility = Visibility.Collapsed;

                    Frame.Navigate(typeof(Activity_Detail), f);
                }

            }


        }
        private async void PreviewControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!_isPreviewing) return;

            if (!_isFocused && _mediaCapture.VideoDeviceController.FocusControl.FocusState != MediaCaptureFocusState.Searching)
            {
                var smallEdge = Math.Min(Window.Current.Bounds.Width, Window.Current.Bounds.Height);

                // Choose to make the focus rectangle 1/4th the length of the shortest edge of the window
                var size = new Size(smallEdge / 4, smallEdge / 4);
                var position = e.GetPosition(sender as UIElement);

                // Note that at this point, a rect at "position" with size "size" could extend beyond the preview area. The following method will reposition the rect if that is the case
                await TapToFocus(position, size);
            }
            else
            {
                await TapUnfocus();
            }
        }
        private async Task TapUnfocus()
        {
            _isFocused = false;

            var roiControl = _mediaCapture.VideoDeviceController.RegionsOfInterestControl;
            await roiControl.ClearRegionsAsync();

            var focusControl = _mediaCapture.VideoDeviceController.FocusControl;
            await focusControl.FocusAsync();
        }
        public async Task TapToFocus(Point position, Size size)
        {
            _isFocused = true;

            var previewRect = GetPreviewStreamRectInControl();
            var focusPreview = ConvertUiTapToPreviewRect(position, size, previewRect);

            // Note that this Region Of Interest could be configured to also calculate exposure 
            // and white balance within the region
            var regionOfInterest = new RegionOfInterest
            {
                AutoFocusEnabled = true,
                BoundsNormalized = true,
                Bounds = focusPreview,
                Type = RegionOfInterestType.Unknown,
                Weight = 100,
            };


            var focusControl = _mediaCapture.VideoDeviceController.FocusControl;
            var focusRange = focusControl.SupportedFocusRanges.Contains(AutoFocusRange.FullRange) ? AutoFocusRange.FullRange : focusControl.SupportedFocusRanges.FirstOrDefault();
            var focusMode = focusControl.SupportedFocusModes.Contains(FocusMode.Single) ? FocusMode.Single : focusControl.SupportedFocusModes.FirstOrDefault();
            var settings = new FocusSettings { Mode = focusMode, AutoFocusRange = focusRange };
            focusControl.Configure(settings);

            var roiControl = _mediaCapture.VideoDeviceController.RegionsOfInterestControl;
            await roiControl.SetRegionsAsync(new[] { regionOfInterest }, true);

            await focusControl.FocusAsync();
        }
        public Rect GetPreviewStreamRectInControl()
        {
            var result = new Rect();

            var previewResolution = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;

            // In case this function is called before everything is initialized correctly, return an empty result
            if (PreviewControl == null || PreviewControl.ActualHeight < 1 || PreviewControl.ActualWidth < 1 ||
                previewResolution == null || previewResolution.Height == 0 || previewResolution.Width == 0)
            {
                return result;
            }

            var streamWidth = previewResolution.Width;
            var streamHeight = previewResolution.Height;

            // For portrait orientations, the width and height need to be swapped


            // Start by assuming the preview display area in the control spans the entire width and height both (this is corrected in the next if for the necessary dimension)
            result.Width = PreviewControl.ActualWidth;
            result.Height = PreviewControl.ActualHeight;

            // If UI is "wider" than preview, letterboxing will be on the sides
            if ((PreviewControl.ActualWidth / PreviewControl.ActualHeight > streamWidth / (double)streamHeight))
            {
                var scale = PreviewControl.ActualHeight / streamHeight;
                var scaledWidth = streamWidth * scale;

                result.X = (PreviewControl.ActualWidth - scaledWidth) / 2.0;
                result.Width = scaledWidth;
            }
            else // Preview stream is "wider" than UI, so letterboxing will be on the top+bottom
            {
                var scale = PreviewControl.ActualWidth / streamWidth;
                var scaledHeight = streamHeight * scale;

                result.Y = (PreviewControl.ActualHeight - scaledHeight) / 2.0;
                result.Height = scaledHeight;
            }

            return result;
        }
        private Rect ConvertUiTapToPreviewRect(Point tap, Size size, Rect previewRect)
        {
            // Adjust for the resulting focus rectangle to be centered around the position
            double left = tap.X - size.Width / 2, top = tap.Y - size.Height / 2;

            // Get the information about the active preview area within the CaptureElement (in case it's letterboxed)
            double previewWidth = previewRect.Width, previewHeight = previewRect.Height;
            double previewLeft = previewRect.Left, previewTop = previewRect.Top;

            // Transform the left and top of the tap to account for rotation

            // For portrait orientations, the information about the active preview area needs to be rotated

            // Normalize width and height of the focus rectangle
            var width = size.Width / previewWidth;
            var height = size.Height / previewHeight;

            // Shift rect left and top to be relative to just the active preview area
            left -= previewLeft;
            top -= previewTop;

            // Normalize left and top
            left /= previewWidth;
            top /= previewHeight;

            // Ensure rectangle is fully contained within the active preview area horizontally
            left = Math.Max(left, 0);
            left = Math.Min(1 - width, left);

            // Ensure rectangle is fully contained within the active preview area vertically
            top = Math.Max(top, 0);
            top = Math.Min(1 - height, top);

            // Create and return resulting rectangle
            return new Rect(left, top, width, height);
        }
        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            await CleanupCameraAsync();
        }
        private async void Application_Suspending(object sender, SuspendingEventArgs e)
        {
            // Handle global application events only if this page is active
            if (Frame.CurrentSourcePageType == typeof(MainPage))
            {
                var deferral = e.SuspendingOperation.GetDeferral();
                await CleanupCameraAsync();
                deferral.Complete();
            }
        }
        private async Task StartPreviewAsync()
        {
            try
            {

                _mediaCapture = new MediaCapture();
                await _mediaCapture.InitializeAsync();
                PreviewControl.Source = _mediaCapture;

                await _mediaCapture.StartPreviewAsync();
                _isPreviewing = true;

                _displayRequest.RequestActive();
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;

            }
            catch (UnauthorizedAccessException)
            {
                // This will be thrown if the user denied access to the camera in privacy settings
                System.Diagnostics.Debug.WriteLine("The app was denied access to the camera");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MediaCapture initialization failed. {0}", ex.Message);
            }
        }
        private async Task CleanupCameraAsync()
        {
            if (_mediaCapture != null)
            {
                if (_isPreviewing)
                {
                    await _mediaCapture.StopPreviewAsync();
                }

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PreviewControl.Source = null;
                    if (_displayRequest != null)
                    {
                        _displayRequest.RequestRelease();
                    }

                    _mediaCapture.Dispose();
                    _mediaCapture = null;
                });
            }

        }
        private string Check_File(string str)
        {
            string ans = null;
            bool check = true;
            string[] values = File.ReadAllText(@"Assets/list_monument.txt").Split(',');
            string[] names = str.Split(' ');
            foreach (string item in values)
            {
               
              
                    if (str.ToLower() == item.ToLower() && check)
                    {
                        ans = item;
                        check = false;
                        break;
                    }
                
            }
            if (!check)
            {
                foreach (string nam in names)
                {
                    foreach (string item in values)
                    {
                        string[] temp = item.Split(' ');
                        foreach (string item2 in temp)
                        {
                            if (item2.ToLower() == nam.ToLower() && check)
                            {
                                ans = item;
                                check = false;
                                break;
                            }
                        }
                    }
                }
            }
            return ans;
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
        private double getDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;
            dist *= 1.609344 * 1000;
            return dist;
        }

    }
}
