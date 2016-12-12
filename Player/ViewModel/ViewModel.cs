using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Player.Commands;
using Player.Models;
using Player.Types;

namespace Player.ViewModel
{
    public class ViewModel : ViewModelBase
    {

        #region ctor

        public ViewModel()
        {
            SliderValue = 0;
            IsNotLoading = true;
            IsPlaying = false;
            AlbumImage = new BitmapImage();
            SetDefaultBindings();
            MediaElement = new MediaElement {LoadedBehavior = MediaState.Manual};
            MediaElement.MediaOpened += MediaElementOnMediaOpened;
            InitializeTimer();
            SetCommands();
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

        private List<PlayerList> playList;

        public List<PlayerList> PlayList
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


        private bool isNotLoading;

        public bool IsNotLoading
        {
            get { return isNotLoading; }
            set
            {
                isNotLoading = value;
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


        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                OnPropertyChanged();
            }
        }


        private bool isLoaded;

        public bool IsLoaded
        {
            get { return isLoaded; }
            set
            {
                isLoaded = value;
                OnPropertyChanged();
            }
        }


        private double sliderValue;

        public double SliderValue
        {
            get { return sliderValue; }
            set
            {
                sliderValue = value;
                OnPropertyChanged();
            }
        }

        private double maxValue;

        public double MaxValue
        {
            get { return maxValue; }
            set
            {
                maxValue = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region privateMethods

        private void MediaElementOnMediaOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            MaxValue = MediaElement.NaturalDuration.TimeSpan.TotalSeconds;
        }


        private async Task GetPlayList()
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = @"Выберите папку с вашей музыкальной библиотекой.";
            DialogResult dialogResult = folderDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                SetDefaultBindings();
                if (IsPlaying)
                {
                    MediaElement.Stop();
                    IsPlaying = false;
                }
                IsNotLoading = false;
                await Task.Run(() =>
                {
                    PlayList = new FolderLoader().GetFolder(folderDialog.SelectedPath);
                });
                IsLoaded = PlayList.Count > 0;
                IsNotLoading = true;
            }
        }


        private void PlayMusicFromList(object index)
        {
            var intIndex = (int) index;
            SetBindings(PlayList[intIndex]);
            MediaElement.Play();
            IsPlaying = true;
        }


        private void PlayOnButton()
        {
            if (SelectedIndex >= 0)
            {
                if (IsPlaying)
                {
                    MediaElement.Pause();
                    IsPlaying = false;
                }
                else
                {
                    MediaElement.Play();
                    IsPlaying = true;
                }
            }
        }


        private void SetBindings(PlayerList musicInfo)
        {
            SliderValue = 0;
            SongName = musicInfo.SongName;
            ArtistName = musicInfo.Artist;
            AlbumName = musicInfo.Album;
            MediaElement.Source = new Uri(musicInfo.FilePath);
            AlbumImage = new LoadImage().LoadAlbumArt(musicInfo.FilePath);
        }


        private void PlayForward()
        {
            SelectedIndex++;
            try
            {
                SetBindings(PlayList[SelectedIndex]);
            }
            catch (ArgumentOutOfRangeException)
            {
                IsPlaying = false;
                SelectedIndex--;
                MediaElement.Stop();
            }

        }


        private void PlayBackward()
        {
            SelectedIndex--;
            try
            {
                SetBindings(PlayList[SelectedIndex]);
            }
            catch (ArgumentOutOfRangeException)
            {
                IsPlaying = false;
                SelectedIndex++;
                MediaElement.Stop();
            }
        }


        private void SetCommands()
        {
            Action<object> playMusicFromList = PlayMusicFromList;
            Func<Task> playListGettingCommand = GetPlayList;
            Action playOnButton = PlayOnButton;
            Action playForward = PlayForward;
            Action playBackword = PlayBackward;
            PlayFromList = new CommandWithParammeter(playMusicFromList);
            FolderLoadCommand = new AsyncCommand(playListGettingCommand);
            PlayOnButtonCommand = new SimpleCommand(playOnButton);
            PlayForwardCommand = new SimpleCommand(playForward);
            PlayBackwardCommand = new SimpleCommand(playBackword);
        }


        private void SetDefaultBindings()
        {
            SongName = "Song name(not Sandstorm)";
            ArtistName = "Artist";
            AlbumName = "Album";
            AlbumImage = new LoadImage().SetDefaultImage();
        }


        private void InitializeTimer()
        {
             DispatcherTimer dispatcherTimer = new DispatcherTimer();
             dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
             dispatcherTimer.Tick += TimerTick;
             dispatcherTimer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (IsPlaying && !IsDragging)
            {
                SliderValue = MediaElement.Position.TotalSeconds;
                if (SliderValue == MaxValue)
                {
                    PlayForward();
                }
            }
        }
        #endregion

        #region Commands

        public ICommand FolderLoadCommand { get; private set; }
        public ICommand PlayFromList { get; private set; }
        public ICommand PlayOnButtonCommand { get; private set; }
        public ICommand PlayForwardCommand { get; private set; }
        public ICommand PlayBackwardCommand { get; private set; }

        #endregion


        #region Public methods from view for slider

        public void OnStartDrag()
        {
            IsDragging = true;
        }


        public void OnCompleteDrag()
        {
            IsDragging = false;
            MediaElement.Position = TimeSpan.FromSeconds(SliderValue);
        }
        #endregion

        #region private properties

        private bool IsDragging { get; set; }

        #endregion
    }
}