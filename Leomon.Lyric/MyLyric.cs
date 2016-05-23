using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Leomon.Lyric
{
    /// <summary>
    /// 该类表示歌词的相关信息。
    /// </summary>
    public class MyLyric
    {
        #region 歌词相关的信息定义 字段 属性
        //ar 艺人名
        private string artist = "";

        //ti 歌名
        private string song = "";

        //al 专辑
        private string album = "";

        //by 编者
        private string author = "";

        //offset, if lyric has.
        private double offset = 0;

        //歌词列表
        private List<string> lyricTextLines = new List<string>();

        //对应时间列表
        private List<string> lyricTimeLines = new List<string>();

        //对应转化的double型时间，单位为ms
        private List<double> lyricTimeLinesDValue = new List<double>();
        /// <summary>
        /// 属性：获得演唱人
        /// </summary>
        public string Artist
        {
            get { return artist; }
        }

        /// <summary>
        /// 属性：获得歌名
        /// </summary>
        public string SongName
        {
            get { return song; }
        }

        /// <summary>
        /// 属性：获得专辑名
        /// </summary>
        public string Album
        {
            get { return album; }
        }

        /// <summary>
        /// 属性：获得作词人
        /// </summary>
        public string Author
        {
            get { return author; }
        }

        /// <summary>
        /// 属性：获取歌词的整体偏移量。单位ms
        /// </summary>
        public double Offset
        {
            get
            {
                return offset;
            }
        }

        /// <summary>
        /// 属性：获取所有解析得到的歌词文本
        /// </summary>
        public List<string> LyricTextLines
        {
            get { return lyricTextLines; }
        }

        /// <summary>
        /// 属性：获取所有解析得到歌词文本对应时间。
        /// </summary>
        public List<string> LyricTimeLines
        {
            get { return lyricTimeLines; }
        }

        /// <summary>
        /// 属性：获得每行歌词对应的转换后的double型时间值。
        /// </summary>
        public List<double> LyricTimeLinesDValue
        {
            get
            {
                return lyricTimeLinesDValue;
            }
        }
        #endregion

        #region 公开方法
        /// <summary>
        /// 构造方法：实例化MyLyric对象。
        /// </summary>
        public MyLyric() { }

        /// <summary>
        /// 构造方法：实例化MyLyric对象(推荐使用，不易出错！)。
        /// </summary>
        /// <param name="lyricPath">歌词文件路径</param>
        /// <param name="encoding">指定打开歌词文件的编码方式，默认为Encoding.Default</param>
        public MyLyric(string lyricPath, Encoding encoding)
        {
            //打开歌词文件，并将歌词文件读取进来。
            string fileString = OpenLyricFile(lyricPath, encoding);
            AnalyzeLyricFile(fileString);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lrcStr"></param>
        public MyLyric(string lrcStr)
        {
            AnalyzeLyricFile(lrcStr);
        }

        /// <summary>
        /// 方法：解析lrc歌词文件。该方法在使用歌词类的空构造函数时必须被调用一次，否则无法得到解析的文件。
        /// </summary>
        /// <param name="lyricPath">歌词文件路径</param>
        public void AnalyzeLyricFile(string fileString)
        {
            //进行歌词的解析
            StartAnalyzeLyric(fileString);
            //接下来对歌词进行按时间的排序
            //不知道效率如何，但是毕竟是自己想的，就试试吧，，
            //拼字
            for (int i = 0; i < LyricTimeLines.Count; i++)
            {
                LyricTimeLines[i] += '\n' + LyricTextLines[i];
            }
            //排序
            LyricTimeLines.Sort();
            //拆字
            for (int i = 0; i < LyricTimeLines.Count; i++)
            {
                string[] splitStr =
                    LyricTimeLines[i].Split('\n');
                LyricTimeLines[i] = splitStr[0];
                ConvertTimeToDoubleValue(splitStr[0]);
                LyricTextLines[i] = splitStr[1];
            }
        }

        #endregion

        #region 私有方法
        /// <summary>
        /// 打开歌词文件流，并将歌词文件字符串读取出来并返回解析。
        /// </summary>
        /// <param name="lyricPath">歌词文件路径</param>
        /// <returns>歌词文件中的字符串</returns>
        private string OpenLyricFile(string lyricPath, Encoding encoding)
        {
            string tempStr = "";
            if (encoding == null)
            {
                encoding = Encoding.Default;
            }
            using (StreamReader sr = new StreamReader(lyricPath, encoding))
            {
                try
                {
                    //读取文件所有字节。
                    tempStr = sr.ReadToEnd();
                }
                catch (DirectoryNotFoundException e)
                {
                    tempStr = "";
                    throw new DirectoryNotFoundException(e.Message);
                }
                catch (FileNotFoundException e)
                {
                    tempStr = "";
                    throw new FileNotFoundException(e.Message);
                }
                catch (Exception e)
                {
                    tempStr = "";
                    throw new Exception(e.Message);
                }
            }
            return tempStr;
        }

        /// <summary>
        /// 对得到的字符串进行解析。
        /// </summary>
        /// <param name="fileString">待解析的歌词字符串</param>
        private void StartAnalyzeLyric(string fileString)
        {
            //以行为单位进行第一步分割
            string[] stringLine;
            stringLine = fileString.Split(new char[]{'\n'}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < stringLine.Length; i++)
            {
                if (stringLine[i].Contains('['))     //如果该行拥有[表明改行是需要解析行。
                {
                    string tempStr = stringLine[i].Trim();
                    //Console.WriteLine(tempStr);
                    #region 解析歌词的描述歌曲信息的部分
                    if (tempStr.Contains("[ar:"))
                    {
                        //解析得到艺人的名字
                        this.artist = GetSongInformation(tempStr);
                    }
                    else if (tempStr.Contains("[ti:"))
                    {
                        //解析得到歌曲名称
                        this.song = GetSongInformation(tempStr);
                    }
                    else if (tempStr.Contains("[al:"))
                    {
                        //解析得到歌曲的专辑
                        this.album = GetSongInformation(tempStr);
                    }
                    else if (tempStr.Contains("[by:"))
                    {
                        //解析得到歌词的作者
                        this.author = GetSongInformation(tempStr);
                    }
                    #endregion
                    //否则文本内容则为歌词的正文部分，即歌词和对应的时间。
                    else
                    {
                        if (tempStr.Contains("offset"))
                        {
                            if (!double.TryParse(GetSongInformation(tempStr), out this.offset))
                            {
                                this.offset = 0;
                            }
                        }
                        else
                        {
                            GetLyricContentAndTime(tempStr);
                        }
                    }
                }
            }
        }


        private void GetLyricContentAndTime(string tempStr)
        {
            //以中括号分割
            string[] lineStr = tempStr.Split(']');
            int indexM = lineStr.Length;
            for (int i = 0; i < indexM - 1; i++)
            {
                string time = lineStr[i].Substring(1);
                if (time.Contains('.'))
                    time = time.Replace('.', ':');
                if (time[1] >= '0' && time[1] <= '9')
                {
                    LyricTimeLines.Add(time);
                    if (lineStr[indexM - 1].Trim() == "")
                    {
                        LyricTextLines.Add("Music ~ ~ ~");
                    }
                    else
                    {
                        LyricTextLines.Add(lineStr[indexM - 1]);
                    }
                }
            }
        }

        /// <summary>
        /// 返回ms
        /// </summary>
        /// <param name="time"></param>
        private void ConvertTimeToDoubleValue(string time)
        {
            string[] t = time.Split(':');
            double dTime = 0;
            if (t.Length == 2)
            {
                //分
                int min = 0,
                    sec = 0;    //秒
                if (int.TryParse(t[0], out min) && int.TryParse(t[1], out sec))
                {
                    //保证全部得以转换方可以保存
                    dTime = (min * 60 + sec * 1) * 1000.0;    //单位是秒，没有精确到ms
                }
            }
            else
            {
                int min = 0,    //分
                    sec = 0,    //秒
                    mill = 0;   //毫秒
                if (int.TryParse(t[0], out min) &&
                    int.TryParse(t[1], out sec) &&
                    int.TryParse(t[2], out mill))
                {
                    dTime = (min * 60 + sec * 1) * 1000.0 + mill;
                }
            }
            lyricTimeLinesDValue.Add(dTime);
        }

        private string GetSongInformation(string tempStr)
        {
            int index = tempStr.IndexOf(":") + 1;
            string str = tempStr.Substring(index, tempStr.Length - index - 1);
            return str;
        }
        /// <summary>
        /// 获取文件的编码方式
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns></returns>
        private Encoding GetEncoding(string fileName)
        {
            Encoding en = Encoding.Default;
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                en = GetEncoding(fs);
                fs.Close();
            }
            catch
            {
                en = Encoding.Default;
            }
            return en;
        }
        /// <summary>
        /// 获取文件流的编码方式
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <returns></returns>
        private Encoding GetEncoding(FileStream fs)
        {
            Encoding en = Encoding.Default;
            try
            {
                BinaryReader br = new BinaryReader(fs, Encoding.Default);
                byte[] buffer = br.ReadBytes(2);
                br.Close();
                //开始判断编码类型
                if (buffer[0] >= 0xEF)
                {
                    if (buffer[0] == 0xEF && buffer[1] == 0xBB)
                    {
                        en = Encoding.UTF8;
                    }
                    else if (buffer[0] == 0xEF && buffer[1] == 0xFF)
                    {
                        en = Encoding.BigEndianUnicode;
                    }
                    else
                    {
                        en = Encoding.Default;
                    }
                }
                else
                {
                    en = Encoding.Default;
                }
            }
            catch
            {
                en = Encoding.Default;
            }
            return en;
        }
        #endregion
    }
}
