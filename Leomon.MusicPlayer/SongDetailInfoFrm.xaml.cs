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
    /// SongDetailInfoFrm.xaml 的交互逻辑
    /// </summary>
    public partial class SongDetailInfoFrm : Window
    {
        public SongDetailInfoFrm()
        {
            InitializeComponent();
        }
        public SongDetailInfoFrm(MySong mSong, bool hasLrc)
        {
            InitializeComponent();
            if (mSong != null)
            {
                this.songAlbumTB.Text = mSong.SongAlbum;
                this.songArtistTB.Text = mSong.SongArtist;
                this.songPathTB.Text = mSong.FilePath;
                this.songTitleTB.Text = mSong.SongTitle;
                //音频格式
                string tempStr = mSong.FilePath.Substring(mSong.FilePath.LastIndexOf(".") + 1);
                this.songFormationTB.Text = tempStr;
                //
                if (hasLrc)
                {
                    this.songHasLrcTB.Text = "有";
                }
                else
                {
                    this.songHasLrcTB.Text = "无";
                }
            }
        }
    }
}
