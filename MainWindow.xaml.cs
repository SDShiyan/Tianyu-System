using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tianyu_System.Views;

namespace Tianyu_System;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // 默认导航到主页
        MainFrame.Navigate(new HomePage());
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            MaxButton_Click(sender, e);
        }
        else
        {
            this.DragMove();
        }
    }

    private void MinButton_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void MaxButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.WindowState == WindowState.Maximized)
        {
            this.WindowState = WindowState.Normal;
        }
        else
        {
            // 获取工作区大小（不包括任务栏）
            var workArea = SystemParameters.WorkArea;
            this.MaxHeight = workArea.Height;
            this.MaxWidth = workArea.Width;
            
            // 设置窗口位置为工作区左上角
            this.Left = workArea.Left;
            this.Top = workArea.Top;
            
            this.WindowState = WindowState.Maximized;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void NavButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            switch (button.Tag.ToString())
            {
                case "HomePage":
                    MainFrame.Navigate(new HomePage());
                    break;
                case "HomeworkPage":
                    MainFrame.Navigate(new HomeworkPage());
                    break;
                case "GamesPage":
                    MainFrame.Navigate(new GamesPage());
                    break;
                case "SettingsPage":
                    MainFrame.Navigate(new SettingsPage());
                    break;
            }
        }
    }
}