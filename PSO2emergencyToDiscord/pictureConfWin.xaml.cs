using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;

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

        internal pictureConfWin(botRun bot)
        {
            //Fonts.SystemFontFamilies;
            this.bot = bot;
            this.conf = bot.cp;
            backup = copyConf(bot.cp);

            InitializeComponent();
            this.Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            fontsizebox.Text = bot.cp.fontsize.ToString();
            widthBox.Text = bot.cp.width.ToString();
            field1box.Text = bot.cp.field1.ToString();
            field2box.Text = bot.cp.field2.ToString();
            field3box.Text = bot.cp.field3.ToString();

            FontFamily src = new FontFamily(bot.cp.fontname);
            System.Console.WriteLine(src.Source);

            int count = 0;
            foreach(var item in fontCombo.ItemsSource)
            {
                FontFamily f = (FontFamily)item;
                string font = "";

                if(f.Source == "EVA-Matisse_Standard" || f.Source == "EVA-Matisse_Classic")
                {
                    if(f.Source == "EVA-Matisse_Standard")
                    {
                        font = "EVA-マティス-スタンダード EB";
                    }

                    if (f.Source == "EVA-Matisse_Classic")
                    {
                        font = "EVA-マティス-クラシック EB";
                    }
                }
                else
                {
                    font = f.Source;
                }

                if(font == bot.cp.fontname)
                {
                    break;
                }
                count++;
            }

            fontCombo.SelectedIndex = count;

            Color c = new Color();
            c.R = bot.cp.r;
            c.G = bot.cp.g;
            c.B = bot.cp.g;
            c.A = bot.cp.a;
            Brush fontColor = new SolidColorBrush(c);
            colorSample.Background = fontColor;

        }

        private configPicture copyConf(configPicture src)
        {
            configPicture dst = new configPicture();

            dst.fontname = src.fontname;
            dst.fontsize = src.fontsize;
            dst.r = src.r;
            dst.g = src.g;
            dst.b = src.b;
            dst.a = src.a;
            dst.field1 = src.field1;
            dst.field2 = src.field2;
            dst.field2 = src.field3;

            return dst;
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            bot.postEmg();
        }

        private void DailytestButton_Click(object sender, RoutedEventArgs e)
        {
            bot.postDaily();
        }

        private void OKbutton_Click(object sender, RoutedEventArgs e)
        {
            bot.savePicture();
            //this.Visibility = Visibility.Hidden;
            this.Close();
        }

        private void Cancelbutton_Click(object sender, RoutedEventArgs e)
        {
            bot.cp.fontname = backup.fontname;
            bot.cp.fontsize = backup.fontsize;
            bot.cp.r = backup.r;
            bot.cp.g = backup.g;
            bot.cp.b = backup.b;
            bot.cp.a = backup.a;
            bot.cp.field1 = backup.field1;
            bot.cp.field2 = backup.field2;
            bot.cp.field3 = backup.field3;

            //this.Visibility = Visibility.Hidden;
            this.Close();
        }

        //================この下からイベント=======================

        private void fontsizebox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        }

        private void widthBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        }

        private void field1box_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        }

        private void field2box_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        }

        private void field3box_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        }

        private void fontsizebox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(fontsizebox.Text, out bot.cp.fontsize);
        }

        private void widthBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(widthBox.Text,out bot.cp.width);
        }

        private void field1box_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(field1box.Text,out bot.cp.field1);
        }

        private void field2box_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(field2box.Text,out bot.cp.field2);
        }

        private void field3box_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(field3box.Text, out bot.cp.field3);
        }

        private void fontCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FontFamily f = (FontFamily)fontCombo.SelectedItem;

            //なんともいえないあれ
            if (f.Source == "EVA-Matisse_Standard" || f.Source == "EVA-Matisse_Classic")
            {
                if (f.Source == "EVA-Matisse_Standard")
                {
                    bot.cp.fontname = "EVA-マティス-スタンダード EB";
                }

                if (f.Source == "EVA-Matisse_Classic")
                {
                    bot.cp.fontname = "EVA-マティス-クラシック EB";
                }

            }
            else
            {
                bot.cp.fontname = f.Source;
            }
        }

        private void colorchangebutton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Color color = Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B);
                colorSample.Background = new SolidColorBrush(color);

                bot.cp.r = cd.Color.R;
                bot.cp.g = cd.Color.G;
                bot.cp.b = cd.Color.B;
                bot.cp.a = cd.Color.A;
            }
        }
    }
}
