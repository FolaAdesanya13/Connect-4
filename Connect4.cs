using System;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Controls.Shapes;

internal class MyProject
{
    private const int Rows = 6;
    private const int Columns = 7;

    private readonly int[,] board = new int[Rows, Columns];
    private TextBlock status = new TextBlock();
    private readonly Button[,] cells = new Button[Rows, Columns];
    private int currentPlayer = 1;
    private bool gameOver;

    public Window win;

    public MyProject()
    {
        win = new Window
        {
            Title = "Connect 4",
            WindowState = WindowState.FullScreen,
            Background = new SolidColorBrush(Color.Parse("#10243A")),
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            MinWidth = 700,
            MinHeight = 600,
        };

        ShowStartScreen();
    }

    private void ShowStartScreen()
    {
        StackPanel startScreen = new StackPanel
        {
            Spacing = 24,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };

        startScreen.Children.Add(new TextBlock
        {
            Text = "CONNECT 4",
            FontSize = 64,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Center,
        });

        Button play = new Button
        {
            Content = "PLAY",
            FontSize = 28,
            FontWeight = FontWeight.Bold,
            Padding = new Avalonia.Thickness(42, 16),
            HorizontalAlignment = HorizontalAlignment.Center,
            Background = new SolidColorBrush(Color.Parse("#21A179")),
            Foreground = Brushes.White,
        };
        play.Click += (_, _) => StartGame();
        startScreen.Children.Add(play);

        Button quit = new Button
        {
            Content = "QUIT",
            FontSize = 16,
            Padding = new Avalonia.Thickness(24, 10),
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        quit.Click += (_, _) => win.Close();
        startScreen.Children.Add(quit);

        win.Content = startScreen;
    }

    private void StartGame()
    {
        Array.Clear(board, 0, board.Length);
        currentPlayer = 1;
        gameOver = false;

        Grid gameLayout = new Grid
        {
            Margin = new Avalonia.Thickness(24),
            RowDefinitions = RowDefinitions.Parse("Auto,Auto,Auto,*,Auto"),
        };

        TextBlock title = new TextBlock
        {
            Text = "CONNECT 4",
            FontSize = 36,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Avalonia.Thickness(0, 0, 0, 8),
        };

        status = new TextBlock
        {
            Text = "RED'S TURN",
            FontSize = 26,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.Red,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Avalonia.Thickness(0, 0, 0, 14),
        };

        Grid boardGrid = new Grid
        {
            Width = 700,
            Height = 650,
            Background = new SolidColorBrush(Color.Parse("#165DAB")),
            RowDefinitions = RowDefinitions.Parse("72,*,*,*,*,*,*"),
            ColumnDefinitions = ColumnDefinitions.Parse("*,*,*,*,*,*,*"),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };

        for (int column = 0; column < Columns; column++)
        {
            Button dropButton = new Button
            {
                Content = "▼ DROP",
                FontSize = 16,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.Parse("#0D477E")),
                Margin = new Avalonia.Thickness(4),
                Tag = column,
            };
            dropButton.Click += OnCellClick;
            Grid.SetRow(dropButton, 0);
            Grid.SetColumn(dropButton, column);
            boardGrid.Children.Add(dropButton);
        }

        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                Ellipse disc = new Ellipse
                {
                    Width = 72,
                    Height = 72,
                    Fill = new SolidColorBrush(Color.Parse("#10243A")),
                    Stroke = new SolidColorBrush(Color.Parse("#0D477E")),
                    StrokeThickness = 3,
                    Margin = new Avalonia.Thickness(8),
                    IsHitTestVisible = false,
                };

                Button cell = new Button
                {
                    Content = disc,
                    Background = Brushes.Transparent,
                    BorderThickness = new Avalonia.Thickness(0),
                    Padding = new Avalonia.Thickness(0),
                    Margin = new Avalonia.Thickness(2),
                    Tag = column,
                };
                cell.Click += OnCellClick;

                cells[row, column] = cell;
                Grid.SetRow(cell, row + 1);
                Grid.SetColumn(cell, column);
                boardGrid.Children.Add(cell);
            }
        }

        Grid.SetRow(title, 0);
        Grid.SetRow(status, 1);
        gameLayout.Children.Add(title);
        gameLayout.Children.Add(status);

        Viewbox boardView = new Viewbox
        {
            Child = boardGrid,
            Stretch = Stretch.Uniform,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        Grid.SetRow(boardView, 3);
        gameLayout.Children.Add(boardView);

        StackPanel buttons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Avalonia.Thickness(0, 14, 0, 0),
        };

        Button newGame = new Button
        {
            Content = "NEW GAME",
            FontSize = 16,
            Padding = new Avalonia.Thickness(18, 10),
        };
        newGame.Click += (_, _) => StartGame();
        buttons.Children.Add(newGame);

        Button mainMenu = new Button
        {
            Content = "MAIN MENU",
            FontSize = 16,
            Padding = new Avalonia.Thickness(18, 10),
        };
        mainMenu.Click += (_, _) => ShowStartScreen();
        buttons.Children.Add(mainMenu);

        Button quit = new Button
        {
            Content = "QUIT",
            FontSize = 16,
            Padding = new Avalonia.Thickness(18, 10),
        };
        quit.Click += (_, _) => win.Close();
        buttons.Children.Add(quit);

        Grid.SetRow(buttons, 4);
        gameLayout.Children.Add(buttons);
        win.Content = gameLayout;
    }

    private void OnCellClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (gameOver || sender is not Button { Tag: int column })
        {
            return;
        }

        int row = Rows - 1;
        while (row >= 0 && board[row, column] != 0)
        {
            row--;
        }

        if (row < 0)
        {
            return;
        }

        board[row, column] = currentPlayer;
        Ellipse disc = (Ellipse)cells[row, column].Content!;
        disc.Fill = currentPlayer == 1 ? Brushes.Red : Brushes.Yellow;
        disc.Stroke = currentPlayer == 1
            ? new SolidColorBrush(Color.Parse("#B5121B"))
            : new SolidColorBrush(Color.Parse("#D6A900"));

        if (HasWon(row, column))
        {
            gameOver = true;
            status.Text = currentPlayer == 1 ? "RED WINS" : "YELLOW WINS";
            status.Foreground = currentPlayer == 1 ? Brushes.Red : Brushes.Yellow;
            return;
        }

        if (IsBoardFull())
        {
            gameOver = true;
            status.Text = "DRAW!";
            status.Foreground = Brushes.White;
            return;
        }

        currentPlayer = currentPlayer == 1 ? 2 : 1;
        status.Text = currentPlayer == 1 ? "RED'S TURN" : "YELLOW'S TURN";
        status.Foreground = currentPlayer == 1 ? Brushes.Red : Brushes.Yellow;
    }

    private bool HasWon(int row, int column)
    {
        return CountConnected(row, column, 0, 1) >= 4
            || CountConnected(row, column, 1, 0) >= 4
            || CountConnected(row, column, 1, 1) >= 4
            || CountConnected(row, column, 1, -1) >= 4;
    }

    private int CountConnected(int row, int column, int rowStep, int columnStep)
    {
        int count = 1 + CountDirection(row, column, rowStep, columnStep)
            + CountDirection(row, column, -rowStep, -columnStep);
        return count;
    }

    private int CountDirection(int row, int column, int rowStep, int columnStep)
    {
        int count = 0;
        int nextRow = row + rowStep;
        int nextColumn = column + columnStep;

        while (nextRow >= 0 && nextRow < Rows
            && nextColumn >= 0 && nextColumn < Columns
            && board[nextRow, nextColumn] == currentPlayer)
        {
            count++;
            nextRow += rowStep;
            nextColumn += columnStep;
        }

        return count;
    }

    private bool IsBoardFull()
    {
        for (int column = 0; column < Columns; column++)
        {
            if (board[0, column] == 0)
            {
                return false;
            }
        }

        return true;
    }
}
