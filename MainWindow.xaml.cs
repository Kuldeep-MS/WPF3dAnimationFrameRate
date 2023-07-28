using System.Diagnostics;
using System.Text;
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
        //private Viewport3D v3d = new Viewport3D();
        public MainWindow()
        {
            InitializeComponent();

            // Start the timer on the UI thread to measure frame rate
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            // Set up 3D animation
            rotateTransform.Rotation = rotation;
            ModelVisual3D modelVisual = new ModelVisual3D();
            modelVisual.Content = Create3DCube();
            modelVisual.Transform = rotateTransform;
            viewport3D.Children.Add(modelVisual);
            CompositionTarget.Rendering += CompositionTarget_Rendering;

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
    }
}
