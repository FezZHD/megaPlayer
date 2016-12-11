using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Player.Types;
using System.Threading.Tasks;

namespace Player.Models
{
    public class FolderLoader
    {

        public ObservableCollection<PlayerList> GetFolder(string path)
        {
            var list = new ObservableCollection<PlayerList>();
            GetMusicList(path, list);
            return list;
        }


        private void GetMusicList(string path, ObservableCollection<PlayerList> playList)
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
               catch (UnauthorizedAccessException ex)
               {
                  Debug.WriteLine(ex.Message);
                  continue;
               }
               catch (DirectoryNotFoundException ex)
               {
                  Debug.WriteLine(ex.Message);
                  continue;
               }
                    
            }
        }
            

        private PlayerList AddMusicDescription(string path)
        {
            TagLib.File musicFile = TagLib.File.Create(path);
            return musicFile.Tag != null ? new PlayerList(musicFile.Tag.Title, musicFile.Tag.FirstPerformer, musicFile.Tag.Album, path) : new PlayerList(Path.GetFileName(path), "Unknown Artist", "Unknown Album", path);
        }
    }
}