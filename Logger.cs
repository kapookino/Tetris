using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tetris.Core;

namespace Tetris
{
    internal class Logger
    {
        public Logger()
        {
         //   GameEvents.OnRequestLog += Log;

       //     File.WriteAllText("debug.txt", string.Empty);
       //     File.AppendAllText("debug.txt","Time,Detail,Message"+ Environment.NewLine);

        }
        private void Log(string detail, string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
       //     File.AppendAllText("debug.txt",timestamp+","+ detail+","+message + Environment.NewLine);
        }
    }

}
