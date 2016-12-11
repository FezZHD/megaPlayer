using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Player.Commands;
using Player.Models;
using Player.Types;

namespace Player.ViewModel
{
    public class ViewModel:ViewModelBase
    {

        #region ctor
        public ViewModel()
        {
            Func<Task> playListCommand = GetPlayList;
            SongName = "Song name(not Sandstorm)";
            ArtistName = "Artist";
            AlbumName = "Album";
            MediaElement = new MediaElement();
            AlbumImage = new BitmapImage();
            SetDefaultImage();
            FolderLoadCommand = new AsyncCommand(playListCommand);
        }
        #endregion


        #region Properties

        private bool isPlaying;

        public bool IsPlaying
        {
            get { return isPlaying; }
            set
            {
                isPlaying = value;
                OnPropertyChanged();
            }
        }


        private MediaElement mediadElement;

        public MediaElement MediaElement
        {
            get { return mediadElement; }
            set
            {
                mediadElement = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<PlayerList> playList;

        public ObservableCollection<PlayerList> PlayList
        {
            get { return playList; }
            set
            {
                playList = value;
                OnPropertyChanged();
            }
        }

        private BitmapImage image;

        public BitmapImage AlbumImage
        {
            get { return image; }
            set
            {
                image = value;
                OnPropertyChanged();
            }
        }

        private string songName;

        public string SongName
        {
            get { return songName; }
            set
            {
                songName = value;
                OnPropertyChanged();
            }
        }

        private string artistName;

        public string ArtistName
        {
            get { return artistName; }
            set
            {
                artistName = value;
                OnPropertyChanged();
            }
        }

        private string albumName;

        public string AlbumName
        {
            get { return albumName; }
            set
            {
                albumName = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region privateMethods

        private void SetDefaultImage()
        { 
            AlbumImage.BeginInit();
            AlbumImage.UriSource = new Uri("../images/note.png", UriKind.Relative);
            AlbumImage.EndInit();
        }

        private async Task GetPlayList()
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = @"Выберите папку с вашей музыкальной библиотекой.";
            DialogResult dialogResult = folderDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                PlayList = await new FolderLoader().GetFolder(folderDialog.SelectedPath);
            }
        }

        #endregion

        #region Commands

        public ICommand FolderLoadCommand { get; private set; }

        #endregion

    }
}