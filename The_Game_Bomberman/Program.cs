using System;
using System.Collections.Generic;
using System.Threading;

namespace The_Game_Bomberman
{

    class MainClass
    {
        static List<List<Cell>> polegon = new List<List<Cell>>();

        private static GameArea area = null;

        public static void Main(string[] args)
        {

            Console.Clear();
            Console.Write("How are you? I'm sure you want to play in very exciting game!\nAre you interested?\nWell!\n");
           
            bool gameFinished = false;

            while (!gameFinished)
            {
                int blocksToGenerate = AskBlocksCount();

                Console.Clear();

                string[] polegonPattern = @"
1HHHHHHHHHHHHHHHHHHHHHHHHH2
V   YXY       YXY         V
VUW WYW W W W WYW W W W W V
V                         V
V W W W WYW W W W W W W W V
V       YXY               V
V W W W WYW W W W W W W W V
V                         V
V W W W W W W W W WYW W W V
V                 YXY     V
V W W W W W W W W WYW W W V
V                         V
V W W W W W W W W W W W W V
V                         V
V W WYW W W W W W W W W W V
V   YXY                   V
V W WYW W W W W W W W W W V
V                        TV
4HHHHHHHHHHHHHHHHHHHHHHHHH3   



 
     ".Trim().Split("\n".ToCharArray());

                area = new GameArea(polegonPattern);
                area.GenerateBlocks(blocksToGenerate);
                area.Print();

                try
                {
                    bool shouldRedrawArea = false;

                    while (true)
                    {
                        if (area.TryBangBombsByTimer())
                        {
                            shouldRedrawArea = true;
                        }

                        if (Console.KeyAvailable)
                        {
                            var keyInfo = Console.ReadKey();
                            HandleMoveEvents(keyInfo);
                            HandleBombEvents(keyInfo);
                            shouldRedrawArea = true;
                        }

                        if (shouldRedrawArea)
                        {
                            area.Print();
                        }

                        shouldRedrawArea = false;
                        Thread.Sleep(16);
                    }
                }
                catch (GameInterruptedException exc)
                {
                    area.Print();

                    switch (exc.result)
                    {
                        case GameFinishResult.WIN:
                            Console.Clear();
                            Console.Write("Player is winner!\nPlease, press any key to start the next game!");
                            Console.ReadLine();
                            break;

                        case GameFinishResult.LOSE:
                            Console.Clear();
                            Console.Write("Player is dead!\nPlease, press any key to start the next game!");
                            Console.ReadLine();
                            break;
                    }

                    gameFinished = !AskShouldPlayAgain();
                    Console.Clear();
                }
            }
        }

        static int AskBlocksCount()
        {
            Console.Write($"\nВведите кол-во блоков (min = {GameArea.MIN_BLOCKS_COUNT}, max = {GameArea.MAX_BLOCKS_COUNT}): ");

            while (true)
            {
                try
                {
                    
                    if (GameArea.MAX_BLOCKS_COUNT > 200) {
                        Console.Write("Вы можете создать не более 200 блоков!");
                    }
                    return int.Parse(Console.ReadLine());
                }
                catch
                {
                   
                    Console.Write("Ввод не верен, повторите: ");
                }
            }
        }



        static bool AskShouldPlayAgain()
        {
            Console.Write("Повторить игру (yes|no)?\nОтвет: ");

            while (true)
            {
                try
                {
                    switch (Console.ReadLine())
                    {
                        case "yes":
                            return true;
                        case "no":
                            return false;
                        default:
                            throw new Exception("Incorrect input!");
                    }
                }
                catch
                {
                    Console.Write("Ввод не верен, повторите:");
                }
            }
        }



        static void TryMove(int currentRow, int currentCol, int deltaRow, int deltaCol)            // движение происходит только в том случае, если мы можем
        {                                                                                          // сместиться на необходимую клетку.
            if (area.CanMove(currentRow + deltaRow, currentCol + deltaCol))
            {
                area.Move(currentRow + deltaRow, currentCol + deltaCol);
            }
        }




        static void MoveAction(int deltaRow, int deltaCol)                 // движение игрока на определенную клетку(величину), то есть шаг
        {
            Point playerPoint = area.GetPlayerPoint();
            TryMove(playerPoint.row, playerPoint.col, deltaRow, deltaCol);
        }






        static void HandleMoveEvents(ConsoleKeyInfo keyInfo)               // обрабатывание команд движения игрока.
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    MoveAction(-1, 0);
                    break;
                case ConsoleKey.LeftArrow:
                    MoveAction(0, -1);
                    break;
                case ConsoleKey.RightArrow:
                    MoveAction(0, 1);
                    break;
                case ConsoleKey.DownArrow:
                    MoveAction(1, 0);
                    break;
                default:
                    break;
            }

            if (area.IsPlayerAchivedTarget())
            {
                throw new GameInterruptedException(GameFinishResult.WIN);
            }

            if (area.IsPlayerAchivedHostile())
            {
                throw new GameInterruptedException(GameFinishResult.LOSE);
            }
        }




        private static void HandleBombEvents(ConsoleKeyInfo keyInfo)         // метод, который обрабатывает нажатие клавиши,
        {                                                                    // то есть установка и взрыв бомбы.

            if (keyInfo.Key == ConsoleKey.Spacebar)                              // если мы нажали на Spacebar, то мы установили бомбу на том месте,
                                                                                 // где находится игрок.
            {
                Point playerPoint = area.GetPlayerPoint();
                area.SetBomb(playerPoint.row, playerPoint.col);
            }
            else if (keyInfo.Key == ConsoleKey.Enter)                         // если мы нажали на Enter, то все бомбы взрываются.
            {

                area.BangAllExistenBombs();
            }
        }
    }
}