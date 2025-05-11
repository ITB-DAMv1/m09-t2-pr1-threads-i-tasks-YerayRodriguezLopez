using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingsDinner
{
    public class Philosopher
    {
        private int id;
        private Chopstick leftChopstick;
        private Chopstick rightChopstick;
        private Random random = new Random();
        private int eatCount = 0;
        private TimeSpan maxHungryTime = TimeSpan.Zero;
        private DateTime lastHungryStart;
        private CancellationToken token;

        public Philosopher(int id, Chopstick left, Chopstick right, CancellationToken token)
        {
            this.id = id;
            leftChopstick = left;
            rightChopstick = right;
            this.token = token;
        }

        public void Dine()
        {
            while (!token.IsCancellationRequested)
            {
                Think();
                lastHungryStart = DateTime.Now;

                bool hasEaten = false;
                lock (leftChopstick.Lock)
                {
                    Log($"picked up left chopstick", ConsoleColor.DarkYellow);

                    if (Monitor.TryEnter(rightChopstick.Lock, 1000))
                    {
                        try
                        {
                            Log($"picked up right chopstick", ConsoleColor.DarkYellow);

                            TimeSpan hungryDuration = DateTime.Now - lastHungryStart;
                            if (hungryDuration > maxHungryTime) maxHungryTime = hungryDuration;
                            if (hungryDuration.TotalSeconds > 15)
                            {
                                Log($"was hungry too long", ConsoleColor.Red);
                                Environment.Exit(1);
                            }

                            Eat();
                            eatCount++;
                            hasEaten = true;
                        }
                        finally
                        {
                            Log($"released right chopstick", ConsoleColor.DarkCyan);
                            Monitor.Exit(rightChopstick.Lock);
                        }
                    }
                    else
                    {
                        Log($"could not pick up right chopstick", ConsoleColor.Magenta);
                    }

                    Log($"released left chopstick", ConsoleColor.DarkCyan);
                }

                if (!hasEaten)
                    Thread.Sleep(random.Next(100, 300)); // Wait a bit before retrying
            }
        }

        private void Think()
        {
            Log("is thinking", ConsoleColor.DarkBlue);
            Thread.Sleep(random.Next(500, 2000));
        }

        private void Eat()
        {
            Log("is eating", ConsoleColor.DarkGreen);
            Thread.Sleep(random.Next(500, 1000));
        }

        private void Log(string action, ConsoleColor bg)
        {
            Console.BackgroundColor = bg;
            Console.ForegroundColor = (ConsoleColor)(id % 15);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Philosopher {id} {action}");
            Console.ResetColor();
        }

        public string GetStatistics()
        {
            return $"{id},{maxHungryTime.TotalSeconds:F2},{eatCount}";
        }
    }
}
