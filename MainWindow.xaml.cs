using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Collections.Generic;
using GMap.NET.WindowsPresentation;

namespace Project2_Code
{    
    public partial class MainWindow : Window
    {
        AsterixParser parser;
        AsterixSimulation simulation;
        DispatcherTimer simulationTimer;
        bool active; 

        public MainWindow()
        {
            InitializeComponent();
            this.simulationTimer = new DispatcherTimer();
            this.simulationTimer.Tick += new EventHandler(UpdateSimulation);
            this.simulationTimer.Interval = new TimeSpan(0, 0, 1);            
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Asterix files |*.ast";
            openFile.Title = "Select asterix file";
            openFile.ShowDialog();
            string fileName = openFile.FileName;

            if (fileName.Length != 0)
            {
                this.parser = new AsterixParser(fileName);
                this.simulation = new AsterixSimulation(parser);
                DataGrid.DataContext = parser.CAT48table.DefaultView;                
                FilterFixed.IsEnabled = true;
                FilterPure.IsEnabled = true;
                FilterGround.IsEnabled = true;
                PlayButton.IsEnabled = true;
                ResetButton.IsEnabled = true;
                ExportButton.IsEnabled = true;
                SpeedSlider.IsEnabled = true;
                TimeSlider.IsEnabled = true;
                TimeSlider.Minimum = Math.Floor(simulation.CAT48list[0].TIME);
                TimeSlider.Maximum = Math.Ceiling(simulation.CAT48list[^1].TIME);
                SpeedBox.IsEnabled = true;
                TimeBox.IsEnabled = true;
            }
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (!this.active)
            {
                this.simulationTimer.Start();
                PlayButton.Content = new Image { Source = this.FindResource("pauseIcon") as DrawingImage, Width = 30, Height = 30 };
                this.active = true;
            }
            else
            {
                this.simulationTimer.Stop();
                PlayButton.Content = new Image { Source = this.FindResource("playIcon") as DrawingImage, Width = 30, Height = 30 };
                this.active = false;
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            this.simulationTimer.Stop();
            PlayButton.Content = new Image { Source = this.FindResource("playIcon") as DrawingImage, Width = 30, Height = 30 };
            SpeedSlider.Value = SpeedSlider.Minimum;
            TimeSlider.Value = TimeSlider.Minimum;
            this.active = false;
            this.simulation.Reset();
            gmap.Markers.Clear();
        }

        private void Speed_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (this.simulation != null)
            {
                this.simulation.simSpeed = SpeedSlider.Value;
            }
        }

        private void Time_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (this.simulation != null)
            {
                gmap.Markers.Clear();
                this.simulation.Reset();
                this.simulation.simSpeed = SpeedSlider.Value;
                this.simulation.time = TimeSlider.Value;
            }
        }

        private void ExportCSV_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog openFile = new SaveFileDialog();
            openFile.Filter = "CSV |*.csv";
            openFile.Title = "Export to CSV";
            openFile.ShowDialog();
            string fileName = openFile.FileName;
            if (fileName.Length != 0)
            {
                this.parser.ExportToCSV(fileName);
            }        
        }

        private void Filter_Toggle(object sender, RoutedEventArgs e)
        {
            List<string> activeFilters = new List<string>();
            if (FilterFixed.IsChecked) activeFilters.Add("TN <> 1838");
            if (FilterPure.IsChecked) activeFilters.Add("TYP_020 LIKE '*ModeS*'");
            if (FilterGround.IsChecked) activeFilters.Add("HEIGHT > 1");

            string filter = string.Join(" AND ", activeFilters.ToArray());
            parser.CAT48table.DefaultView.RowFilter = filter;         
        }

        private void UpdateSimulation(object sender, EventArgs e)
        {
            this.simulation.Update();
            this.TimeSlider.Value = this.simulation.time;
            gmap.Markers.Clear();
            foreach (Aircraft a in this.simulation.aircrafts.Values)
            {
                if (FilterFixed.IsChecked && a.trackNumber == 1838) continue;
                if (FilterPure.IsChecked && a.type < 4) continue;
                if (FilterGround.IsChecked && a.height < 1) continue;
                GMap.NET.PointLatLng point = new GMap.NET.PointLatLng(a.latitude, a.longitude);
                Polyline indicator = new Polyline();
                indicator.Points.Add(new Point(0, -15));
                indicator.Points.Add(new Point(-10, 15));
                indicator.Points.Add(new Point(0, 5));
                indicator.Points.Add(new Point(10, 15));
                indicator.Points.Add(new Point(0, -15));
                indicator.Stroke = Brushes.Red;
                indicator.Fill = Brushes.Red;
                indicator.StrokeThickness = 1;

                double scale = 0.3 + 0.07 * (gmap.Zoom - 7);
                TransformGroup transform = new TransformGroup();
                transform.Children.Add(new RotateTransform(a.heading));
                transform.Children.Add(new ScaleTransform(scale, scale));
                indicator.RenderTransform = transform;
                indicator.ToolTip = new ToolTip { Content = $"{a.id}\n{Math.Round(a.groundSpeed, 2)} kt\n{Math.Round(a.height, 2)} m\n{Utils.DecToDMS(a.latitude, a.longitude)}" };

                GMapMarker marker = new GMapMarker(point);
                marker.Shape = indicator;
                gmap.Markers.Add(marker);
            }
        }


        private void gmapLoaded(object sender, RoutedEventArgs e)
        {
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            // choose your provider here
            gmap.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
            gmap.MinZoom = 7;
            gmap.MaxZoom = 17;
            // whole world zoom
            gmap.Zoom = 12;
            gmap.CenterPosition = new GMap.NET.PointLatLng(41.3007023333, 2.1020581944); // radar coordinates
            // lets the map use the mousewheel to zoom
            gmap.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            // lets the user drag the map
            gmap.CanDragMap = true;
            // lets the user drag the map with the left mouse button
            gmap.DragButton = MouseButton.Left;
        }        
    }
}
