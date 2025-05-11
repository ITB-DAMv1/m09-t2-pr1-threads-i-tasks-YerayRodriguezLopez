namespace KingsDinner
{
    class Program
    {
        static void Main()
        {
            Chopstick[] chopsticks = new Chopstick[5];
            for (int i = 0; i < 5; i++) chopsticks[i] = new Chopstick();

            CancellationTokenSource cts = new CancellationTokenSource();
            Philosopher[] philosophers = new Philosopher[5];
            Thread[] threads = new Thread[5];

            for (int i = 0; i < 5; i++)
            {
                philosophers[i] = new Philosopher(i, chopsticks[i], chopsticks[(i + 1) % 5], cts.Token);
                threads[i] = new Thread(philosophers[i].Dine);
                threads[i].Start();
            }

            Thread.Sleep(30000);
            cts.Cancel();
            Thread.Sleep(1000);

            using (StreamWriter sw = new StreamWriter("../../../stats.csv"))
            {
                sw.WriteLine("PhilosopherID,MaxHungryTime(s),EatCount");
                foreach (var p in philosophers)
                    sw.WriteLine(p.GetStatistics());
            }

            using (StreamReader sr = new StreamReader("../../../stats.csv"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                    Console.WriteLine(line);
            }

            Environment.Exit(0);
        }
    }
}
