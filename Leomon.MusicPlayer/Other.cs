/*
 * Author : leomon
 * Email : 1964416932@qq.com
 * Website : null
 * Reference : null
 * */
using Leomon.SPLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Leomon.MusicPlayer
{
    /// <summary>
    /// 其他方面的业务逻辑，我也不晓得该怎么归类了。。反正很烦心。。。。
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 保存所有歌曲路径到文件中。
        /// </summary>
        /// <param name="songList">歌曲</param>
        private void SaveSongListPath(List<MySong> songList)
        {
            Editor editor = new Editor();
            if (saveSongList)
            {
                //保存歌曲数量
                editor.PutInt32("songCount", songList.Count);
                //保存歌曲路径。
                for (int i = 0; i < songList.Count; i++)
                {
                    string item = "song_index" + i.ToString();
                    editor.PutString(item, songList[i].FilePath);
                }
            }
            else
            {
                //保存歌曲数量
                editor.PutInt32("songCount", 0);
            }
            SharedPreferences mySharedPreferences = new SharedPreferences(@"配置文件夹\songlist.xml", false);
            mySharedPreferences.Save(editor);
        }

        /// <summary>
        /// 加载保存的歌曲列表。
        /// </summary>
        private void LoadSongListPath()
        {
            SharedPreferences mySharedPreferences = new SharedPreferences(@"配置文件夹\songlist.xml", true);
            int count = mySharedPreferences.GetInt32("songCount", 0);
            if (count > 0)
            {
                List<string> fileNames = new List<string>();
                for (int i = 0; i < count; i++)
                {
                    string item = "song_index" + i.ToString();
                    fileNames.Add(mySharedPreferences.GetString(item, ""));
                }
                AddSong(false, fileNames.ToArray());
            }
        }

        /// <summary>
        /// 保存用户配置。
        /// </summary>
        private void SaveApplicationSettings()
        {
            //新建一个目录，，，如果存在则不需要建立
            if (!Directory.Exists(@"配置文件夹"))
            {
                Directory.CreateDirectory("配置文件夹");
                //表示是程序是第一次运行。
                isFirstTimeRunning = true;
            }
            else
            {
                isFirstTimeRunning = false;
            }
            //保存歌曲列表
             SaveSongListPath(songList);
            //软件配置文件
            Editor editor = new Editor();
            if (saveConfig)
            {
                #region 要保存的配置信息。
                //保存背景图片路径
                editor.PutString("borderBackImagePath", borderImagePath);
                //保存音量
                editor.PutDouble("volume", volumeValuePrb.Value);
                //保存播放器的播放模式：
                editor.PutInt32("playmodeIndex", playmodeIndex);
                if (rememberExitPosition)
                {
                    //保存播放器被关闭时的状态：
                    //首先判断此时播放器是否处于正在播放或者是正在暂停的状态。如果是则保存当前状态。
                    if (playerState == PlayerState.Pause || playerState == PlayerState.Play && saveSongList)
                    {
                        //接下来就需要保存此时正在播放的歌曲的名称和该歌曲已经播放的进度，以便下次启动时能够继续播放。
                        //首先就是正在播放的歌曲索引号，便于下次运行时查找。
                        editor.PutInt32("nowplayingIndex", nowplayingIndex);
                        //然后时播放的进度:
                        editor.PutDouble("pausePosition", channelStatusPrb.Value);
                    }
                }
                //保存是否显示桌面歌词标志。
                editor.PutBoolean("showDskLyric", showDesktopLyricItem.IsChecked);
                //保存歌词文件编码方式
                editor.PutString("lyricFileEncoding", encoding.CodePage.ToString());
                //保存窗体配置
                //透明度值
                editor.PutDouble("windowOpacity", BorderImage.Opacity);
                //未播放歌词颜色
                editor.PutString("unplayedLyricForeColor", unplayedForecolor.ToString());
                //已经播放歌词的颜色
                editor.PutString("playedLyricForeColor", playedForecolor.ToString());
                //正在播放歌曲名称的标题前景色
                editor.PutString("songNameForeColor", nowPlayingSong.Foreground.ToString());
                //保存窗体标题颜色
                editor.PutString("windowTitleForeColor", WindowTitle.Foreground.ToString());
                //保存列表标题颜色
                editor.PutString("titleForeColor", this.titleTB.Foreground.ToString());
                //保存歌曲列表项颜色
                editor.PutString("unselectedItemForeColor", this.songListLB.Foreground.ToString());
                //保存是否显示频谱图
                editor.PutBoolean("showSpectrum", showSpectrum);
                //保存是否自动加载歌词
                editor.PutBoolean("autoLoadLyricFile", autoLoadLyricFile);

                //桌面歌词方面的配置
                //歌词文字大小
                editor.PutInt32("dskLyricFontSize", dskLrcFontSize);
                //
                editor.PutString("dskLyricFontStyle", dskLrcFontStyle);
                //歌词字体
                editor.PutString("dskLyricFontFamily", dskLrcFontFamily);
                //未播放歌词前景色
                editor.PutString("dskLrcUnplayedForeColor", dskLrcUnplayedForecolor.ToString());
                //已经播放的歌词前景色
                editor.PutString("dskLrcPlayedForeColor", dskLrcPlayedForecolor.ToString());
            }
            editor.PutBoolean("saveConfig", saveConfig);
            editor.PutBoolean("isFirstTimeRunning", false);
            editor.PutString("userName", userName);
            editor.PutBoolean("saveSongList", saveSongList);
            //保存记忆歌曲退出时的位置标志。
            editor.PutBoolean("remeberExitPosition", rememberExitPosition);
            #endregion
            //保存
            try
            {
                SharedPreferences sp = new SharedPreferences(@"配置文件夹\Application.config", false);
                sp.Save(editor);
            }
            catch (Exception)
            {
                //检查到异常，则移除配置文件重新生成配置文件即可
                try
                {
                    File.Delete(@"配置文件夹\Application.config");
                    SharedPreferences sp = new SharedPreferences(@"配置文件夹\Application.config", false);
                    sp.Save(editor);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 载入应用程序保存的配置信息。并设置相关控件的属性等。
        /// </summary>
        private void LoadApplicationSettings()
        {

            if (File.Exists(@"配置文件夹\Application.config"))
            {
                try
                {
                    //读取应用程序配置信息。
                SharedPreferences sp = new SharedPreferences(@"配置文件夹\Application.config", true);
                //读取并设置是否保存配置
                saveConfig = sp.GetBoolean("saveConfig", true);
                saveSongList = sp.GetBoolean("saveSongList", true);
                isFirstTimeRunning = sp.GetBoolean("isFirstTimeRunning", true);
                userName = sp.GetString("userName", "");
                if (File.Exists(@"配置文件夹\songlist.xml") && saveSongList)
                {
                    LoadSongListPath();
                }
                if (saveConfig)
                {
                    //读取并设置窗口透明度
                    BorderImage.Opacity = sp.GetDouble("windowOpacity", 1d);
                    borderOpacity = BorderImage.Opacity;
                    //读取并设置是否显示频谱图
                    showSpectrum = sp.GetBoolean("showSpectrum", true);
                    //读取并设置是否自动加载歌词文件
                    autoLoadLyricFile = sp.GetBoolean("autoLoadLyricFile", true);
                    //读取并设置歌词文件的编码方式
                    string str = sp.GetString("lyricFileEncoding", "null");
                    if ( str!= "null")
                    {
                        encoding = Encoding.GetEncoding(Int32.Parse(str));
                    }
                    //读取并设置桌面歌词字体大小
                    dskLrcFontSize = sp.GetInt32("dskLyricFontSize", 40);
                    string tempStr = string.Empty;
                    //读取并设置桌面歌词字体
                    tempStr = sp.GetString("dskLyricFontFamily", "null");
                    if (tempStr != "null" && tempStr != "")
                        dskLrcFontFamily = tempStr;
                    //
                    tempStr = sp.GetString("dskLyricFontStyle", "null");
                    if (tempStr != "null" && tempStr != "")
                    {
                        dskLrcFontStyle = tempStr;
                    }
                    //读取并设置已播放歌词颜色
                    tempStr = sp.GetString("playedLyricForeColor", "null");
                    if (tempStr != "null")
                        playedForecolor = (Color)(ColorConverter.ConvertFromString(tempStr));
                    //读取并设置未播放歌词的颜色
                    tempStr = sp.GetString("unplayedLyricForeColor", "null");
                    if (tempStr != "null")
                        unplayedForecolor = (Color)(ColorConverter.ConvertFromString(tempStr));
                    //读取并设置歌名前景色
                    tempStr = sp.GetString("songNameForeColor", "null");
                    if (tempStr != "null")
                        nowPlayingSong.Foreground = GetBrushFromString(tempStr);
                    //读取并设置标题前景色
                    tempStr = sp.GetString("windowTitleForeColor", "null");
                    if (tempStr != "null")
                        WindowTitle.Foreground = GetBrushFromString(tempStr);
                    //读取并设置桌面歌词未播放颜色
                    tempStr = sp.GetString("dskLrcUnplayedForeColor", "null");
                    if (tempStr != "null")
                        dskLrcUnplayedForecolor = (Color)(ColorConverter.ConvertFromString(tempStr));
                    //读取并设置桌面歌词已播放的
                    tempStr = sp.GetString("dskLrcPlayedForeColor", "null");
                    if (tempStr != "null")
                         dskLrcPlayedForecolor = (Color)(ColorConverter.ConvertFromString(tempStr));
                    //读取并设置标题颜色
                    tempStr = sp.GetString("titleForeColor", "null");
                    if (tempStr != "null")
                        titleTB.Foreground = GetBrushFromString(tempStr);
                    //读取并设置列表项目颜色
                    tempStr = sp.GetString("unselectedItemForeColor", "null");
                    if (tempStr != "null")
                        songListLB.Foreground = GetBrushFromString(tempStr);
                    //读取并更换背景图片。
                    borderImagePath = sp.GetString("borderBackImagePath", "null");
                    if (borderImagePath != "null")
                    {
                        ChangeBorderBackImage(borderImagePath);
                    }
                    //读取并设置为上次关闭时的音量。
                    this.volumeValuePrb.Value = sp.GetDouble("volume", 50);
                    //读取并设置是否显示桌面歌词
                    showDesktopLyricItem.IsChecked = sp.GetBoolean("showDskLyric", true);
                    //读取并设置为上次关闭时设置的播放模式
                    playmodeIndex = sp.GetInt32("playmodeIndex", -1) - 1;
                    PlayerButtonClickDown(playmodeBt);
                    #region 播放器状态还原代码。有点烦。。。
                    rememberExitPosition = sp.GetBoolean("remeberExitPosition", true);
                    if (songListLB.Items.Count != 0)
                    {
                        if (rememberExitPosition)
                        {
                            if (saveSongList && saveConfig)
                            {
                                if (sp.GetInt32("nowplayingIndex", -2) != -2)
                                {
                                    //读取并设置为上次关闭时播放器状态
                                    songListLB.SelectedIndex = sp.GetInt32("nowplayingIndex", -1);
                                    //进行播放
                                    PlayerButtonClickDown(mediaControlBt);
                                    bassPlayer.ChannelPosition = TimeSpan.FromMilliseconds(sp.GetDouble("pausePosition", 0));
                                    GetLyricIndex(bassPlayer.ChannelPosition.TotalMilliseconds);
                                    //再暂停，等待用户操作。
                                    PlayerButtonClickDown(mediaControlBt);
                                }
                            }
                        }
                    }
                
                    #endregion
                  }
                }
                catch (Exception)
                {
                    try
                    {
                        File.Delete(@"配置文件夹\Application.config");
                        MessageBox.Show("Sorry, 检测到配置文件损坏，因为格式有误，无法读取。已强制删除配置文件。");
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (isFirstTimeRunning)
            {
                //表示是第一次运行程序。
                //WelcomeToUse(string.Format("", userName));
                isFirstTimeRunning = false;
                //TestFunc();
            }
        }

        private Brush GetBrushFromString(string p)
        {
            BrushConverter bc = new BrushConverter();
            return (Brush)bc.ConvertFromString(p);
        }

        private Color GetColorFromBrush(Brush b)
        {
            return (Color)(ColorConverter.ConvertFromString(b.ToString()));
        }

        /// <summary>
        /// 更换Image控件的图片。
        /// </summary>
        /// <param name="image">image对象</param>
        /// <param name="newSource">待更新的图片路径</param>
        private void ChangeImageSource(Image image, string newSource)
        {
            BitmapImage bi = GetBitmapImage(newSource);
            if (bi != null)
            {
                image.Source = bi;
            }
        }

        /// <summary>
        /// 更改border的背景图片。
        /// </summary>
        /// <param name="path">新背景图片的地址。</param>
        /// <returns></returns>
        private void ChangeBorderBackImage(string path)
        {
            if (GetBitmapImage(path) != null)
            {
                BorderImage.ImageSource = GetBitmapImage(path);
            }
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
        /// 第一次运行时加载测试用。
        /// </summary>
        private void TestFunc()
        {
            if (isFirstTimeRunning)
            {
                if (Directory.Exists(Environment.CurrentDirectory + "\\测试歌曲文件夹"))
                {
                    string[] songs = Directory.EnumerateFiles(Environment.CurrentDirectory + "\\测试歌曲文件夹",
                        "*.mp3", SearchOption.TopDirectoryOnly).ToArray();
                    AddSong(false, songs);
                    if (MessageBox.Show(string.Format("提示：偷偷在后台加载了{0}首歌曲。开始播放？ ^o^", songs.Length), 
                        "Music Player使用指南", MessageBoxButton.YesNo, MessageBoxImage.Question)
                         == MessageBoxResult.Yes)
                    {
                        PlayerButtonClickDown(mediaControlBt);
                    }
                    else
                    {
                        nowLyricTextBlock.Text = "";
                        ShowUIElementWithAnimation(playerMainGrid, true);
                    }
                }
            }
        }
    }
}
