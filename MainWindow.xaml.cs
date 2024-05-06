using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Diagnostics;
using GMap.NET.WindowsPresentation;


namespace Project2_Code
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AsterixSimulation simulation;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.ShowDialog();
            string fileName = openFile.FileName;

            AsterixParser parser = new AsterixParser(fileName);
            simulation = new AsterixSimulation(parser);

            DispatcherTimer simulationTimer = new DispatcherTimer();
            simulationTimer.Tick += new EventHandler(UpdateSimulation);
            simulationTimer.Interval = new TimeSpan(0, 0, 1);
            simulationTimer.Start();

        }

        private void UpdateSimulation(object sender, EventArgs e)
        {
            Debug.WriteLine(simulation.time);
            simulation.Update();
            gmap.Markers.Clear();
            foreach (Aircraft a in simulation.aircrafts.Values)
            {
                GMap.NET.PointLatLng point = new GMap.NET.PointLatLng(a.latitude, a.longitude);
                //Debug.WriteLine(point);
                Rectangle indicator = new Rectangle { Width = 5, Height = 5, Fill = System.Windows.Media.Brushes.Red };
                indicator.ToolTip = new ToolTip { Content = $"{a.id}\n{a.groundSpeed} kt\n{a.flightLevel}" };
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
