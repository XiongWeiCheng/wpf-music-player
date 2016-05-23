/*
 * Author : leomon
 * Email : 1964416932@qq.com
 * Website : null
 * Reference : null
 * */
using DoubanFM.Bass;
using Leomon.Lyric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Leomon.MusicPlayer
{
    /// <summary>
    /// 把字段和属性等的定义都方在这儿吧。。。
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 播放器状态
        /// </summary>
        private enum PlayerState
        {
            /// <summary>
            /// 停止模式
            /// </summary>
            Stop,
            /// <summary>
            /// 播放模式
            /// </summary>
            Play,
            /// <summary>
            /// 暂停模式
            /// </summary>
            Pause
        }
        /// <summary>
        /// 播放模式选择
        /// </summary>
        private enum PlayMode
        {
            /// <summary>
            /// 单曲播放
            /// </summary>
            SinglePlay,
            /// <summary>
            /// 单曲循环
            /// </summary>
            SingleCycle,
            /// <summary>
            /// 列表循环
            /// </summary>
            LoopPlay,
            /// <summary>
            /// 顺序播放
            /// </summary>
            OrderPlay,
            /// <summary>
            /// 随机播放
            /// </summary>
            RandomPlay
        }
        //歌曲列表
        private List<MySong> songList = new List<MySong>();
        //记录当前播放器的状态。
        private PlayerState playerState = PlayerState.Stop;
        //
        private PlayMode playMode = PlayMode.OrderPlay;
        //
        private BassEngine bassPlayer = null;
        //记录播放暂停位置。
        private Stack<TimeSpan> pauseTime = new Stack<TimeSpan>(1);
        //记录当前正在播放的歌曲
        private int nowplayingIndex = -1;
        //记录下一个将要播放的歌曲的索引号。
        private int nextSongIndex = 0;
        private DispatcherTimer timer = new DispatcherTimer();
        //
        private MyLyric lyric = null;
        //
        private DesktopLyric dskLrc = null;

        //正在播放歌词的行数.
        private int lyricLineIndex;
        //歌词时间间隔。
        private double timeIntervalBetween;
        //偏移量。
        private double offset;
        //需要着色的textblock行号。id=0表示第一行，id=1表示第二行桌面歌词。
        private int id;
        //标志是否需呀开启桌面歌词显示
        //private bool showDskLyric = true;

        //记录当前正在播放歌曲的名称
        private string nowPlayingSongName = string.Empty;
        //
        private List<int> randomIndexList = new List<int>();
        //
        private string borderImagePath = string.Empty;
        //
        /// <summary>
        /// 用来定时使用，主要用来等待触发翻转面板事件。
        /// </summary>
        private Timer timerClock = new Timer(1000);
        //定时时间值
        private const int count = 2;
        //定时计数值
        private int tick = count;
        //堆栈，保存声音状态
        private Stack<double> prbValue = new Stack<double>(1);
        //播放模式图片存放列表
        private string[] playmodeListsPath = { 
                                                "pictures/单曲播放.png",
                                                "pictures/单曲循环.png", 
                                                "pictures/列表循环.png",
                                                "pictures/顺序播放.png",
                                                "pictures/随机播放.png"
                                             };
        /// <summary>
        /// 播放模式名称
        /// </summary>
        private string[] playmodeName = {
                                            "单曲播放",
                                            "单曲循环",
                                            "列表循环",
                                            "顺序播放",
                                            "随机播放"
                                        };
        private int playmodeIndex = 3;

        //显示动态频谱图。
        private bool showSpectrum = true;

        //自动加载歌词？
        private bool autoLoadLyricFile = true;

        //桌面歌词未播放放的颜色
        private Color dskLrcUnplayedForecolor = Colors.DarkBlue;
        //桌面歌词已经播放的颜色
        private Color dskLrcPlayedForecolor = Colors.Orange;
        //桌面歌词字体的大小
        private int dskLrcFontSize = 40;

        private Color unplayedForecolor = Colors.Blue;
        private Color playedForecolor = Colors.DarkGreen;

        //桌面歌词字体
        private string dskLrcFontFamily = "微软雅黑";

        //
        private string dskLrcFontStyle = "Bold";

        //自动保存配置?
        private bool saveConfig = true;
        //自动保存歌曲列表？
        private bool saveSongList = true;

        //是否记忆退出的位置呢？
        private bool rememberExitPosition = true;
        //歌词文件打开的编码方式。
        private Encoding encoding = Encoding.Default;
        //歌词文件存放位置
        private string lyricFilePath = string.Empty;
        /// <summary>
        /// 表示是否需要关闭窗口。
        /// </summary>
        private bool shouldClose = false;

        /// <summary>
        /// 表示是否是第一次运行应用程序。
        /// </summary>
        private bool isFirstTimeRunning = true;

        private string userName = "哈喽，哈喽";

        private bool ClearLyric = false;
    }
}
