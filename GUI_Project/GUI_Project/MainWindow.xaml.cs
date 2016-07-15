using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Button a = new Button();

            SettingBtn.TouchDown += SettingBtn_TouchDown;
        }

        private void SettingBtn_TouchDown(object sender, TouchEventArgs e)
        {
            var a = new DoubleAnimation(1, 0.9, Duration.Automatic);
            Storyboard sb = new Storyboard();
            sb.Children.Add(a);
            Storyboard.SetTarget(a, SettingBtn);
            Storyboard.SetTargetProperty(a, new PropertyPath("(ScaleX)"));
        }
    }
}
