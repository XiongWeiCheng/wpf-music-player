using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Leomon.LyricDownloader;
using System.Xml;
using System.Net;
using System.Threading;

namespace Leomon.MusicPlayer
{
    /// <summary>
    /// DowloadLrcFrm.xaml 的交互逻辑
    /// </summary>
    public partial class DowloadLrcFrm : Window
    {
        //字段：待解析的歌曲文件名称。
        private string songString = "";
        ////字段：解析到的歌名
        //private string songName = "";
        ////字段：解析到的歌手
        //private string singer = "";

        class LyricInfo
        {
            private string id = "";

            public string Id
            {
                get { return id; }
                set { id = value; }
            }
            private string artist = "";

            public string Artist
            {
                get { return artist; }
                set { artist = value; }
            }
            private string title = "";

            public string Title
            {
                get { return title; }
                set { title = value; }
            }
            XmlNode node = null;

            public XmlNode Node
            {
                get { return node; }
                set { node = value; }
            }
            public LyricInfo(XmlNode node)
            {
                this.node = node;
                 id = node.Attributes["id"].Value;
                 artist = node.Attributes["artist"].Value;
                 title = node.Attributes["title"].Value;
            }
            public override string ToString()
            {
                int i = int.Parse(id);
                return string.Format("{0}       {1}                 {2}", i.ToString("00000000"), artist, title);
            }
        }
        /// <summary>
        /// 待下载的歌词列表。
        /// </summary>
        private List<LyricInfo> LyricList = new List<LyricInfo>();

        public DowloadLrcFrm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">歌曲文件名</param>
        public DowloadLrcFrm(string str)
        {
            InitializeComponent();
            this.songString = str;
            analyzeModeComboBox.SelectedIndex = 0;
            //GetSystemInternetState();
        }
     

