using System;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using static System.Console;

namespace zmeyka
{
    class Program
    {
        private const int MapWidth = 30;
        private const int MapHeight = 20;

        private const int ScreenWidth = MapWidth*3;
        private const int ScreenHeight = MapHeight * 3;

        private const int FrameMs = 10;

        private const ConsoleColor BorderColor = ConsoleColor.Gray;

        private const ConsoleColor HeadColor = ConsoleColor.Red;
        private const ConsoleColor BodyColor = ConsoleColor.Yellow;

        private const ConsoleColor FoodColor = ConsoleColor.Red;

        private static readonly Random Random = new Random();

        static void Main(string[] args)
        {
            SetWindowSize(ScreenWidth, ScreenHeight);
            SetBufferSize(ScreenWidth, ScreenHeight);
            CursorVisible = false;

            while (true)
            {
                StartGame();

                Thread.Sleep(1000);
                ReadKey();
            }

            
        }


        static void StartGame()
        {
            Clear();

            DrawBorder();

            Direction currentMoevement = Direction.Right;

            var snake = new Snake(10, 5, HeadColor, BodyColor);

            Pixel food = GenFood(snake);
            food.Draw();

            int score = 0;

            Stopwatch sw = new Stopwatch();

            while (true)
            {
                sw.Restart();

                Direction oldMoevement = currentMoevement;

                while (sw.ElapsedMilliseconds <= FrameMs)
                    if (currentMoevement == oldMoevement)
                    {
                        currentMoevement = ReadMoevement(currentMoevement);
                    }

                if(snake.Head.X == food.X && snake.Head.Y == food.Y)
                {
                    snake.Move(currentMoevement, true);


                    food = GenFood(snake);
                    food.Draw();

                    score++;


                    Task.Run(() => Beep(1200, 200));
                }
                else
                {
                    snake.Move(currentMoevement); 
                }



                if (snake.Head.X == MapWidth - 1
                    || snake.Head.X == 0
                    || snake.Head.Y == MapHeight - 1
                    || snake.Head.Y == 0
                    || snake.Body.Any(b => b.X == snake.Head.X && b.Y == snake.Head.Y))
                    break;

            }

            snake.Clear();
            food.Clear();

            SetCursorPosition(ScreenWidth / 3, ScreenHeight / 2);
            WriteLine("Game over, score: " + score);

            Task.Run(() => Beep(200, 700));
        }


        static Pixel GenFood(Snake snake)
        {
            Pixel food;

            do
            {
                food = new Pixel(Random.Next(1, MapWidth - 2), Random.Next(1, MapHeight - 2), FoodColor);
            } while (snake.Head.X == food.X && snake.Head.Y == food.Y
                    || snake.Body.Any(b => b.X == food.X && b.Y == food.Y));


            return food;
        }

        static Direction ReadMoevement(Direction currentDirection)
        {
            if (!KeyAvailable)
                return currentDirection;

            ConsoleKey key = ReadKey(true).Key;

            currentDirection = key switch
            {
                ConsoleKey.UpArrow when currentDirection != Direction.Down => Direction.Up,
                ConsoleKey.DownArrow when currentDirection != Direction.Up => Direction.Down,
                ConsoleKey.RightArrow when currentDirection != Direction.Left => Direction.Right,
                ConsoleKey.LeftArrow when currentDirection != Direction.Right => Direction.Left,

                _ => currentDirection
            };

            return currentDirection;
        }

        static void DrawBorder()
        {
            for (int i = 0; i < MapWidth-1; i++)
            {
                new Pixel(i, 0, BorderColor).Draw();
                new Pixel(i, MapHeight-2, BorderColor).Draw();
            }

            for (int i = 0; i < MapHeight-1; i++)
            {
                new Pixel(0, i, BorderColor).Draw();
                new Pixel(MapWidth - 1, i, BorderColor).Draw();
            }
        }
    }
}
