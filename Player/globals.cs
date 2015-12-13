using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    public class globals
    {
        public string[] filesPath { get; set; }
        public static bool mediaPlayerIsPlaying = false;
        public static bool userIsDraggingSlider = false;
        public static List<PlayerList> saveList; 
    }
}
