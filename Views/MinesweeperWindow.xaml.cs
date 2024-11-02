using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Tianyu_System.Properties;

namespace Tianyu_System.Views
{
    public partial class MinesweeperWindow : Window
    {
        private const int ROWS = 9;
        private const int COLS = 9;
        private const int MINES = 10;
        
        private readonly Button[,] buttons;
        private readonly bool[,] mines;
        private readonly int[,] numbers;
        private bool[,] revealed;
        private int minesLeft;
        private bool isGameOver;
        private bool isFirstClick = true;
        
        private DispatcherTimer? timer;
        private int elapsedSeconds;

        public MinesweeperWindow()
        {
            InitializeComponent();
            
            buttons = new Button[ROWS, COLS];
            mines = new bool[ROWS, COLS];
            numbers = new int[ROWS, COLS];
            revealed = new bool[ROWS, COLS];
            
            InitializeTimer();
            InitializeGame();
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick!;
            elapsedSeconds = 0;
            TimeText.Text = "0";
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            elapsedSeconds++;
            TimeText.Text = elapsedSeconds.ToString();
        }

        private void InitializeGame()
        {
            MineField.Children.Clear();
            isGameOver = false;
            isFirstClick = true;
            minesLeft = MINES;
            MinesLeftText.Text = minesLeft.ToString();
            
            if (timer != null)
            {
                timer.Stop();
                elapsedSeconds = 0;
                TimeText.Text = "0";
            }

            // 重置数组
            Array.Clear(mines, 0, mines.Length);
            Array.Clear(numbers, 0, numbers.Length);
            Array.Clear(revealed, 0, revealed.Length);

            // 初始化按钮网格
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLS; j++)
                {
                    var button = new Button
                    {
                        Width = 45,
                        Height = 45,
                        Margin = new Thickness(1),
                        Tag = new Point(i, j),
                        Background = new SolidColorBrush(Color.FromRgb(102, 51, 153))
                    };
                    
                    button.Click += Cell_Click;
                    button.MouseRightButtonDown += Cell_RightClick;
                    
                    buttons[i, j] = button;
                    MineField.Children.Add(button);
                }
            }

            // 随机放置地雷
            Random random = new Random();
            int minesPlaced = 0;
            while (minesPlaced < MINES)
            {
                int row = random.Next(ROWS);
                int col = random.Next(COLS);
                
                if (!mines[row, col])
                {
                    mines[row, col] = true;
                    minesPlaced++;
                }
            }

