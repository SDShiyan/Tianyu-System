using System.Windows;
using System.Windows.Controls;

namespace Tianyu_System.Views
{
    public partial class GamesPage : Page
    {
        public GamesPage()
        {
            InitializeComponent();
        }

        private void Game2048Button_Click(object sender, RoutedEventArgs e)
        {
            var game2048Window = new Game2048Window();
            game2048Window.Show();
        }

        private void MinesweeperButton_Click(object sender, RoutedEventArgs e)
        {
            var minesweeperWindow = new MinesweeperWindow();
            minesweeperWindow.Show();
        }
    }
} 