using System;
namespace The_Game_Bomberman
{
    public abstract class Utils
    {

      public static long CurrentTimeMillis() // возвращение времени в миллисекундах.
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }
}