            RecalculateNumbers();
        }

        private void RecalculateNumbers()
        {
            // 计算每个格子周围的地雷数
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLS; j++)
                {
                    if (!mines[i, j])
                    {
                        numbers[i, j] = CountAdjacentMines(i, j);
                    }
                }
            }
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            if (isGameOver) return;
            
            var button = (Button)sender;
            var position = (Point)button.Tag;
            int row = (int)position.X;
            int col = (int)position.Y;

            if (button.Content is string content && content == "🚩")
                return;

            if (isFirstClick)
            {
                isFirstClick = false;
                EnsureSafeFirstClick(row, col);
                if (timer != null) timer.Start();
            }

            if (mines[row, col])
            {
                GameOver(false);
            }
            else
            {
                RevealCell(row, col);
                CheckWin();
            }
        }

        private void EnsureSafeFirstClick(int row, int col)
        {
            // 移除点击位置及其周围的地雷
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newRow = row + i;
                    int newCol = col + j;
                    if (newRow >= 0 && newRow < ROWS && 
                        newCol >= 0 && newCol < COLS && 
                        mines[newRow, newCol])
                    {
                        mines[newRow, newCol] = false;
                        PlaceNewMine(row, col);
                    }
                }
            }
            RecalculateNumbers();
        }

        private void PlaceNewMine(int excludeRow, int excludeCol)
        {
            Random random = new Random();
            while (true)
            {
                int row = random.Next(ROWS);
                int col = random.Next(COLS);
                
                if (!mines[row, col] && 
                    (Math.Abs(row - excludeRow) > 1 || 
                     Math.Abs(col - excludeCol) > 1))
                {
                    mines[row, col] = true;
                    break;
                }
            }
        }

        private void Cell_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (isGameOver) return;
            
            var button = (Button)sender;
            
            if (button.Content is string content)
            {
                if (content == "🚩")
                {
                    button.Content = null;
                    button.Background = new SolidColorBrush(Color.FromRgb(102, 51, 153));
                    minesLeft++;
                }
            }
            else
            {
                button.Content = "🚩";
                button.Background = new SolidColorBrush(Colors.Red);
                minesLeft--;
            }
            
            MinesLeftText.Text = minesLeft.ToString();
            e.Handled = true;
        }

        private void RevealCell(int row, int col)
        {
            if (row < 0 || row >= ROWS || col < 0 || col >= COLS || revealed[row, col])
                return;

            revealed[row, col] = true;
            buttons[row, col].IsEnabled = false;
            buttons[row, col].Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));

            if (numbers[row, col] > 0)
            {
                buttons[row, col].Content = numbers[row, col].ToString();
                buttons[row, col].FontSize = 24;
                buttons[row, col].FontWeight = FontWeights.Bold;
                
                switch (numbers[row, col])
                {
                    case 1:
                        buttons[row, col].Foreground = new SolidColorBrush(Colors.Blue);
                        break;
                    case 2:
                        buttons[row, col].Foreground = new SolidColorBrush(Colors.Green);
                        break;
                    case 3:
                        buttons[row, col].Foreground = new SolidColorBrush(Colors.Red);
                        break;
                    case 4:
                        buttons[row, col].Foreground = new SolidColorBrush(Colors.DarkBlue);
                        break;
                    case 5:
                        buttons[row, col].Foreground = new SolidColorBrush(Colors.DarkRed);
                        break;
                    case 6:
                        buttons[row, col].Foreground = new SolidColorBrush(Colors.Teal);
                        break;
                    case 7:
                        buttons[row, col].Foreground = new SolidColorBrush(Colors.Purple);
                        break;
                    case 8:
                        buttons[row, col].Foreground = new SolidColorBrush(Colors.Black);
                        break;
                }
            }
            else
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        RevealCell(row + i, col + j);
                    }
                }
            }
        }

        private int CountAdjacentMines(int row, int col)
        {
            int count = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newRow = row + i;
                    int newCol = col + j;
                    if (newRow >= 0 && newRow < ROWS && newCol >= 0 && newCol < COLS)
                    {
                        if (mines[newRow, newCol]) count++;
                    }
                }
            }
            return count;
        }

        private void RevealAllMines()
        {
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLS; j++)
                {
                    if (mines[i, j])
                    {
                        buttons[i, j].Content = "💣";
                        buttons[i, j].Background = new SolidColorBrush(Colors.Red);
                    }
                }
            }
        }

        private void CheckWin()
        {
            int unrevealedCount = 0;
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLS; j++)
                {
                    if (!revealed[i, j] && !mines[i, j])
                        unrevealedCount++;
                }
            }

            if (unrevealedCount == 0)
            {
                GameOver(true);
            }
        }

        private void GameOver(bool isWin)
        {
            isGameOver = true;
            if (timer != null) timer.Stop();
            
            if (isWin)
            {
                if (elapsedSeconds < Settings.Default.MinesweeperBestTime)
                {
                    Settings.Default.MinesweeperBestTime = elapsedSeconds;
                    Settings.Default.Save();
                    MessageBox.Show($"新纪录！\n用时：{elapsedSeconds}秒", "扫雷");
                }
                else
                {
                    MessageBox.Show($"恭喜你赢了！\n用时：{elapsedSeconds}秒", "扫雷");
                }
            }
            else
            {
                RevealAllMines();
                MessageBox.Show("游戏结束！", "扫雷");
            }
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame();
        }
    }
} 