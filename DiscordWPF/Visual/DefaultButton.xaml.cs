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
using System.Windows.Threading;

namespace DiscordWPF.Visual
{
    /// <summary>
    /// Interaction logic for DefaultButton.xaml
    /// </summary>
    public partial class DefaultButton : UserControl
    {
        public DefaultButton()
        {
            InitializeComponent();
            cl.Tick += Cl_Tick;

            if (ButtonFill == DefaultButtonFill.Blue)
            {
                background.Color = borderbrush.Color = Color.FromRgb(114, 137, 218);
                foreground.Color = Color.FromRgb(255, 255, 255);
            }
            else if (ButtonFill == DefaultButtonFill.BlueBorder)
            {
                foreground.Color = borderbrush.Color = Color.FromRgb(114, 137, 218);
                background.Color = Color.FromArgb(1,255, 255, 255);
            }
            else if (ButtonFill == DefaultButtonFill.Gray)
            {
                background.Color = borderbrush.Color = Color.FromRgb(79,84,92);
                foreground.Color = Color.FromRgb(255, 255, 255);
            }
            else if (ButtonFill == DefaultButtonFill.Green)
            {   
                background.Color = borderbrush.Color = Color.FromRgb(67,181,129);
                foreground.Color = Color.FromRgb(255, 255, 255);
            }
            else if (ButtonFill == DefaultButtonFill.RedBorder)
            {
                foreground.Color = borderbrush.Color = Color.FromRgb(185,67,69);
                background.Color = Color.FromArgb(1,255, 255, 255);
            }
            else
            {
                foreground.Color = Color.FromRgb(255, 255, 255);
                borderbrush.Color = Color.FromRgb(115, 117, 121);
                background.Color = Color.FromArgb(0, 0, 0, 0);
            }
        }

        private bool Clicked = false;
        private void Cl_Tick(object sender, EventArgs e)
        {
            Clicked = false;
            cl.Stop();
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(DefaultButton));
        public static readonly DependencyProperty ButtonFillProperty = DependencyProperty.Register("ButtonFill", typeof(DefaultButtonFill), typeof(DefaultButton),new PropertyMetadata(DefaultButtonFill.Blue));

        private bool selfHovered = false;

        public static DispatcherTimer cl = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };


        public bool Hovered
        {
            get
            {
                return selfHovered;
            }
            set
            {
                Color ToColor = new Color(); //BorderBrush
                Color ToColor2 = new Color(); //Background
                Color ToColor3 = new Color(); //Foreground
                if(ButtonFill == DefaultButtonFill.Blue)
                {
                    ToColor = ToColor2 = value ? Color.FromRgb(103, 123, 196) : Color.FromRgb(111, 140, 222);
                    ToColor3 = value ? Color.FromRgb(245, 245, 245) : Color.FromRgb(255, 255, 255);
                }
                else if(ButtonFill == DefaultButtonFill.BlueBorder)
                {
                    ToColor = ToColor3 = value ? Color.FromRgb(103, 123, 196) : Color.FromRgb(111, 140, 222);
                    ToColor2 = Color.FromArgb(1,255, 255, 255);
                }
                else if(ButtonFill == DefaultButtonFill.Gray)
                {
                    ToColor = ToColor2 = value ? Color.FromRgb(103, 123, 196) : Color.FromRgb(111, 140, 222);
                    ToColor3 = value ? Color.FromRgb(245, 245, 245) : Color.FromRgb(255, 255, 255);
                }
                else if(ButtonFill == DefaultButtonFill.RedBorder)
                {
                    ToColor = ToColor3 = value ? Color.FromRgb(166,66,68) : Color.FromRgb(110, 61, 65);
                    ToColor2 = Color.FromArgb(1, 255, 255, 255);
                }
                else if(ButtonFill == DefaultButtonFill.Green)
                {
                    ToColor = ToColor2 = value ? Color.FromRgb(60,163,116) : Color.FromRgb(67, 181, 129);
                    ToColor3 = value ? Color.FromRgb(245, 245, 245) : Color.FromRgb(255, 255, 255);
                }
                else
                {
                    ToColor = value ? Color.FromRgb(175, 176, 178) : Color.FromRgb(115, 117, 121);
                    ToColor2 = Color.FromArgb(1, 255, 255, 255);
                    ToColor3 = Color.FromRgb(175, 176, 178);
                }
                borderbrush.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = ToColor, Duration = TimeSpan.FromMilliseconds(150) });
                background.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = ToColor2, Duration = TimeSpan.FromMilliseconds(150) });
                foreground.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation { To = ToColor3, Duration = TimeSpan.FromMilliseconds(150) });
                selfHovered = value;
            }
        }

        public string Text {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public DefaultButtonFill ButtonFill
        {
            get
            {
                return (DefaultButtonFill)GetValue(ButtonFillProperty);
            }
            set
            {
                SetValue(ButtonFillProperty, value);
                if (value == DefaultButtonFill.Blue)
                {
                    background.Color = borderbrush.Color = Color.FromRgb(114, 137, 218);
                    foreground.Color = Color.FromRgb(255, 255, 255);
                }
                else if (value == DefaultButtonFill.BlueBorder)
                {
                    foreground.Color = borderbrush.Color = Color.FromRgb(114, 137, 218);
                    background.Color = Color.FromArgb(1, 255, 255, 255);
                }
                else if (value == DefaultButtonFill.Gray)
                {
                    background.Color = borderbrush.Color = Color.FromRgb(79, 84, 92);
                    foreground.Color = Color.FromRgb(255, 255, 255);
                }
                else if (value == DefaultButtonFill.Green)
                {
                    background.Color = borderbrush.Color = Color.FromRgb(67, 181, 129);
                    foreground.Color = Color.FromRgb(255, 255, 255);
                }
                else if (value == DefaultButtonFill.RedBorder)
                {
                    foreground.Color = borderbrush.Color = Color.FromRgb(185, 67, 69);
                    background.Color = Color.FromArgb(1, 255, 255, 255);
                }
                else
                {
                    foreground.Color = Color.FromRgb(255, 255, 255);
                    borderbrush.Color = Color.FromRgb(115, 117, 121);
                    background.Color = Color.FromArgb(0, 0, 0, 0);
                }
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Hovered = true;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Hovered = false;
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clicked = true;
        }

        public event RoutedEventHandler Click;

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Clicked && Click != null)
            {
                Click.Invoke(sender, (RoutedEventArgs)(e));
            }
        }
    }

    public enum DefaultButtonFill { Blue,Green,Gray,BlueBorder,RedBorder,GrayBorder };

}
