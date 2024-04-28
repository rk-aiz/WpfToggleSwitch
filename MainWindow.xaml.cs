using System.Dynamic;
using System.Globalization;
using System.Text;
using System.Threading.Channels;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfToggleSwitch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            dynamic expObj = new ExpandoObject();
            expObj.IsOn = true;

            DataContext = expObj;
            InitializeComponent();
        }

        void onDragDelta(object sender, DragDeltaEventArgs e)
        {
            //Move the Thumb to the mouse position during the drag operation
            double yadjust = myCanvas.Height + e.VerticalChange;
            double xadjust = myCanvas.Width + e.HorizontalChange;
            if ((xadjust >= 0) && (yadjust >= 0))
            {
                myCanvas.Width = xadjust;
                myCanvas.Height = yadjust;
                Canvas.SetLeft(myThumb, Canvas.GetLeft(myThumb) +
                                        e.HorizontalChange);
                Canvas.SetTop(myThumb, Canvas.GetTop(myThumb) +
                                        e.VerticalChange);
            }
        }

        void onDragStarted(object sender, DragStartedEventArgs e)
        {
            myThumb.Background = Brushes.Orange;
        }

        void onDragCompleted(object sender, DragCompletedEventArgs e)
        {
            myThumb.Background = Brushes.Blue;
        }
    }
}