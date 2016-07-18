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
    /// Interaction logic for Sidepanel.xaml
    /// </summary>
    public partial class Sidepanel : UserControl
    {
        public Sidepanel()
        {
            InitializeComponent();
        }

        private async void ChangeButtonIcon(bool show)
        {
            Storyboard storyBoard = new Storyboard();
            DoubleAnimation dAnim = new DoubleAnimation();
            ImageSource imageSource;
            if (show)
            {
                imageSource = new BitmapImage(new Uri("icons/leftArrow.png", UriKind.Relative));
            }
            else
            {
                imageSource = new BitmapImage(new Uri("icons\\rightArrow.png", UriKind.Relative));
            }
            dAnim.Duration = new Duration(TimeSpan.FromMilliseconds(100));
            dAnim.From = 1;
            dAnim.To = 0;
            ToggleButton.BeginAnimation(Image.OpacityProperty, dAnim);
            await Task.Delay(200);
            ToggleButton.Source = imageSource;
            dAnim.From = 0;
            dAnim.To = 1;
            ToggleButton.BeginAnimation(Image.OpacityProperty, dAnim);
        }

        private void ToggleSidepanel()
        {
            if(this.Margin.Left == -240)
            {
                ThicknessAnimation slideAnimation = new ThicknessAnimation();
                slideAnimation.From = new Thickness(-240, -100, 0, 0);
                slideAnimation.To = new Thickness(0, -100, 0, 0);
                slideAnimation.AccelerationRatio = 0.6;
                slideAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                this.BeginAnimation(UserControl.MarginProperty, slideAnimation);
                ChangeButtonIcon(true);
            }
            else if(this.Margin.Left == 0)
            {
                ThicknessAnimation slideAnimation = new ThicknessAnimation();
                slideAnimation.From = new Thickness(0, -100, 0, 0);
                slideAnimation.To = new Thickness(-240, -100, 0, 0);
                slideAnimation.AccelerationRatio = 0.6;
                slideAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                this.BeginAnimation(UserControl.MarginProperty, slideAnimation);
                ChangeButtonIcon(false);
            }
        }

        private void ToggleButton_TouchUp(object sender, TouchEventArgs e)
        {
            ToggleSidepanel();
        }

        private void ToggleButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           
            previouspoint = e.GetPosition(this);
        }

        private Point previouspoint;
        private void ToggleButton_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed && this.Margin.Left <= 0 && this.Margin.Left >= -240)
            {
                this.Margin = new Thickness(this.Margin.Left + e.GetPosition(this).X - previouspoint.X,-100,0,0);
            }
        }

        private void ToggleButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ToggleSidepanel();
        }
    }
}
