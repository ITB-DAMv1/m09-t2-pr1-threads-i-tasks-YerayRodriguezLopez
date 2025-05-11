namespace Asteroids
{
    class Program
    {
        static int shipPos;
        static int width = Console.WindowWidth;
        static int height = Console.WindowHeight;
        static bool running = true;
        static List<(int x, int y)> asteroids = new();
        static int dodged = 0;
        static int lives = 0;
        static object lockObj = new();
        static DateTime startTime;
        const int maxLives = 3;
        const int minDelay = 50; // capped minimum speed (ms)

        static async Task Main(string[] args)
        {
            StartGame();

            Task inputTask = Task.Run(HandleInput);
            Task logicTask = Task.Run(GameLogic);
            Task drawTask = Task.Run(DrawLoop);

            await Task.WhenAny( Task.Run(() =>
            {
                while (running) Thread.Sleep(100);
            }));

            running = false;
            Console.Clear();
            Console.WriteLine($"Game Over. Dodged: {dodged}, Lives: {lives}");
        }

        static void StartGame()
        {
            shipPos = width / 2;
            startTime = DateTime.Now;
            dodged = 0;
            lives = 0;
            asteroids.Clear();
        }

        static async Task HandleInput()
        {
            while (running)
            {
                var key = Console.ReadKey(true).Key;
                lock (lockObj)
                {
                    if ((key == ConsoleKey.A || key == ConsoleKey.LeftArrow) && shipPos > 0) shipPos--;
                    else if ((key == ConsoleKey.D || key == ConsoleKey.RightArrow) && shipPos < width - 1) shipPos++;
                    else if (key == ConsoleKey.Q) running = false;
                }
            }
        }

        static async Task GameLogic()
        {
            Random rand = new();
            while (running)
            {
                int elapsedSeconds = (int)(DateTime.Now - startTime).TotalSeconds;
                int delay = Math.Max(200 - elapsedSeconds * 5, minDelay);

                lock (lockObj)
                {
                    var newX = rand.Next(0, width);
                    asteroids.Add((newX, 0));

                    for (int i = asteroids.Count - 1; i >= 0; i--)
                    {
                        var (x, y) = asteroids[i];
                        y++;

                        if (y >= height - 1)
                        {
                            if (x == shipPos)
                            {
                                lives++;
                                Console.Beep();
                                if (lives >= maxLives)
                                {
                                    running = false;
                                }
                                else
                                {
                                    asteroids.Clear();
                                    shipPos = width / 2;
                                    startTime = DateTime.Now;
                                }
                                break;
                            }
                            else
                            {
                                dodged++;
                                asteroids.RemoveAt(i);
                            }
                        }
                        else
                        {
                            asteroids[i] = (x, y);
                        }
                    }
                }
                await Task.Delay(delay);
            }
        }

        static async Task DrawLoop()
        {
            while (running)
            {
                Console.Clear();
                lock (lockObj)
                {
                    foreach (var (x, y) in asteroids)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write("*");
                    }
                    Console.SetCursorPosition(shipPos, height - 1);
                    Console.Write("^");
                }
                await Task.Delay(50);
            }
        }
    }
}
