using System;

namespace Ping_Pong
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (PingPongGame game = new PingPongGame())
            {
                game.Run();
            }
        }
    }
}

