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
        private string discordURL;
        sendDiscord discord;
        getPSO2 pso2;

        public MainWindow()
        {
            InitializeComponent();
            
            //見なかったことにして♡
            discordURL = "https://discordapp.com/api/webhooks/348322898089345032/yzcePYWS5nxgRIMNTKKgFPxgOTnEQY9aPY3FXyj5VR_hnO_aivZciwAjgO0EORUUBIPF";
            discord = new sendDiscord(discordURL);
            pso2 = new getPSO2();
        }

        private void postButton_Click(object sender, RoutedEventArgs e) //投稿ボタンが押された時
        {
            discord.sendContent(postBox.Text);
            postBox.Text = "";
        }
    }
}
