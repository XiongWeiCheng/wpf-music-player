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
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WPFSoundVisualizationLib;

namespace Leomon.MusicPlayer
{
    /// <summary>
    /// 与布局有关的业务逻辑，包括界面切换等等。
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 播放器按钮等以动画方式显示和消失。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="enter"></param>
        private void ShowUIElementWithAnimation(object sender, bool enter)
        {
            if (isFirstTimeRunning == false)
            {
                Grid grid = sender as Grid;
                if (grid != null)
                {
                    string str = grid.Tag.ToString();

                    if (str == "0")
                    {
                        double from = 0, to = 0;
                        double from1 = 0, to1 = 0;
                        if (enter)
                        {
                            from = 0;
                            to = 1;
                            from1 = 1;
                            to1 = 0.4;
                        }
                        else
                        {
                            from = 1;
                            to = 0;
                            from1 = 0.4;
                            to1 = 1;
                        }
                        DoubleAnimation da = new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(500));
                        DoubleAnimation da1 = new DoubleAnimation(from1, to1, TimeSpan.FromMilliseconds(500));
                        this.musicControlSP.BeginAnimation(OpacityProperty, da);
                        this.lyricSP.BeginAnimation(OpacityProperty, da1);
                    }
                    else if (str == "1")
                    {
                        double from = 0, to = 0;
                        if (enter)
                        {
                            from = 0;
                            to = 1;
                        }
                        else
                        {
                            from = 1;
                            to = 0;
                        }
                        DoubleAnimation da = new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(500));
                        this.volumeBt.BeginAnimation(OpacityProperty, da);
                        this.playmodeBt.BeginAnimation(OpacityProperty, da);
                    }
                }
            }
        }

        /// <summary>
        /// 动画翻转面板。
        /// </summary>
        /// <param name="grid"></param>
        private void FlipUIElement(Grid grid)
        {
            //翻转动作开始
            #region 翻转动画代码段
            Storyboard story = null;
            if (grid.Name == mainGrid.Name)
            {
                grid.Visibility = System.Windows.Visibility.Visible;
                //如果是主面板。
                if (grid.Opacity == 1)
                {
                    //此时应当翻转该面板并且置透明度为0
                    story = BeginStoryBoardNow(grid, 1, -1, 1, 0);
                }
                else
                {
                    story = BeginStoryBoardNow(grid, -1, 1, 0, 1);
                }
            }
            else
            {
                grid.Visibility = System.Windows.Visibility.Visible;
                //如果是歌曲播放列表。
                if (grid.Opacity == 1)
                {
                    story = BeginStoryBoardNow(grid, 1, -1, 1, 0);
                }
                else
                {
                    story = BeginStoryBoardNow(grid, -1, 1, 0, 1);
                }
            }
            story.Begin(grid);
            #endregion
        }

        private Storyboard BeginStoryBoardNow(Grid grid, int scaleXF,int scaleXT, int opacityF, int opacityT)
        {
            Storyboard story = new Storyboard();
            story.Completed += story_Completed;
            ScaleTransform scaleTransform = new ScaleTransform(1, 1);
            TransformGroup group = new TransformGroup();
            group.Children.Add(scaleTransform);
            grid.RenderTransform = group;
            DoubleAnimation daKeyF = new DoubleAnimation(scaleXF,scaleXT, TimeSpan.FromSeconds(1));
            DoubleAnimation daOp = new DoubleAnimation(opacityF, opacityT, TimeSpan.FromSeconds(1));
            Storyboard.SetTargetName(daKeyF, grid.Name);
            Storyboard.SetTargetProperty(daKeyF,
                new PropertyPath("(Grid.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetName(daOp, grid.Name);
            Storyboard.SetTargetProperty(daOp, new PropertyPath(Grid.OpacityProperty));
            story.Children.Add(daKeyF);
            story.Children.Add(daOp);
            return story;
        }

        /// <summary>
        /// 当动画结束时，改变透明度。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void story_Completed(object sender, EventArgs e)
        {
            if (mainGrid.Opacity == 0)
            {
                mainGrid.Visibility = System.Windows.Visibility.Hidden;
                playlistGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                mainGrid.Visibility = System.Windows.Visibility.Visible;
                playlistGrid.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        /// <summary>
        /// 开始计数。
        /// </summary>
        //private void StartCount()
        //{
        ////timerClock.Enabled = true;
        //    //timerClock.Elapsed += timerClock_Elapsed;
        //    //timerClock.Start();
        //}

        //void timerClock_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    this.Dispatcher.Invoke(new Action(
        //        () =>
        //        {
        //            if (tick > 0)
        //            {
        //                tick--;
        //                if (tick == 0)
        //                {
        //                    //释放定时器资源。
        //                    //进行面板的翻转。
        //                    FlipUIElement(mainGrid);
        //                    FlipUIElement(playlistGrid);
        //                    tick = count;
        //                    //停止定时器
        //                    if (timerClock != null)
        //                    {
        //                        timerClock.Enabled = false;
        //                        timerClock.Stop();
        //                        timerClock.Elapsed -= timerClock_Elapsed;
        //                    }
        //                }
        //            }
        //        }));
        //}

        /// <summary>
        /// 根据按钮名次判断媒体播放器哪个按下了。
        /// </summary>
        /// <param name="name">按钮的名称</param>
        private void PlayerButtonClickDown(Image image)
        {
            string name = image.Name;
            if (name == PlayerIconImage.Name)   //切换页面
            {
                //进行面板的翻转。
                FlipUIElement(mainGrid);
                FlipUIElement(playlistGrid);
            }
            if (name == previousSongBt.Name)    //上一曲按钮。
            {
                PreviousSong();
            }
            else if (name == mediaControlBt.Name)   //播放按钮
            {
                    if (playerState != PlayerState.Play)
                    {
                        if (PlayMusic())
                        {
                            playerState = PlayerState.Play;
                            ChangeImageSource(image, @"pictures/图标/暂停.png");
                        }
                    }
                
                    else if (playerState == PlayerState.Play)
                    {
                        ChangeImageSource(image, @"pictures/图标/播放.png");
                        PauseMusic();
                        playerState = PlayerState.Pause;
                    }
            }
            else if (name == nextSongBt.Name)   //下一曲按钮
            {
                NextSong();
            }
            else if (name == volumeBt.Name)     //喇叭标志按钮
            {
                //更换图片标志
                if (!image.Source.ToString().Contains("vmute"))
                {
                    prbValue.Clear();
                    prbValue.Push(volumeValuePrb.Value);
                    volumeValuePrb.Value = 0;
                }
                else
                {
                    //如果堆栈为空，则不使用该代码。
                    if (prbValue.Count != 0)
                    {
                        volumeValuePrb.Value = prbValue.Pop();
                    }
                }
            }
            else if (name == playmodeBt.Name)    //播放模式按钮
            {
                //更换图片
                playmodeIndex++;
                if (playmodeIndex == 5)
                {
                    playmodeIndex = 0;
                }
                ChangeImageSource(playmodeBt, string.Format(@"{0}", playmodeListsPath[playmodeIndex]));
                //更改提示
                playmodeTT.Content = string.Format("{0}",playmodeName[playmodeIndex]);
                ChangePlayMode();
            }
            else if (name == addSongsBt.Name)       //添加歌曲按钮
            {
                AddSong(true, null);
            }
            else if (name == deleteSongBt.Name) //删除按钮
            {
                RemoveSong();
            }
            else if (name == playBt.Name)       //播放控制按钮
            {
                //改变图片
                if (image.Source.ToString().Contains("播放"))
                {
                    if (playerState != PlayerState.Play)
                    {
                        if (PlayMusic())
                        {
                            playerState = PlayerState.Play;
                            ChangeImageSource(image, @"pictures/图标/暂停.png");
                        }
                    }
                }
                else
                {
                    if (playerState == PlayerState.Play)
                    {
                        if (PauseMusic())
                        {
                            playerState = PlayerState.Pause;
                            ChangeImageSource(image, @"pictures/图标/播放.png");
                        }
                    }
                }
            }
            else if (name == minimizeBt.Name)   //最小化按钮
            {
                this.WindowState = System.Windows.WindowState.Minimized;
            }
            else if (name == closeBt.Name)  //关闭按钮
            {
                //判断是否在播放幻灯片，如果是在播放，则立刻恢复Border的透明度，防止下次启动时，窗体为透明、
                if (stopPlayPPT == false)
                {
                    StopPlayPPT();
                }
                //动画关闭窗口
                Storyboard sb = FindResource("BorderClosingAnimation") as Storyboard;
                //sb.BeginAnimation()
                sb.Completed += sb_Completed;
                sb.Begin(this);

                if (dskLrc != null)
                {
                    dskLrc.Close();
                }
                SaveApplicationSettings();
                //关闭音频流
                bassPlayer.Stop();
            }
        }

        void sb_Completed(object sender, EventArgs e)
        {
            //当动画执行完成了，才可以关闭窗口。
            shouldClose = true;
            this.Close();
        }

        /// <summary>
        /// 动画更新TextBlock控件。
        /// </summary>
        /// <param name="nowLyricTextBlock"></param>
        private void UpdateNowLyricLineWithAnimation()
        {
            DoubleAnimation daOpacity = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(600));
            DoubleAnimation daFontSize1 = new DoubleAnimation(27, 21, TimeSpan.FromMilliseconds(300));
            DoubleAnimation daFontSize0 = new DoubleAnimation(21, 14, TimeSpan.FromMilliseconds(300));
            //daFontSize.AccelerationRatio = 0.8;
            //daFontSize.DecelerationRatio = 0.2;
            nowLyricTextBlock.BeginAnimation(FontSizeProperty, daFontSize1);
            preLyricTextBlock.BeginAnimation(FontSizeProperty, daFontSize0);
            nowLyricTextBlock.BeginAnimation(OpacityProperty, daOpacity);
            preLyricTextBlock.BeginAnimation(OpacityProperty, daOpacity);
            nextLyricTextBlock.BeginAnimation(OpacityProperty, daOpacity);
        }

        /// <summary>
        /// 动画效果最小化窗口
        /// </summary>
        private void MinimizeWindowWithAnimation()
        {

        }

        /// <summary>
        /// 动画效果显示窗口
        /// </summary>
        private void ShowWindowWithAnimation()
        {
            //DoubleAnimation daShowSize = new DoubleAnimation(0, 380, TimeSpan.FromMilliseconds(1000));
            //DoubleAnimation daOpcity = new DoubleAnimation(0, 0.9, TimeSpan.FromMilliseconds(1000));
            //this.BeginAnimation(HeightProperty, daShowSize);
            //this.BeginAnimation(OpacityProperty, daOpcity);
        }

        /// <summary>
        /// 在tooltip中动态显示预览时间。
        /// </summary>
        /// <param name="e"></param>
        /// <param name="prb"></param>
        private void ShowTimeOnToolTip(MouseEventArgs e, ProgressBar prb)
        {
            if (playerState != PlayerState.Stop)
            {
                //获取坐标
                Point p = e.GetPosition(prb);
                //根据坐标值改变prb的value
                double value = (p.X / prb.ActualWidth) * prb.Maximum;
                //显示toolTip
                channelSatusValueTT.Content = string.Format("时间:{0}", GetStardandTimeString(TimeSpan.FromMilliseconds(value)));
            }
        }

        /// <summary>
        /// 动画改变搜索框的透明度，来显示或者隐藏。
        /// </summary>
        /// <param name="fromValue">初始值</param>
        /// <param name="toValue">终止值</param>
        private void SetSearchTextBoxOpacity(int fromValue, int toValue)
        {
            DoubleAnimation searchTBOpacityDa = new DoubleAnimation(fromValue, toValue, TimeSpan.FromMilliseconds(500));
            searchTBOpacityDa.Completed += new EventHandler((o, e) =>
            {
                if (fromValue >= toValue)
                {
                    searchTB.Height = 0;
                }
            });
            if (fromValue >= toValue)
            {
                //表示是隐藏
                searchTB.BeginAnimation(OpacityProperty, searchTBOpacityDa);
            }
            else
            {
                //表示是显示
                searchTB.Height = 22;
                searchTB.BeginAnimation(OpacityProperty, searchTBOpacityDa);
            }
        }

        /// <summary>
        /// 欢迎使用动画。
        /// </summary>
        private void WelcomeToUse(string str)
        {

            nowLyricTextBlock.Text = str;
            DoubleAnimation daOpacity = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.5));
            DoubleAnimation daFontSize = new DoubleAnimation(30, 30, TimeSpan.FromSeconds(4));
            daFontSize.Completed += daFontSize_Completed;
            nowLyricTextBlock.BeginAnimation(OpacityProperty, daOpacity);
            nowLyricTextBlock.BeginAnimation(FontSizeProperty, daFontSize);
        }
        //表示是否需要更换文字。
        private bool changeStr = true;
        private bool stopAnimation = false;
        //更换文字信息内容索引号。
        private int strId = 0;
        /// <summary>
        /// 当动画结束时发生。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void daFontSize_Completed(object sender, EventArgs e)
        {
            if (stopAnimation == false)
            {
                //状态取反。
                changeStr = !changeStr;
                if (changeStr == false)
                {
                    strId++;
                    if (strId >= 4)
                    {
                        strId = 0;
                    }
                    //throw new NotImplementedException();
                    DoubleAnimation daOpacity = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500));
                    DoubleAnimation daFontSize = new DoubleAnimation(30, 0, TimeSpan.FromMilliseconds(500));
                    daFontSize.Completed += daFontSize_Completed;
                    nowLyricTextBlock.BeginAnimation(OpacityProperty, daOpacity);
                    nowLyricTextBlock.BeginAnimation(FontSizeProperty, daFontSize);
                }
                else
                {
                    if (strId == 1)
                    {
                        WelcomeToUse("");
                    }
                    else if (strId == 2)
                    {
                        WelcomeToUse("希望你喜欢这个小小播放器！");
                    }
                    else if (strId == 3)
                    {
                        WelcomeToUse("好了，废话说完了。请等待片刻！ ^_^");
                        stopAnimation = true;
                    }
                }
            }
            else
            {
               
                if (MessageBox.Show("如果没有看到完整的欢迎语句，可以在关闭播放器后，将音乐播放器同目录下的配置文件夹删除掉(恢复初始态)，再次运行播放器即可。^o^", "温馨提示：",
                     MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK)
                {
                    TestFunc();
                }
                nextLyricTextBlock.Text = "";
                nowLyricTextBlock.Text = "";
                preLyricTextBlock.Text = "";
                isFirstTimeRunning = false;
                //显示出来咯
                //ShowUIElementWithAnimation(playerMainGrid, true);
            }
        }

        //void ScrollTextBlock(TextBlock textBlock)
        //{
        //}
        private void ChangeLrcOffset(double offset)
        {
            SetLyricForecolor(this.nowLyricTextBlock,
                playedForecolor, 0,
                playedForecolor, offset,
                unplayedForecolor, offset,
                unplayedForecolor, offset
                );
        }

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
    }
}
