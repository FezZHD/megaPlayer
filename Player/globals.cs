using System;
using System.Collections.Generic;


namespace Player
{
    public class globals
    {
        public string[] filesPath { get; set; }
        public static bool mediaPlayerIsPlaying = false;
        public static bool userIsDraggingSlider = false;
        public static int clickedItemIndex;
        public static List<PlayerList> saveList; 
        public static bool ignoreChange = false;
        public static double albumPicHeight;
        public static double albumPicWidth;

    }
}
