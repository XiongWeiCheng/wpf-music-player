using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Leomon.MusicPlayer
{
    /// <summary>
    /// 继承自EventArgs的类。废话。。。
    /// </summary>
    public class MySettingValueEventArgs : EventArgs
    {
        public readonly double OpacityValue;
        public readonly int DesktopLyricFontSize;
        public readonly string DesktopLyricFontFamily;
        public readonly string DesktopLyricFontType;
        public readonly bool? ShowSpectrum;
        public readonly bool? AutoLoadLyricFile;
        public readonly Brush SongNameForeColor;
        public readonly Brush UnplayedLyricForeColor;
        public readonly Brush PlayedLyricForeColor;
        public readonly Color DesktopUnplayedLyricForeColor;
        public readonly Color DesktopPlayedLyricForeColor;
        public readonly Brush WindowTitleForeColor;
        public readonly Brush ListBoxTitleForeColor;
        public readonly Brush UnselectedItemForeColor;
        public readonly Brush SelectedItemForeColor;
        public readonly bool? SaveConfig;
        public readonly bool? SaveSongList;
        public readonly bool? RememberExitPosition;
        public readonly Encoding FileEncoding;
        public MySettingValueEventArgs(double opacityValue,
            int   desktopLyricFontSize,
            string desktopLyricFontFamily,
            string desktopLyricFontType,
            bool? showSpectrum,
            bool? autoLoadLyricFile,
            bool? saveConfig,
            bool? saveSongList,
            bool? remeberExitPos,
            Brush songNameForeColor,
            Brush unplayedLyricForeColor,
            Brush playedLyricForeColor,
            Color desktopUnplayedLyricForeColor,
            Color desktopPlayedLyricForeColor,
            Brush windowTitleForeColor,
            Brush listBoxTitleForeColor,
            Brush unselectedItemForeColor,
            Brush selectedItemForeColor,
            Encoding fileEncoding)
        {
            this.OpacityValue = opacityValue;
            this.DesktopLyricFontSize = desktopLyricFontSize;
            this.DesktopLyricFontFamily = desktopLyricFontFamily;
            this.DesktopLyricFontType = desktopLyricFontType;
            this.ShowSpectrum = showSpectrum;
            this.AutoLoadLyricFile = autoLoadLyricFile;
            this.SongNameForeColor = songNameForeColor;
            this.UnplayedLyricForeColor = unplayedLyricForeColor;
            this.PlayedLyricForeColor = playedLyricForeColor;
            this.DesktopPlayedLyricForeColor = desktopPlayedLyricForeColor;
            this.DesktopUnplayedLyricForeColor = desktopUnplayedLyricForeColor;
            this.WindowTitleForeColor = windowTitleForeColor;
            this.ListBoxTitleForeColor = listBoxTitleForeColor;
            this.UnselectedItemForeColor = unselectedItemForeColor;
            this.SelectedItemForeColor = selectedItemForeColor;
            this.SaveConfig = saveConfig;
            this.SaveSongList = saveSongList;
            this.RememberExitPosition = remeberExitPos;
            this.FileEncoding = fileEncoding;
        }
    }
}
