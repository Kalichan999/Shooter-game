using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;

namespace ShootingGame
{
    public class Constant
    {
        public const string BulletSymbol = "*";
        public const string AnimalSymbol = "#";
        public const string Pistol = "Pistol";
        public const string Rifle = "Rifle";
        public const string PistolSymbol = "-";
        public const string RifleSymbol = "=";
        public const int GameTimeSeconds = 35;
    }

    public class Coordinate
    {
        public int X;
        public int Y;
        public string Symbol;
    }

    public class Gun : Coordinate
    {
        public int Id;
        public string Name;
        public int Speed;
    }

    class Bullet : Coordinate
    {
    }

    class Animal : Coordinate
    {
    }

    public class Program
    {
        private static readonly List<Gun> guns = new List<Gun> { new Gun { Id = 1, Name = Constant.Pistol, Speed = 60, Symbol = Constant.PistolSymbol }, new Gun { Id = 2, Name = Constant.Rifle, Speed = 30, Symbol = Constant.RifleSymbol } };

        static Gun curGun;
        static Bullet curBullet;
        static Animal curAnimal;
        static int points = 0;

        static int seconds = Constant.GameTimeSeconds;
        static System.Timers.Timer timer;

        static void Main(string[] args)
        {
            Init();
            curGun = SelectGun();
            if (curGun == null)
                InvalidSelection();
            else
                Console.Clear();

            StartTimer();
            PrintHeader(points);
            MoveGun(0);
            ShowAnimal();

            ConsoleKeyInfo keyInfo;
            while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Escape)
            {
                if (keyInfo.Key == ConsoleKey.UpArrow)
                    MoveGun(-1);
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                    MoveGun(1);
                else if (keyInfo.Key == ConsoleKey.Spacebar)
                {
                    curBullet = new Bullet
                    {
                        X = 2,
                        Y = curGun.Y,
                        Symbol = Constant.BulletSymbol
                    };
                    MoveBullet();
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                    RestartGame();
            }
        }


        private static void StartTimer()
        {
            timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Interval = 1000;
            timer.Enabled = true;
        }


        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            seconds--;
            PrintHeader(points);
            if (seconds == 0)
            {
                Console.Clear();
                timer.Stop();

                string str = "Game Over!!!!\nYour Score: " + points;
                Console.WriteLine(str);

                Console.WriteLine("\n\nEnter: Restart the Game\nEsc: Exit");
            }
        }


        public static Gun SelectGun()
        {
            Console.Write("Controls\nMove Gun Up: Up Arrow\nMove Gun Down: Down Arrow\nShoot: Space Bar\nExit: Esc\nAnimal: #\nGun: -/=\n");
            Console.WriteLine("\nSelect Gun\n1. Pistol (Speed 3/5)\n2. Rifle (Speed 5/5)\n\nPress 1 or 2\nand Hit Enter to Start");

            string strSelectedGun = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(strSelectedGun))
            {
                bool success = int.TryParse(strSelectedGun, out int selectedGun);
                if (success)
                {
                    Gun gun = guns.Where(x => x.Id == selectedGun).FirstOrDefault();
                    if (gun != null)
                    {
                        gun.X = 0;
                        gun.Y = 1;
                        return gun;
                    }
                }
            }

            return null;
        }


        static void InvalidSelection()
        {
            Console.WriteLine("Invalid Selection!!!\nPress Enter to start again");
            Console.ReadLine();
            Console.Clear();
            SelectGun();
        }


        static void Init()
        {
            Console.BufferHeight = Console.WindowHeight = 15;
            Console.BufferWidth = Console.WindowWidth = 30;
        }


        static void PrintObject(int x, int y, string str)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(str);
        }


        static void ShowAnimal()
        {
            Random random = new Random();

            curAnimal = new Animal
            {
                X = random.Next(10, Console.WindowWidth),
                Y = random.Next(1, Console.WindowHeight),
                Symbol = Constant.AnimalSymbol
            };

            PrintObject(curAnimal.X, curAnimal.Y, curAnimal.Symbol);
        }


        static void MoveBullet()
        {
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Thread.Sleep(curGun.Speed);

                Bullet newBullet = new Bullet
                {
                    X = curBullet.X + 1,
                    Y = curBullet.Y,
                    Symbol = Constant.BulletSymbol
                };

                PrintObject(curBullet.X, curBullet.Y, " ");
                PrintObject(newBullet.X, newBullet.Y, "*");

                curBullet = newBullet;


                if (curAnimal.X == curBullet.X && curAnimal.Y == curBullet.Y)
                {
                    PrintObject(curAnimal.X, curAnimal.Y, " ");
                    PrintHeader(points += 10);
                    ShowAnimal();
                    break;
                }

                if (curBullet.X == (Console.WindowWidth - 1))
                {
                    PrintObject(curBullet.X, curBullet.Y, " ");
                    break;
                }
            }
        }


        static void PrintHeader(int pts)
        {
            PrintObject(0, 0, new string(' ', Console.WindowWidth));
            PrintObject(0, 0, string.Format("Points: {0}, Time: {1}", pts, seconds));
        }


        static void MoveGun(int y)
        {
            int newPos = curGun.Y + y;
            if (newPos > 0 && newPos < Console.WindowHeight)
            {
                Gun newGun = curGun;
                PrintObject(0, curGun.Y, " ");
                newGun.Y = newPos;

                PrintObject(0, newGun.Y, newGun.Symbol);

                curGun = newGun;
            }
        }


        static void RestartGame()
        {
            curGun = null;
            curBullet = null;
            curAnimal = null;
            points = 0;

            seconds = 35;
            timer = null;

            Console.Clear();
            Main(null);
        }
    }
}
