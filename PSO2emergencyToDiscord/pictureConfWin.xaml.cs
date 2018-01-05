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
using System.Windows.Shapes;

namespace PSO2emergencyToDiscord
{
    /// <summary>
    /// pictureConfWin.xaml の相互作用ロジック
    /// </summary>
    public partial class pictureConfWin : Window
    {
        configPicture conf;
        configPicture backup;

        botRun bot;

        public pictureConfWin(configPicture cp)
        {
            //Fonts.SystemFontFamilies;
            this.conf = cp;
            InitializeComponent();
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            fontsizebox.Text = conf.fontsize.ToString();
            widthBox.Text = conf.width.ToString();
            field1box.Text = conf.field1.ToString();
            field2box.Text = conf.field2.ToString();
            field3box.Text = conf.field3.ToString();
        }
    }
}