        private void analyzeModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb != null)
            {
                AnalyzeSongString(songString, cb.SelectedIndex);
            }
        }
        /// <summary>
        /// 解析歌曲文件名称，得到歌手和歌曲名称。
        /// </summary>
        /// <param name="str">待解析的歌曲文件名</param>
        /// <param name="mode">解析模式</param>
        private void AnalyzeSongString(string str, int mode)
        {
            if (!str.Contains("-"))
            {
                mode = 2;
            }
            if (str != "")
            {
                switch (mode)
                {
                    case 0:             //歌名--歌手
                        string[] strTemp = str.Split('-');
                        songNameTB.Text = strTemp[0];
                        singerTB.Text = strTemp[1];
                        break;
                    case 1:             //歌手--歌名
                        string[] strTemp2 = str.Split('-');
                        songNameTB.Text = strTemp2[1];
                        singerTB.Text = strTemp2[0];
                        break;
                    case 2:             //其他
                        songNameTB.Text = str;
                        singerTB.Text = "";
                        break;
                    default:
                        songNameTB.Text = "";
                        singerTB.Text = "";
                        break;
                }
            }
            else
            {
                //MessageBox.Show("歌曲文件名格式不正确，无法解析！");
            }
        }
        /// <summary>
        /// 获取系统是否联网了。
        /// </summary>
        /// <returns></returns>
        private bool GetSystemInternetState()
        {
            bool success = true;
            this.Dispatcher.Invoke(new Action(() =>
                {
                    Ping ping = new Ping();
                    try
                    {
                        PingReply rp = ping.Send("119.75.218.45");
                        if (rp != null)
                        {
                            if (rp.Status == IPStatus.Success)
                            {
                                if (matchedLrcListBox.Items.Count != 0)
                                {
                                    matchedLrcListBox.Items.Clear();
                                }
                            }
                            else
                            {
                                if (matchedLrcListBox.Items.Count != 0)
                                {
                                    matchedLrcListBox.Items.Clear();
                                }
                                //AddItemToListBox(this.matchedLrcListBox, "网络未连接，无法下载！", false);
                                this.progressBar.Visibility = System.Windows.Visibility.Collapsed;
                                success = false;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        if (matchedLrcListBox.Items.Count != 0)
                        {
                            matchedLrcListBox.Items.Clear();
                        }
                        //AddItemToListBox(this.matchedLrcListBox, "网络未连接，无法下载！", false);
                        this.progressBar.Visibility = System.Windows.Visibility.Collapsed;
                        success = false;
                    }
                }));
            return success;
        }

        private MyLyricDownLoader lrcDld;
        private XmlDocument xml;
        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            //this.progressBar.Visibility = System.Windows.Visibility.Visible;
            LyricList.Clear();
            if (matchedLrcListBox.Items.Count != 0)
            {
                matchedLrcListBox.Items.Clear();
            }
            //开启新的进程搜索
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            SearchLrcOnInternet();
        }

        private void SearchLrcOnInternet()
        {
            if (true)
            {
                lrcDld = new MyLyricDownLoader(false);
                lrcDld.SelectSong += lrcDld_SelectSong;
                lrcDld.WebException += lrcDld_WebException;
                this.Dispatcher.Invoke(new Action(() =>
                    {
                        xml = lrcDld.SearchLyric(singerTB.Text, songNameTB.Text);
                    }));
            }
        }

        void lrcDld_WebException(object sender, EventArgs e)
        {
            WebException ex = sender as WebException;
            AddItemToListBox(this.matchedLrcListBox, "网络未连接。", new SolidColorBrush(Colors.Red), false);
            MessageBox.Show("呀！！" + ex.Message + "\n可能是网络已经断开连接。");
        }

        void lrcDld_SelectSong(object sender, EventArgs e)
        {
            XmlNodeList list = sender as XmlNodeList;
            AddItemToListBox(this.matchedLrcListBox, "id                    歌手                  歌名", new SolidColorBrush(Colors.DarkGreen), false);
            if (list != null)
            {
                foreach (XmlNode node in list)
                {
                    LyricInfo l = new LyricInfo(node);
                    LyricList.Add(l);
                    AddItemToListBox(this.matchedLrcListBox, l.ToString(), new SolidColorBrush(Colors.Blue), true);
                }
                if (matchedLrcListBox.Items.Count == 1)
                {
                    AddItemToListBox(this.matchedLrcListBox, "没有搜索到歌词", new SolidColorBrush(Colors.Red), false);
                }
            }
        }

        public event EventHandler GetLyricFile;

        protected void OnGetLyricFile(string lrcStr)
        {
            if (GetLyricFile != null)
            {
                this.GetLyricFile(lrcStr, new EventArgs());
            }
        }

        /// <summary>
        /// 向ListBox中添加项目
        /// </summary>
        /// <param name="lb">listBox对象</param>
        /// <param name="itemStr">内容</param>
        /// <param name="addMouseClikEentHandler">是否注册鼠标双击事件</param>
        /// <param name="foreground">前景色</param>
        private void AddItemToListBox(ListBox lb, string itemStr, SolidColorBrush foreground, bool addMouseClikEentHandler)
        {
            ListBoxItem lbItem = new ListBoxItem();
            lbItem.Content = itemStr;
            lbItem.Foreground = foreground;
            if (addMouseClikEentHandler)
            {
                lbItem.MouseDoubleClick += new MouseButtonEventHandler((o, s) =>
                {
                    try
                    {
                        lrcDld.CurrentSong = LyricList[matchedLrcListBox.SelectedIndex - 1].Node;
                        //MessageBox.Show(lrcDld.CurrentSong.Attributes["id"].Value + "    " + lrcDld.CurrentSong.Attributes["title"].Value);
                        string str = lrcDld.DownloadLyric(xml);
                        OnGetLyricFile(str);
                    }
                    catch { }
                });
            }
            lb.Items.Add(lbItem);
        }

    }
}
