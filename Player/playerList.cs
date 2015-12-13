using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    public  class PlayerList
    {
        public string songName { get; set; }
        private string artist { get; set; }
        private string album { get; set; }
        private string filePath { get; set; }

        public  PlayerList(string name, string artistName, string albumName, string path)
        {
            songName = name;
            artist = artistName;
            album = albumName;
            filePath = path;
        }
    }
}
