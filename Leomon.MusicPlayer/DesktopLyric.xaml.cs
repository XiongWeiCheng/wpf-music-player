using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Leomon.MusicPlayer
{
    /// <summary>
    /// DesktopLyric.xaml 的交互逻辑
    /// </summary>
    public partial class DesktopLyric : Window
    {
        //字体大小。
        private int fontsize = 40;
        /// <summary>
        /// 未唱的歌词颜色
        /// </summary>
        public Color FontBackColor
        { get; set; }
        /// <summary>
        /// 已经唱过的歌词颜色。
        /// </summary>
        public Color FontForeColor
        { get; set; }
        public DesktopLyric()
        {
            InitializeComponent();
            ResetLyricForecolor();
        }

        public DesktopLyric(string fontFamily, int fontSize, Color playedFontForeColor, Color unPlayedFontForeColor)
        {
            InitializeComponent();
            this.fontsize = fontSize;
            //this.ChangeFontFamily(fontFamily);
            this.FontBackColor = unPlayedFontForeColor;
            this.FontForeColor = playedFontForeColor;
            ResetLyricForecolor();
        }

        private ImageSource GetImageSource(string newSource)
        {
            BitmapImage bit = new BitmapImage();
            bit.BeginInit();
            bit.UriSource = new Uri(newSource, UriKind.RelativeOrAbsolute);
            bit.EndInit();
            return bit;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch { }
        }

        /// <summary>
        /// 改变透明度。
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private void ChangeUIElementOpacity(TextBlock tb, int from, int to)
        {
            DoubleAnimation da = new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(888));
            tb.BeginAnimation(OpacityProperty, da);
        }

        /// <summary>
        /// 改变字体大小。
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private void ChangeUIElementFontSize(TextBlock tb, int from, int to)
        {
            DoubleAnimation daF = new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(222));
            tb.BeginAnimation(FontSizeProperty,daF);
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            GradientStopCollection gsc = new GradientStopCollection();
            gsc.Add(new GradientStop(Color.FromArgb(0xD0, 0x3D, 
                0xB4, 0x83),0));
            //FFB9DEDA
            gsc.Add(new GradientStop(Color.FromArgb(0xD0, 0x8E,
                0xCD, 0xEA), 1));
            gsc.Add(new GradientStop(Color.FromArgb(0xD0, 0xB9,
                0xDE, 0xDA), 0.50));
            LinearGradientBrush lgb = new LinearGradientBrush(gsc);
            mainBorder.Background = lgb;
            controlSP.Opacity = 0.8;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            //ChangeUIElementOpacity(mainBorder, 1, 0);
            mainBorder.Background = new SolidColorBrush(Colors.Transparent);
            controlSP.Opacity = 0;
        }

        /// <summary>
        /// 设置歌词颜色
        /// </summary>
        /// <param name="textBlock"></param>
        /// <param name="color0"></param>
        /// <param name="offset0"></param>
        /// <param name="color1"></param>
        /// <param name="offset1"></param>
        /// <param name="color2"></param>
        /// <param name="offset2"></param>
        /// <param name="color3"></param>
        /// <param name="offset3"></param>
        private void SetLyricForecolor(TextBlock textBlock,
           Color color0, double offset0,
           Color color1, double offset1,
           Color color2, double offset2,
           Color color3, double offset3
           )
        {
            //设置tb的前景色为LinearBrush..
            GradientStopCollection gsc = new GradientStopCollection();
            gsc.Add(new GradientStop(color0, offset0));
            gsc.Add(new GradientStop(color1, offset1));
            gsc.Add(new GradientStop(color2, offset2));
            gsc.Add(new GradientStop(color3, offset3));
            //采用的是水平渐变色。
            LinearGradientBrush lgb = new LinearGradientBrush(gsc, 0.0);
            //lgb.MappingMode = BrushMappingMode.Absolute;
            //lgb.StartPoint = new Point(0, textBlock.ActualHeight);
            //lgb.EndPoint = new Point(textBlock.ActualWidth, textBlock.ActualHeight);
            textBlock.Foreground = lgb;
        }

        /// <summary>
        /// 改变歌词颜色
        /// </summary>
        public void ResetLyricForecolor()
        {
            SetLyricForecolor(this.lrcLineOneTB,
                FontForeColor, 0,
                FontForeColor, 0,
                FontBackColor, 0,
                FontBackColor, 0);
            SetLyricForecolor(this.lrcLineTwoTB,
                FontForeColor, 0,
                FontForeColor, 0,
                FontBackColor, 0,
                FontBackColor, 0);
        }
        
        /// <summary>
        /// 改变歌词内容
        /// </summary>
        /// <param name="text0"></param>
        /// <param name="text1"></param>
        public void ChangeLyricText(string text0, string text1)
        {
            ResetLyricForecolor();
            this.lrcLineOneTB.Text = text0;
            this.lrcLineTwoTB.Text = text1;
        }

        /// <summary>
        /// 动画渐显歌词。
        /// </summary>
        /// <param name="line0">当line0>0时表示对行1使用动画，下同。</param>
        /// <param name="line1">当line1>0时表示对行2使用动画。</param>
        public void ShowDesktopLyricWithAnimation(int line0, int line1)
        {
            if (line0 > 0)
            {
                ChangeUIElementOpacity(lrcLineOneTB, 0, 1);
                ChangeUIElementFontSize(lrcLineOneTB, 0, fontsize);
            }
            if (line1 > 0)
            {
                ChangeUIElementOpacity(lrcLineTwoTB, 0, 1);
                ChangeUIElementFontSize(lrcLineTwoTB, 0, fontsize);
            }
        }

        /// <summary>
        /// 改变偏移量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="offset"></param>
        public void ChangeStopOffset(int id, double offset)
        {
            switch (id)
            {
                case 0:
                    {
                        SetLyricForecolor(this.lrcLineOneTB,
                        FontForeColor, 0,
                        FontForeColor, offset,
                        FontBackColor, offset,
                        FontBackColor, offset);
                    }
                    break;
                case 1:
                    {
                        SetLyricForecolor(this.lrcLineTwoTB,
                        FontForeColor, 0,
                        FontForeColor, offset,
                        FontBackColor, offset,
                        FontBackColor, offset);
                    }
                    break;
                default:
                    break;
            }
        }

        internal void ChangeFontSize(int dskLrcFontSize)
        {
            try
            {
                fontsize = dskLrcFontSize;
                lrcLineOneTB.FontSize = dskLrcFontSize;
                lrcLineTwoTB.FontSize = dskLrcFontSize;
            }
            // 2014-02-26捕获到一个异常，但不常发生。暂时没有处理。
            catch { }
        }

        internal void ChangeFontForeColor(Color dskLrcPlayedForecolor, Color dskLrcUnplayedForecolor)
        {
            FontBackColor = dskLrcUnplayedForecolor;
            FontForeColor = dskLrcPlayedForecolor;
        }

        internal int GetFontSize()
        {
            return (int)lrcLineOneTB.FontSize;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        public event EventHandler PlayerButtonClick;
        protected void OnPlayerButtonClick(string state)
        {
            if (PlayerButtonClick!=null)
            {
                this.PlayerButtonClick(state, new EventArgs());
            }
        }
        private void previousSongBT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnPlayerButtonClick("上一曲");
        }

        private void playerSongBT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnPlayerButtonClick("播放");
        }

        private void nextSongBT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnPlayerButtonClick("下一曲");
        }

        private void pauseSongBT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnPlayerButtonClick("暂停");
        }

        private void closeDskLrcFrmBT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnPlayerButtonClick("关闭");
        }
        /// <summary>
        /// 获得image的source。
        /// </summary>
        /// <param name="imagePath">图片的地址</param>
        /// <returns>返回BitmapImage类型.</returns>
        private BitmapImage GetBitmapImage(string imagePath)
        {
            BitmapImage bit = null;
            if (imagePath.Trim() != "")
            {
                try
                {
                    bit = new BitmapImage();
                    bit.BeginInit();
                    bit.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
                    bit.EndInit();
                }
                catch (Exception)
                {
                    bit = null;
                }
            }
            return bit;
        }
        
        /// <summary>
        /// 歌词字体设置
        /// </summary>
        /// <param name="dskLrcFontFamily"></param>
        internal void ChangeFontFamily(string dskLrcFontFamily)
        {
            lrcLineOneTB.FontFamily = new FontFamily(dskLrcFontFamily);
            lrcLineTwoTB.FontFamily = lrcLineOneTB.FontFamily;
        }

        internal void ChangeFontStyle(string dskLrcFontStyle)
        {
            if (dskLrcFontStyle == "Bold")
            {
                lrcLineOneTB.FontStyle = FontStyles.Normal;
                lrcLineOneTB.FontWeight = FontWeights.Bold;
            }
            else if (dskLrcFontStyle == "Italic")
            {
                lrcLineOneTB.FontStyle = FontStyles.Italic;
                lrcLineOneTB.FontWeight = FontWeights.Normal;
            }
            else if (dskLrcFontStyle == "Italic, Bold")
            {
                lrcLineOneTB.FontStyle = FontStyles.Italic;
                lrcLineOneTB.FontWeight = FontWeights.Bold;
            }
            else
            {
                lrcLineOneTB.FontStyle = FontStyles.Normal;
                lrcLineOneTB.FontWeight = FontWeights.Normal;
            }
            lrcLineTwoTB.FontStyle = lrcLineOneTB.FontStyle;
            lrcLineTwoTB.FontWeight = lrcLineOneTB.FontWeight;

        }

        internal string GetFontFamily()
        {
            return lrcLineOneTB.FontFamily.ToString();
        }

        internal string GetFontStyle()
        {
            string ret = "Normal";
            if (lrcLineOneTB.FontStyle == FontStyles.Normal &&
                lrcLineOneTB.FontWeight == FontWeights.Normal)
            {
                ret = "Normal";
            }
            if (lrcLineOneTB.FontStyle == FontStyles.Italic &&
                lrcLineOneTB.FontWeight == FontWeights.Normal)
            {
                ret = "Italic";
            }
            if (lrcLineOneTB.FontStyle == FontStyles.Italic &&
                lrcLineOneTB.FontWeight == FontWeights.Bold)
            {
                ret = "Italic, Bold";
            }
            if (lrcLineOneTB.FontStyle == FontStyles.Normal &&
                lrcLineOneTB.FontWeight == FontWeights.Bold)
            {
                ret = "Bold";
            }
            return ret;
        }
    }
}
