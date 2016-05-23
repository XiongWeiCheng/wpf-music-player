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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Leomon.MusicPlayer
{
    /// <summary>
    /// AboutForm.xaml 的交互逻辑
    /// </summary>
    public partial class AboutForm : Window
    {
        private string decString = "";
        public AboutForm()
        {
            InitializeComponent();
            AddDeclarationText();
        }
        private void AddDeclarationText()
        {
            decString = "1.Music Player 采用的是基于WPF的界面布局。" + Environment.NewLine +
                "2.Music Player 音频播放使用的是bass音频库，分32位和64位版本。" + Environment.NewLine +
                "3.Music Player 使用了开放源代码WPF豆瓣音乐播放器中对Bass.Net进行的二次封装类库，即DoubanFM.Bass.dll。为表示对原作者的尊重，在此表示十分感谢，没有那些无私贡献的大神们，我都不知道得花多久才能完成。" + Environment.NewLine +
                "4.Music Player 频谱图以及唱片显示控件使用的是WPFSoundVisualizationLib.dll类库。" + Environment.NewLine + 
                "5.Music Player 使用的图标均来自于网络。感谢提供者。" + Environment.NewLine + 
                "6.Music Player 下载歌词的库基于[Created by 吕亮 at 2009-1-15 10:08:16算法来源于网络]创建的类修改而来。感谢大神！"
                ;
            this.declarationTB.Text = decString;
        }
    }
}
