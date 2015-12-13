namespace Player
{
    public  class PlayerList
    {
        public string SongName { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string FilePath { get; set; }

        public PlayerList(string name, string artistName, string albumName, string path)
        {
            SongName = name;
            Artist = artistName;
            Album = albumName;
            FilePath = path;
        }
    }
}
