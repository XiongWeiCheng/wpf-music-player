using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Leomon.MusicPlayer
{
    public partial class MainWindow : Window
    {
        //搜索歌曲后台线程
        BackgroundWorker searchBW = null;
        SearchSongForm searchFrm = null;
        //搜索窗口
        private void OpenSearchSongForm()
        {
            if (songList.Count == 0)
            {
                MessageBox.Show("歌曲列表为空，无法搜索歌曲！");
                return;
            }
            searchFrm = new SearchSongForm();
            searchFrm.SearchButtonClick += searchFrm_SearchButtonClick;
            searchFrm.ShowDialog();
        }

        void searchFrm_SearchButtonClick(object sender, EventArgs e)
        {
            string keyValue = sender as string;
            if (keyValue != null)
            {
                if (searchBW == null)
                {
                    searchBW = new BackgroundWorker();
                    searchBW.DoWork += searchBW_DoWork;
                    searchBW.WorkerReportsProgress = true;
                    searchBW.ProgressChanged += searchBW_ProgressChanged;
                    searchBW.RunWorkerCompleted += searchBW_RunWorkerCompleted;
                }
                if (searchBW.IsBusy != true)
                {
                    searchBW.RunWorkerAsync(keyValue);
                }
                else if (searchBW.IsBusy == true)
                {
                    MessageBox.Show("请等待当前搜索进程结束！", "搜索歌曲", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                    
            }
        }

        void searchBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            searchFrm.Progress = songList.Count;
            int result = (int)e.Result;
            if (result == songList.Count + 1)
            {
                searchFrm.StatusText = "搜索失败，没有找到歌曲.";
            }
            else
            {
                songListLB.SelectedIndex = result;
                songListLB.ScrollIntoView(songListLB.SelectedItem);
                searchFrm.StatusText = "模糊匹配成功！ ^o^";
            }
        }

        void searchBW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int i = (int)e.UserState;
            //匹配成功！
            searchFrm.Progress = i;
        }

        void searchBW_DoWork(object sender, DoWorkEventArgs e)
        {
            string keyValue = e.Argument as string;
            bool success = false;
            if (keyValue != null)
            {
                //拆分关键字
                string[] keyArray = keyValue.Split(' ');
                //存放匹配结果，匹配结果必须保证在50%以上相似才可以。
                int checkedCount = 0;
                //逐项进行匹配
                for (int i = 0; i < songList.Count; i++)
                {
                    for (int j = 0; j < keyArray.Length; j++)
                    {
                        if (songList[i].SongName.Contains(keyArray[j]))
                            checkedCount++;
                    }
                    //判定是否有0.8以上匹配上了
                    if (((double)checkedCount / (double)keyArray.Length) >= 0.8)
                    {
                        //记下索引号
                        e.Result = i;
                        success = true;
                        break;
                    }
                    searchBW.ReportProgress(0, i);
                }
                if (success == false)
                {
                    e.Result = songList.Count + 1;
                }
            }
        }
    }
}
