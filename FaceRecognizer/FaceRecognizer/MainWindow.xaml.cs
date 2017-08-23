using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfCap;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace FaceRecognizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Result { get; set; }
        public double ProgressPercent { get; set; }
        public MainWindow()
        {

            InitializeComponent();
            Name = "Ben";
            Result = "You look like a: " + Name;
            ReturnText.DataContext = this;
            ProgressPercent = .0 * 502;
            ProgressBar.DataContext = this;


        }



        private void TakePictureButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 1; i++)
            {
                ProgressPercent += .01 * 502;
                this.ProgressBar.Width = ProgressPercent;
            }
        }

        private void RetakeButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
