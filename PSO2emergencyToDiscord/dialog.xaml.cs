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
    /// dialog.xaml の相互作用ロジック
    /// </summary>
    public partial class dialog : Window
    {
        private string PwindowTitle;
        private string PtitleStr;
        private string Pdetail;
        

        public string windowTitle
        {
            get { return this.PwindowTitle; }
            set
            {
                this.PwindowTitle = value;
                dialogWindow.Title = PwindowTitle;
            }
        }
  
        public string titleStr
        {
            get { return this.PtitleStr; }
            set
            {
                this.titleLabel.Content = value;
                this.PtitleStr = value;
            }
        }

        public string detail
        {
            get { return this.Pdetail; }
            set
            {
                this.detailLabel.Content = value;
                this.Pdetail = value;
            }
        }

        public dialog()
        {
            InitializeComponent();
        }

        public void show()
        {
            this.Visibility = Visibility.Visible;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            //this.Close();
        }

        private void dialogWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
