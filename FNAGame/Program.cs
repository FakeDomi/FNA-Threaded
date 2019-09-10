using System;
using Microsoft.Xna.Framework;

namespace FNAGame
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            using (Game game = new GameClass())
            {
                game.Run();
            }
        }
    }
}
