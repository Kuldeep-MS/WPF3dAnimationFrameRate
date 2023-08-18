using System;
using System.Diagnostics;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace wpfframerate
{
    public partial class MainWindow : Window
    {
        private Stopwatch frameTimer = new Stopwatch();
        private int frameCount = 0;
        private double frameRate = 0;

        // 3D animation variables
        private AxisAngleRotation3D rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
        private RotateTransform3D rotateTransform = new RotateTransform3D();
        PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
        PerformanceCounter ramCounter = new PerformanceCounter("Process", "Working Set", Process.GetCurrentProcess().ProcessName);
        //PerformanceCounter gpuCounter = new PerformanceCounter("NVIDIA GPU", "% GPU Usage", Process.GetCurrentProcess().ProcessName);
        //PerformanceCounter gpuMemoryCounter = new PerformanceCounter("NVIDIA GPU", "% GPU Memory Usage", Process.GetCurrentProcess().ProcessName);
        //private Viewport3D v3d = new Viewport3D();
        int col = 1;
        int row = 0;
        int count = 1;
        Queue<float> queue = new Queue<float>();
        float sum = 0;
        public MainWindow()
        {
            InitializeComponent();

            var counters = PerformanceCounterCategory.GetCategories()
        .SelectMany(x => x.GetCounters("")).Where(x => x.CounterName.Contains("GPU"));

            foreach (var counter in counters)
            {
                Console.WriteLine("{0} - {1}", counter.CategoryName, counter.CounterName);
            }

            // Start the timer on the UI thread to measure frame rate
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Tick += TimerElapsed;
            timer.Start();



            // Set up 3D animation
            rotateTransform.Rotation = rotation;
            ModelVisual3D modelVisual = new ModelVisual3D();
            modelVisual.Content = Create3DCube();
            modelVisual.Transform = rotateTransform;
            viewport3D.Children.Add(modelVisual);
            CompositionTarget.Rendering += CompositionTarget_Rendering;

            bool fval = false;
            AppContext.TryGetSwitch("Switch.System.Windows.Media.EnableHardwareAccelerationInRdp", out fval);

            HwAcc.Content = $"IsHardwarAccEnabled: {fval} ";
            ProcessNameLabel.Content = $"Process Name: {Process.GetCurrentProcess().ProcessName} ";

        }

        void TimerElapsed(object source, EventArgs e)
        {
            float c = cpuCounter.NextValue();
            if(queue.Count <= 20)
            {
                queue.Enqueue(c);
                sum += c;
            } else
            {
                float temp = queue.Dequeue();
                sum -= temp;
            }
            float r = ramCounter.NextValue() / (1024*1024);
            //string g = gpuCounter.NextValue().ToString();

            CpuUsageLabel.Content = $"CPU Usage: {sum/queue.Count} ";
            RamUsageLabel.Content = $"Ram Uagae: {r} MB";
            //GpuUsageLabel.Content = $"GPU Usage: {gpuCounter.NextValue()} ";
            //GpuMemryUsageLabel.Content = $"GPU Memory Usage: {(gpuMemoryCounter.NextValue()) / (1024 * 1024)} MB";

        }

        private Model3DGroup Create3DCube()
        {
            Model3DGroup cube = new Model3DGroup();

            MeshGeometry3D meshGeometry = new MeshGeometry3D();
            meshGeometry.Positions.Add(new Point3D(-1, -1, -1));
            meshGeometry.Positions.Add(new Point3D(1, -1, -1));
            meshGeometry.Positions.Add(new Point3D(1, 1, -1));
            meshGeometry.Positions.Add(new Point3D(-1, 1, -1));
            meshGeometry.Positions.Add(new Point3D(-1, -1, 1));
            meshGeometry.Positions.Add(new Point3D(1, -1, 1));
            meshGeometry.Positions.Add(new Point3D(1, 1, 1));
            meshGeometry.Positions.Add(new Point3D(-1, 1, 1));

            meshGeometry.TriangleIndices.Add(0);
            meshGeometry.TriangleIndices.Add(1);
            meshGeometry.TriangleIndices.Add(2);
            meshGeometry.TriangleIndices.Add(0);
            meshGeometry.TriangleIndices.Add(2);
            meshGeometry.TriangleIndices.Add(3);
            meshGeometry.TriangleIndices.Add(4);
            meshGeometry.TriangleIndices.Add(6);
            meshGeometry.TriangleIndices.Add(5);
            meshGeometry.TriangleIndices.Add(4);
            meshGeometry.TriangleIndices.Add(7);
            meshGeometry.TriangleIndices.Add(6);
            meshGeometry.TriangleIndices.Add(4);
            meshGeometry.TriangleIndices.Add(5);
            meshGeometry.TriangleIndices.Add(1);
            meshGeometry.TriangleIndices.Add(4);
            meshGeometry.TriangleIndices.Add(1);
            meshGeometry.TriangleIndices.Add(0);
            meshGeometry.TriangleIndices.Add(2);
            meshGeometry.TriangleIndices.Add(6);
            meshGeometry.TriangleIndices.Add(7);
            meshGeometry.TriangleIndices.Add(3);
            meshGeometry.TriangleIndices.Add(2);
            meshGeometry.TriangleIndices.Add(7);
            meshGeometry.TriangleIndices.Add(6);
            meshGeometry.TriangleIndices.Add(4);
            meshGeometry.TriangleIndices.Add(0);
            meshGeometry.TriangleIndices.Add(3);
            meshGeometry.TriangleIndices.Add(7);
            meshGeometry.TriangleIndices.Add(4);
            meshGeometry.TriangleIndices.Add(3);
            meshGeometry.TriangleIndices.Add(0);
            meshGeometry.TriangleIndices.Add(1);
            meshGeometry.TriangleIndices.Add(5);
            meshGeometry.TriangleIndices.Add(6);
            meshGeometry.TriangleIndices.Add(2);

            DiffuseMaterial material = new DiffuseMaterial(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue));
            GeometryModel3D model = new GeometryModel3D(meshGeometry, material);
            cube.Children.Add(model);

            return cube;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Calculate the frame rate and reset the counter
            frameRate = frameCount;
            frameCount = 0;

            // Update the UI with the frame rate information
            FrameRateLabel.Content = $"Frame Rate: {frameRate} FPS";
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            // Increment the frame count on each rendering event
            frameCount++;

            // Update the 3D rotation angle
            double angleIncrement = 1.0; // Adjust the rotation speed
            rotation.Angle += angleIncrement;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            col++;
            if(col >= 5)
            {
                col = 0;
                DisplayGrid.RowDefinitions.Add(new RowDefinition());
                row++;
            }
            //ModelVisual3D modelVisual = new ModelVisual3D();
            //modelVisual.Content = Create3DCube();
            //modelVisual.Transform = rotateTransform;
            //Viewport3D vp3D = new Viewport3D();
            Frame fp = new Frame();
            fp.Source = new Uri("Page1.xaml", UriKind.Relative);
            fp.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            //vp3D.Camera = new PerspectiveCamera(new Point3D(0, 0, 5), new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), 60);
            //vp3D.Children.Add(modelVisual);
            DisplayGrid.Children.Add(fp);
            Grid.SetColumn(fp, col);
            Grid.SetRow(fp, row);
            count++;

            Num.Content = $"Numbe of Cubes: {count}";


        }
    }
}
