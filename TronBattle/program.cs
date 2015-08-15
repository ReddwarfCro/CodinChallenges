using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace TronBattle
{

    public class Move
    {
        public static string Up = "UP";
        public static string Down = "DOWN";
        public static string Left = "LEFT";
        public static string Right = "RIGHT";
    }

    public class Map
    {
        public Map()
        {
            MapArray = new Field[30, 20];

            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    MapArray[i, j] = new Field();
                }
            }
        }
        public static Field[,] MapArray { get; set; }
        public static Field[][] ss { get; set; }
    }

    public class Field
    {
        public Field()
        {
            OwnerId = -1;
        }

        public int OwnerId { get; set; }
    }

    public class Enemy
    {
        public Coords Position { get; set; }
        public Coords LastPosition { get; set; }
        public double Distance { get; set; }
        public int PlayerId { get; set; }
    }

    public class Coords
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class CanGo
    {
        public const int checkfld = 3;
        public Coords Position = new Coords();

        public bool Left(Coords pos)
        {
            if (pos.X > 0)
            {
                Position.X = pos.X - 1;
                Position.Y = pos.Y;
                if (Map.MapArray[Position.X, Position.Y] != null)
                {
                    if (Map.MapArray[Position.X, Position.Y].OwnerId == -1)
                        return true;
                }
            }
            return false;
        }

        public bool LeftTwo(Coords pos)
        {
            bool canGo = true;
            Position.X = pos.X;
            for (int i = 0; i < checkfld; i++)
            {
                if (Position.X > 0)
                {
                    Position.X = Position.X - 1;
                    Position.Y = pos.Y;
                    if (Map.MapArray[Position.X, Position.Y] != null)
                    {
                        if (Map.MapArray[Position.X, Position.Y].OwnerId == -1 && canGo == true)
                        {
                            canGo = true;
                        }
                        else
                        {
                            canGo = false;
                        }
                    }
                }
                else
                {
                    canGo = false;
                }
            }
            return canGo;
        }

        public bool Right(Coords pos)
        {
            if (pos.X < 29)
            {
                Position.X = pos.X + 1;
                Position.Y = pos.Y;
                if (Map.MapArray[Position.X, Position.Y] != null)
                {
                    if (Map.MapArray[Position.X, Position.Y].OwnerId == -1)
                        return true;
                }
            }
            return false;
        }

        public bool RightTwo(Coords pos)
        {
            bool canGo = true;
            Position.X = pos.X;
            for (int i = 0; i < checkfld; i++)
            {
                if (Position.X < 29)
                {
                    Position.X = Position.X + 1;
                    Position.Y = pos.Y;
                    if (Map.MapArray[Position.X, Position.Y] != null)
                    {
                        if (Map.MapArray[Position.X, Position.Y].OwnerId == -1 && canGo == true)
                        {
                            canGo = true;
                        }
                        else
                        {
                            canGo = false;
                        }
                    }
                }
                else
                {
                    canGo = false;
                }
            }
            return canGo;
        }

        public bool Up(Coords pos)
        {
            if (pos.Y > 0)
            {
                Position.X = pos.X;
                Position.Y = pos.Y - 1;
                if (Map.MapArray[Position.X, Position.Y] != null)
                {
                    if (Map.MapArray[Position.X, Position.Y].OwnerId == -1)
                        return true;
                }
            }
            return false;
        }

        public bool UpTwo(Coords pos)
        {
            bool canGo = true;
            Position.Y = pos.Y;
            for (int i = 0; i < checkfld; i++)
            {
                if (Position.Y > 0)
                {
                    Position.X = pos.X;
                    Position.Y = Position.Y - 1;
                    if (Map.MapArray[Position.X, Position.Y] != null)
                    {
                        if (Map.MapArray[Position.X, Position.Y].OwnerId == -1 && canGo == true)
                        {
                            canGo = true;
                        }
                        else
                        {
                            canGo = false;
                        }
                    }
                }
                else
                {
                    canGo = false;
                }
            }
            return canGo;
        }

        public bool Down(Coords pos)
        {
            if (pos.Y < 19)
            {
                Position.X = pos.X;
                Position.Y = pos.Y + 1;
                if (Map.MapArray[Position.X, Position.Y] != null)
                {
                    if (Map.MapArray[Position.X, Position.Y].OwnerId == -1)
                        return true;
                }
            }
            return false;
        }

        public bool DownTwo(Coords pos)
        {
            bool canGo = true;
            Position.Y = pos.Y;
            for (int i = 0; i < checkfld; i++)
            {
                if (Position.Y < 19)
                {
                    Position.X = pos.X;
                    Position.Y = Position.Y + 1;
                    if (Map.MapArray[Position.X, Position.Y] != null)
                    {
                        if (Map.MapArray[Position.X, Position.Y].OwnerId == -1 && canGo == true)
                        {
                            canGo = true;
                        }
                        else
                        {
                            canGo = false;
                        }
                    }
                }
                else
                {
                    canGo = false;
                }
            }
            return canGo;
        }
    }

    public class Game
    {
        public Game()
        {
            MyId = -1;
            MyPosition = new Coords { X = -1, Y = -1 };
            Targets = new List<Enemy>(3);
        }
        public void Init()
        {
            // ReSharper disable once UnusedVariable
            var map = new Map();
        }

        public int MyId { get; set; }
        public Coords MyPosition { get; set; }
        public List<Enemy> Targets { get; set; }

        public double GetDistance(Coords start, Coords end)
        {
            int y = end.Y - start.Y;
            int x = end.X - start.X;
            double dist = Math.Sqrt(x * x + y * y);
            return dist;
        }

    }

    static class Player
    {


        static void Main()
        {
            var game = new Game();


            game.Init();

            string[] inputs;
            string direction = "";
            // game loop
            while (true)
            {
                // ReSharper disable once PossibleNullReferenceException
                inputs = Console.ReadLine().Split(' ');
                int noOfPlayers = int.Parse(inputs[0]); // total number of players (2 to 4).
                game.MyId = int.Parse(inputs[1]); // your player number (0 to 3).

                for (int i = 0; i < noOfPlayers; i++)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    inputs = Console.ReadLine().Split(' ');
                    int startX = int.Parse(inputs[0]); // starting X coordinate of lightcycle (or -1)
                    int startY = int.Parse(inputs[1]); // starting Y coordinate of lightcycle (or -1)
                    int tronX = int.Parse(inputs[2]); // starting X coordinate of lightcycle (can be the same as X0 if you play before this player)
                    int tronY = int.Parse(inputs[3]); // starting Y coordinate of lightcycle (can be the same as Y0 if you play before this player)
                    if (startX != -1)
                    {
                        Map.MapArray[startX, startY].OwnerId = i;
                        Map.MapArray[tronX, tronY].OwnerId = i;
                        if (i == game.MyId)
                        {
                            game.MyPosition.X = tronX;
                            game.MyPosition.Y = tronY;
                        }
                        else
                        {
                            if (!game.Targets.Any())
                            {
                                game.Targets.Add(new Enemy { Position = new Coords { X = tronX, Y = tronY }, PlayerId = i });
                            }
                            else
                            {
                                var target = game.Targets.FirstOrDefault(w => w.PlayerId == i);
                                if (target != null)
                                {
                                    target.LastPosition = target.Position;
                                    target.Position.X = tronX;
                                    target.Position.Y = tronY;
                                }
                            }
                        }
                    }
                    else
                    {
                        ClearDeadBody(i);
                    }

                }
                Console.WriteLine(GetWhereToGo(direction, game));

            }
            // ReSharper disable once FunctionNeverReturns
        }

        public static string GetWhereToGo(string direction, Game game) {

            foreach (var target in game.Targets)
            {
                target.Distance = game.GetDistance(game.MyPosition, target.Position);
            }

            var primTarget = game.Targets.OrderBy(o => o.Distance).FirstOrDefault(d => d.Distance < 4);


            if (primTarget != null)
            {
                direction = GetOrientation(game.MyPosition, primTarget);
            }

            var compass = new CanGo();

            if (compass.UpTwo(game.MyPosition) && direction == Move.Up)
            {
                direction = Move.Up;
            }
            else if (compass.RightTwo(game.MyPosition) && direction == Move.Right)
            {
                direction = Move.Right;
            }
            else if (compass.DownTwo(game.MyPosition) && direction == Move.Down)
            {
                direction = Move.Down;
            }
            else if (compass.LeftTwo(game.MyPosition) && direction == Move.Left)
            {
                direction = Move.Left;
            }
            else
            {
                if (compass.UpTwo(game.MyPosition))
                {
                    direction = Move.Up;
                }
                else if (compass.RightTwo(game.MyPosition))
                {
                    direction = Move.Right;
                }
                else if (compass.DownTwo(game.MyPosition))
                {
                    direction = Move.Down;
                }
                else if (compass.LeftTwo(game.MyPosition))
                {
                    direction = Move.Left;
                }
                else if (compass.Up(game.MyPosition))
                {
                    direction = Move.Up;
                }
                else if (compass.Right(game.MyPosition))
                {
                    direction = Move.Right;
                }
                else if (compass.Down(game.MyPosition))
                {
                    direction = Move.Down;
                }
                else if (compass.Left(game.MyPosition))
                {
                    direction = Move.Left;
                }

            }
            return direction;
        }

        public static string GetOrientation(Coords my, Enemy enemy)
        {
            string where = "";

            var enemyCords = GetNextCell(enemy.LastPosition, enemy.Position);

            if (my.X < enemyCords.X)
            {
                where = Move.Right;
            }
            else if (my.X > enemyCords.X)
            {
                where = Move.Left;
            }
            else if (my.Y < enemyCords.Y)
            {
                where = Move.Down;
            }
            else if (my.Y > enemyCords.Y)
            {
                where = Move.Up;
            }

            return where;
        }

        public static Coords GetNextCell(Coords prevCoords, Coords currCoords)
        {
            Coords where = currCoords;
            if (prevCoords == null)
                prevCoords = currCoords;
            if (prevCoords.X == currCoords.X)
            {
                where.X = currCoords.X;
                if (prevCoords.Y < currCoords.Y)
                {
                    where.Y = currCoords.Y + 1;
                }
                else
                {
                    where.Y = currCoords.Y - 1;
                }

            }
            else
            {
                where.Y = currCoords.Y;
                if (prevCoords.X < currCoords.X)
                {
                    where.X = currCoords.X + 1;
                }
                else
                {
                    where.X = currCoords.X - 1;
                }
            }

            return where;
        }

        public static void ClearDeadBody(int deadBodyId)
        {
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    if (Map.MapArray[i, j].OwnerId == deadBodyId)
                    {
                        Map.MapArray[i, j].OwnerId = -1;
                    }
                }
            }
        }
    }
}
