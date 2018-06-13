namespace The_Game_Bomberman
{
    public class Cell
    {
        public CellType type;
        public bool fire = false;
        public bool bomb = false;
        public bool target = false;
        public bool hostile = false;
        public string image;
        public long actionTime = -1;

        public Cell(CellType type)
        {
            this.type = type;
            this.image = "";
        }

        public Cell(CellType type, bool bomb, bool target, bool hostile)
        {
            this.type = type;
            this.bomb = bomb;
            this.target = target;
            this.hostile = hostile;
            this.image = "";
        }

        public Cell(CellType type, string image)
        {
            this.type = type;
            this.image = image;
        }

        public override string ToString()
        {
            return image;
        }
    }
}
