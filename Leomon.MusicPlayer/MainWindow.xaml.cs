/*
 * Author : leomon
 * Email : 1964416932@qq.com
 * Website : null
 * Reference : null
 * */
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
using DoubanFM.Bass;
using Leomon.Lyric;
using System.Windows.Media.Animation;
using System.Net.NetworkInformation;
using System.IO;

namespace Leomon.MusicPlayer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //初始化布局面板中需要的资源等。
            //InitializeLayoutSevice();
            //播放器初始化为停止模式
            playerState = PlayerState.Stop;
            //
            InitializeMyPlayer();
            #region 临时加入的代码，初始化歌曲列表
            if (songListLB.SelectedIndex + 1 > 0)
            {
                this.titleTB.Text = string.Format("歌曲列表(共{0}首，当前选中第{1}首)", songListLB.Items.Count,
                songListLB.SelectedIndex + 1);
            }
            else
            {
                this.titleTB.Text = string.Format("歌曲列表(共{0}首)", songList.Count);
            }
            #endregion
        }

        #region 事件响应业务代码块
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //拖动窗体
            try
            {
                this.DragMove();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 用来触发面板翻转事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            //StartCount();
        }


        private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// 按钮事件响应。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void musicPlayerButton_ClickDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            if (image != null)
            {
                //根据名称判断是哪个按钮按下的。
                PlayerButtonClickDown(image);
            }
        }

        /// <summary>
        /// 当在区域二中移动鼠标时，则将控件设置为可见，否则当静止等状态时隐藏控件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowUIElementWithAnimation(sender, true);
        }

        /// <summary>
        /// 移除区域２时则隐藏控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            ShowUIElementWithAnimation(sender, false);
        }

        /// <summary>
        /// ProgressBar的响应事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void channelStatusPrb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ProgressBar prb = sender as ProgressBar;
            if (prb != null)
            {
                if (prb.Name == channelStatusPrb.Name)
                {
                    //改变播放进度条
                    //获取坐标
                    Point p = e.GetPosition(prb);
                    //根据坐标值改变prb的value
                    ChangeChannelPosition(prb, (p.X / prb.ActualWidth) * prb.Maximum);
                }
                else if (prb.Name == volumeValuePrb.Name)
                {
                    //改变音量。
                    ChangeVolumeValue(prb, e);
                }
            }
        }

        private void volumeValuePrb_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = volumeValuePrb.Value;
            if (value == 0)
            {
                //设置为静音标志
                ChangeImageSource(volumeBt, @"pictures/音量标志/vmute.png");
            }
            else if (value > 0 && value <= 25)
            {
                ChangeImageSource(volumeBt, @"pictures/音量标志/v0.png");
            }
            else if (value > 25 && value <= 50)
            {
                ChangeImageSource(volumeBt, @"pictures/音量标志/v1.png");
            }
            else if (value > 50 && value <= 75)
            {
                ChangeImageSource(volumeBt, @"pictures/音量标志/v2.png");
            }
            else if (value > 75 && value <= 100)
            {
                ChangeImageSource(volumeBt, @"pictures/音量标志/v3.png");
            }
            if (bassPlayer != null)
            {
                //改变音量
                bassPlayer.Volume = value / 100.0;
            }

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (shouldClose == false)
            {
                e.Cancel = true;
                PlayerButtonClickDown(closeBt);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //
            LoadApplicationSettings();
            //
            //之所以放在这个加载配置过程之后，是因为可能spectrum没有得到配置信息就已经初始化完了。那样保存的配置就无效。
            if (bassPlayer != null && showSpectrum)
            {
                SpectrumAnalyzer.RegisterSoundPlayer(bassPlayer);
            }
            nowLyricTextBlock.Foreground = GetBrushFromString(unplayedForecolor.ToString());
            preLyricTextBlock.Foreground = nowLyricTextBlock.Foreground;
            //
            //ShowWindowWithAnimation();
            //ScrollTextBlock(nowPlayingSong);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null)
            {
                if (item.Name == "changeBackImageItem") //更换背景菜单项被点击。
                {
                    Microsoft.Win32.OpenFileDialog opf = new Microsoft.Win32.OpenFileDialog();
                    opf.Filter = "图片(*.jpg)|*.jpg|图片(*.png)|*.png|图片(*.bmp)|*.bmp|所有图片|*.*";
                    if (opf.ShowDialog() == true)
                    {
                        ChangeBorderBackImage(opf.FileName);
                        borderImagePath = opf.FileName;
                    }
                    SaveApplicationSettings();
                }
                else if (item.Name == "songTagInfoItem")    //歌曲详细信息菜单项被点击。
                {
                    //GetSongTagInformation(songList[nowplayingIndex].FilePath);
                }
                else if (item.Name == "showDesktopLyricItem")   //显示桌面歌词菜单项被点击。
                {
                    //状态取反。
                    showDesktopLyricItem.IsChecked = !showDesktopLyricItem.IsChecked;
                    if (showDesktopLyricItem.IsChecked && dskLrc != null)
                    {
                        LoadLyric(songList[nowplayingIndex]);
                    }
                    else
                    {
                        if (dskLrc != null)
                        {
                            dskLrc.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    }

                }
                else if (item.Name == "detailSettingItem")  //详细设置菜单项被点击。
                {
                    if (dskLrc != null)
                    {
                        dskLrcFontSize = dskLrc.GetFontSize();
                        dskLrcFontFamily = dskLrc.GetFontFamily();
                        dskLrcFontStyle = dskLrc.GetFontStyle();
                        dskLrcPlayedForecolor = dskLrc.FontForeColor;
                        dskLrcUnplayedForecolor = dskLrc.FontBackColor;
                        
                    }
                    //打开设置窗体
                    SettingForm sForm = new SettingForm(BorderImage.Opacity,
                        this.dskLrcFontSize,
                        this.dskLrcFontFamily,
                        this.dskLrcFontStyle,
                        showSpectrum, autoLoadLyricFile,
                        saveConfig, saveSongList, rememberExitPosition, 
                        nowPlayingSong.Foreground,
                        GetBrushFromString(unplayedForecolor.ToString()),
                        GetBrushFromString(playedForecolor.ToString()),
                        dskLrcUnplayedForecolor,
                        dskLrcPlayedForecolor,
                        WindowTitle.Foreground,
                        titleTB.Foreground,
                        songListLB.Foreground,
                        new SolidColorBrush(Colors.Blue),
                        encoding);
                    sForm.SettingValueChangedEventHandler += sForm_SettingValueChangedEventHandler;
                    try
                    {
                        sForm.ShowDialog();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else if (item.Name == "aboutSoftwareItem")  //关于软件菜单项被点击。
                {
                    (new AboutForm()).ShowDialog();
                }
                else if (item.Name == "exitSoftwareItem")   //退出应用菜单项被点击。
                {
                    PlayerButtonClickDown(closeBt);
                }
                else if (item.Name == "deleteSelectedItem")
                {
                    DeleteSong();
                }
                else if (item.Name == "removeSelectedItem")
                {
                    PlayerButtonClickDown(deleteSongBt);
                }
                else if (item.Name == "removeAllItems")
                {
                    StopMusic();
                    //开始移除所有歌曲
                    songListLB.Items.Clear();
                    songList.Clear();
                    int i = songList.Count;
                    ResetValue();
                }
                else if (item.Name == "loadLyricFileItem")  //手动加载歌词菜单项被点击
                {
                    //首先判断有没有歌曲正在播放着
                    if (nowPlayingSongName != "")
                    {
                        Microsoft.Win32.OpenFileDialog opf = new Microsoft.Win32.OpenFileDialog();
                        opf.Filter = "歌词文件|*.lrc|所有文件|*.*";
                        if (opf.ShowDialog() == true)
                        {
                            if (opf.FileName.Contains(nowPlayingSongName))
                            {
                                if (lyric != null)
                                {
                                    lyric = null;
                                }
                                InitializeMyLyric(opf.FileName, "");
                            }
                            else
                            {
                                MessageBoxResult mbr = MessageBox.Show("歌词文件名称与当前播放歌曲可能无法匹配，确认加载？",
                                "载入歌词", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                                if (mbr == MessageBoxResult.OK)
                                {
                                    if (lyric != null)
                                    {
                                        //释放掉lyricd对象。
                                        lyric = null;
                                    }
                                    InitializeMyLyric(opf.FileName, "");
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("抱歉，当没有歌曲播放时无法载入歌词。^_^", "载入歌词", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else if (item.Name == "playPPTItem")    //播放幻灯片菜单项被选中了。
                {
                    //MessageBox.Show(item.Name);
                }
                else if (item.Name == "addImageDirItem")    //添加图片目录菜单项
                {
                    //MessageBox.Show(item.Name);
                    AddImageDirectory();
                }
                else if (item.Name == "addImageFilesItem")    //添加图片菜单项。
                {
                    //MessageBox.Show(item.Name);
                    AddImageFiles();
                }
                else if (item.Name == "pptPlayerControlItem")   //ppt播放控制按钮。
                {
                    if (item.Header.ToString() == "开始放映")
                    {
                        if (PlayPPT(imageList))
                        {
                            item.Header = "停止放映";
                            randomPlayModeItem.IsEnabled = false;
                            loopPlayModeItem.IsEnabled = false;
                        }
                    }
                    else
                    {
                        //停止放映
                        if (StopPlayPPT())
                        {
                            item.Header = "开始放映";
                            randomPlayModeItem.IsEnabled = true;
                            loopPlayModeItem.IsEnabled = true;
                        }
                    }

                }
                else if (item.Name == "addFilesItem")
                {
                    AddSong(true, null);
                    SaveApplicationSettings();
                }
                else if (item.Name == "addDirItem")
                {
                    AddSongDirectory();
                    SaveApplicationSettings();
                }
                else if (item.Name == "loopPlayModeItem")
                {
                    index = 0;
                    myPPTPlayMode = PPTPlayMode.LoopPlay;
                    item.IsChecked = true;
                    randomPlayModeItem.IsChecked = false;
                }
                else if (item.Name == "randomPlayModeItem")
                {
                    myPPTPlayMode = PPTPlayMode.RandomPlay;
                    item.IsChecked = true;
                    loopPlayModeItem.IsChecked = false;
                }
                else if (item.Name == "addSongItem")    //添加歌曲菜单项
                {
                    AddSong(true, null);
                    SaveApplicationSettings();
                }
                else if (item.Name == "addSongFolderItem")  //添加歌曲文件夹菜单项。
                {
                    AddSongDirectory();
                    SaveApplicationSettings();
                }
                else if (item.Name == "autoAddImages")
                {
                    AutoLoadImagesFromDisk();
                }
                else if (item.Name == "clearLyricItem") //清空歌词显示。
                {
                    ClearLyric = !ClearLyric;
                    item.IsChecked = !ClearLyric;
                    //if (dskLrc != null)
                    //{
                    //    dskLrc.Close();
                    //    dskLrc = null;
                    //}
                    //if (lyric != null)
                    //{
                    //    lyric = null;
                    //}
                    if (ClearLyric)
                    {
                        nextLyricTextBlock.Visibility = Visibility.Collapsed;
                        nowLyricTextBlock.Visibility = Visibility.Collapsed;
                        preLyricTextBlock.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        nextLyricTextBlock.Visibility = Visibility.Visible;
                        nowLyricTextBlock.Visibility = Visibility.Visible;
                        preLyricTextBlock.Visibility = Visibility.Visible;
                    }
                }
                else if (item.Name == "switchPageItem")     //翻转页面
                {
                    FlipUIElement(mainGrid);
                    FlipUIElement(playlistGrid);
                }

                else if (item.Name == "previousSongItem")
                {
                    PlayerButtonClickDown(previousSongBt);
                }
                else if (item.Name == "nextSongItem")
                {
                    PlayerButtonClickDown(nextSongBt);
                }
                else if (item.Name == "downloadLricFileItem")
                {
                    SaveApplicationSettings();
                    DowloadLrcFrm dldFrm = new DowloadLrcFrm(nowPlayingSongName);
                    dldFrm.GetLyricFile += dldFrm_GetLyricFile;
                    dldFrm.ShowDialog();
                }
                else if (item.Name == "searchSongItem")
                {
                    //弹出搜索对话框。
                    OpenSearchSongForm();
                }
                else if (item.Name == "fastLocateItem")
                {
                    if (songListLB.Items.Count > 0)
                    {
                        songListLB.SelectedIndex = nowplayingIndex;
                        songListLB.ScrollIntoView(songListLB.SelectedItem);
                    }
                }
            }
        }

        void dldFrm_GetLyricFile(object sender, EventArgs e)
        {
            string strLrc = sender as string;
            if (strLrc != null)
            {
                //关掉歌词显示。
                if (lyric != null)
                {
                    lyric = null;
                    nowLyricTextBlock.Text = "";
                    nextLyricTextBlock.Text = "";
                    preLyricTextBlock.Text = "";
                }
                if (dskLrc != null)
                {
                    dskLrc.Close();
                    dskLrc = null;
                }
                InitializeMyLyric("", strLrc);
                SaveLrcFile(strLrc);
            }
        }
        /// <summary>
        /// 将下载到的歌词文件先保存起来。
        /// </summary>
        /// <param name="strLrc"></param>
        private void SaveLrcFile(string strLrc)
        {
            string path = songList[nowplayingIndex].FileDirectory + "\\" + songList[nowplayingIndex].SongName + ".lrc";
            //判断当前歌曲的歌词是否存在
            if (hasLyric)
            {
                MessageBoxResult rslt = 
                    MessageBox.Show("歌词文件已经存在，是否替换原来歌词文件？", "保存歌词文件", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (rslt == MessageBoxResult.Yes)
                {
                    try
                    {
                        //字符流写出器
                        StreamWriter sw = new StreamWriter(path, false, Encoding.Default);
                        //写出信息
                        sw.Write(strLrc);
                        //清空缓存
                        sw.Flush();
                        //关闭连接
                        sw.Close();
                        lyricFilePath = path;
                        hasLyric = true;
                    }
                    catch { }
                }
            }
            else
            {
                try
                {
                    //字符流写出器
                    StreamWriter sw = new StreamWriter(path, false, Encoding.Default);
                    //写出信息
                    sw.Write(strLrc);
                    //清空缓存
                    sw.Flush();
                    //关闭连接
                    sw.Close();
                    lyricFilePath = path;
                    hasLyric = true;
                }
                catch { }
            }
        }

        /// <summary>
        /// 将设置信息反映回来。
        /// </summary>
        void sForm_SettingValueChangedEventHandler(object sender, MySettingValueEventArgs e)
        {
            BorderImage.Opacity = e.OpacityValue;
            borderOpacity = BorderImage.Opacity;
            showSpectrum = (bool)e.ShowSpectrum;
            autoLoadLyricFile = (bool)e.AutoLoadLyricFile;
            saveConfig = (bool)e.SaveConfig;
            saveSongList = (bool)e.SaveSongList;
            rememberExitPosition = (bool)e.RememberExitPosition;
            dskLrcFontSize = e.DesktopLyricFontSize;
            dskLrcFontFamily = e.DesktopLyricFontFamily;
            dskLrcFontStyle = e.DesktopLyricFontType;
            nowPlayingSong.Foreground = e.SongNameForeColor;
            WindowTitle.Foreground = e.WindowTitleForeColor;
            //preLyricTextBlock.Foreground = e.UnplayedLyricForeColor;
            //nowLyricTextBlock.Foreground = e.PlayedLyricForeColor;
            unplayedForecolor = GetColorFromBrush(e.UnplayedLyricForeColor);
            playedForecolor = GetColorFromBrush(e.PlayedLyricForeColor);

            titleTB.Foreground = e.ListBoxTitleForeColor;
            songListLB.Foreground = e.UnselectedItemForeColor;
            dskLrcPlayedForecolor = e.DesktopPlayedLyricForeColor;
            dskLrcUnplayedForecolor = e.DesktopUnplayedLyricForeColor;
            //主动改变桌面歌词
            if (dskLrc != null && dskLrc.IsVisible)
            {
                dskLrc.ChangeFontSize(dskLrcFontSize);
                dskLrc.ChangeFontForeColor(dskLrcPlayedForecolor, dskLrcUnplayedForecolor);
                //dskLrc.ChangeFontFamily(dskLrcFontFamily);
                //dskLrc.ChangeFontStyle(dskLrcFontStyle);
            }
            //改变歌词前景色
            preLyricTextBlock.Foreground = GetBrushFromString(unplayedForecolor.ToString());
            nowLyricTextBlock.Foreground = preLyricTextBlock.Foreground;
            //判断编码方式改变没有，如果改变。则重新加载歌词
            if (encoding != e.FileEncoding)
            {
                encoding = e.FileEncoding;
                lyric = null;
                InitializeMyLyric(lyricFilePath, "");
            }
            //保存配置。
            SaveApplicationSettings();
        }

        private void songListLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (songListLB.SelectedIndex + 1 > 0)
            {
                this.titleTB.Text = string.Format("歌曲列表(共{0}首，当前选中第{1}首)", songListLB.Items.Count,
                songListLB.SelectedIndex + 1);
            }
            else
            {
                this.titleTB.Text = string.Format("歌曲列表(共{0}首)", songList.Count);
            }
        }

        private void songListLB_KeyDown(object sender, KeyEventArgs e)
        {
            //判断是什么按键按下
            if (e.Key == Key.Enter)
            {
                //方便处理
                //直接转到ItemClick上去了
                item_MouseDoubleClick(null, null);
            }
        }

        private void Prb_MouseEnter(object sender, MouseEventArgs e)
        {
            //判断是哪个prb
            ProgressBar prb = sender as ProgressBar;
            if (prb != null)
            {
                if (prb.Name == "volumeValuePrb")
                {
                    volumeValueTT.Content = string.Format("音量{0}%", prb.Value.ToString("00"));
                }
                else if (prb.Name == channelStatusPrb.Name)
                {
                    ShowTimeOnToolTip(e, prb);
                }
            }
        }

        private void channelStatusPrb_MouseMove(object sender, MouseEventArgs e)
        {
            ShowTimeOnToolTip(e, channelStatusPrb);
        }
        /// <summary>
        /// 应用程序响应键盘按下事件，并判断做出相关动作。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Key downKey = e.Key;
            switch (downKey)
            {
                case Key.Space:
                    //空格键按下时，就暂停或播放歌曲。
                    PlayerButtonClickDown(mediaControlBt);
                    break;
                case Key.PageUp:
                    //当左方向键按下时，则切换到上一曲。
                    PlayerButtonClickDown(previousSongBt);
                    break;
                case Key.PageDown:
                    //当右方向键按下时，则切换到下一曲。
                    PlayerButtonClickDown(nextSongBt);
                    break;
                case Key.Up:
                    //当上方向键按下时，则增加音量。
                    volumeValuePrb.Value += 5;
                    break;
                case Key.Down:
                    //当下方向键按下时，则减小音量。
                    volumeValuePrb.Value -= 5;
                    break;
                case Key.Delete:
                    //删除歌曲
                    DeleteSong();
                    break;
                case Key.Left:
                    //当左方向键按下时，则切换到上一曲。
                    PlayerButtonClickDown(previousSongBt);    
                    //ChangeChannelPosition(channelStatusPrb, channelStatusPrb.Value - 20);
                    break;
                case Key.Right:
                    //当右方向键按下时，则切换到下一曲。
                    PlayerButtonClickDown(nextSongBt);
                    //ChangeChannelPosition(channelStatusPrb, channelStatusPrb.Value + 20);
                    break;
                default: break;
            }
        }

        
        private void songListLB_MouseEnter(object sender, MouseEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox != null)
            {
                ScrollViewer.SetVerticalScrollBarVisibility(listBox, ScrollBarVisibility.Auto);
                
            }
            //显示歌曲搜索框
            //SetSearchTextBoxOpacity(0, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void songListLB_MouseLeave(object sender, MouseEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox != null)
            {
                ScrollViewer.SetVerticalScrollBarVisibility(listBox, ScrollBarVisibility.Hidden);
            }
            //隐藏歌曲搜索框
            //SetSearchTextBoxOpacity(1, 0);
        }

        /// <summary>
        /// 鼠标滚轮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Prb_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //ProgressBar prb = sender as ProgressBar;
            int i = (int)(e.Delta / 50);
            ////MessageBox.Show(i.ToString());
            //if (prb != null)
            //{
             volumeValuePrb.Value += i;
            //}
        }
        #endregion




    }
}
