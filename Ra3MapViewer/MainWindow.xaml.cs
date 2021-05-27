using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ra3MapViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        static readonly string Ra3MapFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Red Alert 3\Maps";

        private Map selectedMap;

        public ObservableCollection<Map> Maps { get; set; } = new ObservableCollection<Map>();

        public Map SelectedMap { get => selectedMap; set { selectedMap = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedMap))); } }

        public Visibility ImportVisibility => isGameFolder ? Visibility.Collapsed : Visibility.Visible;

        private bool isGameFolder = true;

        public MainWindow()
        {
            InitializeComponent();
            Scan();
        }

        private void Scan(string path = null)
        {
            isGameFolder = path is null;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImportVisibility)));
            if (isGameFolder)
                path = Ra3MapFolder;

            Maps.Clear();
            foreach (var map in MapParser.Scan(path).OrderBy(m => m.MaxPlayers).ThenBy(m => m.DisplayName))
                Maps.Add(map);

            ShowMsg($"已解析地图 {Maps.Count} 个");
        }

        private void ShowMsg(string message, Brush color = null)
        {
            msg.Text = message;
            msg.Foreground = color ?? Brushes.Black;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Actions_Click(object sender, RoutedEventArgs e)
            => (FindResource("menu") as ContextMenu).IsOpen = true;

        private void Menu_Scan_Click(object sender, RoutedEventArgs e)
            => Scan();

        private void Menu_ScanExternal_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog { Description = "选择包含地图集的目录" };
            if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var path = dialog.SelectedPath;
                if (path.Contains(Ra3MapFolder))
                    path = null;

                Scan(path);
            }
        }

        private void Menu_OpenGameFolder_Click(object sender, RoutedEventArgs e)
            => Process.Start("explorer.exe", Ra3MapFolder);

        private void ContextMenu_Import_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileSystem.CopyDirectory(selectedMap.FolderPath, Ra3MapFolder, UIOption.AllDialogs);
                ShowMsg($"已导入: {selectedMap.FileId}");
            }
            catch { }
        }

        private void ContextMenu_OpenFolder_Click(object sender, RoutedEventArgs e)
            => Process.Start("explorer.exe", selectedMap.FolderPath);

        private void ContextMenu_MoveToTrash_Click(object sender, RoutedEventArgs e)
        {
            FileSystem.DeleteDirectory(selectedMap.FolderPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
            ShowMsg($"已删除: {selectedMap.FolderPath}");
            Maps.Remove(selectedMap);
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}
