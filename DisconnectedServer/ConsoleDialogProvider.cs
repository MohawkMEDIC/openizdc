using DisconnectedClient.Core;
using System;

namespace DisconnectedServer
{
    /// <summary>
    /// Represents a console based dialog provider
    /// </summary>
    internal class ConsoleDialogProvider : IDialogProvider
    {
        public ConsoleDialogProvider()
        {
        }

        /// <summary>
        /// Alert has been raised
        /// </summary>
        public void Alert(string text)
        {
            Console.WriteLine("ALERT >>>> {0}", text);
        }

        /// <summary>
        /// Confirmation dialog
        /// </summary>
        /// <param name="text"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public bool Confirm(string text, string title)
        {
            Console.WriteLine("CONFIRM >>>> {0}", text);
            return true;
        }
    }
}