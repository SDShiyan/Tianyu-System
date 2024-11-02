using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Tianyu_System.Properties;

namespace Tianyu_System.Views
{
    internal class GameState
    {
        public required int[,] Board { get; init; }
        public required int Score { get; init; }
    }

    public partial class Game2048Window : Window
    {
        private int[,] board = new int[4, 4];
        private readonly Border[,] tiles = new Border[4, 4];
        private int score = 0;
        private readonly Random random = new Random();

        private Stack<GameState> undoStack = new Stack<GameState>();

        public Game2048Window()
        {
            InitializeComponent();
            InitializeBoard();
            AddRandomTile();
            AddRandomTile();
            
            // 添加键盘事件
            KeyDown += Game2048Window_KeyDown;

            // 显示最高分
            BestScoreText.Text = Settings.Default.Game2048BestScore.ToString();
        }

        private void InitializeBoard()
        {
            // 清空数组
            Array.Clear(board, 0, board.Length);
            score = 0;
            ScoreText.Text = "0";

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var tile = new Border
                    {
                        Background = GetTileColor(0),
                        Margin = new Thickness(5),
                        Child = new TextBlock
                        {
                            Text = "",
                            FontSize = 40,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    };

                    Grid.SetRow(tile, i);
                    Grid.SetColumn(tile, j);
                    GameGrid.Children.Add(tile);
                    tiles[i, j] = tile;
                }
            }
        }

