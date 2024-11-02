using System.Windows;
using System.Windows.Controls;
using Tianyu_System.Properties;

namespace Tianyu_System.Views
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            UpdateAvatarVisibility();
        }

        public void UpdateAvatarVisibility()
        {
            AvatarImage.Visibility = Settings.Default.ShowAvatar 
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }
    }
} 