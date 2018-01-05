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

namespace PSO2emergencyToDiscord
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        //private string discordURL;
        sendDiscord discord;
        getPSO2 pso2;
        botRun bot;

        pictureConfWin confWin;


        public MainWindow()
        {
            InitializeComponent();

            log.writeLog("PSO2emergencyToDiscordが起動しました。");

            //設定ファイルが存在するかしないか
            if (System.IO.File.Exists("discordconf.xml"))
            {
                discord = new sendDiscord();
            }
            else
            {
                discord = new sendDiscord("");
            }

            pso2 = new getPSO2();
            bot = new botRun(discord,pso2);

        }

        //---------------------イベント-----------------------

        private void postButton_Click(object sender, RoutedEventArgs e) //投稿ボタンが押された時
        {
            bot.postText(postBox.Text);
            //await discord.sendContent(postBox.Text);
            postBox.Text = "";
        }

        //urlボタン
        private void urlButton_Click(object sender, RoutedEventArgs e)
        {
            discord.setUrl(urlBox.Text);
            discord.save();
        }

        //再取得ボタン
        private void reGetButton_Click(object sender, RoutedEventArgs e)
        {
            //pso2.reGet();
            //bot.reloadEmg();
            Task t = bot.asyncReloademg();
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            urlBox.Text = discord.getUrl();
            //rodosCheckBox.IsChecked = bot.rodosNotify;
        }

        //閉じるボタン
        private void mainWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void rodosCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bot.rodosNotify = (bool)rodosCheckBox.IsChecked;
            //System.Console.WriteLine("ロドス通知:{0}", bot.rodosNotify);
        }

        private void pictureCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bot.picturepost = (bool)pictureCheckBox.IsChecked;
            //System.Console.WriteLine("画像投稿:{0}", bot.picturepost);
        }

        private void configPictureButton_Click(object sender, RoutedEventArgs e)
        {
            if (confWin == null)
            {
                confWin = new pictureConfWin(bot);
                confWin.Show();
            }
            else
            {
                confWin.Visibility = Visibility.Visible;
            }
        }
    }
}
