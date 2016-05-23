using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Leomon.MusicPlayer
{
    /// <summary>
    /// PPT放映时代码段。
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 图片路径列表。
        /// </summary>
        private List<string> imageList;
        ///// <summary>
        ///// 用来存放幻灯片放映前的背景。
        ///// </summary>
        //private string preBackImage = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        private int index = 0;
        /// <summary>
        /// 
        /// </summary>
        private bool stopPlayPPT = true;
        /// <summary>
        /// 动画启动前，需要保存border的透明度。以便恢复。
        /// </summary>
        private double borderOpacity = 0d;
        /// <summary>
        /// 图片播放模式。
        /// </summary>
        private enum PPTPlayMode
        {
            LoopPlay = 0,
            RandomPlay
        }
        private PPTPlayMode myPPTPlayMode = PPTPlayMode.LoopPlay;

        private void AddImageFiles()
        {
            Microsoft.Win32.OpenFileDialog opf = new Microsoft.Win32.OpenFileDialog();
            opf.Filter = "图片|*.jpg;*.png;*.bmp|所有文件|*.*";
            opf.FileName = "";
            opf.Multiselect = true;
            opf.Title = "选择图片，可多选...";
            if (opf.ShowDialog() == true)
            {
                if (opf.FileNames.Length > 0)
                {
                    index = 0;
                    //
                    StopPlayPPT();
                    if (imageList != null)
                    {
                        imageList.Clear();
                    }
                    else
                    {
                        imageList = new List<string>();
                    }
                    //保存图片列表，并通知准备播放ppt图片。
                    imageList = opf.FileNames.ToList<string>();
                    if (MessageBoxResult.Yes == MessageBox.Show(string.Format("提示：共加载了{0}张图片，开始放映？", 
                               imageList.Count), "幻灯片放映", MessageBoxButton.YesNo, MessageBoxImage.Information))
                    {
                        MenuItem_Click(pptPlayerControlItem, null);
                    }
                }
            }
        }

        private void AddImageDirectory()
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.Description = "选择图片的目录";
            fbd.ShowNewFolderButton = true;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (fbd.SelectedPath.Trim() != "")
                {
                    //
                    index = 0;
                    //暂停播放，因为可能需要播放新的图片幻灯片了。
                    StopPlayPPT();
                    if (imageList != null)
                    {
                        imageList.Clear();
                    }
                    else
                    {
                        imageList = new List<string>();
                    }
                    //枚举目录中所有符合条件的图片
                    this.Dispatcher.Invoke(new Action(
                        () =>
                        {
                            //首先判断是否是根目录。
                            string[] drivers = Directory.GetLogicalDrives();
                            if (drivers.Contains(fbd.SelectedPath.Trim()))
                            {
                                //开启新的进程搜索
                                Thread thread = new Thread(SearchForAllImages);
                                thread.Start(fbd.SelectedPath);
                                MessageBox.Show("请耐心等待，加载所有的图片可能需要一段时间。^_^", "加载图片...", 
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                DirectorySecurity dirSecurity = new DirectorySecurity(fbd.SelectedPath, AccessControlSections.Access);
                                if (!dirSecurity.AreAccessRulesProtected)
                                {
                                    imageList.AddRange(EnumerateImageFiles(fbd.SelectedPath));
                                    if (MessageBoxResult.Yes == MessageBox.Show(string.Format("提示：在目录{0}下共查找并添加了{1}张图片。开始放映？", fbd.SelectedPath,
                                    imageList.Count), "幻灯片放映", MessageBoxButton.YesNo, MessageBoxImage.Information))
                                    {
                                       MenuItem_Click(pptPlayerControlItem, null);
                                    }
                                }
                            }
                            
                        }));
                }
            }
        }

        private void SearchForAllImages(object p)
        {
            string path = p.ToString();
            //获取所有顶层目录
            string[] allTopDirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
            foreach (var item in allTopDirs)
            {
                try
                {
                    DirectorySecurity dirSecurity = new DirectorySecurity(item, AccessControlSections.Access);
                    if (!dirSecurity.AreAccessRulesProtected)
                    {
                        //判断是否拥有访问的权限。
                        imageList.AddRange(EnumerateImageFiles(item));
                    }
                }
                catch { }
            }
            if (MessageBoxResult.Yes == MessageBox.Show(string.Format("提示：在目录{0}下共查找并添加了{1}张图片。开始放映？", path,
                                    imageList.Count), "幻灯片放映", MessageBoxButton.YesNo, MessageBoxImage.Information))
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    MenuItem_Click(pptPlayerControlItem, null);
                }));
            }
        }

        private void AutoLoadImagesFromDisk()
        {
            //if (MessageBox.Show("提示：全盘自动搜索所有图片可能需要较长的一段时间。需要耐心等待，是否开始搜索？^", "加载图片...",
            //        MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            //{
            //    index = 0;
            //    StopPlayPPT();
            //    if (imageList != null)
            //    {
            //        imageList.Clear();
            //    }
            //    else
            //    {
            //        imageList = new List<string>();
            //    }
            //    string[] drivers = Directory.GetLogicalDrives();
            //    foreach (string driver in drivers)
            //    {
            //        //开启新的进程搜索
            //        Thread thread = new Thread(SearchForAllImages);
            //        thread.Start(driver);
            //    }
            //}
        }

        private List<string> EnumerateImageFiles(string path)
        {
            List<string> imageFiles = new List<string>();
            //枚举得到jpg图片
            imageFiles.AddRange(
                Directory.EnumerateFiles(path, "*.jpg", SearchOption.AllDirectories));
            //枚举得到png图片
            imageFiles.AddRange(
            Directory.EnumerateFiles(path, "*.png", SearchOption.AllDirectories));
            //枚举得到bmp图片
            imageFiles.AddRange(
                Directory.EnumerateFiles(path, "*.bmp", SearchOption.AllDirectories));
            return imageFiles;
        }

        /// <summary>
        /// 开始播放幻灯片。
        /// </summary>
        /// <param name="images">待播放图片的路径</param>
        private bool PlayPPT(List<string> images)
        {
            bool successful = false;
            if (imageList != null)
            {
                //如果image的数量低于2，则不播放
                if (images.Count <= 1)
                {
                    successful = false;
                    MessageBox.Show("抱歉，请至少选择2张图片才可以进行循环的幻灯片播放。", "幻灯片放映", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    //pptTimer.IsEnabled = true;
                    //pptTimer.Start();
                    //可以开始播放了。
                    borderOpacity = BorderImage.Opacity;
                    BorderImage.Opacity = 0;
                    prePPTImage.Visibility = System.Windows.Visibility.Visible;
                    nowPPTImage.Visibility = System.Windows.Visibility.Visible;
                    stopPlayPPT = false;
                    if (myPPTPlayMode == PPTPlayMode.LoopPlay)
                    {
                        ChangeImageWithAnimation(index);
                    }
                    else
                    {
                        ChangeImageWithAnimation(GetRandomImageIndex(imageList));
                    }
                    successful = true;
                }
            }
            else
            {
                //MessageBox.Show("请先添加图片才可以放映。");
                MessageBoxResult result = MessageBox.Show("尚未添加图片，现在添加？\n提示：选择“是”添加图片文件，选择“否”添加图片文件目录。",
                    "幻灯片放映", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
                if(result == MessageBoxResult.Yes)
                {
                    AddImageFiles();
                }
                else if (result == MessageBoxResult.No)
                {
                    AddImageDirectory();
                }
            }
            return successful;
        }

        private int GetRandomImageIndex(List<string> imageList)
        {
            Random r = new Random();
            //MessageBox.Show(r.ToString());
            return r.Next(imageList.Count);
        }
        /// <summary>
        /// 停止放映
        /// </summary>
        /// <returns></returns>
        private bool StopPlayPPT()
        {
            bool successful = false;
            stopPlayPPT = true;
            //恢复border的透明度
            BorderImage.Opacity = borderOpacity;
            //将播放幻灯片用image透明度设置为0
            prePPTImage.Opacity = 0;
            prePPTImage.Visibility = System.Windows.Visibility.Collapsed;
            nowPPTImage.Visibility = System.Windows.Visibility.Collapsed;
            nowPPTImage.Opacity = 0;
            successful = true;
            return successful;
        }

        /// <summary>
        /// 动画方式切换图片：通过改变透明度来实现的一种动画效果。
        /// </summary>
        private void ChangeImageWithAnimation(int imageIndex)
        {
            if (!stopPlayPPT)
            {
                DoubleAnimation daPrePPTImage = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(5000));
                daPrePPTImage.Completed += daWindowImage_Completed;
                DoubleAnimation daNowPPTImage = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(6000));
                daNowPPTImage.Completed += daBorderImage_Completed;
                if (myPPTPlayMode == PPTPlayMode.LoopPlay)
                {
                    if ((imageIndex + 1) != imageList.Count)
                    {
                        prePPTImage.Source = nowPPTImage.Source;
                        nowPPTImage.Source = GetBitmapImage(imageList[imageIndex]);
                    }
                    else
                    {
                        prePPTImage.Source = nowPPTImage.Source;
                        nowPPTImage.Source = GetBitmapImage(imageList[0]);
                    }
                }
                else if (myPPTPlayMode == PPTPlayMode.RandomPlay)
                {
                    prePPTImage.Source = nowPPTImage.Source;
                    nowPPTImage.Source = GetBitmapImage(imageList[GetRandomImageIndex(imageList)]);
                }
                //开始动画
                prePPTImage.BeginAnimation(OpacityProperty, daPrePPTImage);
                nowPPTImage.BeginAnimation(OpacityProperty, daNowPPTImage);

            }
            else
            {
                prePPTImage.Opacity = 0;
                nowPPTImage.Opacity = 0;
            }
        }

        void daBorderImage_Completed(object sender, EventArgs e)
        {
            if (stopPlayPPT)
            {
                prePPTImage.Opacity = 0;
                nowPPTImage.Opacity = 0;
            }
            else
            {
                if (myPPTPlayMode == PPTPlayMode.LoopPlay)
                {
                    ChangeImageWithAnimation(index++);
                    if (index >= imageList.Count)
                    {
                        index = 0;
                    }
                }
                else
                {
                    ChangeImageWithAnimation(GetRandomImageIndex(imageList));
                }
            }
        }

        void daWindowImage_Completed(object sender, EventArgs e)
        {
            
        }
    }
}
