using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using Discord;
using Discord.Net;
using Discord.Net.WebSockets;
using Discord.WebSocket;
using System.Windows.Media.Animation;

namespace DiscordWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Prog pr;
        //public bool connected = false;
        TextBox tbox;
        public Window Aut;
        GuildHelper[] guildHelpers;
        ChannelHelper[] channelHelpers;
        DMChannelHelper[] DMChannelHelpers;
        SocketDMChannel current_;

        public string cont;
        public TokenType tokenType = TokenType.Bot;

        public MainWindow()
        {
            InitializeComponent();
            tbox = new TextBox { Height = 32, VerticalAlignment = VerticalAlignment.Top };
            Visual.DefaultButton b_ok = new Visual.DefaultButton { Text = "OK", VerticalAlignment = VerticalAlignment.Bottom, Height = 32, Width = 100, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 0, 120, 10) };
            Visual.DefaultButton b_cancel = new Visual.DefaultButton { Text = "Cancel", ButtonFill = Visual.DefaultButtonFill.RedBorder, VerticalAlignment = VerticalAlignment.Bottom, Height = 32, Width = 100, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 0, 10, 10) };
            Grid grid = new Grid();
            pr = new Prog(messagelist, "", this);
            grid.Children.Add(tbox);
            grid.Children.Add(b_ok);
            grid.Children.Add(b_cancel);
            b_ok.Click += B_ok_Click;
            voicegrid.IsEnabled = false;
            voicegrid.Visibility = Visibility.Hidden;
            while (!pr.connected)
            {
                Aut = new Window { Height = 256, Width = 512, ResizeMode = ResizeMode.NoResize, Title = "Login" };
                Aut.Content = grid;
                Aut.KeyDown += Aut_KeyDown;
                tbox.Text = "";
                if (Aut.ShowDialog().Value)
                {
                    Console.WriteLine(tbox.Text);
                }
                else
                {
                    Console.WriteLine("hi");
                }
            }
            UpdateLists();
        }

        private void Aut_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                tokenType = TokenType.Bot;
            else if (e.Key == Key.F2)
                tokenType = TokenType.User;
            else if (e.Key == Key.F3)
                tokenType = TokenType.Webhook;
            else if (e.Key == Key.F4)
                tokenType = TokenType.Bearer;
            Aut.Title = $"Login for {tokenType}";
        }

        public Visual.MessageControl lastmsgcontrol;

        public void AddMessage(SocketMessage message)
        {
            if (lastmsgcontrol != null)
            {
                if (lastmsgcontrol.Messages[lastmsgcontrol.Messages.Length - 1].Author == message.Author)
                {
                    lastmsgcontrol.AddMessage(message);
                }
                else
                {
                    Visual.MessageControl messageControl = new Visual.MessageControl();
                    messageControl.prog = pr;
                    messageControl.AddMessage(message);
                    lastmsgcontrol = messageControl;
                    messagelist.Children.Add(messageControl);
                }
            }
            else
            {
                Visual.MessageControl messageControl = new Visual.MessageControl();
                messageControl.prog = pr;
                messageControl.AddMessage(message);
                lastmsgcontrol = messageControl;
                messagelist.Children.Add(messageControl);
            }
        }

        public void AddMessage(IMessage message)
        {
            if (lastmsgcontrol != null)
            {
                if (lastmsgcontrol.Messages[lastmsgcontrol.Messages.Length - 1].Author == message.Author)
                {
                    lastmsgcontrol.AddMessage(message);
                }
                else
                {
                    Visual.MessageControl messageControl = new Visual.MessageControl();
                    messageControl.prog = pr;
                    messageControl.AddMessage(message);
                    lastmsgcontrol = messageControl;
                    messagelist.Children.Add(messageControl);
                }
            }
            else
            {
                Visual.MessageControl messageControl = new Visual.MessageControl();
                messageControl.prog = pr;
                messageControl.AddMessage(message);
                lastmsgcontrol = messageControl;
                messagelist.Children.Add(messageControl);
            }
        }

        public void AddMessageX(SocketMessage message)
        {
            SolidColorBrush scb = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            if (message.Content.Contains("@here") || message.Content.Contains("@everyone"))
                scb = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 128, 255));
            for (int x = 0; x < message.MentionedUsers.Count; x++)
            {
                if (message.MentionedUsers.ToArray()[x].Id == pr.client.CurrentUser.Id)
                {
                    scb = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 140, 0));
                }
            }
            string FilePath = $"{System.IO.Directory.GetCurrentDirectory()}\\cache\\user-{message.Author.Id}.png";
            if (!System.IO.Directory.Exists($"{System.IO.Directory.GetCurrentDirectory()}\\cache\\"))
                System.IO.Directory.CreateDirectory($"{System.IO.Directory.GetCurrentDirectory()}\\cache\\");
            if (message.Author.GetDefaultAvatarUrl() != null)
                new System.Net.WebClient().DownloadFile(message.Author.GetDefaultAvatarUrl(), FilePath);
            else
                new System.Net.WebClient().DownloadFile("https://vignette.wikia.nocookie.net/discord-wikia/images/5/5e/Default.png/revision/latest/scale-to-width-down/340?cb=20191215094354&path-prefix=ru", FilePath);
            System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon { Text = message.Content, BalloonTipText = message.Content, BalloonTipTitle = message.Author.ToString() };
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(FilePath);
            notifyIcon.ShowBalloonTip(1000);
            messagelist.Children.Add(new TextBlock { Foreground = scb, Text = $"#{message.Channel.Name} {message.Author}({message.Source}) at {message.CreatedAt} - {message.Content}" });
            Console.WriteLine($"{message.Attachments.ToArray().Length} - Embed's on message");
            if (message.Attachments.ToArray().Length > 0)
            {
                for (int i = 0; i < message.Attachments.ToArray().Length; i++)
                {
                    Console.WriteLine($"FINDED");
                    if (message.Attachments.ToArray()[i] != null)
                    {
                        string Uri = $"{System.IO.Directory.GetCurrentDirectory()}\\cache\\embed-{message.Channel.Id}-{message.Channel.CreatedAt.DateTime.Ticks}";
                        if (!System.IO.Directory.Exists($"{ System.IO.Directory.GetCurrentDirectory()}\\cache\\"))
                            System.IO.Directory.CreateDirectory($"{ System.IO.Directory.GetCurrentDirectory()}\\cache\\");
                        new System.Net.WebClient().DownloadFile(message.Attachments.ToArray()[i].Url, Uri);
                        Size size = new Size((double)message.Attachments.ToArray()[i].Width, (double)message.Attachments.ToArray()[i].Height);
                        ImageHelper imgHelp = new ImageHelper(message.Attachments.ToArray()[i].Url);
                        if (size.Width > this.messagelist.ActualWidth / 2)
                        {
                            size.Width = messagelist.ActualWidth / 2;
                            size.Height = ((double)message.Attachments.ToArray()[i].Height) * ((double)message.Attachments.ToArray()[i].Width / size.Width);
                        }
                        System.Windows.Controls.Image img = new System.Windows.Controls.Image { Width = size.Width, Height = size.Height, Source = new BitmapImage(new Uri(Uri)) };
                        img.MouseUp += imgHelp.GoToImagePage;
                        messagelist.Children.Add(img);
                        Console.WriteLine($"Power - {messagelist.Children.Count}");
                    }
                }

            }
        }

        public void Logout()
        {
            tbox = new TextBox { Height = 32, VerticalAlignment = VerticalAlignment.Top };
            Button b_ok = new Button { Content = "OK", IsDefault = true, VerticalAlignment = VerticalAlignment.Bottom, Height = 32, Width = 100, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 0, 120, 0) };
            Button b_cancel = new Button { Content = "Cancel", IsCancel = true, VerticalAlignment = VerticalAlignment.Bottom, Height = 32, Width = 100, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 0, 10, 0) };
            Grid grid = new Grid();
            pr = new Prog(messagelist, "", this);
            grid.Children.Add(tbox);
            grid.Children.Add(b_ok);
            grid.Children.Add(b_cancel);
            b_ok.Click += B_ok_Click;
            while (!pr.connected)
            {
                Aut = new Window { Height = 256, Width = 512, ResizeMode = ResizeMode.NoResize, Title = "Login" };
                Aut.Content = grid;
                Aut.KeyDown += Aut_KeyDown;
                tbox.Text = "";
                if (Aut.ShowDialog().Value)
                {
                    Console.WriteLine(tbox.Text);
                }
                else
                {
                    Console.WriteLine("hi");
                }
            }
        }

        public void UpdateLists()
        {
            userid.Text = pr.client.CurrentUser.Username;
            userdisc.Text = $"#{pr.client.CurrentUser.Discriminator}";
            if (pr.client.Status == UserStatus.Online)
                statusColor.Color = System.Windows.Media.Color.FromRgb(67, 181, 129);
            else if (pr.client.Status == UserStatus.DoNotDisturb)
                statusColor.Color = System.Windows.Media.Color.FromRgb(240, 71, 71);
            else if (pr.client.Status == UserStatus.Idle)
                statusColor.Color = System.Windows.Media.Color.FromRgb(250, 166, 26);
            else if (pr.client.Status == UserStatus.Invisible)
                statusColor.Color = System.Windows.Media.Color.FromRgb(116, 127, 141);
            else if (pr.client.Status == UserStatus.AFK)
                statusColor.Color = System.Windows.Media.Color.FromRgb(250, 166, 26);
            else if (pr.client.Status == UserStatus.Offline)
                statusColor.Color = System.Windows.Media.Color.FromRgb(116, 127, 141);
            if (pr.client.CurrentUser.IsBot)
                usertype.Visibility = Visibility.Visible;
            else
                usertype.Visibility = Visibility.Hidden;
            if (pr.client.CurrentUser.GetAvatarUrl() != null)
            {
                if (!System.IO.File.Exists($"{System.IO.Directory.GetCurrentDirectory()}\\temp{pr.client.CurrentUser.Id}") && pr.client.CurrentUser.GetAvatarUrl() != null)
                    new System.Net.WebClient().DownloadFile(pr.client.CurrentUser.GetAvatarUrl(), $"{System.IO.Directory.GetCurrentDirectory()}\\temp{pr.client.CurrentUser.Id}");
            }
            else
            {
                if (!System.IO.File.Exists($"{System.IO.Directory.GetCurrentDirectory()}\\temp{pr.client.CurrentUser.Id}") && pr.client.CurrentUser.GetAvatarUrl() != null)
                    new System.Net.WebClient().DownloadFile(pr.client.CurrentUser.GetDefaultAvatarUrl(), $"{System.IO.Directory.GetCurrentDirectory()}\\temp{pr.client.CurrentUser.Id}");
            }
            useravatar.ImageSource = new BitmapImage(new Uri($"{System.IO.Directory.GetCurrentDirectory()}\\temp{pr.client.CurrentUser.Id}"));
            servers.Children.Clear();
            channels.Children.Clear();
            messagelist.Children.Clear();
            SocketGuild[] socketGuilds = pr.client.Guilds.ToArray();
            if (pr.currentguild != null)
            {
                SocketChannel[] socketChannels = pr.currentguild.Channels.ToArray();
                channelHelpers = new ChannelHelper[socketChannels.Length];
                for (int i = 0; i < socketChannels.Length; i++)
                {
                    channelHelpers[i] = new ChannelHelper(socketChannels[i], pr);
                    if (pr.client.GetGuild(pr.currentguild.Id).GetTextChannel(channelHelpers[i].Channel.Id) != null)
                    {
                        Button but = new Button { Content = pr.client.GetGuild(pr.currentguild.Id).GetTextChannel(channelHelpers[i].Channel.Id).Name, Height = 32 };
                        but.Click += channelHelpers[i].Select;
                        channels.Children.Add(but);
                    }
                    else if (pr.client.GetGuild(pr.currentguild.Id).GetVoiceChannel(channelHelpers[i].Channel.Id) != null)
                    {
                        Button but = new Button { Content = pr.client.GetGuild(pr.currentguild.Id).GetVoiceChannel(channelHelpers[i].Channel.Id).Name + " (Voice)", Height = 32 };
                        but.Click += channelHelpers[i].Select;
                        channels.Children.Add(but);
                    }
                    else if (pr.client.GetGuild(pr.currentguild.Id).GetCategoryChannel(channelHelpers[i].Channel.Id) != null)
                    {
                        TextBlock but = new TextBlock { Text = pr.client.GetGuild(pr.currentguild.Id).GetCategoryChannel(channelHelpers[i].Channel.Id).Name, Height = 32 };
                        channels.Children.Add(but);
                    }

                }
            }
            else
            {
                SocketDMChannel[] socketDMChannels = pr.client.DMChannels.ToArray();
                SocketGroupChannel[] socketGroupChannels = pr.client.GroupChannels.ToArray();
                DMChannelHelper[] dMChannelHelpers = new DMChannelHelper[pr.client.DMChannels.ToArray().Length];
                GroupChannelHelper[] gChannelHelpers = new GroupChannelHelper[pr.client.GroupChannels.ToArray().Length];
                for (int i = 0; i < dMChannelHelpers.Length; i++)
                {
                    dMChannelHelpers[i] = new DMChannelHelper(socketDMChannels[i], pr);
                    if (socketDMChannels[i] != null)
                    {
                        Button but = new Button { Content = $"@{socketDMChannels[i].Recipient.Username}#{socketDMChannels[i].Recipient.Discriminator}", Height = 32 };
                        but.Click += dMChannelHelpers[i].Select;
                        channels.Children.Add(but);
                    }
                }
                for (int i = 0; i < gChannelHelpers.Length; i++)
                {
                    gChannelHelpers[i] = new GroupChannelHelper(socketGroupChannels[i], pr);
                    if (socketGroupChannels[i] != null)
                    {
                        Button but = new Button { Content = $"#{socketGroupChannels[i].Name}", Height = 32 };
                        but.Click += gChannelHelpers[i].Select;
                        channels.Children.Add(but);
                    }
                }
            }
            guildHelpers = new GuildHelper[socketGuilds.Length + 1];
            guildHelpers[0] = new GuildHelper(null, pr);
            Button butx = new Button { Content = "Home", Height = 32 };
            butx.Click += guildHelpers[0].Select;
            servers.Children.Add(butx);

            for (int i = 1; i < socketGuilds.Length + 1; i++)
            {
                guildHelpers[i] = new GuildHelper(socketGuilds[i - 1], pr);
                Button but = new Button { Content = guildHelpers[i].Guild.Name, Height = 32 };
                but.Click += guildHelpers[i].Select;
                servers.Children.Add(but);
            }
        }

        private void B_ok_Click(object sender, RoutedEventArgs e)
        {
            pr = new Prog(messagelist, tbox.Text, this);
            pr.MainAsync().GetAwaiter().GetResult();
            Aut.Close();
        }

        bool shiftpressed;
        bool ctrlpressed = false;

        private void tx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
                shiftpressed = true;

            else if (e.Key == Key.Enter && !shiftpressed)
            {
                if (pr.currentguild != null)
                {
                    SocketGuildUser[] socketGuildUsers = pr.client.GetGuild(pr.currentguild.Id).GetTextChannel(pr.currentChannel.Id).Users.ToArray();
                    string FinalMessage = tx.Text;
                    for (int i = 0; i < socketGuildUsers.Length; i++)
                    {
                        FinalMessage = FinalMessage.Replace($"@{socketGuildUsers[i].Username}#{socketGuildUsers[i].Discriminator}", $"<@{socketGuildUsers[i].Id}>");
                    }
                    pr.client.GetGuild(pr.currentguild.Id).GetTextChannel(pr.currentChannel.Id).SendMessageAsync(tx.Text);
                }
                else
                {
                    SocketDMChannel[] socketDMChannels = pr.client.DMChannels.ToArray();
                    for (int i = 0; i < socketDMChannels.Length; i++)
                    {
                        if (socketDMChannels[i].Id == pr.currentChannel.Id)
                        {
                            string FinalMessage = tx.Text.Replace($"@{socketDMChannels[i].Recipient.Username}#{socketDMChannels[i].Recipient.Discriminator}", $"<@{socketDMChannels[i].Recipient.Id}>").Replace($"@{pr.client.CurrentUser.Username}#{pr.client.CurrentUser.Discriminator}", $"<@{pr.client.CurrentUser.Id}>");
                            socketDMChannels[i].SendMessageAsync(FinalMessage);
                        }
                    }
                }
                tx.Text = "";
            }
            else if (e.Key == Key.Enter)
            {
                if (!HiddenWrite)
                    if (pr.currentguild != null)
                        pr.client.GetGuild(pr.currentguild.Id).GetTextChannel(pr.currentChannel.Id).EnterTypingState();

                    else if (pr.CurrentUserId != 0)
                    {
                        if (current_ != null)
                        {
                            if (current_.Id != pr.currentChannel.Id)
                            {
                                SocketDMChannel[] socketDMChannels = pr.client.DMChannels.ToArray();
                                for (int i = 0; i < socketDMChannels.Length; i++)
                                {
                                    if (socketDMChannels[i].Id == pr.currentChannel.Id)
                                        current_ = socketDMChannels[i];
                                }
                            }
                        }
                        else
                        {
                            SocketDMChannel[] socketDMChannels = pr.client.DMChannels.ToArray();
                            for (int i = 0; i < socketDMChannels.Length; i++)
                            {
                                if (socketDMChannels[i].Id == pr.currentChannel.Id)
                                    current_ = socketDMChannels[i];
                            }
                        }


                        current_.EnterTypingState(RequestOptions.Default);
                    }
                    else
                    {
                        SocketGroupChannel[] socketDMChannels = pr.client.GroupChannels.ToArray();
                        for (int i = 0; i < socketDMChannels.Length; i++)
                        {
                            if (socketDMChannels[i].Id == pr.currentChannel.Id)
                                socketDMChannels[i].EnterTypingState();
                        }
                    }

            }



        }

        public VoiceChannelHelper vch;


        private void join_Click(object sender, RoutedEventArgs e)
        {
            if (vch != null)
                if (!vch.joined)
                    vch.Join();
                else
                    vch.Leave();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Logout();
        }

        Window changeActionsWindow;
        bool HiddenWrite = false;

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                if (pr.currentChannel != null)
                {
                    string FilePath = $"{System.IO.Directory.GetCurrentDirectory()}\\messages\\Message-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}.txt";
                    string xContent = "";
                    if (pr.currentguild != null)
                    {
                        IMessage[] tmp = pr.client.GetGuild(pr.currentguild.Id).GetTextChannel(pr.currentChannel.Id).GetMessagesAsync(1000000).FlattenAsync().GetAwaiter().GetResult().ToArray();

                        xContent = $"{tmp.Length} messages on channel";
                        for (int i = 0; i < tmp.Length; i++)
                        {
                            xContent = $"{xContent}\r{tmp[i].Author} at {tmp[i].CreatedAt} - {tmp[i].Content}";
                        }
                        if (!System.IO.Directory.Exists($"{System.IO.Directory.GetCurrentDirectory()}\\messages\\"))
                            System.IO.Directory.CreateDirectory($"{System.IO.Directory.GetCurrentDirectory()}\\messages\\");
                        //System.IO.File.Create(FilePath);
                        System.IO.File.WriteAllText(FilePath, xContent);
                        MessageBox.Show($"Messages saved to {FilePath}, Length {tmp.Length}");
                    }
                    else
                    {
                        IMessage[] tmp = pr.client.GetDMChannelAsync(pr.currentChannel.Id).GetAwaiter().GetResult().GetMessagesAsync(1000000).FlattenAsync().GetAwaiter().GetResult().ToArray();
                        xContent = $"{tmp.Length} messages on channel";
                        for (int i = 0; i < tmp.Length; i++)
                        {
                            xContent = $"{xContent}\r{tmp[i].Author} at {tmp[i].CreatedAt} - {tmp[i].Content}";
                        }
                        if (!System.IO.Directory.Exists($"{System.IO.Directory.GetCurrentDirectory()}\\messages\\"))
                            System.IO.Directory.CreateDirectory($"{System.IO.Directory.GetCurrentDirectory()}\\messages\\");
                        //System.IO.File.Create(FilePath);
                        System.IO.File.WriteAllText(FilePath, xContent);
                        MessageBox.Show($"Messages saved to {FilePath}, Length {tmp.Length}");
                    }
                }
                else
                {

                }
            } else if (e.Key == Key.F2)
            {
                changeActionsWindow = new Window { Title = "Change Activity", Width = 256, Height = 160, ResizeMode = ResizeMode.NoResize, WindowStyle = WindowStyle.None, AllowsTransparency = true };
                Grid cg = new Grid();
                cg.RowDefinitions.Add(new RowDefinition { Height = new GridLength(32) });
                cg.RowDefinitions.Add(new RowDefinition { Height = new GridLength(32) });
                cg.RowDefinitions.Add(new RowDefinition { Height = new GridLength(32) });
                cg.RowDefinitions.Add(new RowDefinition { Height = new GridLength(32) });
                cg.RowDefinitions.Add(new RowDefinition { Height = new GridLength(32) });
                Button[] butons = { new Button { Content = "Online", Height = 32 }, new Button { Content = "Do not a distrub", Height = 32 }, new Button { Content = "Idle", Height = 32 }, new Button { Content = "Invisible", Height = 32 }, new Button { Content = "Logout", Height = 32 } };
                butons[0].Click += MainWindow_Click;
                butons[1].Click += MainWindow_Click1;
                butons[2].Click += MainWindow_Click2;
                butons[3].Click += MainWindow_Click3;
                butons[4].Click += Button_Click;
                cg.Children.Add(butons[0]);
                cg.Children.Add(butons[1]);
                cg.Children.Add(butons[2]);
                cg.Children.Add(butons[3]);
                cg.Children.Add(butons[4]);
                Grid.SetRow(butons[1], 1);
                Grid.SetRow(butons[2], 2);
                Grid.SetRow(butons[3], 3);
                Grid.SetRow(butons[4], 4);
                changeActionsWindow.Content = cg;
                changeActionsWindow.ShowDialog();
            } else if (e.Key == Key.F3)
            {
                if (HiddenWrite)
                {
                    MessageBox.Show("Hidden is f");
                }
                else
                {
                    MessageBox.Show("Hidden is t");
                }
                HiddenWrite = !HiddenWrite;
            }
            if (e.Key == Key.V && ctrlpressed)
            {
                IDataObject iData = Clipboard.GetDataObject();

                if (MessageBox.Show("Do you wanna send picture from clipboard?", "DSTX", MessageBoxButton.YesNo) == MessageBoxResult.Yes && iData.GetDataPresent(DataFormats.Bitmap))
                {
                    Console.WriteLine($"{System.IO.Directory.Exists($"{System.IO.Directory.GetCurrentDirectory()}\\cache\\")},{System.IO.Directory.GetCurrentDirectory()}\\cache\\");
                    if (!System.IO.Directory.Exists($"{System.IO.Directory.GetCurrentDirectory()}\\cache\\"))
                    {
                        System.IO.Directory.CreateDirectory($"{System.IO.Directory.GetCurrentDirectory()}\\cache\\");
                    }
                    string s2 = $"{System.IO.Directory.GetCurrentDirectory()}\\cache\\file{current_.Id}-{DateTime.Now.Ticks}.png";
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(((BitmapImage)iData.GetData("png"))));

                    using (var fileStream = new System.IO.FileStream(s2, System.IO.FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                    current_.SendFileAsync(s2, tx.Text);
                }
            }

            if (e.Key == Key.LeftCtrl)
                ctrlpressed = true;
        }

        private void MainWindow_Click(object sender, RoutedEventArgs e)
        {
            pr.client.SetStatusAsync(UserStatus.Online);
            changeActionsWindow.Close();
        }

        private void MainWindow_Click1(object sender, RoutedEventArgs e)
        {
            pr.client.SetStatusAsync(UserStatus.DoNotDisturb);
            changeActionsWindow.Close();
        }

        private void MainWindow_Click2(object sender, RoutedEventArgs e)
        {
            pr.client.SetStatusAsync(UserStatus.Idle);
            changeActionsWindow.Close();
        }

        private void MainWindow_Click3(object sender, RoutedEventArgs e)
        {
            pr.client.SetStatusAsync(UserStatus.Invisible);
            changeActionsWindow.Close();
        }

        private void mute_Click(object sender, RoutedEventArgs e)
        {

        }

        private void deafan_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Point pos = actions.PointToScreen(new Point(0, 0));
            changeActionsWindow = new Window { Title = "Menu", ShowInTaskbar = false, Left = pos.X, Top = pos.Y - 192, Width = 256, Height = 192, ResizeMode = ResizeMode.NoResize, WindowStyle = WindowStyle.None, AllowsTransparency = true };
            Grid cg = new Grid();
            cg.RowDefinitions.Add(new RowDefinition { Height = new GridLength(32) });
            cg.RowDefinitions.Add(new RowDefinition { Height = new GridLength(32) });
            cg.RowDefinitions.Add(new RowDefinition { Height = new GridLength(32) });
            cg.RowDefinitions.Add(new RowDefinition { Height = new GridLength(32) });
            cg.RowDefinitions.Add(new RowDefinition { Height = new GridLength(32) });
            cg.RowDefinitions.Add(new RowDefinition { Height = new GridLength(32) });
            Button[] butons = { new Button { Content = "Load Last Messages on this channel", Height = 32 }, new Button { Content = "Online", Height = 32 }, new Button { Content = "Do not a distrub", Height = 32 }, new Button { Content = "Idle", Height = 32 }, new Button { Content = "Invisible", Height = 32 }, new Button { Content = "Logout", Height = 32 } };
            butons[0].Click += MainWindow_Click4;
            butons[1].Click += MainWindow_Click;
            butons[2].Click += MainWindow_Click1;
            butons[3].Click += MainWindow_Click2;
            butons[4].Click += MainWindow_Click3;
            butons[5].Click += Button_Click;
            cg.Children.Add(butons[0]);
            cg.Children.Add(butons[1]);
            cg.Children.Add(butons[2]);
            cg.Children.Add(butons[3]);
            cg.Children.Add(butons[4]);
            cg.Children.Add(butons[5]);
            Grid.SetRow(butons[1], 1);
            Grid.SetRow(butons[2], 2);
            Grid.SetRow(butons[3], 3);
            Grid.SetRow(butons[4], 4);
            Grid.SetRow(butons[5], 4);
            changeActionsWindow.Content = cg;
            changeActionsWindow.ShowDialog();
        }

        private void MainWindow_Click4(object sender, RoutedEventArgs e)
        {

        }

        private void servers_MouseEnter(object sender, MouseEventArgs e)
        {
            servers.BeginAnimation(WidthProperty, new DoubleAnimation { To = 256, Duration = TimeSpan.FromMilliseconds(200) });
        }

        private void servers_MouseLeave(object sender, MouseEventArgs e)
        {
            servers.BeginAnimation(WidthProperty, new DoubleAnimation { To = 48, Duration = TimeSpan.FromMilliseconds(200) });
        }

        internal void AddMessage(Task<IMessage> task)
        {
            AddMessage((SocketMessage)task.GetAwaiter().GetResult());
        }

        private void tx_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
                shiftpressed = false;
            if (e.Key == Key.LeftCtrl)
                ctrlpressed = false;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
                ctrlpressed = false;
        }
    }

    public class Prog
    {
        public bool connected = false;
        public StackPanel Messageresult;
        public int server;
        public ulong channel;
        public DiscordSocketClient client;
        public string token;
        public SocketGuild currentguild;
        public SocketChannel currentChannel;
        public MainWindow parent;
        public string Content;
        public bool dest = false;
        public ulong CurrentUserId = 0;
        public ChannelType ctype = ChannelType.None;

        public Prog(StackPanel mr, string ClientToken, MainWindow par)
        {
            parent = par;
            Messageresult = mr;
            token = ClientToken;
            client = new DiscordSocketClient();
        }

        public Task Make_Server(string ServerName)
        {

            return Task.CompletedTask;
        }

        private Task Client_UserUpdated(SocketUser arg1, SocketUser arg2)
        {
            if (CurrentUserId != 0)
            {
                if (arg2.Status != arg1.Status)
                    if (arg2.Status == UserStatus.Online)
                        parent.activedUserColor.Color = System.Windows.Media.Color.FromRgb(67, 181, 129);
                    else if (arg2.Status == UserStatus.DoNotDisturb)
                        parent.activedUserColor.Color = System.Windows.Media.Color.FromRgb(240, 71, 71);
                    else if (arg2.Status == UserStatus.Idle)
                        parent.activedUserColor.Color = System.Windows.Media.Color.FromRgb(250, 166, 26);
                    else if (arg2.Status == UserStatus.Invisible)
                        parent.activedUserColor.Color = System.Windows.Media.Color.FromRgb(116, 127, 141);
                    else if (arg2.Status == UserStatus.AFK)
                        parent.activedUserColor.Color = System.Windows.Media.Color.FromRgb(250, 166, 26);
                    else if (arg2.Status == UserStatus.Offline)
                        parent.activedUserColor.Color = System.Windows.Media.Color.FromRgb(116, 127, 141);
            }
            return Task.CompletedTask;
        }

        private Task Client_CurrentUserUpdated(SocketSelfUser arg1, SocketSelfUser arg2)
        {
            Console.WriteLine("TEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\rTEST\r");
            parent.Dispatcher.Invoke(() =>
            {
                if (arg2.Status != arg1.Status)
                    if (arg2.Status == UserStatus.Online)
                        parent.statusColor.Color = System.Windows.Media.Color.FromRgb(67, 181, 129);
                    else if (arg2.Status == UserStatus.DoNotDisturb)
                        parent.statusColor.Color = System.Windows.Media.Color.FromRgb(240, 71, 71);
                    else if (arg2.Status == UserStatus.Idle)
                        parent.statusColor.Color = System.Windows.Media.Color.FromRgb(250, 166, 26);
                    else if (arg2.Status == UserStatus.Invisible)
                        parent.statusColor.Color = System.Windows.Media.Color.FromRgb(116, 127, 141);
                    else if (arg2.Status == UserStatus.AFK)
                        parent.statusColor.Color = System.Windows.Media.Color.FromRgb(250, 166, 26);
                    else if (arg2.Status == UserStatus.Offline)
                        parent.statusColor.Color = System.Windows.Media.Color.FromRgb(116, 127, 141);
            });
            return Task.CompletedTask;

        }

        private Task Client_UserIsTyping(SocketUser arg1, ISocketMessageChannel arg2)
        {
            Console.WriteLine(arg2.Id);
            if (currentguild != null)
            {
                if (currentChannel.Id == arg2.Id)
                {

                }
            }
            else
            {
                if (currentChannel.Id == arg2.Id)
                {
                    parent.typingText.Text = $"{arg1.Username} are typing";
                    parent.typingStack.Visibility = Visibility.Visible;

                }
            }
            return Task.CompletedTask;
        }

        private Task Client_ChannelUpdated(SocketChannel arg1, SocketChannel arg2)
        {
            parent.UpdateLists();
            return Task.CompletedTask;
        }

        private Task Client_ChannelDestroyed(SocketChannel arg)
        {
            parent.UpdateLists();
            return Task.CompletedTask;
        }

        private Task Client_ChannelCreated(SocketChannel arg)
        {
            parent.UpdateLists();
            return Task.CompletedTask;
        }

        private Task Client_UserLeft(SocketGuildUser arg)
        {
            parent.UpdateLists();
            return Task.CompletedTask;
        }

        private Task Client_JoinedGuild(SocketGuild arg)
        {
            parent.UpdateLists();
            return Task.CompletedTask;
        }

        public Task MainAsync()
        {
            // Tokens should be considered secret data, and never hard-coded.
            TokenType tt = TokenType.Bot;
            parent.Dispatcher.Invoke(() => {
                tt = parent.tokenType;
            });

            client.Log += LogAsync;
            client.Ready += ReadyAsync;
            client.MessageReceived += MessageReceivedAsync;
            client.JoinedGuild += Client_JoinedGuild;
            client.UserLeft += Client_UserLeft;
            client.ChannelCreated += Client_ChannelCreated;
            client.ChannelUpdated += Client_ChannelUpdated;
            client.ChannelDestroyed += Client_ChannelDestroyed;
            client.UserIsTyping += Client_UserIsTyping;
            client.CurrentUserUpdated += Client_CurrentUserUpdated;
            client.UserUpdated += Client_UserUpdated;
            //MessageBox.Show("Wait");
            client.LoginAsync(tt, token);
            client.StartAsync();

            // Block the program until it is closed.
            //await Task.Delay(-1);
            return Task.CompletedTask;
        }

        public void SelectServer(ulong Id)
        {
            if (Id == 0)
            {
                currentguild = null;
                parent.UpdateLists();
                parent.ac.Text = "Before send messages,please,select channel";
                parent.tx.Visibility = Visibility.Hidden;
                parent.ac.Visibility = Visibility.Visible;
                parent.tx.IsEnabled = false;
                parent.voicegrid.IsEnabled = false;
                parent.voicegrid.Visibility = Visibility.Hidden;
                parent.servername.Text = "Home";
            }
            else
            {
                SocketGuild[] socketGuilds = client.Guilds.ToArray();
                for (int i = 0; i < socketGuilds.Length; i++)
                {
                    if (socketGuilds[i].Id == Id)
                        currentguild = socketGuilds[i];
                }
                parent.UpdateLists();
                parent.ac.Text = "Before send messages,please,select channel";
                parent.tx.Visibility = Visibility.Hidden;
                parent.ac.Visibility = Visibility.Visible;
                parent.tx.IsEnabled = false;
                parent.voicegrid.IsEnabled = false;
                parent.voicegrid.Visibility = Visibility.Hidden;
                parent.servername.Text = currentguild.Name;
            }
            this.ctype = ChannelType.None;
            parent.lastmsgcontrol = null;
        }

        public void Destroy()
        {
            dest = true;
            client.LogoutAsync();
        }

        public void SelectChannel(ulong Id)
        {
            parent.messagelist.Children.Clear();
            parent.lastmsgcontrol = null;
            SocketChannel[] socketChannels;
            SocketMessage[] socketMessages;
            if (currentguild != null)
            {
                socketChannels = currentguild.Channels.ToArray();
            }
            else
            {
                socketChannels = client.DMChannels.ToArray();
            }
            for (int i = 0; i < socketChannels.Length; i++)
            {
                if (socketChannels[i].Id == Id)
                    currentChannel = socketChannels[i];
            }

            if (currentguild != null)
            {
                CurrentUserId = 0;
                parent.activedUserColor.Color = System.Windows.Media.Color.FromArgb(0, 0, 0, 0);
                if (this.client.GetGuild(currentguild.Id).GetTextChannel(currentChannel.Id) != null)
                {
                    SocketTextChannel cur = client.GetGuild(currentguild.Id).GetTextChannel(currentChannel.Id);
                    ctype = ChannelType.Text;
                    if (cur.GetPermissionOverwrite(client.CurrentUser) != null)
                    {
                        if (cur.GetPermissionOverwrite(client.CurrentUser).Value.SendMessages == PermValue.Allow)
                        {
                            parent.ac.Visibility = Visibility.Hidden;
                            parent.tx.Visibility = Visibility.Visible;
                            parent.tx.IsEnabled = true;
                        }
                        else
                        {
                            parent.ac.Visibility = Visibility.Visible;
                            parent.tx.Visibility = Visibility.Hidden;
                            parent.tx.IsEnabled = false;
                            parent.ac.Text = "You haven't permisions to send messages on this channel";
                        }
                        if (cur.GetPermissionOverwrite(client.CurrentUser).Value.ReadMessageHistory == PermValue.Allow || cur.GetPermissionOverwrite(client.CurrentUser).Value.ReadMessageHistory == PermValue.Inherit)
                        {
                            IMessage[] tmp = cur.GetMessagesAsync(100).FlattenAsync().GetAwaiter().GetResult().ToArray();
                            for (int i = 0; i < tmp.Length; i++)
                            {
                                parent.AddMessage(tmp[i]);
                                Console.WriteLine("try Add message");
                            }
                        }
                    }
                    else
                    {
                        parent.ac.Visibility = Visibility.Hidden;
                        parent.tx.Visibility = Visibility.Visible;
                        parent.tx.IsEnabled = true;
                        parent.ac.Text = "You haven't permisions to send messages on this channel";
                    }
                    parent.voicegrid.IsEnabled = false;
                    parent.voicegrid.Visibility = Visibility.Hidden;
                    parent.channelname.Text = client.GetGuild(currentguild.Id).GetTextChannel(currentChannel.Id).Name;
                    parent.activedUserColor.Color = System.Windows.Media.Color.FromArgb(0, 0, 0, 0);
                }
                else if (client.GetGuild(currentguild.Id).GetVoiceChannel(currentChannel.Id) != null)
                {
                    parent.voicegrid.IsEnabled = true;
                    parent.voicegrid.Visibility = Visibility.Visible;
                    parent.join.Content = "Join";
                    parent.vch = new VoiceChannelHelper(client.GetGuild(currentguild.Id).GetVoiceChannel(currentChannel.Id), this);
                    parent.ac.Visibility = Visibility.Hidden;
                    parent.channelname.Text = client.GetGuild(currentguild.Id).GetVoiceChannel(currentChannel.Id).Name;
                    parent.activedUserColor.Color = System.Windows.Media.Color.FromArgb(0, 0, 0, 0);
                }
            }
            else
            {
                if (client.GetDMChannelAsync(Id) != null)
                {
                    parent.ac.Visibility = Visibility.Hidden;
                    parent.tx.Visibility = Visibility.Visible;
                    parent.tx.IsEnabled = true;
                    parent.voicegrid.IsEnabled = false;
                    parent.voicegrid.Visibility = Visibility.Hidden;
                    ctype = ChannelType.DM;
                    for (int i = 0; i < client.DMChannels.ToArray().Length; i++)
                        if (client.DMChannels.ToArray()[i].Id == Id)
                        {
                            SocketUser arg2 = client.DMChannels.ToArray()[i].Recipient;
                            if (arg2.Status == UserStatus.Online)
                                parent.activedUserColor.Color = System.Windows.Media.Color.FromRgb(67, 181, 129);
                            else if (arg2.Status == UserStatus.DoNotDisturb)
                                parent.activedUserColor.Color = System.Windows.Media.Color.FromRgb(240, 71, 71);
                            else if (arg2.Status == UserStatus.Idle)
                                parent.activedUserColor.Color = System.Windows.Media.Color.FromRgb(250, 166, 26);
                            else if (arg2.Status == UserStatus.Invisible)
                                parent.activedUserColor.Color = System.Windows.Media.Color.FromRgb(116, 127, 141);
                            else if (arg2.Status == UserStatus.AFK)
                                parent.activedUserColor.Color = System.Windows.Media.Color.FromRgb(250, 166, 26);
                            else if (arg2.Status == UserStatus.Offline)
                                parent.activedUserColor.Color = System.Windows.Media.Color.FromRgb(116, 127, 141);
                            parent.channelname.Text = client.DMChannels.ToArray()[i].Recipient.Username;
                            CurrentUserId = client.DMChannels.ToArray()[i].Recipient.Id;
                        }
                }
                else if (client.GetGroupChannelAsync(Id) != null)
                {
                    parent.ac.Visibility = Visibility.Hidden;
                    parent.tx.Visibility = Visibility.Visible;
                    parent.tx.IsEnabled = true;
                    parent.voicegrid.IsEnabled = false;
                    parent.voicegrid.Visibility = Visibility.Hidden;
                    parent.channelname.Text = client.GetGuild(currentguild.Id).GetVoiceChannel(currentChannel.Id).Name;
                    parent.activedUserColor.Color = System.Windows.Media.Color.FromArgb(0, 0, 0, 0);
                }
            }

            //parent.UpdateLists();
        }
        
        public SocketGuild[] GetServers()
        {
            return client.Guilds.ToArray();
        }

        public SocketChannel[] GetChannels()
        {
            return currentguild.Channels.ToArray();
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"{client.CurrentUser} is connected!,his status is {client.CurrentUser.Status}");
            connected = true;
            parent.Dispatcher.Invoke(() =>
            {
                parent.ac.Text = "Before send messages,please,select guild and channel";
                parent.tx.Visibility = Visibility.Hidden;
                parent.ac.Visibility = Visibility.Visible;
                parent.Aut.Close();
            });
            return Task.CompletedTask;
        }

        private Task MessageReceivedAsync(SocketMessage message)
        {
            if (!dest)
            {
                if (message.Channel.Id == currentChannel.Id)
                    parent.Dispatcher.Invoke(() =>
                    {

                        parent.AddMessage(message);
                    });
            }
            return Task.CompletedTask;
        }

        public void Send(string messaage)
        {

        }
    }

    public enum ChannelType { None,Text,DM,Group,Voice };

    public class VoiceChannelHelper
    {
        public SocketVoiceChannel channel;
        public bool joined;
        public Prog parent;
        public VoiceChannelHelper(SocketVoiceChannel socketVoiceChannel,Prog par)
        {
            channel = socketVoiceChannel;
            joined = false;
            parent = par;
            par.client.UserVoiceStateUpdated += Client_UserVoiceStateUpdated;
        }

        private Task Client_UserVoiceStateUpdated(SocketUser arg1, SocketVoiceState arg2, SocketVoiceState arg3)
        {
            if(arg1.Id == parent.client.CurrentUser.Id)
            {
                if(arg3.VoiceChannel.Id == channel.Id)
                {
                    parent.parent.join.Content = "Leave";
                    joined = true;
                }
                else
                {
                    parent.parent.join.Content = "Join";
                    joined = false;
                }

            }
            return Task.CompletedTask;
        }

        public void Join()
        {
            channel.ConnectAsync();
        }

        public void Leave()
        {
            channel.DisconnectAsync();
        }
    }

    public class MessageHelper
    {
        TextBlock result;
        string Message;
        public MessageHelper(TextBlock result_, string message)
        {
            result = result_;
            Message = message;
        }
        public Task Main()
        {

            result.Text = $"{result.Text}\r{Message}";
            return Task.CompletedTask;
        }
    }
    public class GuildHelper
    {
        public SocketGuild Guild;
        Prog Parent;

        public GuildHelper(SocketGuild socketGuild, Prog parent)
        {
            Guild = socketGuild;
            Parent = parent;
        }

        public void Select(object sender, EventArgs e)
        {
            if(Guild != null)
            Parent.SelectServer(Guild.Id);
            else
                Parent.SelectServer(0);

        }
    }
    public class ChannelHelper
    {
        public SocketChannel Channel;
        Prog Parent;
        public ChannelHelper(SocketChannel socketChannel, Prog parent)
        {
            Channel = socketChannel;
            Parent = parent;
        }

        public void Select(object sender, EventArgs e)
        {
            Parent.SelectChannel(Channel.Id);
        }
    }
    public class ImageHelper
    {
        public string Link;
        public ImageHelper(string link)
        {
            Link = link;
        }

        public void GoToImagePage(object sender,EventArgs e)
        {
            Process.Start(Link);
        }
    }

    public class DMChannelHelper
    {
        public SocketChannel channel;
        Prog prog;
        public DMChannelHelper(SocketChannel c, Prog p)
        {
            channel = c;
            prog = p;
        }
        public void Select(object sender, EventArgs e)
        {
            prog.SelectChannel(channel.Id);
        }
    }

    public class GroupChannelHelper
    {
        public SocketChannel channel;
        Prog prog;
        public GroupChannelHelper(SocketChannel c, Prog p)
        {
            channel = c;
            prog = p;
        }
        public void Select(object sender, EventArgs e)
        {
            prog.SelectChannel(channel.Id);
        }
    }

    public class MessageTextChannelHelper
    {
        SocketMessage messag;
        Prog parent;
        Window menu;
        public MessageTextChannelHelper(SocketMessage message,Prog paren)
        {
            messag = message;
            parent = paren;
        }

        public void OpenMenu(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Right)
            {
                menu = new Window { Height = 192, Width = 256, ResizeMode = ResizeMode.NoResize, WindowStyle = WindowStyle.None };
                Button[] butts = { new Button { Content = "Edit" }, new Button { Content = "Remove" }, new Button { Content = "Copy Link" }, new Button { Content = "Remove" } };
            }
        }

        public void DeleteMessage(object sender, EventArgs e)
        {

        }

        public void EditMessage(object sender, EventArgs e)
        {

        }

        public void CopyMessageLink(object sender,EventArgs e)
        {

        }

        Window Reactions;
        ListBox sp;

        public void AddReaction(object sender, EventArgs e)
        {
            sp = new ListBox();
            Reactions = new Window { Content = sp,Width=256,Height = 512,ResizeMode = ResizeMode.NoResize,Left = (System.Windows.SystemParameters.PrimaryScreenWidth - 512)/2,Top = (System.Windows.SystemParameters.PrimaryScreenHeight - 512) / 2 };
        }

        public void RemoveReaction(object sender, EventArgs e)
        {

        }
    }
}
