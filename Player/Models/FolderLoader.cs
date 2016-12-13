using System;
using System.Collections.Generic;
using System.IO;
using Player.Types;

namespace Player.Models
{
    public class FolderLoader
    {

        public List<PlayerList> GetFolder(string path)
        {
            var list = new List<PlayerList>();
            GetMusicList(path, list);
            list.Sort((x, y) => String.Compare(x.SongName, y.SongName, StringComparison.Ordinal));
            return list;
        }


        private void GetMusicList(string path, List<PlayerList> playList)
        {
           foreach (var directories in Directory.GetDirectories(path))
           {
               try
               {
                   foreach (var file in Directory.GetFiles(directories, "*.mp3"))
                    {
                        playList.Add(AddMusicDescription(file));
                    }
                    GetMusicList(directories, playList);
               }
               catch (UnauthorizedAccessException)
               {
               }
               catch (DirectoryNotFoundException)
               {
               }
                    
            }
        }
            

        private PlayerList AddMusicDescription(string path)
        {
            TagLib.File musicFile = TagLib.File.Create(path);
            var musicinfo = new PlayerList(musicFile.Tag.Title ?? Path.GetFileName(path),
                musicFile.Tag.FirstPerformer,
                musicFile.Tag.Album, 
                path);
            return musicinfo;
        }
    }
}