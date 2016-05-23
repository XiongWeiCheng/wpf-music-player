/*
 * Author : leomon
 * Email : 1964416932@qq.com
 * Website : null
 * Reference : null
 * 日志：2014-6-4 增加了快速删除歌曲功能，增加了快进快捷键为左右方向键，上下歌曲切换为pageDown,pageUp。
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Un4seen.Bass;
using Un4seen.BassAsio;
using Un4seen.Bass.AddOn;
using Un4seen.Bass.Misc;
using Leomon.Lyric;
using Leomon.SPLibrary;
using DoubanFM.Bass;
using System.Windows.Threading;
using Un4seen.Bass.AddOn.Tags;
using System.Windows.Media.Animation;
using System.IO;
using System.Windows.Media.Imaging;

namespace Leomon.MusicPlayer
{
    /// <summary>
    /// 播放器有关的业务逻辑
    /// </summary>
    public partial class MainWindow : Window
    {        
        /// <summary>
        /// 播放器初始化调用的方法。
        /// </summary>
        private void InitializeMyPlayer()
        {
            try
            {
                DoubanFM.Bass.BassEngine.ExplicitInitialize(null);
                //_player.Settings.Device = Bass.BassEngine.Instance.Device;
                //BassEngine.Instance.SetDownloadRateRestriction(_player.Settings.EnableDownloadRateRestriction);
                bassPlayer = BassEngine.Instance;
                bassPlayer.Volume = volumeValuePrb.Value / 100.0;
                bassPlayer.TrackEnded += bassPlayer_TrackEnded;
                timer.Interval = TimeSpan.FromMilliseconds(50);
                timer.Tick += timer_Tick;
            }
            catch (DoubanFM.Bass.BassInitializationFailureException ex)
            {
                MessageBox.Show(ex.Message, string.Empty, MessageBoxButton.OK, MessageBoxImage.Error);
                App.Current.Shutdown(0);
            }
        }

        void dskLrc_PlayerButtonClick(object sender, EventArgs e)
        {
            string str = sender as string;
            if (str != null)
            {
                if (str == "播放")
                {
                    if (playerState == PlayerState.Stop || playerState == PlayerState.Pause)
                    {
                        PlayerButtonClickDown(mediaControlBt);
                    }
                }
                else if (str == "暂停")
                {
                    if (playerState == PlayerState.Play)
                    {
                        PlayerButtonClickDown(mediaControlBt);
                    }
                }
                else if (str == "上一曲")
                {
                    PlayerButtonClickDown(previousSongBt);
                }
                else if (str == "下一曲")
                {
                    PlayerButtonClickDown(nextSongBt);
                }
                else if (str == "关闭")
                {
                    MenuItem_Click(showDesktopLyricItem, null);
                }
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            //更新播放的位置
            this.channelPositonTB.Text = GetStardandTimeString(bassPlayer.ChannelPosition);
            this.channelStatusPrb.Value = bassPlayer.ChannelPosition.TotalMilliseconds;
            UpdateLyric();
        }
        private double startTime = 0;
        private double endTime = 0;
        private bool enalbeAnimation = true;
        private void UpdateLyric()
        {
            //如果歌词文件存在则更新歌词文件
            if (lyric != null)
            {
                //获取到当前已经播放到的位置,单位ms.
                double nowPositon = bassPlayer.ChannelPosition.TotalMilliseconds;
                //给已经唱过的歌词着色
                offset = (nowPositon - startTime) / timeIntervalBetween;
                if (offset >= 1.0)
                {
                    offset = 1.0;
                }
                if (offset < 0)
                {
                    offset = 0;
                }
                if (dskLrc != null && dskLrc.IsVisible)
                     dskLrc.ChangeStopOffset(id, offset);
                ChangeLrcOffset(offset);
                //根据时间确定要显示的行号。
                if (nowPositon >= startTime && nowPositon < endTime)
                {
                    lyricLineIndex = lyricLineIndex + 0;
                }
                else if (nowPositon > endTime && lyricLineIndex != lyric.LyricTimeLinesDValue.Count - 1)
                {
                    lyricLineIndex = lyricLineIndex + 1;
                    ChangeLrcTimeInterval();
                    //更新歌词文本信息
                    if (lyricLineIndex + 1 != lyric.LyricTextLines.Count)
                    {
                        preLyricTextBlock.Text = lyric.LyricTextLines[lyricLineIndex - 1];
                        nowLyricTextBlock.Text = lyric.LyricTextLines[lyricLineIndex];
                        nextLyricTextBlock.Text = lyric.LyricTextLines[lyricLineIndex + 1];
                    }
                    else
                    {
                        preLyricTextBlock.Text = lyric.LyricTextLines[lyricLineIndex - 1];
                        nowLyricTextBlock.Text = lyric.LyricTextLines[lyricLineIndex];
                        nextLyricTextBlock.Text = "The End ~ ~ ~";
                    }
                    if (enalbeAnimation)
                    {
                        UpdateNowLyricLineWithAnimation();
                    }
                    //this.lyricTestTB.Text = string.Format("{0}", lyric.LyricTextLines[lyricLineIndex]);
                    //桌面歌词，当lyricIndex是偶数时会改变
                    if (lyricLineIndex % 2 == 0)
                    {
                        if (dskLrc != null && dskLrc.IsVisible)
                        {
                            dskLrc.ResetLyricForecolor();
                            //表示显示第一行
                            id = 0;
                            //
                            dskLrc.ChangeLyricText(nowLyricTextBlock.Text, nextLyricTextBlock.Text);
                            //
                            if (enalbeAnimation)
                            {
                                dskLrc.ShowDesktopLyricWithAnimation(0, 1);
                            }
                        }
                    }
                    else
                    {
                        //表示着色第二行
                        id = 1;
                        if (dskLrc != null && dskLrc.IsVisible)
                        {
                            //
                            dskLrc.ChangeLyricText(nextLyricTextBlock.Text, nowLyricTextBlock.Text);
                            //
                            if (enalbeAnimation)
                            {
                                dskLrc.ShowDesktopLyricWithAnimation(1, 0);
                            }
                            //
                            dskLrc.ChangeStopOffset(0, 0);
                        }
                    }
                    if (dskLrc != null && dskLrc.IsVisible)
                             dskLrc.ChangeStopOffset(id, 0);
                }
            }

        }

        private void ChangeLrcTimeInterval()
        {
            //获取当前歌词的函数和起始时间
            startTime = lyric.LyricTimeLinesDValue[lyricLineIndex];
            //获取歌词的结束时间
            endTime = 0d;
            //判断歌词的索引有没有越界，若有则另行处理。这是index是歌词最后行号。
            if (lyricLineIndex != lyric.LyricTimeLinesDValue.Count - 1)
            {
                endTime = lyric.LyricTimeLinesDValue[lyricLineIndex + 1];
                //计算timeInterval
                timeIntervalBetween = endTime - startTime;
                //
                if (timeIntervalBetween >= 1000)
                {
                    timeIntervalBetween -= 100;
                }
            }
            else
            {
                timeIntervalBetween = 6666;
            }
        }

        /// <summary>
        /// 当播放结束，引发。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bassPlayer_TrackEnded(object sender, EventArgs e)
        {
            #region 复位控件
            ResetValue();
            #endregion
            NextSong();
        }

        private void ResetValue()
        {
            //停止定时器
            timer.IsEnabled = false;
            timer.Stop();
            //置0状态标志
            this.channelStatusPrb.Value = 0;
            this.channelPositonTB.Text = GetStardandTimeString(TimeSpan.FromMilliseconds(0));
            this.channelLengthTB.Text = GetStardandTimeString(TimeSpan.FromMilliseconds(0));
            this.nowPlayingSong.Text = "正在播放：无";
            playerState = PlayerState.Stop;
            ChangeImageSource(mediaControlBt, @"pictures/图标/播放.png");
            if (bassPlayer.IsPlaying)
            {
                bassPlayer.Stop();
            }
            if (songList.Count != 0)
            {
                songListLB.SelectedIndex = nowplayingIndex;
            }
            if (lyric != null)
            {
                //释放掉。
                lyric = null;
            }
            if (dskLrc != null)
            {
                dskLrc.ResetLyricForecolor();
                dskLrc.ChangeLyricText("", "");
            }
            //
            nowLyricTextBlock.Text = "";
            preLyricTextBlock.Text = "";
            nextLyricTextBlock.Text = "";
            this.titleTB.Text = "歌曲列表(共0首)";
        }
        /// <summary>
        /// 添加歌曲按钮
        /// </summary>
        /// <param name="openFileDialog">是否打开选择文件对话框</param>
        /// <param name="fileNames">文件路径列表，可为null</param>
        private void AddSong(bool openFileDialog, string[] fileNames)
        {
            if (openFileDialog == true)
            {
                Microsoft.Win32.OpenFileDialog opf = new Microsoft.Win32.OpenFileDialog();
                opf.Filter = "支持的音频文件|*.mp3;*.mp1;*.mp2;*.mpa;*.mp3pro;*.wav;*.cda;*.cue;*.m4a;*.mp4;*.aac;*.aa;*.ac3;*.wma;*.wmv|所有文件|*.*";
                opf.Multiselect = true;
                opf.FileName = "";
                opf.Title = "选择音频文件...";
                if (opf.ShowDialog() == true)
                {
                    fileNames = opf.FileNames;
                }
            }
            if (fileNames != null)
            {
                //向列表中添加歌曲文件
                foreach (var str in fileNames)
                {
                    bool exists = false;
                    MySong mSong = new MySong(str);
                    for (int i = 0; i < songList.Count; i++)
                    {
                        if (mSong.SongName == songList[i].SongName)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        ListBoxItem item = new ListBoxItem();
                        //item.Foreground = new SolidColorBrush(Colors.White);
                        //item.FontSize = 15;
                        item.Content = mSong.SongName;
                        item.Style = (Style)FindResource("ListBoxItemStyle");
                        //item.Style = null;
                        songList.Add(mSong);
                        item.MouseDoubleClick += item_MouseDoubleClick;
                        songListLB.Items.Add(item);
                    }
                    mSong = null;
                }
            }
            this.titleTB.Text = string.Format("歌曲列表(共{0}首)", songListLB.Items.Count);
        }
        /// <summary>
        /// 添加歌曲的文件夹。
        /// </summary>
        private void AddSongDirectory()
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.Description = "选择歌曲文件夹...";
            fbd.ShowNewFolderButton = true;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.Dispatcher.Invoke(new Action(() =>
                    {
                        string[] files = null;
                        //获取所有mp3文件。
                        files = 
                            Directory.EnumerateFiles(fbd.SelectedPath, "*.mp3", SearchOption.AllDirectories).ToArray();
                        AddSong(false, files);
                    }));
            }
        }

        void item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = songListLB.SelectedIndex;
            //准备更改正在播放的歌曲文件
            //如果此时歌曲正在播放，停止该播放流，并切换。
            ResetValue();
            songListLB.SelectedIndex = index;
            PlayerButtonClickDown(mediaControlBt);
        }

        /// <summary>
        /// 移除列表中选中歌曲
        /// </summary>
        private void RemoveSong()
        {
            //判断是否有歌曲被选择，如果选择了，但是是正在播放的，则询问是否移除，否则正常移除
            if (songListLB.Items.Count != 0)
            {
                if (songListLB.SelectedIndex != -1)
                {
                    int selectedIndex = songListLB.SelectedIndex;
                    if (songListLB.SelectedItem.ToString().Contains(nowPlayingSongName))
                    {
                        MessageBoxResult rslt =
                            MessageBox.Show("歌曲正在播放，是否移除？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (rslt == MessageBoxResult.Yes)
                        {
                            //移除保存歌曲的列表歌曲
                            this.songList.RemoveAt(selectedIndex);
                            //播放下一曲
                            nowplayingIndex = nowplayingIndex - 1;
                            NextSong();
                        }
                    }
                    else
                    {

                        //移除列表歌曲
                        this.songListLB.Items.RemoveAt(selectedIndex);
                        //移除保存歌曲的列表歌曲
                        this.songList.RemoveAt(selectedIndex);
                    }

                }
                else
                {
                    //提示用户选择要删除的歌曲，目前一次只能删除一首歌曲，没办法啊。。。。
                    MessageBox.Show("请选择要移除的歌曲。");
                }
            }
        }

        /// <summary>
        /// 删除列表中选中歌曲
        /// </summary>
        private void DeleteSong()
        {
            //判断是否有歌曲被选择，如果选择了，但是是正在播放的，则询问是否移除，否则正常移除
            if (songListLB.Items.Count != 0)
            {
                if (songListLB.SelectedIndex != -1)
                {
                    int selectedIndex = songListLB.SelectedIndex;
                    string songPath = songList.ElementAt(selectedIndex).FilePath;
                    //MessageBox.Show(songPath);
                    if (songListLB.SelectedItem.ToString().Contains(nowPlayingSongName))
                    {
                        {
                            try
                            {
                                //移除列表歌曲
                                this.songListLB.Items.RemoveAt(selectedIndex);
                                //移除保存歌曲的列表歌曲
                                this.songList.RemoveAt(selectedIndex);
                                //播放下一曲
                                nowplayingIndex = nowplayingIndex - 1;
                                NextSong();
                                File.Delete(songPath);
                            }
                            catch (Exception ee) { MessageBox.Show(ee.Message); }
                        }
                    }
                    else
                    {
                        try
                        {
                            File.Delete(songPath);
                            //移除列表歌曲
                            this.songListLB.Items.RemoveAt(selectedIndex);
                            //移除保存歌曲的列表歌曲
                            this.songList.RemoveAt(selectedIndex);
                        }
                        catch (Exception ee) { MessageBox.Show(ee.Message); }       
                    }

                }
                else
                {
                    //提示用户选择要删除的歌曲，目前一次只能删除一首歌曲，没办法啊。。。。
                    MessageBox.Show("请选择要删除的歌曲。");
                }
            }
        }

        /// <summary>
        /// 上一首
        /// </summary>
        private void PreviousSong()
        {
            ChangeSong(1);
        }

        /// <summary>
        /// 下一首
        /// </summary>
        private void NextSong()
        {
            ChangeSong(0);
        }

        private void ChangeSong(int flag)
        {
            //取得当前的播放状态。
            playMode = GetPlayMode(playmodeIndex);
            if (playMode == PlayMode.SinglePlay)    //单曲播放
            {
                //停止播放.
                ResetValue();
            }
            else if (playMode == PlayMode.SingleCycle)  //单曲循环
            {
                ResetValue();
                //继续播放这首歌
                PlayerButtonClickDown(mediaControlBt);
            }
            else if (playMode == PlayMode.LoopPlay) //列表循环
            {
                nextSongIndex = nowplayingIndex;
                if (flag == 0)
                {
                    nextSongIndex++;
                    if (nextSongIndex == songList.Count)
                    {
                        nextSongIndex = 0;
                        nowplayingIndex = nextSongIndex;
                    }
                    ResetValue();
                    songListLB.SelectedIndex = nextSongIndex;
                    PlayerButtonClickDown(mediaControlBt);
                }
                else
                {
                    nextSongIndex--;
                    if (nextSongIndex == -1)
                    {
                        nextSongIndex = songList.Count;
                        NextSong();
                    }
                    ResetValue();
                    songListLB.SelectedIndex = nextSongIndex;
                    PlayerButtonClickDown(mediaControlBt);
                }

            }
            else if (playMode == PlayMode.OrderPlay)    //顺序播放
            {
                nextSongIndex = nowplayingIndex;
                if (flag == 0)
                {
                    nextSongIndex++;
                    if (nextSongIndex == songList.Count)
                    {
                        nextSongIndex = 0;
                        nowplayingIndex = nextSongIndex;
                        //停止播放。
                        bassPlayer.Stop();
                        ResetValue();
                    }
                    else
                    {
                        ResetValue();
                        songListLB.SelectedIndex = nextSongIndex;
                        PlayerButtonClickDown(mediaControlBt);
                    }
                }
                else
                {
                    nextSongIndex--;
                    if (nextSongIndex == -1)
                    {
                        nextSongIndex = songList.Count;
                        //停止播放。
                        bassPlayer.Stop();
                        ResetValue();
                    }
                    else
                    {
                        ResetValue();
                        songListLB.SelectedIndex = nextSongIndex;
                        PlayerButtonClickDown(mediaControlBt);
                    }
                }
            }
            else if (playMode == PlayMode.RandomPlay)   //随机播放
            {
                Random r = new Random();
                nextSongIndex = r.Next(songList.Count);
                //MessageBox.Show(nextSongIndex.ToString());
                nowplayingIndex = nextSongIndex;
                ResetValue();
                songListLB.SelectedIndex = nextSongIndex;
                PlayerButtonClickDown(mediaControlBt);
            }
        }

        /// <summary>
        /// 播放列表中选中的歌曲
        /// </summary>
        private bool PlayMusic()
        {
            bool ok = true;
            if (songList.Count == 0)
            {
                playerState = PlayerState.Stop;
            }
            if (playerState == PlayerState.Stop)
            {
                //判断是否加载了歌曲
                if (songList.Count == 0)
                {
                    MessageBoxResult result =
                        MessageBox.Show("歌曲列表为空，是否添加？", "歌曲列表", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        AddSong(true, null);
                        if (songList.Count == 0)
                        {
                            ok = false;
                        }
                    }
                    else
                    {
                        ok = false;
                    }
                }
                if (songList.Count != 0)
                {
                    //判断是否选择了歌曲，如果没有选择，则自动选择第一首
                    if (songListLB.SelectedIndex < 0)
                    {
                        songListLB.SelectedIndex = 0;
                        nowplayingIndex = 0;
                    }
                    else
                    {
                        nowplayingIndex = songListLB.SelectedIndex;
                    }
                    if (File.Exists(songList[nowplayingIndex].FilePath))
                    {
                        //打开歌曲文件
                        bassPlayer.OpenFile(songList[nowplayingIndex].FilePath);
                        //专辑图片
                        myAlbumArtDisplay.AlbumArtImage = GetSongAlbumImage(songList[nowplayingIndex].FilePath);
                        if (autoLoadLyricFile)
                        {
                            //判断是否存在同名的歌词文件，仅支持lrc格式歌词。
                            LoadLyric(songList[nowplayingIndex]);
                        }
                        //设置时间。
                        this.channelStatusPrb.Maximum = bassPlayer.ChannelLength.TotalMilliseconds;
                        //设置标签时间
                        channelLengthTB.Text = GetStardandTimeString(bassPlayer.ChannelLength);
                    }
                    else
                    {
                        ok = false;
                        songListLB.Items.RemoveAt(nowplayingIndex);
                        songList.RemoveAt(nowplayingIndex);
                        nowplayingIndex = nowplayingIndex - 1;
                        NextSong();
                        MessageBox.Show("文件不存在！自动从列表中移除。");
                    }
                }
            }
            if (playerState == PlayerState.Pause)
            {
                //先将暂停的位置还原回来，再播放
                bassPlayer.ChannelPosition = pauseTime.Pop();
            }
            //播放歌曲。
            if (ok == true)
            {
                timer.Start();
                timer.IsEnabled = true;
                bassPlayer.Play();
                songListLB.ScrollIntoView(songListLB.SelectedItem);
                this.nowPlayingSong.Text = string.Format("{0}", songList[nowplayingIndex].SongName);
                nowPlayingSongName = songList[nowplayingIndex].SongName;
                mediaControlTT.Content = "暂停播放";
                songListLB_SelectionChanged(null, null);
            }
            return ok;
        }

        private ImageSource GetSongAlbumImage(string p)
        {
            BitmapImage bitmapImage = new BitmapImage();
            try
            {
                TagLib.File file = TagLib.File.Create(p);
                if (file.Tag.Pictures.Length > 0)
                {
                    byte[] bin = (byte[])(file.Tag.Pictures[0].Data.Data);
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = new MemoryStream(bin);
                    bitmapImage.EndInit();
                }
                else
                {
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(@"pictures/Music.png", UriKind.Relative);
                    bitmapImage.EndInit();
                }
            }
            catch
            {
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(@"pictures/Music.png", UriKind.Relative);
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }
        /// <summary>
        /// 是否存在歌词文件
        /// </summary>
        private bool hasLyric = false;
       
        /// <summary>
        /// 判断同名歌词文件是否存在，若存在则自动加载。
        /// </summary>
        /// <param name="mySong"></param>
        private void LoadLyric(MySong mySong)
        {
            hasLyric = false;
            string path = mySong.FilePath;
            path = path.Replace("mp3", "lrc");
            //尝试在该目录下加载同名歌词文件
            string dir = path.Substring(0, path.LastIndexOf('\\'));
            string[] fileNames = System.IO.Directory.EnumerateFiles(dir, "*.lrc",
                System.IO.SearchOption.TopDirectoryOnly).ToArray();
            foreach (var item in fileNames)
            {
                if (item.Contains(mySong.SongName))
                {
                    hasLyric = true;
                    InitializeMyLyric(item, "");
                    this.lyricFilePath = item;
                    break;
                }
            }
            if (hasLyric == false)
            {
                if (showDesktopLyricItem.IsChecked == true && dskLrc != null && 
                    dskLrc.IsVisible)
                {
                    dskLrc.ChangeLyricText("没有找到歌词......", "No lyric file exists...");
                    dskLrc.ShowDesktopLyricWithAnimation(1, 1);
                    dskLrc.Show();
                }
                lyricFilePath = string.Empty;
            }
        }
        /// <summary>
        /// 初始化歌词
        /// </summary>
        /// <param name="path">歌词所在的路径</param>
        /// <param name="lrcStr">歌词文件</param>
        private void InitializeMyLyric(string path, string lrcStr)
        {
            if (path != "" || lrcStr != "")
            {
                try
                {
                    timer.IsEnabled = false;
                    timer.Stop();
                    lyricFilePath = path;
                    if (lyric == null)
                    {
                        if (path != "" && lrcStr == "")
                        {
                            lyric = new MyLyric(path, encoding);
                        }
                        else if (path == "" && lrcStr != "")
                        {
                            lyric = new MyLyric(lrcStr);
                        }
                    }
                    //当可以显示歌词时则显示桌面歌词。
                    if (dskLrc == null)
                    {
                        dskLrc = new DesktopLyric(dskLrcFontFamily, dskLrcFontSize, dskLrcPlayedForecolor, dskLrcUnplayedForecolor);
                        
                        dskLrc.PlayerButtonClick += dskLrc_PlayerButtonClick;
                    }
                    //if (hasLyric == false)
                    //{
                    //    dskLrc.ChangeLyricText("没有找到歌词......", "No lyric file exists...");
                    //}
                    id = 0;
                    offset = 0;
                    lyricLineIndex = 0;
                    ChangeLrcTimeInterval();
                    if (bassPlayer.ChannelPosition.TotalMilliseconds != 0)
                    {
                        GetLyricIndex(bassPlayer.ChannelPosition.TotalMilliseconds);
                    }
                    dskLrc.Visibility = System.Windows.Visibility.Visible;
                    if (showDesktopLyricItem.IsChecked == false)
                    {
                        dskLrc.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    //初始化歌词显示。
                    dskLrc.ResetLyricForecolor();
                    dskLrc.ChangeLyricText(lyric.LyricTextLines[lyricLineIndex], lyric.LyricTextLines[lyricLineIndex + 1]);
                    //lyricTestTB.Text = lyric.LyricTextLines[lyricLineIndex];
                    //播放器中的歌词初始化显示。
                    nowLyricTextBlock.Text = lyric.LyricTextLines[lyricLineIndex];
                    nextLyricTextBlock.Text = lyric.LyricTextLines[lyricLineIndex + 1];
                    dskLrc.Show();
                    dskLrc.ShowDesktopLyricWithAnimation(1, 1);
                    dskLrc.ChangeStopOffset(0, 0);
                    dskLrc.ChangeStopOffset(1, 0);
                    timer.Start();
                    timer.IsEnabled = true;
                }
                catch
                {
                    timer.Start();
                    timer.IsEnabled = true;
                    lyric = null;
                    if (dskLrc != null)
                    {
                        dskLrc.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
            }
        }
        /// <summary>
        /// 获取歌曲的tag信息，如专辑，歌手等等。
        /// </summary>
        /// <param name="filePath">歌曲的路径</param>
        private void GetSongTagInformation(string filePath)
        {
            TAG_INFO tagInfo = new TAG_INFO(filePath);
            string str = tagInfo.album + Environment.NewLine +
                tagInfo.artist + Environment.NewLine +
                tagInfo.disc;
            MessageBox.Show(str);
        }

        /// <summary>
        /// 将时间表示成00:00的格式。
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        private string GetStardandTimeString(TimeSpan timeSpan)
        {
            string temp = string.Empty;
            temp = string.Format("{0}:{1}", timeSpan.Minutes.ToString("00"),
                timeSpan.Seconds.ToString("00"));
            return temp;
        }

        /// <summary>
        /// 暂停选中歌曲
        /// </summary>
        private bool PauseMusic()
        {
            bool ok = false;
            if (playerState == PlayerState.Play)
            {
                //保存此时的时间。
                pauseTime.Clear();
                pauseTime.Push(bassPlayer.ChannelPosition);
                //暂停
                bassPlayer.Pause();
                ok = true;
                mediaControlTT.Content = "播放歌曲";
            }
            return ok;
        }

        private void StopMusic()
        {
            ResetValue();
        }

        /// <summary>
        /// 更换播放模式。
        /// </summary>
        private void ChangePlayMode()
        {
            PlayMode pm = GetPlayMode(playmodeIndex);
            if (pm == PlayMode.RandomPlay)
            {
                if (songList.Count != 0)
                {
                    Random r = new Random();
                    for (int i = 0; i < songList.Count; i++)
                    {
                        randomIndexList.Add(r.Next(songList.Count));
                    }
                    string str = string.Empty;
                    for (int i = 0; i < randomIndexList.Count; i++)
                    {
                        str = str + randomIndexList[i].ToString() + Environment.NewLine;
                    }
                    //MessageBox.Show(str);
                }
            }
        }

        /// <summary>
        /// 改变播放器的音量。
        /// </summary>
        /// <param name="prb"></param>
        /// <param name="e"></param>
        private void ChangeVolumeValue(ProgressBar prb, MouseButtonEventArgs e)
        {
            //获取坐标
            Point p = e.GetPosition(prb);
            //根据坐标值改变prb的value
            prb.Value = (p.X / prb.ActualWidth) * prb.Maximum;
        }
        /// <summary>
        /// 改变文件播放位置。
        /// </summary>
        /// <param name="prb"></param>
        /// <param name="e"></param>
        private void ChangeChannelPosition(ProgressBar prb, double value)
        {
            if (playerState != PlayerState.Stop)
            {
                prb.Value = value; 
                //将value值转换为channel的position
                TimeSpan t = TimeSpan.FromMilliseconds(prb.Value);
                bassPlayer.ChannelPosition = t;
                if (playerState == PlayerState.Pause)
                {
                    bassPlayer.Play();
                    playerState = PlayerState.Play;
                    //更换播放图标
                    ChangeImageSource(mediaControlBt, @"pictures\图标\暂停.png");
                }
                channelPositonTB.Text = GetStardandTimeString(t);
                GetLyricIndex(prb.Value);
            }
        }

        /// <summary>
        /// 获取播放状态。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private PlayMode GetPlayMode(int index)
        {
            PlayMode p = PlayMode.OrderPlay;
            switch (index)
            {
                case 0:
                    p = PlayMode.SinglePlay;
                    break;
                case 1:
                    p = PlayMode.SingleCycle;
                    break;
                case 2:
                    p = PlayMode.LoopPlay;
                    break;
                case 3:
                    p = PlayMode.OrderPlay;
                    break;
                case 4:
                    p = PlayMode.RandomPlay;
                    break;
                default:
                    break;
            }
            return p;
        }

        /// <summary>
        /// 根据时间跳转到指定的歌词处
        /// </summary>
        /// <param name="time"></param>
        private void GetLyricIndex(double time)
        {
            //MessageBox.Show(time.ToString());
           
            if (lyric != null)
            { 
                //暂停时间
                timer.Stop();
                if (time <= lyric.LyricTimeLinesDValue[0])
                {
                    lyricLineIndex = 0;
                }
                else if (time > lyric.LyricTimeLinesDValue[lyric.LyricTimeLinesDValue.Count - 1])
                {
                    lyricLineIndex = lyric.LyricTimeLinesDValue.Count - 1;
                }
                else
                {
                    for (int i = 1; i < lyric.LyricTimeLinesDValue.Count; i++)
                    {
                        if (time < lyric.LyricTimeLinesDValue[i + 1] && time >= lyric.LyricTimeLinesDValue[i])
                        {
                            lyricLineIndex = i;
                            break;
                        }
                    }
                }
                //MessageBox.Show(lyricLineIndex.ToString());
                if (lyricLineIndex != 0 && lyricLineIndex < lyric.LyricTimeLines.Count - 1)
                {
                    preLyricTextBlock.Text = lyric.LyricTextLines[lyricLineIndex - 1];
                    nowLyricTextBlock.Text = lyric.LyricTextLines[lyricLineIndex];
                    nextLyricTextBlock.Text = lyric.LyricTextLines[lyricLineIndex + 1];
                    UpdateNowLyricLineWithAnimation();
                }
                else if (lyricLineIndex == 0)
                {
                    nowLyricTextBlock.Text = lyric.LyricTimeLines[0];
                    nextLyricTextBlock.Text = lyric.LyricTimeLines[1];
                    UpdateNowLyricLineWithAnimation();
                }
                else if (lyricLineIndex == lyric.LyricTimeLines.Count - 1)
                {
                    preLyricTextBlock.Text = lyric.LyricTextLines[lyricLineIndex - 1];
                    nowLyricTextBlock.Text = lyric.LyricTextLines[lyricLineIndex];
                    nextLyricTextBlock.Text = "The End ~ ~ ~";
                    UpdateNowLyricLineWithAnimation();
                }
                if(dskLrc != null && dskLrc.IsVisible)
                {
                    if (id == 1)
                    {
                        dskLrc.ChangeLyricText(nowLyricTextBlock.Text, nextLyricTextBlock.Text);
                        id = 0;
                    }
                    if (id == 0)
                    {
                        dskLrc.ChangeLyricText(nextLyricTextBlock.Text, nowLyricTextBlock.Text);
                        id = 1;
                    }
                    dskLrc.ShowDesktopLyricWithAnimation(1, 1);
                }
                ChangeLrcTimeInterval();
                timer.Start();
            }
            
            //MessageBox.Show(lyricLineIndex.ToString());
        }
    }
}
