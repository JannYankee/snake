using static System.Console;

namespace SnakeGame
{
    public enum Direction { Up, Down, Left, Right }

    public class Pixel
    {
        private const char PixelChar = 'O';
        public int X { get; set; }
        public int Y { get; set; }
        public ConsoleColor Color { get; set; }

        public Pixel(int x, int y, ConsoleColor color)
        {
            X = x;
            Y = y;
            Color = color;
        }

        public void Draw()
        {
            ForegroundColor = Color;
            SetCursorPosition(X, Y);
            Write(PixelChar);
        }

        public void Clear()
        {
            SetCursorPosition(X, Y);
            Write(' ');
        }
    }

    public class Snake
    {
        public List<Pixel> Body { get; private set; }
        public Direction CurrentDirection { get; set; }
        private bool growNextMove = false;

        public Snake(int startX, int startY)
        {
            Body = new List<Pixel>
            {
                new Pixel(startX, startY, ConsoleColor.Green)
            };
            CurrentDirection = Direction.Right;
        }

        public void Move()
        {
            Pixel head = Body.Last();
            int newX = head.X, newY = head.Y;

            switch (CurrentDirection)
            {
                case Direction.Up: newY--; break;
                case Direction.Down: newY++; break;
                case Direction.Left: newX--; break;
                case Direction.Right: newX++; break;
            }

            Pixel newHead = new Pixel(newX, newY, ConsoleColor.Green);
            Body.Add(newHead);
            newHead.Draw();

            if (!growNextMove)
            {
                Body.First().Clear();
                Body.RemoveAt(0);
            }
            else
            {
                growNextMove = false;
            }
        }

        public void Grow()
        {
            growNextMove = true;
        }

        public bool CheckCollision(int width, int height)
        {
            Pixel head = Body.Last();
            return head.X <= 0 || head.X >= width - 1 || head.Y <= 0 || head.Y >= height - 1 ||
                   Body.Take(Body.Count - 1).Any(p => p.X == head.X && p.Y == head.Y);
        }
    }

    class Program
    {
        private const int MapWidth = 40;
        private const int MapHeight = 20;
        private const int FrameMilliseconds = 100;
        private static readonly Random Random = new Random();

        static void Main()
        {
            SetWindowSize(MapWidth + 2, MapHeight + 2);
            CursorVisible = false;
            while (true)
            {
                RunGame();
                Thread.Sleep(2000);
                Clear();
                ReadKey();
            }
        }

        static void RunGame()
        {
            Clear();
            DrawBorders();
            Snake snake = new Snake(MapWidth / 2, MapHeight / 2);
            Pixel food = GenerateFood(snake);
            food.Draw();
            int score = 0;

            while (true)
            {
                if (KeyAvailable)
                {
                    ConsoleKey key = ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.UpArrow when snake.CurrentDirection != Direction.Down:
                            snake.CurrentDirection = Direction.Up;
                            break;
                        case ConsoleKey.DownArrow when snake.CurrentDirection != Direction.Up:
                            snake.CurrentDirection = Direction.Down;
                            break;
                        case ConsoleKey.LeftArrow when snake.CurrentDirection != Direction.Right:
                            snake.CurrentDirection = Direction.Left;
                            break;
                        case ConsoleKey.RightArrow when snake.CurrentDirection != Direction.Left:
                            snake.CurrentDirection = Direction.Right;
                            break;
                    }
                }

                if (snake.Body.Last().X == food.X && snake.Body.Last().Y == food.Y)
                {
                    snake.Grow();
                    food = GenerateFood(snake);
                    food.Draw();
                    score++;
                }

                snake.Move();
                if (snake.CheckCollision(MapWidth, MapHeight)) break;

                Thread.Sleep(FrameMilliseconds);
            }

            SetCursorPosition(MapWidth / 3, MapHeight / 2);
            WriteLine($"Game Over! Score: {score}");
        }

        static void DrawBorders()
        {
            Clear();
            for (int x = 0; x < MapWidth; x++)
            {
                new Pixel(x, 0, ConsoleColor.Gray).Draw();
                new Pixel(x, MapHeight - 1, ConsoleColor.Gray).Draw();
            }
            for (int y = 0; y < MapHeight; y++)
            {
                new Pixel(0, y, ConsoleColor.Gray).Draw();
                new Pixel(MapWidth - 1, y, ConsoleColor.Gray).Draw();
            }
        }

        static Pixel GenerateFood(Snake snake)
        {
            Pixel food;
            do
            {
                food = new Pixel(Random.Next(1, MapWidth - 2), Random.Next(1, MapHeight - 2), ConsoleColor.Red);
            } while (snake.Body.Any(p => p.X == food.X && p.Y == food.Y));
            return food;
        }
    }
}
