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
    /// SearchSongForm.xaml 的交互逻辑
    /// </summary>
    public partial class SearchSongForm : Window
    {
        public int Progress
        {
            get
            {
                return (int)this.statusPrb.Value;
            }
            set
            {
                this.statusPrb.Value = value;
            }
        }
        public string StatusText
        {
            get
            {
                return this.statusTB.Text;
            }
            set
            {
                statusTB.Text = value;
            }
        }
        public bool StatusPrbIsIndeterminate
        {
            get
            {
                return this.statusPrb.IsIndeterminate;
            }
            set
            {
                this.statusPrb.IsIndeterminate = value;
            }
        }
        public SearchSongForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalCount">总的个数</param>
        public SearchSongForm(int totalCount)
            :base()
        {
            this.statusPrb.Maximum = totalCount;
        }
        private void keyValueTB_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                tb.Clear();
                this.statusPrb.IsIndeterminate = true;
                this.statusTB.Text = "等待搜索中......";
            }
        }

        public event EventHandler SearchButtonClick;
        protected void OnSearchButtonClick(string keyValue)
        {
            if (SearchButtonClick != null)
            {
                this.SearchButtonClick(keyValue, new EventArgs());
            }
        }

        private void searchBT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (keyValueTB.Text != "在此输入关键字")
            {
                StartSearching();
            }
        }

        private void StartSearching()
        {

            //判断搜索框中是否存在关键词
            if (keyValueTB.Text.Trim() == "")
            {
                keyValueTB.Text = "在此输入关键字";
                statusTB.Text = "等待搜索中......";
                statusPrb.IsIndeterminate = true;
            }
            else
            {
                OnSearchButtonClick(keyValueTB.Text);
                statusPrb.IsIndeterminate = false;
                statusPrb.Value = 0;
                statusTB.Text = string.Format("正在搜索中......");
            }
        }

        private void keyValueTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartSearching();
            }
        }
    }
}
