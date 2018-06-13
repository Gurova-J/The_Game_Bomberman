using System;
namespace The_Game_Bomberman
{
    public class GameInterruptedException : Exception
    {
        public readonly GameFinishResult result;

        public GameInterruptedException(GameFinishResult result) => this.result = result;
        
    }
}
