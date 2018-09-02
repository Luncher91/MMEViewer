using Microsoft.Win32;
using MMEData;
using MMEViewer.MMEViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MMEViewer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<UIElement> dataViewControls = new List<UIElement>();
        public MainWindow()
        {
            CutOffFrequency = 0;

            InitializeComponent();

            dataViewControls.Add(oxyDiagram);
            dataViewControls.Add(browser);
            dataViewControls.Add(documentReader);
            dataViewControls.Add(imageBoxBorder);
            dataViewControls.Add(videoGrid);

            imageBox.RenderTransformOrigin = new Point(0.5, 0.5);

#if DEBUG
            MMEDataSet dataset = new MMEDataSet(
                //@"C:\Users\Andreas\Documents\Visual Studio 2015\Projects\MMEViewer\MMEData\testdata\VW1FGS15\VW1FGS15.mme");
                @"C:\Users\Andreas\Documents\Visual Studio 2015\Projects\MMEViewer\MMEData\testdata\3239_lc\3239_lc.mme");
            dataExplorerTreeView.ItemsSource = new[] { new MMETestModel(dataset) };
#endif
        }

        private void openFileMenu_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openMMEFile = new OpenFileDialog();

            openMMEFile.CheckFileExists = true;
            openMMEFile.DefaultExt = ".mme";
            openMMEFile.Filter = "MME Dataset|*.mme|TDMS data|*.tdms";
            openMMEFile.Multiselect = false;

            if (openMMEFile.ShowDialog() == true)
            {
                if (openMMEFile.FileName.EndsWith(".mme"))
                {
                    MMEDataSet dataset = new MMEDataSet(openMMEFile.FileName);
                    dataExplorerTreeView.ItemsSource = new[] { new MMETestModel(dataset) };
                }
                else
                {
                    MMEDataSet dataset = MMEDataSet.FromTDMS(openMMEFile.FileName);
                    dataExplorerTreeView.ItemsSource = new[] { new MMETestModel(dataset) };
                }
            }
        }

        private void dataExplorerTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

        private void dataExplorerTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            object selectedItem = dataExplorerTreeView.SelectedItem;

            if (selectedItem is MMEChannelModel)
            {
                DisplayChannel();
            }
            else if(selectedItem is MMEPhotoModel)
            {
                MMEPhotoModel phoModel = selectedItem as MMEPhotoModel;
                LoadPhotoAttributes(phoModel);

                ShowImage(phoModel.ImagePath);
            }
            else if (selectedItem is MMEMovieModel)
            {
                MMEMovieModel movModel = selectedItem as MMEMovieModel;
                LoadMovieAttributes(movModel);

                ShowMovie(movModel.ImagePath, 
                    int.Parse(
                                movModel.ActualMovie.Attributes.FirstOrDefault(
                                    a => a.Key.Equals("Number of images", StringComparison.InvariantCultureIgnoreCase)
                                )
                                .Value.Value
                            )
                        );
            }
            else if(selectedItem is MMEDocumentModel)
            {
                MMEDocumentModel docModel = selectedItem as MMEDocumentModel;

                LoadDocumentAttributes(docModel);

                ShowDocument(docModel.DocumentPath);
            }
        }

        MMEChannelModel selectedChannel = null;
        private void DisplayChannel()
        {
            MMEChannelModel chnModel = dataExplorerTreeView.SelectedItem as MMEChannelModel;

            if (chnModel == null)
                if (selectedChannel == null)
                    return;
                else
                    chnModel = selectedChannel;
            else
                selectedChannel = chnModel;


            LoadChannelAttributes(chnModel);

            chnModel.RefreshDataPoints();
            ShowChannel(chnModel);
        }

        private void ShowDocument(string documentPath)
        {
            if(File.Exists(documentPath))
            {
                if (documentPath.EndsWith(".txt"))
                {
                    Paragraph para = new Paragraph();
                    para.Inlines.Add(File.ReadAllText(documentPath));
                    documentReader.Document = new FlowDocument(para)
                    {
                        FontFamily = new FontFamily("Global Monospace")
                    };
                    documentReader.ViewingMode = FlowDocumentReaderViewingMode.Scroll;

                    ShowFlowDocumentControl();
                }
                else if (documentPath.EndsWith(".pdf"))
                {
                    browser.Navigate(documentPath);

                    ShowBrowser();
                }
                else if (EndsWithExtension(documentPath, "html", "htm", "txt", "xml"))
                {
                    browser.Navigate(documentPath);

                    ShowBrowser();
                }
                else if (EndsWithExtension(documentPath, "jpg", "jpeg", "bmp", "png", "gif", "tif"))
                {
                    ShowImage(documentPath);
                    ShowImageControl();
                }
            }
        }

        private bool EndsWithExtension(string filePath, params string[] extensions)
        {
            return extensions.Any(a => filePath.EndsWith("." + a, StringComparison.InvariantCultureIgnoreCase));
        }

        private void LoadDocumentAttributes(MMEDocumentModel docModel)
        {
            // nothing todo
        }

        private void ShowMovie(string imagePath, int numberOfFrames = 0)
        {
            NumberOfFrames = numberOfFrames;

            videoElement.LoadedBehavior = MediaState.Manual;
            videoElement.UnloadedBehavior = MediaState.Stop;
            videoElement.ScrubbingEnabled = true;
            videoElement.Source = new Uri(imagePath);

            videoElement.Play();
            videoElement.Pause();

            sliderLabel.Content = videoElement.Position.ToString("hh\\:mm\\:ss\\.fff");

            ShowVideoControl();
        }

        public void ShowBrowser()
        {
            ShowDataViewUIElement(browser);
        }

        private void ShowDataViewUIElement(UIElement showUIControl)
        {
            dataViewControls.ForEach(a => a.Visibility = Visibility.Hidden);
            showUIControl.Visibility = Visibility.Visible;
        }

        public void ShowFlowDocumentControl()
        {
            ShowDataViewUIElement(documentReader);
        }

        public void ShowChartControl()
        {
            ShowDataViewUIElement(oxyDiagram);
        }

        public void ShowVideoControl()
        {
            ShowDataViewUIElement(videoGrid);
        }

        public void ShowImageControl()
        {
            ShowDataViewUIElement(imageBoxBorder);
        }

        private void LoadMovieAttributes(MMEMovieModel movModel)
        {
            attributeDataGrid.ItemsSource = movModel.ActualMovie.Attributes;
        }

        private void ShowChannel(MMEChannelModel chnModel)
        {
            oxyDiagram.DataContext = null;
            oxyDiagram.DataContext = chnModel;

            ShowChartControl();
        }

        private void ShowImage(string imagePath)
        {
            ResetImageTransform();

            BitmapImage src = new BitmapImage();

            src.BeginInit();
            src.UriSource = new Uri(imagePath, UriKind.Absolute);
            src.EndInit();

            imageBox.Source = src;

            ShowImageControl();
        }

        private void LoadPhotoAttributes(MMEPhotoModel phoModel)
        {
            attributeDataGrid.ItemsSource = phoModel.ActualPhoto.Attributes;
        }

        private void LoadChannelAttributes(MMEChannelModel chnModel)
        {
            if(!chnModel.ActualChannel.IsLoaded)
                chnModel.ActualChannel.Load();

            attributeDataGrid.ItemsSource = chnModel.ActualChannel.Attributes;
        }

        private const double MAX_SCALE = 50.0;
        private const double MIN_SCALE = 0.5;
        private const double SCROLL_SPEED = 0.1;
        private void imageBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScaleTransform scaleTransform = scaleImage;
            double zoom = e.Delta > 0 ? 1.0 + SCROLL_SPEED : 1.0 - SCROLL_SPEED;

            double newXScale = scaleTransform.ScaleX * zoom;
            double newYScale = scaleTransform.ScaleY * zoom;

            if (newXScale < MIN_SCALE)
            {
                scaleTransform.ScaleX = MIN_SCALE;
                scaleTransform.ScaleY = MIN_SCALE;
            }
            else if (newXScale >= MAX_SCALE)
            {
                scaleTransform.ScaleX = MAX_SCALE;
                scaleTransform.ScaleY = MAX_SCALE;
            }
            else
            {
                scaleTransform.ScaleX = newXScale;
                scaleTransform.ScaleY = newYScale;
            }
        }

        Point start;
        Point origin;

        private void imageBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            imageBox.CaptureMouse();
            var tt = translateImage;
            start = e.GetPosition(imageBoxBorder);
            origin = new Point(tt.X, tt.Y);
        }

        private void imageBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (imageBox.IsMouseCaptured)
            {
                var tt = translateImage;
                Vector v = start - e.GetPosition(imageBoxBorder);
                tt.X = origin.X - (v.X/scaleImage.ScaleX);
                tt.Y = origin.Y - (v.Y/scaleImage.ScaleY);

                scaleImage.CenterX = 0.5-tt.X;
                scaleImage.CenterY = 0.5-tt.Y;
            }
        }

        private void imageBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imageBox.ReleaseMouseCapture();
        }

        private void imageBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                ResetImageTransform();
            }
        }

        private void ResetImageTransform()
        {
            translateImage.X = 1;
            translateImage.Y = 1;
            scaleImage.ScaleX = 1;
            scaleImage.ScaleY = 1;
            scaleImage.CenterX = 0.5;
            scaleImage.CenterY = 0.5;
        }

        private void videoSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            videoElement.Position = TimeSpan.FromMilliseconds(videoSlider.Value);
            sliderLabel.Content = videoElement.Position.ToString("hh\\:mm\\:ss\\.fff");
        }

        private void playVideoButton_Click(object sender, RoutedEventArgs e)
        {
            if (!VideoIsPlaying)
            {
                PlayVideo();
            }
            else
            {
                PauseVideo();
            }
        }

        private bool VideoIsPlaying = false;

        private void PlayVideo()
        {
            videoElement.Play();
            playVideoButton.Content = "pause";
            VideoIsPlaying = true;
        }

        private void PauseVideo()
        {
            videoElement.Pause();
            playVideoButton.Content = "play";
            VideoIsPlaying = false;
        }

        private void StopVideo()
        {
            videoElement.Stop();
            playVideoButton.Content = "play";
            videoElement.Position = TimeSpan.Zero;
            RefreshVideoSlider();
            VideoIsPlaying = false;
        }

        private void RefreshVideoSlider()
        {
            videoSlider.Dispatcher.Invoke(() => { videoSlider.Value = videoElement.Position.TotalMilliseconds; });
        }

        private void videoElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            StopVideo();
        }

        private int NumberOfFrames = 0;
        private void videoElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            videoSlider.Minimum = 0;

            if (videoElement.NaturalDuration.HasTimeSpan)
            {
                videoSlider.Maximum = videoElement.NaturalDuration.TimeSpan.TotalMilliseconds;
                if (NumberOfFrames > 0)
                {
                    videoSlider.TickFrequency = videoElement.NaturalDuration.TimeSpan.TotalMilliseconds / (NumberOfFrames - 1);
                    videoSlider.TickPlacement = System.Windows.Controls.Primitives.TickPlacement.TopLeft;
                    videoSlider.SmallChange = videoSlider.TickFrequency;
                    videoSlider.IsSnapToTickEnabled = true;
                }
            }
        }

        private Thread sliderRefreshThread = null;

        public static int CutOffFrequency { get; internal set; }

        private void videoElement_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sliderRefreshThread == null)
            {
                sliderRefreshThread = new Thread(
                        delegate ()
                        {
                            while (videoElement.IsVisible)
                            {
                                if(VideoIsPlaying)
                                    RefreshVideoSlider();

                                Thread.Sleep(50);
                            }
                            sliderRefreshThread = null;
                        }
                    );
                sliderRefreshThread.Start();
            }
        }

        private void cutOffFrequency_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CutOffFrequency = (int)cutOffFrequency.Value;

            if (cutOffLabel != null)
                cutOffLabel.Content = CutOffFrequency;

            DisplayChannel();
        }
    }
}
