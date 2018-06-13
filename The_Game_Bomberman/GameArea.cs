using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace The_Game_Bomberman
{
    public class GameArea
    {
        public readonly static int MIN_BLOCKS_COUNT = 10;
        public readonly static int MAX_BLOCKS_COUNT = 200;
        private string[] pattern;
        private bool isPrepared = false;
        private Point playerPoint = null;
        private int amounthearts = 3;
        private List<List<Cell>> polegon = new List<List<Cell>>();

        readonly static string playerSymbol = "😁 ";
        readonly static string bombSymbol = "💣 ";
        readonly static string targetSymbol = "🍔 ";
        readonly static string fireSymbol = "🔥 ";
        readonly static string hostSymbol = "👿 ";
        readonly static string hearts = "❤️ ";

     
        public GameArea(string[] pattern) => this.pattern = pattern;

        public Cell this[int row, int col]
        {
            get =>  polegon[row][col];
            set => polegon[row][col] = value;
        }

        public List<Cell> this[int row]
        {
            get => polegon[row];
            set => polegon[row] = value;
        }

        private void CheckCoordsNonOutOfPolegon(int row, int col) {
            if (IsOutOfPolegon(row, col)) {
                throw new ArgumentOutOfRangeException($"row = {row}, col = {col} is out of polegon!");
            }
        }

        public void GenerateBlocks(int count)
        {
            count = Math.Min(MAX_BLOCKS_COUNT, Math.Max(MIN_BLOCKS_COUNT, count));
            string[] areaStrings = (string[])this.pattern.Clone();
            List<Point> EmptyPoints = new List<Point>();

            // находим пустые клетки, в котрых можна разместить стенку.

            for (int row = 0; row < areaStrings.Length; ++row)
            {
                for (int col = 0; col < areaStrings[row].Length; ++col)
                {
                    if (areaStrings[row][col] == ' ')
                    {
                        EmptyPoints.Add(new Point(row, col));
                    }
                }
            }

            int availablePlaces = Math.Min(EmptyPoints.Count, count);

            // перемешиваем рандомно клетки в списке.

            Random rand = new Random();
            List<Point> shuffledEmptyPoints = EmptyPoints
                .OrderBy(item => rand.Next(0, EmptyPoints.Count()))
                .ToList();

            for (int i = 0; i < availablePlaces; ++i)
            {
                Point point = shuffledEmptyPoints[i];
                string rowString = areaStrings[point.row];
                StringBuilder rowStringBuilder = new StringBuilder(rowString);
                rowStringBuilder[point.col] = 'C'; 
                areaStrings[point.row] = rowStringBuilder.ToString();
            }

            GeneratePolegon(areaStrings);
        }

        private void GeneratePolegon(string[] areaStrings)
        {
            polegon.Clear();

            for (int row = 0; row < areaStrings.Length; ++row)
            {
                List<Cell> cellsRow = new List<Cell>();

                for (int col = 0; col < areaStrings[row].Length; ++col)
                {
                    switch (areaStrings[row][col])
                    {
                        case 'X':
                            Cell cell_1 = new Cell(CellType.EMPTY, "👿 ");
                            cell_1.hostile = true;
                            cellsRow.Add(cell_1);
                            break;
                        case 'Y':
                            cellsRow.Add(new Cell(CellType.HOSTILE, "💭 "));

                            break;
                        case 'C':
                            cellsRow.Add(new Cell(CellType.DESTRUCTIBLE_WALL, "🔲 "));
                            break;
                        case 'W':
                            cellsRow.Add(new Cell(CellType.WALL, "⬛️ "));
                            break;
                        case '1':
                            cellsRow.Add(new Cell(CellType.WALL, "╔═"));
                            break;
                        case '2':
                            cellsRow.Add(new Cell(CellType.WALL, "╗ "));
                            break;
                        case '3':
                            cellsRow.Add(new Cell(CellType.WALL, "╝ "));
                            break;
                        case '4':
                            cellsRow.Add(new Cell(CellType.WALL, "╚═"));
                            break;
                        case 'V':
                            cellsRow.Add(new Cell(CellType.WALL, "║ "));
                            break;
                        case 'H':
                            cellsRow.Add(new Cell(CellType.WALL, "══"));
                            break;
                        case ' ':
                            cellsRow.Add(new Cell(CellType.EMPTY, ". "));
                            break;
                        case 'U':
                            playerPoint = new Point(row, col);
                            cellsRow.Add(new Cell(CellType.EMPTY, ". "));
                            break;
                        case 'T':
                            Cell cell = new Cell(CellType.EMPTY, "🍔 ");
                            cell.target = true;
                            cellsRow.Add(cell);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }

                polegon.Add(cellsRow);
            }

            isPrepared = true;
        }

        public void Print()
        {
            CheckPolegonInitiated();

            StringBuilder outputString = new StringBuilder(); //позволяет создать строку без лишних выделений памяти.

            for (int row = 0; row < polegon.Count; ++row)
            {
                for (int col = 0; col < this[row].Count; ++col)
                {
                    if (playerPoint.row == row && playerPoint.col == col)
                    {
                        outputString.Append(playerSymbol);
                    }
                    else
                    {
                        Cell cell = this[row, col];

                        if (cell.bomb)
                        {
                            outputString.Append(bombSymbol);
                        }
                        else if (cell.hostile)
                        {
                            outputString.Append(hostSymbol);
                        }
                        else if (cell.target)
                        {
                            outputString.Append(targetSymbol);
                        }
                        else if (cell.fire)
                        {
                            outputString.Append(fireSymbol);
                        }
                        else
                        {
                            outputString.Append(cell.ToString());
                        }
                    }
                }

                outputString.Append("\n");
            }

            outputString.Append("\n");
            outputString.Append(hearts + " " + amounthearts);
            outputString.Append("\n");

            Console.SetCursorPosition(0, 0);
            Console.Write(outputString);
        }

        public bool TryBangBombsByTimer() // проверка всех таймеров(огонь, бомбы, взрыв).
        {
            CheckPolegonInitiated();

            for (int row = 0; row < polegon.Count; ++row)
            {
                for (int col = 0; col < this[row].Count; ++col)
                {
                    Cell cell = this[row, col];

                    if (cell.bomb && cell.actionTime <= Utils.CurrentTimeMillis())
                    {
                        BangBomb(cell, row, col);
                        return true;
                    }

                    if (cell.fire && cell.actionTime <= Utils.CurrentTimeMillis())
                    {
                        PutDownFire(cell);
                        return true;
                    }
                }
            }

            return false;
        }
        public bool IsPlayerAchivedHostile() {
            
            CheckPolegonInitiated();

            return IsHostile(playerPoint.row, playerPoint.col);
        }

        public bool IsPlayerAchivedTarget() {
            
            CheckPolegonInitiated();

            return IsTarget(playerPoint.row, playerPoint.col);
        }

        private bool IsHostile(int row, int col) {
            
            CheckPolegonInitiated();

            return this[row, col].hostile;
        }

        private bool IsTarget(int row, int col) {
            
            CheckPolegonInitiated();

            return this[row, col].target;
        }

        public void SetBomb(int row, int col) // установка бомбы.
        {
            CheckPolegonInitiated();

            if (IsOutOfPolegon(row, col)) return;
            this[row, col].bomb = true;
            this[row, col].actionTime = Utils.CurrentTimeMillis() + 2000;
        }

        private void UndermineCell(int row, int col) // наносим урон клетке (взрыв).
        {
            CheckPolegonInitiated();

            if (IsOutOfPolegon(row, col)) return;

            Cell cell = this[row, col];
            cell.bomb = false;

            if (playerPoint.row == row && playerPoint.col == col)
            {
                DecrementAndCheckHearts();
                PutOnFire(row, col);
            }
            else if (cell.type == CellType.DESTRUCTIBLE_WALL || cell.type == CellType.EMPTY || cell.type == CellType.HOSTILE)
            {
                PutOnFire(row, col);
            }
        }
         
        private void PutOnFire(int row, int col) // поджигаем огонь в клетке
        {
            CheckPolegonInitiated();

            this[row, col] = new Cell(CellType.EMPTY, ". ");
            this[row, col].bomb = false;
            this[row, col].fire = true;

            this[row, col].actionTime = Utils.CurrentTimeMillis() + 500;
        }

        private void PutDownFire(Cell fireCell) // тушим огонь в клетке.
        {
            CheckPolegonInitiated();

            fireCell.bomb = false;
            fireCell.fire = false;
            fireCell.actionTime = -1;
        }

        private void BangBomb(Cell bombCell, int row, int col) // взрыв бомбы, то есть подпаливаем область взрыва.
        {
            CheckPolegonInitiated();

            if (!bombCell.bomb) throw new ArgumentException("");

            UndermineCell(row, col);
            UndermineCell(row + 1, col);
            UndermineCell(row - 1, col);
            UndermineCell(row, col + 1);
            UndermineCell(row, col - 1);
        }

        public void BangAllExistenBombs() // метод который запускает взрыв всех установленных бомб
        {
            CheckPolegonInitiated();

            for (int row = 0; row < polegon.Count; ++row)
            {
                for (int col = 0; col < this[row].Count; ++col)
                {
                    Cell cell = this[row, col];

                    if (cell.bomb)
                    {
                        BangBomb(cell, row, col);
                    }
                }
            }
        }
        private void DecrementAndCheckHearts()  // уменьшение количества жизней и прерывание игры (через исключение), 
        {                                       // если их количество равно нулю ("Player is died!").
            amounthearts--;

            if (amounthearts == 0)
            {
                throw new GameInterruptedException(GameFinishResult.LOSE);
            }
        }

        public bool IsOutOfPolegon(int row, int col)   // проверка, выходят ли эти координаты за размеры игрового поля.
        {
            CheckPolegonInitiated();

            if (row < 0) return true;
            if (col < 0) return true;
            if (row > polegon.Count - 1) return true;
            if (col > this[row].Count - 1) return true;
            return false;
        }

        public bool CanMove(int row, int col)   // проверка, можем ли мы стать на клетку по передаваемой координате(пустая ли клетка).
        {
            CheckPolegonInitiated();

            if (IsOutOfPolegon(row, col)) return false;
            return this[row, col].type == CellType.EMPTY;
        }

        public void Move(int row, int col)
        {
            CheckPolegonInitiated();

            if (this[row, col].fire)
            {
                DecrementAndCheckHearts();
            }

            playerPoint = new Point(row, col);
        }

        public Point GetPlayerPoint() {
            
            CheckPolegonInitiated();

            return playerPoint;
        }



        private void CheckPolegonInitiated()
        {
            if (!isPrepared) throw new Exception("Игровое поле не подготовлено!");
        }
    }
}
