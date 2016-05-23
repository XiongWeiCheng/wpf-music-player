using System;
using System.Collections.Generic;
/*
 * Author : leomon
 * Email : 1964416932@qq.com
 * Website : null
 * Reference : null
 * */
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Leomon.MusicPlayer
{
    /// <summary>
    /// SettingForm.xaml 的交互逻辑
    /// </summary>
    public partial class SettingForm : Window
    {
        #region 字段属性定义区
        /// <summary>
        /// 属性：设置或获取窗体透明度设定值。
        /// </summary>
        public double OpacityValue
        {
            get
            {
                return (this.opacitySlider.Value / 100);
            }
            set
            {
                this.opacitySlider.Value = value * 100;
            }
        }
        /// <summary>
        /// 属性：设置或获取未播放歌词的前景色
        /// </summary>
        public Brush UnplayedLyricForeColor
        {
            get
            {
                return unplayLyricForeColorBT.Background;
            }
            set
            {
                this.unplayLyricForeColorBT.Background = value;
            }
        }
        /// <summary>
        /// 属性：设置或获取已播放歌词的前景色
        /// </summary>
        public Brush PlayedLyricForeColor
        {
            get
            {
                return playedLyricForeColorBT.Background;
            }
            set
            {
                this.playedLyricForeColorBT.Background = value;
            }
        }
        
        /// <summary>
        /// 属性：设置或获取歌曲标题的前景色
        /// </summary>
        public Brush SongNameForeColor
        {
            get
            {
                return songNameForeColorBT.Background;
            }
            set
            {
                this.songNameForeColorBT.Background = value;
            }
        }
        /// <summary>
        /// 属性：窗口标题栏前景色设置
        /// </summary>
        public Brush WindowTitleForeColor
        {
            get
            {
                return windowTitleForeColorBT.Background;
            }
            set
            {
                this.windowTitleForeColorBT.Background = value;
            }
        }
        /// <summary>
        /// 列表的标题栏颜色
        /// </summary>
        public Brush ListBoxTitleForeColor
        {
            get
            {
                return listBoxTitleForeColorBT.Background;
            }
            set
            {
                this.listBoxTitleForeColorBT.Background = value;
            }
        }
        /// <summary>
        /// 未选择歌曲列表前景色
        /// </summary>
        public Brush UnselectedItemForeColor
        {
            get
            {
                return unselectedListItemForeColorBT.Background;
            }
            set
            {
                this.unselectedListItemForeColorBT.Background = value;
            }
        }
        ///// <summary>
        ///// 已选中项目的前景色
        ///// </summary>
        //public Brush SelectedItemForeColor
        //{
        //    get
        //    {
        //        return selectedListItemForeColorBT.Background;
        //    }
        //    set
        //    {
        //        this.selectedListItemForeColorBT.Background = value;
        //    }
        //}
        /// <summary>
        /// 属性：获取或设置是否显示频谱图的标志
        /// </summary>
        public bool? ShowSpectrum
        {
            get
            {
                return showSpectrumCB.IsChecked;
            }
            set
            {
                showSpectrumCB.IsChecked = value;
            }
        }
        /// <summary>
        /// 属性：获取或设置桌面歌词字体大小。
        /// </summary>
        public int DesktopLyricFontSize
        {
            get;
            set;
        }
        public string DeskLyricFontType
        {
            get;
            set;
        }
        /// <summary>
        /// 字体
        /// </summary>
        public string DesktopLyricFontFamily
        {
            get;
            set;
        }
        /// <summary>
        /// 属性：设置或获取桌面歌词未播放前景色
        /// </summary>
        public Color DesktopUnplayedLyricForeColor
        {
            get
            {
                return ((SolidColorBrush)this.dskUndplayLyricForeColorBT.Background).Color;
            }
            set
            {
                this.dskUndplayLyricForeColorBT.Background = new SolidColorBrush(value);
            }
        }
        /// <summary>
        /// 属性：设置或获取桌面歌词已经播放的前景色
        /// </summary>
        public Color DesktopPlayedLyricForeColor
        {
            get
            {
                return ((SolidColorBrush)this.dskPlayedLyricForeColorBT.Background).Color;
            }
            set
            {
                this.dskPlayedLyricForeColorBT.Background = new SolidColorBrush(value);
            }
        }
        /// <summary>
        /// 属性：设置或获取自动加载歌词文件
        /// </summary>
        public bool? AutoLoadLyricFile
        {
            get
            {
                return autoLoadOrDownloadLyricFileCB.IsChecked;
            }
            set
            {
                autoLoadOrDownloadLyricFileCB.IsChecked = value;
            }
        }
        /// <summary>
        /// 属性：设置或获取自动保存配置标志
        /// </summary>
        public bool? SaveConfig
        {
            get
            {
                return saveConfigCB.IsChecked;
            }
            set
            {
                saveConfigCB.IsChecked = value;
            }
        }
        /// <summary>
        /// 属性：设置或获取自动保存歌曲列表标志
        /// </summary>
        public bool? SaveSongList
        {
            get
            {
                return saveSongListCB.IsChecked;
            }
            set
            {
                saveSongListCB.IsChecked = value;
            }
        }
        /// <summary>
        /// 属性：设置或获取是否记忆歌曲退出位置标志
        /// </summary>
        public bool? RememberExitPosition
        {
            get
            {
                return rememberExitPositionCB.IsChecked;
            }
            set
            {
                rememberExitPositionCB.IsChecked = value;
            }
        }

        private Dictionary<int, Encoding> encodingDict = new Dictionary<int, Encoding>(3);
        /// <summary>
        /// 属性：设置或获取默认歌词的编码格式。
        /// </summary>
        public Encoding FileEncoding
        {
            get
            {
                return encodingDict[this.EncodingCB.SelectedIndex];
            }
            set
            {
                for (int i = 0; i < encodingDict.Count; i++)
                {
                    if (value == encodingDict[i])
                    {
                        this.EncodingCB.SelectedIndex = i;
                        break;
                    }
                }
            }
        }
        #endregion

        public delegate void SettingValueChangedCallback(object sender, MySettingValueEventArgs e);
        public event SettingValueChangedCallback SettingValueChangedEventHandler;
        public SettingForm()
        {
            InitializeComponent();
            this.SettingValueChangedEventHandler += SettingForm_SettingValueChangedEventHandler;
        }

        void SettingForm_SettingValueChangedEventHandler(object sender, MySettingValueEventArgs e)
        {

        }

        /// <summary>
        /// 构造方法二。
        /// </summary>
        /// <param name="opacityValue">播放器窗体透明度</param>
        /// <param name="desktopLyricFontSize">桌面歌词大小</param>
        /// <param name="showSpectrum">显示频谱图标志</param>
        /// <param name="autoLoadLyricFile">自动加载歌词标志</param>
        /// <param name="songNameForeColor">歌名前景色</param>
        /// <param name="unplayedLyricForeColor">未播放歌词的前景色</param>
        /// <param name="playedLyricForeColor">已经播放的歌词前景色</param>
        /// <param name="desktopUnplayedLyricForeColor">未播放桌面歌词前景色</param>
        /// <param name="desktopPlayedLyricForeColor">已播放桌面歌词前景色</param>
        public SettingForm(double opacityValue,
            int fontSize,
            string fontName,
            string fontStyle,
            bool? showSpectrum,
            bool? autoLoadLyricFile,
            bool? saveConfig,
            bool? saveSongList,
            bool? remeberExitPos,
            Brush songNameForeColor,
            Brush unplayedLyricForeColor,
            Brush playedLyricForeColor,
            Color desktopUnplayedLyricForeColor,
            Color desktopPlayedLyricForeColor,
            Brush windowTitleForeColor,
            Brush listBoxItemForeColor,
            Brush unselectedItemForeColor,
            Brush selectedItemForeColor,
            Encoding encoding)
        {
            InitializeComponent();
            #region 添加编码方式列表
            this.encodingDict.Add(0, Encoding.Default);
            this.encodingDict.Add(1, Encoding.UTF8);
            this.encodingDict.Add(2, Encoding.Unicode);
            #endregion
            this.OpacityValue = opacityValue;
            this.DesktopLyricFontSize = fontSize;
            this.DesktopLyricFontFamily = fontName;
            this.DeskLyricFontType = fontStyle;
            this.ShowSpectrum = showSpectrum;
            this.AutoLoadLyricFile = autoLoadLyricFile;
            this.SongNameForeColor = songNameForeColor;
            this.UnplayedLyricForeColor = unplayedLyricForeColor;
            this.PlayedLyricForeColor = playedLyricForeColor;
            this.DesktopPlayedLyricForeColor = desktopPlayedLyricForeColor;
            this.DesktopUnplayedLyricForeColor = desktopUnplayedLyricForeColor;
            this.WindowTitleForeColor = windowTitleForeColor;
            this.ListBoxTitleForeColor = listBoxItemForeColor;
            this.UnselectedItemForeColor = unselectedItemForeColor;
            this.SettingValueChangedEventHandler += SettingForm_SettingValueChangedEventHandler;
            this.SaveConfig = saveConfig;
            this.SaveSongList = saveSongList;
            this.RememberExitPosition = remeberExitPos;
            this.FileEncoding = encoding;
            this.dskLyricFontSize.Text = DesktopLyricFontSize.ToString();

            //
            //UpadteColorValueTB();
        }

        private void UpadteColorValueTB()
        {
            this.listBoxTitleForeColorValueTB.Text = this.ListBoxTitleForeColor.ToString();
            this.songNameForeColorValueTB.Text = this.SongNameForeColor.ToString();
            this.unselectedListItemForeColorValue.Text = this.UnselectedItemForeColor.ToString();
            this.windowTitleForeColorValueBT.Text = this.WindowTitleForeColor.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            if (bt != null)
            {
                if (bt.Name != "confirmBT")
                {
                    Color color = GetColor();
                    //设置button背景色为用户选择的颜色。
                    bt.Background = new SolidColorBrush(color);
                }
                else
                {
                    DesktopLyricFontSize = int.Parse(this.dskLyricFontSize.Text);
                }
                RaiseSettingValueChangedEvent();
                //UpadteColorValueTB();
            }
        }

        private Color GetColor()
        {
            Color color = new Color();
            System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
            //
            cd.AllowFullOpen = true;
            //显示颜色选取对话框
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //获取颜色
                color.A = cd.Color.A;
                color.R = cd.Color.R;
                color.G = cd.Color.G;
                color.B = cd.Color.B;
            }
            return color;
        }

        /// <summary>
        /// 透明度滑动条发生改变时触发。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void opacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //同步更新TextBlock显示
            try
            {
                opacityValueTB.Text = string.Format("{0}%", opacitySlider.Value.ToString("00"));
                RaiseSettingValueChangedEvent();
            }
            catch (Exception)
            { }
            //MessageBox.Show(opacitySlider.Value.ToString());
        }

        /// <summary>
        /// 引发事件。
        /// </summary>
        private void RaiseSettingValueChangedEvent()
        {
            SettingValueChangedEventHandler(this,
                new MySettingValueEventArgs(
                    this.OpacityValue,
                this.DesktopLyricFontSize,
                this.DesktopLyricFontFamily,
                this.DeskLyricFontType,
                this.ShowSpectrum,
                this.AutoLoadLyricFile,
                this.SaveConfig,
                this.SaveSongList,
                this.RememberExitPosition,
                this.SongNameForeColor,
                this.UnplayedLyricForeColor,
                this.PlayedLyricForeColor,
                this.DesktopUnplayedLyricForeColor,
                this.DesktopPlayedLyricForeColor,
                this.WindowTitleForeColor,
                this.ListBoxTitleForeColor,
                this.UnselectedItemForeColor,
                this.UnselectedItemForeColor,
                this.FileEncoding));
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            RaiseSettingValueChangedEvent();
        }

        private bool firstTime = true;
        //private bool fontChanged;
        private void EncodingCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (firstTime == false)
            {
                FileEncoding = encodingDict[EncodingCB.SelectedIndex];
                RaiseSettingValueChangedEvent();
            }
            firstTime = false;
        }
    }
}
