using Discord;
using Discord.WebSocket;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiscordWPF.Visual
{
    /// <summary>
    /// Interaction logic for MessageControl.xaml
    /// </summary>
    public partial class MessageControl : UserControl
    {
        public MessageControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty messagesProperty = DependencyProperty.Register("Messages", typeof(SocketMessage[]), typeof(MessageControl), new PropertyMetadata(new SocketMessage[0]));
        public static readonly DependencyProperty imessagesProperty = DependencyProperty.Register("IMessages", typeof(IMessage[]), typeof(MessageControl), new PropertyMetadata(new SocketMessage[0]));
        public static readonly DependencyProperty authorMessageAvatarProperty = DependencyProperty.Register("authorMessageAvatar", typeof(BitmapImage), typeof(MessageControl), new UIPropertyMetadata(null));
        public static readonly DependencyProperty contentProperty = DependencyProperty.Register("content", typeof(string), typeof(MessageControl));
        public static readonly DependencyProperty sendedTimeProperty = DependencyProperty.Register("sendedTime", typeof(string), typeof(MessageControl));
        public static readonly DependencyProperty authorNameProperty = DependencyProperty.Register("AuthorName", typeof(string), typeof(MessageControl));
        public static readonly DependencyProperty ProgrammProperty = DependencyProperty.Register("Programm", typeof(Prog), typeof(MessageControl));

        public void AddMessage(SocketMessage message)
        {
            SocketMessage[] msgs = Messages;
            Console.WriteLine("Created! - 1");
            Array.Resize(ref msgs, Messages.Length + 1);
            Console.WriteLine("Created! - 2");
            msgs[Messages.Length] = message;
            Console.WriteLine("Created! - 3");
            Messages = msgs;
            Console.WriteLine("Created! - 4");
            Console.WriteLine($"-\r-\r-\r-\r-\r-\r-\r-\r-\r-\r-\r\rLength - {msgs.Length}");
        }

        public void AddMessage(IMessage message)
        {
            IMessage[] msgs = Messages;
            Console.WriteLine("Created! - 1");
            Array.Resize(ref msgs, IMessages.Length + 1);
            Console.WriteLine("Created! - 2");
            msgs[Messages.Length] = message;
            Console.WriteLine("Created! - 3");
            IMessages = msgs;
            Console.WriteLine("Created! - 4");
            Console.WriteLine($"-\r-\r-\r-\r-\r-\r-\r-\r-\r-\r-\r\rLength - {msgs.Length}");
        }

        public Prog prog
        {
            get
            {
                return (Prog)GetValue(ProgrammProperty);
            }
            set
            {
                SetValue(ProgrammProperty, value);
            }
        }

        public SocketMessage[] Messages
        {
            get
            {
                return (SocketMessage[])GetValue(messagesProperty);
            }
            set
            {
                Console.WriteLine("begin");
                SetValue(messagesProperty, value);
                Console.WriteLine("Value seted");
                int last = value.Length - 1;
                Console.WriteLine("Last created");
                if (last == 0)
                {
                    Console.WriteLine("Last is 1");
                    SetValue(authorNameProperty, value[value.Length - 1].Author.Username);
                    Console.WriteLine("Author name seted");
                    SetValue(sendedTimeProperty, $"Today at {value[last].CreatedAt.Hour}:{value[last].CreatedAt.Minute}");
                    Console.WriteLine("Time Seted");
                    if (value[value.Length - 1].Author.IsBot || value[value.Length - 1].Author.IsWebhook)
                        isBot.Visibility = Visibility.Visible;
                    if (value[value.Length - 1].Author.IsBot)
                        isBot.Text = "BOT";
                    else
                    {
                        isBot.Visibility = Visibility.Hidden;
                        isBot.Text = "";
                    }
                    if (value[value.Length - 1].Author.IsWebhook)
                        isBot.Text = "WebHook";
                    else
                    {
                        isBot.Visibility = Visibility.Hidden;
                        isBot.Text = "";
                    }
                    if (!System.IO.File.Exists($"{System.IO.Directory.GetCurrentDirectory()}\\cache\\avatar-{value[value.Length - 1].Author.Id}.png"))
                    {
                        if (!System.IO.Directory.Exists($"{System.IO.Directory.GetCurrentDirectory()}\\cache\\"))
                            System.IO.Directory.CreateDirectory($"{System.IO.Directory.GetCurrentDirectory()}\\cache\\");
                        new System.Net.WebClient().DownloadFile(value[value.Length - 1].Author.GetAvatarUrl(), $"{System.IO.Directory.GetCurrentDirectory()}\\cache\\avatar-{value[value.Length - 1].Author.Id}.png");
                    }
                    SetValue(authorMessageAvatarProperty, new BitmapImage(new Uri($"{System.IO.Directory.GetCurrentDirectory()}\\cache\\avatar-{value[value.Length - 1].Author.Id}.png")));
                }
                else
                {

                }
                if(!string.IsNullOrEmpty(value[last].Content))
                {
                    TextBlock ctext = new TextBlock { Text = value[last].Content,Foreground = new SolidColorBrush { Color = System.Windows.Media.Color.FromRgb(255,255,255) } };
                    prop.Children.Add(ctext);
                    if (value[value.Length - 1].Content.Contains($"<@{prog.client.CurrentUser.Id}>") || value[value.Length - 1].Content.Contains("@everyone") || value[value.Length - 1].Content.Contains("@here"))
                        ctext.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(64, 250, 166, 26));
                }
                if (value[value.Length - 1].Attachments.Count > 0)
                {
                    Attachment[] attachments = value[value.Length-1].Attachments.ToArray();
                    for (int i = 0; i < attachments.Length; i++)
                    {
                        ImageHelper img_ = new ImageHelper(attachments[i].Url);
                        new System.Net.WebClient().DownloadFile(attachments[i].Url, $"{System.IO.Directory.GetCurrentDirectory()}\\cache\\image-{value[value.Length - 1].Author.Id}-{value[value.Length - 1].Id}-{i}.png");
                        Size size = new Size((double)attachments[i].Width, (double)attachments[i].Height);
                        if (this.ActualWidth > 256)
                            if (attachments[i].Size > this.ActualWidth / 2)
                            {
                                size.Width = this.ActualWidth / 2;
                                size.Height = ((double)attachments[i].Height) * ((double)attachments[i].Width / size.Width);
                            }
                        System.Windows.Controls.Image img = new System.Windows.Controls.Image { HorizontalAlignment = HorizontalAlignment.Left, Height = size.Height, Width = size.Width };
                        img.MouseUp += img_.GoToImagePage;
                        img.Source = new BitmapImage(new Uri($"{System.IO.Directory.GetCurrentDirectory()}\\cache\\image-{value[value.Length-1].Author.Id}-{value[value.Length-1].Id}-{i}.png"));
                        prop.Children.Add(img);
                        MessageBox.Show($"{img.Height}, {img.Width}");
                    }
                }
                this.Height = prop.Height + 47 + 25;
            }
        }

        public IMessage[] IMessages
        {
            get
            {
                return (IMessage[])GetValue(messagesProperty);
            }
            set
            {
                Console.WriteLine("begin");
                SetValue(messagesProperty, value);
                Console.WriteLine("Value seted");
                int last = value.Length - 1;
                Console.WriteLine("Last created");
                if (last == 0)
                {
                    Console.WriteLine("Last is 1");
                    SetValue(authorNameProperty, value[value.Length - 1].Author.Username);
                    Console.WriteLine("Author name seted");
                    SetValue(sendedTimeProperty, $"Today at {value[last].CreatedAt.Hour}:{value[last].CreatedAt.Minute}");
                    Console.WriteLine("Time Seted");
                    if (value[value.Length - 1].Author.IsBot || value[value.Length - 1].Author.IsWebhook)
                        isBot.Visibility = Visibility.Visible;
                    if (value[value.Length - 1].Author.IsBot)
                        isBot.Text = "BOT";
                    else
                    {
                        isBot.Visibility = Visibility.Hidden;
                        isBot.Text = "";
                    }
                    if (value[value.Length - 1].Author.IsWebhook)
                        isBot.Text = "WebHook";
                    else
                    {
                        isBot.Visibility = Visibility.Hidden;
                        isBot.Text = "";
                    }
                    if (!System.IO.File.Exists($"{System.IO.Directory.GetCurrentDirectory()}\\cache\\avatar-{value[value.Length - 1].Author.Id}.png"))
                    {
                        if (!System.IO.Directory.Exists($"{System.IO.Directory.GetCurrentDirectory()}\\cache\\"))
                            System.IO.Directory.CreateDirectory($"{System.IO.Directory.GetCurrentDirectory()}\\cache\\");
                        new System.Net.WebClient().DownloadFile(value[value.Length - 1].Author.GetAvatarUrl(), $"{System.IO.Directory.GetCurrentDirectory()}\\cache\\avatar-{value[value.Length - 1].Author.Id}.png");
                    }
                    SetValue(authorMessageAvatarProperty, new BitmapImage(new Uri($"{System.IO.Directory.GetCurrentDirectory()}\\cache\\avatar-{value[value.Length - 1].Author.Id}.png")));
                }
                else
                {

                }
                if (!string.IsNullOrEmpty(value[last].Content))
                {
                    TextBlock ctext = new TextBlock { Text = value[last].Content, Foreground = new SolidColorBrush { Color = System.Windows.Media.Color.FromRgb(255, 255, 255) } };
                    prop.Children.Add(ctext);
                    if (value[value.Length - 1].Content.Contains($"<@{prog.client.CurrentUser.Id}>") || value[value.Length - 1].Content.Contains("@everyone") || value[value.Length - 1].Content.Contains("@here"))
                        ctext.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(64, 250, 166, 26));
                }
                if (value[value.Length - 1].Attachments.Count > 0)
                {
                    IAttachment[] attachments = value[value.Length - 1].Attachments.ToArray();
                    for (int i = 0; i < attachments.Length; i++)
                    {
                        ImageHelper img_ = new ImageHelper(attachments[i].Url);
                        new System.Net.WebClient().DownloadFile(attachments[i].Url, $"{System.IO.Directory.GetCurrentDirectory()}\\cache\\image-{value[value.Length - 1].Author.Id}-{value[value.Length - 1].Id}-{i}.png");
                        Size size = new Size((double)attachments[i].Width, (double)attachments[i].Height);
                        if (this.ActualWidth > 256)
                            if (attachments[i].Size > this.ActualWidth / 2)
                            {
                                size.Width = this.ActualWidth / 2;
                                size.Height = ((double)attachments[i].Height) * ((double)attachments[i].Width / size.Width);
                            }
                        System.Windows.Controls.Image img = new System.Windows.Controls.Image { HorizontalAlignment = HorizontalAlignment.Left, Height = size.Height, Width = size.Width };
                        img.MouseUp += img_.GoToImagePage;
                        img.Source = new BitmapImage(new Uri($"{System.IO.Directory.GetCurrentDirectory()}\\cache\\image-{value[value.Length - 1].Author.Id}-{value[value.Length - 1].Id}-{i}.png"));
                        prop.Children.Add(img);
                        MessageBox.Show($"{img.Height}, {img.Width}");
                    }
                }
                this.Height = prop.Height + 47 + 25;
            }
        }


    }
}
