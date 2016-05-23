using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;
namespace Leomon.MusicPlayer
{
    /// <summary>
    /// 歌曲类。
    /// </summary>
    public class MySong
    {
        //字段：歌曲路径。
        private string filePath = string.Empty;
        /// <summary>
        /// 属性：获取歌曲路径。
        /// </summary>
        public string FilePath
        {
            get
            {
                return filePath;
            }
            protected set
            {
                filePath = value;
            }
        }

        private string fileDirectory = "";
        /// <summary>
        /// 属性：获取歌曲文件目录。
        /// </summary>
        public string FileDirectory
        {
            get { return fileDirectory; }
        }        

        //字段：歌曲文件名称。
        private string songName = string.Empty;
        /// <summary>
        /// 属性：获取歌曲名称。
        /// </summary>
        public string SongName
        {
            get
            {
                return songName;
            }
            protected set
            {
                songName = value;
            }
        }

        private string songAlbum = string.Empty;
        /// <summary>
        /// 专辑
        /// </summary>
        public string SongAlbum
        {
            get { return songAlbum; }
            set { songAlbum = value; }
        }
        private string songTitle = string.Empty;
        /// <summary>
        /// 歌名
        /// </summary>
        public string SongTitle
        {
            get { return songTitle; }
            set { songTitle = value; }
        }
        private string songArtist = string.Empty;
        /// <summary>
        /// 歌手
        /// </summary>
        public string SongArtist
        {
            get { return songArtist; }
            set { songArtist = value; }
        }
        //public MySong() { }
        public MySong(string path)
        {
            AnalyzePath(path);
        }

        private void AnalyzePath(string path)
        {
            //GetSongDetailInformation(path);
            this.filePath = path;
            string[] str = path.Split('\\');
            string tempStr = str[str.Length - 1];
            this.songName = tempStr.Substring(0, tempStr.LastIndexOf("."));
            this.fileDirectory = path.Substring(0, path.LastIndexOf('\\'));
        }

        private void GetSongDetailInformation(string path)
        {
            File file = File.Create(path);
            foreach (string item in file.Tag.Artists)
            {
                songArtist += item;
            }
            if (songArtist == "")
            {
                songArtist = "未知艺术家";
            }
            if (file.Tag.Title != null)
            {
                songTitle = file.Tag.Title;
            }
            if (file.Tag.Album != null)
            {
                songAlbum = file.Tag.Album;
            }
        }
    }
}
