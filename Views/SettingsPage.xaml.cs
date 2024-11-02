using System.Windows;
using System.Windows.Controls;
using Tianyu_System.Properties;

namespace Tianyu_System.Views
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            // 从设置中加载状态
            ShowAvatarToggle.IsChecked = Settings.Default.ShowAvatar;
        }

        private void ShowAvatarToggle_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.ShowAvatar = true;
            Settings.Default.Save();
            UpdateHomePageAvatar();
        }

        private void ShowAvatarToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.ShowAvatar = false;
            Settings.Default.Save();
            UpdateHomePageAvatar();
        }

        private void UpdateHomePageAvatar()
        {
            // 获取主窗口
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                // 获取主页面
                if (mainWindow.MainFrame.Content is HomePage homePage)
                {
                    homePage.UpdateAvatarVisibility();
                }
            }
        }
    }
} 