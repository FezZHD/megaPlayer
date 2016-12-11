namespace Player.Types
{
    public class PlayerList
    {
        public string SongName { get; private set; }
        public string Artist { get; private set; }
        public string Album { get; private set; }
        public string FilePath { get; private set; }

        public PlayerList(string name, string artistName, string albumName, string path)
        {
            SongName = name;
            Artist = artistName;
            Album = albumName;
            FilePath = path;
        }
    }
}