        private void AddRandomTile()
        {
            var emptyCells = new List<(int, int)>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (board[i, j] == 0)
                        emptyCells.Add((i, j));
                }
            }

            if (emptyCells.Count > 0)
            {
                var cell = emptyCells[random.Next(emptyCells.Count)];
                board[cell.Item1, cell.Item2] = random.Next(10) < 9 ? 2 : 4;
                UpdateUI();
            }
        }

        private void Game2048Window_KeyDown(object sender, KeyEventArgs e)
        {
            bool moved = false;
            switch (e.Key)
            {
                case Key.Up:
                    moved = MoveUp();
                    break;
                case Key.Down:
                    moved = MoveDown();
                    break;
                case Key.Left:
                    moved = MoveLeft();
                    break;
                case Key.Right:
                    moved = MoveRight();
                    break;
            }

            if (moved)
            {
                AddRandomTile();
                if (IsGameOver())
                {
                    MessageBox.Show($"游戏结束！最终得分：{score}", "Game Over");
                    Close();
                }
            }
        }

        private bool MoveLeft()
        {
            bool moved = false;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    if (board[i, j] != 0)
                    {
                        int col = j;
                        while (col > 0 && board[i, col - 1] == 0)
                        {
                            board[i, col - 1] = board[i, col];
                            board[i, col] = 0;
                            col--;
                            moved = true;
                        }
                        if (col > 0 && board[i, col - 1] == board[i, col])
                        {
                            board[i, col - 1] *= 2;
                            score += board[i, col - 1];
                            board[i, col] = 0;
                            moved = true;
                        }
                    }
                }
            }
            if (moved) UpdateUI();
            return moved;
        }

        private bool MoveRight()
        {
            bool moved = false;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 2; j >= 0; j--)
                {
                    if (board[i, j] != 0)
                    {
                        int col = j;
                        while (col < 3 && board[i, col + 1] == 0)
                        {
                            board[i, col + 1] = board[i, col];
                            board[i, col] = 0;
                            col++;
                            moved = true;
                        }
                        if (col < 3 && board[i, col + 1] == board[i, col])
                        {
                            board[i, col + 1] *= 2;
                            score += board[i, col + 1];
                            board[i, col] = 0;
                            moved = true;
                        }
                    }
                }
            }
            if (moved) UpdateUI();
            return moved;
        }

        private bool MoveUp()
        {
            bool moved = false;
            for (int j = 0; j < 4; j++)
            {
                for (int i = 1; i < 4; i++)
                {
                    if (board[i, j] != 0)
                    {
                        int row = i;
                        while (row > 0 && board[row - 1, j] == 0)
                        {
                            board[row - 1, j] = board[row, j];
                            board[row, j] = 0;
                            row--;
                            moved = true;
                        }
                        if (row > 0 && board[row - 1, j] == board[row, j])
                        {
                            board[row - 1, j] *= 2;
                            score += board[row - 1, j];
                            board[row, j] = 0;
                            moved = true;
                        }
                    }
                }
            }
            if (moved) UpdateUI();
            return moved;
        }

        private bool MoveDown()
        {
            bool moved = false;
            for (int j = 0; j < 4; j++)
            {
                for (int i = 2; i >= 0; i--)
                {
                    if (board[i, j] != 0)
                    {
                        int row = i;
                        while (row < 3 && board[row + 1, j] == 0)
                        {
                            board[row + 1, j] = board[row, j];
                            board[row, j] = 0;
                            row++;
                            moved = true;
                        }
                        if (row < 3 && board[row + 1, j] == board[row, j])
                        {
                            board[row + 1, j] *= 2;
                            score += board[row + 1, j];
                            board[row, j] = 0;
                            moved = true;
                        }
                    }
                }
            }
            if (moved) UpdateUI();
            return moved;
        }

        private void UpdateUI()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var textBlock = (TextBlock)tiles[i, j].Child;
                    textBlock.Text = board[i, j] == 0 ? "" : board[i, j].ToString();
                    tiles[i, j].Background = GetTileColor(board[i, j]);
                    textBlock.Foreground = board[i, j] <= 4 ? 
                        new SolidColorBrush(Colors.Gray) : 
                        new SolidColorBrush(Colors.White);
                }
            }
            ScoreText.Text = score.ToString();

            // 更新最高分
            if (score > Settings.Default.Game2048BestScore)
            {
                Settings.Default.Game2048BestScore = score;
                Settings.Default.Save();
                BestScoreText.Text = score.ToString();
            }
        }

        private bool IsGameOver()
        {
            // 检查是否有空格
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (board[i, j] == 0)
                        return false;

            // 检查是否有可以合并的相邻数字
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (j < 3 && board[i, j] == board[i, j + 1]) return false;
                    if (i < 3 && board[i, j] == board[i + 1, j]) return false;
                }
            }

            return true;
        }

        private void CheckGameOver()
        {
            if (IsGameOver())
            {
                MessageBox.Show($"游戏结束！\n最终得分：{score}", "2048");
            }
        }

        private SolidColorBrush GetTileColor(int value)
        {
            switch (value)
            {
                case 0: return new SolidColorBrush(Color.FromRgb(205, 193, 180));
                case 2: return new SolidColorBrush(Color.FromRgb(238, 228, 218));
                case 4: return new SolidColorBrush(Color.FromRgb(237, 224, 200));
                case 8: return new SolidColorBrush(Color.FromRgb(242, 177, 121));
                case 16: return new SolidColorBrush(Color.FromRgb(245, 149, 99));
                case 32: return new SolidColorBrush(Color.FromRgb(246, 124, 95));
                case 64: return new SolidColorBrush(Color.FromRgb(246, 94, 59));
                case 128: return new SolidColorBrush(Color.FromRgb(237, 207, 114));
                case 256: return new SolidColorBrush(Color.FromRgb(237, 204, 97));
                case 512: return new SolidColorBrush(Color.FromRgb(237, 200, 80));
                case 1024: return new SolidColorBrush(Color.FromRgb(237, 197, 63));
                case 2048: return new SolidColorBrush(Color.FromRgb(237, 194, 46));
                default: return new SolidColorBrush(Color.FromRgb(238, 228, 218));
            }
        }

        private void SaveGameState()
        {
            var state = new GameState
            {
                Board = (int[,])board.Clone(),
                Score = score
            };
            undoStack.Push(state);
        }

        private void UndoMove()
        {
            if (undoStack.Count > 0)
            {
                var state = undoStack.Pop();
                board = (int[,])state.Board.Clone();
                score = state.Score;
                UpdateUI();
            }
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            GameGrid.Children.Clear();
            InitializeBoard();
            AddRandomTile();
            AddRandomTile();
            undoStack.Clear();
        }
    }
} 