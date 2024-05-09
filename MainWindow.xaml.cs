using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using GMap.NET.WindowsPresentation;


namespace Project2_Code
{    
    public partial class MainWindow : Window
    {
        AsterixParser parser;
        AsterixSimulation simulation;
        bool active;
        DispatcherTimer simulationTimer;
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
            }
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (this.simulation == null)
            {
                MessageBox.Show("Load a file first.");
                return;
            }

            if (!this.active)
            {
                simulationTimer.Start();
                PlayButton.Content = new Image { Source = this.FindResource("pauseIcon") as DrawingImage };
                this.active = true;
            }
            else
            {
                simulationTimer.Stop();
                PlayButton.Content = new Image { Source = this.FindResource("playIcon") as DrawingImage };
                this.active = false;
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateSimulation(object sender, EventArgs e)
        {
            Debug.WriteLine(simulation.time);
            simulation.Update();
            gmap.Markers.Clear();
            foreach (Aircraft a in simulation.aircrafts.Values)
            {
                GMap.NET.PointLatLng point = new GMap.NET.PointLatLng(a.latitude, a.longitude);
                Polyline indicator = new Polyline();
                indicator.Points.Add(new Point(0, -15)); //Arreglar geometria centrada en coordenadas
                indicator.Points.Add(new Point(-10, 15)); //Decodificar los BDS 4 5 6 
                indicator.Points.Add(new Point(0, 5));    //Filtros: Transponder fijo = antena en gava academia aviacion
                indicator.Points.Add(new Point(10, 15));    // Filtro blanco puro todos que no son Modo-S en DI Type020 (4 primeros no)
                indicator.Points.Add(new Point(0, -15));     // Filtro on ground
                indicator.Stroke = Brushes.Red;                // boton stop / velocidad / zoom
                indicator.Fill = Brushes.Red;
                indicator.StrokeThickness = 1;

                double scale = 0.3 + 0.07 * (gmap.Zoom - 7);
                TransformGroup transform = new TransformGroup();
                transform.Children.Add(new RotateTransform(a.heading));
                transform.Children.Add(new ScaleTransform(scale, scale));
                indicator.RenderTransform = transform;
                indicator.ToolTip = new ToolTip { Content = $"{a.id}\n{a.groundSpeed} kt\n{a.flightLevel}\n{a.latitude} : {a.longitude}" };

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
