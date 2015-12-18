using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Windows;
using TagLib;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using File = System.IO.File;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;
using SelectionMode = System.Windows.Controls.SelectionMode;

namespace Player
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        public MainWindow()
        {
            InitializeComponent();

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += TimerTick;
            dispatcherTimer.Start();
           
            playerListBox.SelectionMode = SelectionMode.Single;
        }


        private void TimerTick(object sender, EventArgs e)
        {
            if (globals.saveList == null)
            {
                progressSlider.IsEnabled = false;
            }
            else
            {
                progressSlider.IsEnabled = true;
            }
                if ((megaPlayer.Source != null) && (megaPlayer.NaturalDuration.HasTimeSpan) &&
                (!globals.userIsDraggingSlider))
                 {
                    globals.ignoreChange = true;
                    progressSlider.Minimum = 0;
                    progressSlider.Maximum = megaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                    progressSlider.Value = megaPlayer.Position.TotalSeconds;
                    globals.ignoreChange = false;
                    if (progressSlider.Maximum == progressSlider.Value)
                    {
                        globals.clickedItemIndex++;
                        if (globals.clickedItemIndex == globals.saveList.Count)
                        {
                            globals.clickedItemIndex = 0;
                            playerListBox.SelectedIndex = 0;
                            return;
                        }
                        progressSlider.Value = 0;
                        playerListBox.SelectedIndex = globals.clickedItemIndex;
                        playerListBox.ScrollIntoView(playerListBox.Items[globals.clickedItemIndex]);
                    }
                }
        }

        private void progressSlider_DragStarted(object sender, DragStartedEventArgs e)
        {
            globals.userIsDraggingSlider = true;
            megaPlayer.Pause();
            playButton.IsEnabled = false;
            pauseButton.IsEnabled = false;
            playForward.IsEnabled = false;
            playBackward.IsEnabled = false;
        }

        private void progressSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
           
            globals.userIsDraggingSlider = false;
            playButton.IsEnabled = true;
            pauseButton.IsEnabled = true;
            playForward.IsEnabled = true;
            playBackward.IsEnabled = true;
            megaPlayer.Position = TimeSpan.FromSeconds(progressSlider.Value);
            if (globals.mediaPlayerIsPlaying)
            {
                megaPlayer.Play();
                return;
            }
            globals.mediaPlayerIsPlaying = false;
            changingButtonsClass changingButtons = new changingButtonsClass();
            changingButtons.playButtonVision(pauseButton, playButton);


        }

        private void progressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {  
           timer.Text = TimeSpan.FromSeconds(progressSlider.Value).ToString(@"mm\:ss");
            if (!globals.ignoreChange)
            {
                megaPlayer.Position = TimeSpan.FromSeconds(progressSlider.Value);
            }
        }


        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            globals.startToPlay = true;
            if (!globals.mediaPlayerIsPlaying && megaPlayer.Source == null)
            {
                return;
            }
            changingButtonsClass changingButtons = new changingButtonsClass();
            changingButtons.pauseButtonVision(pauseButton, playButton);
            megaPlayer.Play();
            globals.mediaPlayerIsPlaying = true;

        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            globals.startToPlay = false;
            if (!globals.mediaPlayerIsPlaying)
            {
                return;
            }
            globals.mediaPlayerIsPlaying = false;
            megaPlayer.Pause();
            changingButtonsClass changingButtons = new changingButtonsClass();
            changingButtons.playButtonVision(pauseButton, playButton);
            
        }

        private void addingMusicFiles(string[] filePaths, List<PlayerList> outputList)
        {
            foreach (string filePath in filePaths)
            {
                 TagLib.File musicFile = TagLib.File.Create(filePath);
                        if (musicFile != null)
                        {
                            if (musicFile.Tag != null)
                            {
                                outputList.Add(new PlayerList(musicFile.Tag.Title,musicFile.Tag.FirstPerformer,
                                   musicFile.Tag.Album, filePath));
                            }
                            else
                            {
                                outputList.Add(new PlayerList(Path.GetFileName(filePath), null, null, filePath));
                            }
                        }

           } 

        }

        private int findingItemIndex(string path)
        {
            int result = 0;
            for (int index = 0; index < globals.saveList.Count; index++)
            {
                if (globals.saveList[index].FilePath == path)
                {
                    result = index;
                    break;
                }
            }
            return result;
        }

        private void addingMusicFilesFromDialog(string[] fileString, FolderBrowserDialog folderDialog, List<PlayerList> playerList)
        {
            fileString = Directory.GetFiles(folderDialog.SelectedPath, "*.mp3",
                SearchOption.AllDirectories);
            addingMusicFiles(fileString, playerList);
            playerListBox.Items.Clear();
            if (globals.saveList == null)
            {
                 globals.saveList = new List<PlayerList>();
            }
            for (int index = 0; index < playerList.Count;index++) 
            {
                globals.saveList.Add(new PlayerList(playerList[index].SongName,playerList[index].Artist,playerList[index].Album,playerList[index].FilePath));
            }
            globals.saveList.Sort((first, second) => String.Compare(first.SongName, second.SongName, StringComparison.Ordinal));
            for (int index = 0; index < globals.saveList.Count; index++)
            {
                if (globals.saveList[index].Artist == null)
                {

                    playerListBox.Items.Add(globals.saveList[index].SongName + " - " + "Unknown Artist");
                }
                else
                {
                    playerListBox.Items.Add(globals.saveList[index].SongName + " - " +
                                            globals.saveList[index].Artist);
                }
            }
        }


        private void chooseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            bool isEmpty = false;
            globals filePathsString = new globals();
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = @"Выберите папку с вашей музыкальной библиотекой.";
            DialogResult dialogResult = folderDialog.ShowDialog();

           
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                if (globals.saveList != null)
                {
                    if (playerListBox.SelectedIndex != -1)
                    {
                        outputPath = globals.saveList[playerListBox.SelectedIndex].FilePath;
                    }
                    else
                    {
                        isEmpty = true;
                    }
                    addingMusicFilesFromDialog(filePathsString.filesPath, folderDialog, globals.saveList);
                }
                else
                {
                    isEmpty = true;
                    List<PlayerList> playerList = new List<PlayerList>();
                    addingMusicFilesFromDialog(filePathsString.filesPath, folderDialog, playerList);
                }
                   if (!isEmpty)
                    {
                        globals.startToPlay = false;
                        playerListBox.SelectedIndex = findingItemIndex(outputPath);
                    }
            }
           
        }

        private void addFile_Click(object sender, RoutedEventArgs e)
        {
            bool isEmpty = false;
            OpenFileDialog openMusicFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Mp3 files (*.mp3)|*.mp3|All files (*.*)|*.*"
            };
            if (openMusicFileDialog.ShowDialog() == true)
            {
                    if (globals.saveList == null)
                        {
                            List<PlayerList>playerList  = new List<PlayerList>();
                            addingMusicFiles(openMusicFileDialog.FileNames, playerList);
                            globals.saveList = playerList;
                            globals.saveList.Sort((x, y) => String.Compare(x.SongName, y.SongName, StringComparison.Ordinal));
                            isEmpty = true;
                        } 
                    else
                        {
                            if (playerListBox.SelectedIndex != -1)
                            {
                                outputPath = globals.saveList[playerListBox.SelectedIndex].FilePath;
                            }
                            else
                            {
                                isEmpty = true;
                            }
                            addingMusicFiles(openMusicFileDialog.FileNames, globals.saveList);
                            globals.saveList.Sort((x, y) => String.Compare(x.SongName, y.SongName, StringComparison.Ordinal));
                        }
                    playerListBox.Items.Clear();
                    for (int index = 0; index < globals.saveList.Count; index++)
                    {
                        
                        if (globals.saveList[index].Artist == null)
                        {
                            playerListBox.Items.Add(globals.saveList[index].SongName + " - " + "Unknown Artist");
                        }
                        else
                        {
                            playerListBox.Items.Add(globals.saveList[index].SongName + " - " +
                                                    globals.saveList[index].Artist);
                        }
                    }
                if (!isEmpty)
                {
                    globals.startToPlay = false;
                    playerListBox.SelectedIndex = findingItemIndex(outputPath);
                } 
            }
                  
        }

        private void setImage(BitmapImage imageBitmap,string imagePath, MemoryStream pictureMemoryStream = null)
        {
            imageBitmap.BeginInit();
            if (imagePath != null)
            {
                imageBitmap.UriSource = new Uri(imagePath, UriKind.Relative);
            }
               if (pictureMemoryStream != null)
               {
                   imageBitmap.StreamSource = pictureMemoryStream;
               }
            imageBitmap.EndInit();
            albumPic.Stretch = Stretch.Uniform;
            albumPic.Source = imageBitmap;
            
            
        }

        private void playerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            changingButtonsClass changingButtons = new changingButtonsClass();
            globals.mediaPlayerIsPlaying = true;
            globals.clickedItemIndex = playerListBox.SelectedIndex;
            if (globals.clickedItemIndex == -1)
            {
                changingButtons.playButtonVision(pauseButton, playButton);
                return;
            }

            if (File.Exists(globals.saveList[globals.clickedItemIndex].FilePath))
            {
                if (globals.startToPlay)
                {
                    megaPlayer.Source = new Uri(globals.saveList[globals.clickedItemIndex].FilePath);

                    songName.Content = globals.saveList[globals.clickedItemIndex].SongName;
                    artist.Content = globals.saveList[globals.clickedItemIndex].Artist;
                    album.Content = globals.saveList[globals.clickedItemIndex].Album;
                    TagLib.File musicFile = TagLib.File.Create(globals.saveList[globals.clickedItemIndex].FilePath);
                    if (musicFile.Tag.Pictures.Length > 0)
                    {
                        IPicture albumArt = musicFile.Tag.Pictures[0];
                        MemoryStream pictureMemoryStream = new MemoryStream(albumArt.Data.Data);
                        pictureMemoryStream.Seek(0, SeekOrigin.Begin);

                        BitmapImage albumArtImage = new BitmapImage();
                        setImage(albumArtImage, null, pictureMemoryStream);
                    }
                    else
                    {
                        BitmapImage noteImage = new BitmapImage();
                        setImage(noteImage, "images/note.png");
                    }

                    megaPlayer.Play();
                   

                    progressSlider.Value = 0;
                }
            }
            else
            {
                MessageBox.Show(@"Файл удалён. Будет проигрываться следующий трек");
                playerListBox.SelectedIndex++;
            }
            
            globals.mediaPlayerIsPlaying = true; 
            changingButtons.pauseButtonVision(pauseButton, playButton);
            globals.startToPlay = true;

        }

        private void playForward_Click(object sender, RoutedEventArgs e)
        {
            globals.clickedItemIndex++;
          
            if (!globals.mediaPlayerIsPlaying)
            {
                return;
            }
            
            if (globals.clickedItemIndex > (globals.saveList.Count - 1))
            {
                globals.clickedItemIndex--;
                MessageBox.Show(@"Список треков окончен");
                return;
            }
            playerListBox.SelectedIndex = globals.clickedItemIndex;
            progressSlider.Value = 0;
            playerListBox.ScrollIntoView(playerListBox.Items[globals.clickedItemIndex]);
            globals.mediaPlayerIsPlaying = true;
            

        }

        private void playBackward_Click(object sender, RoutedEventArgs e)
        {
            globals.clickedItemIndex--;
           
            if (!globals.mediaPlayerIsPlaying)
            {
                return;
            }
            if (globals.clickedItemIndex < 0)
            {
                MessageBox.Show(@"Невозможно воиспроизвети , т.к предыдущий трек является первым в спивке.");
                globals.clickedItemIndex++;
                return;
            }
            playerListBox.SelectedIndex = globals.clickedItemIndex;
            progressSlider.Value = 0;
            playerListBox.ScrollIntoView(playerListBox.Items[globals.clickedItemIndex]);
            globals.mediaPlayerIsPlaying = true;
        }


        private void progressSlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            megaPlayer.Pause();
        }

        private void progressSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            megaPlayer.Play();
        }


        private void clearListButton_Click(object sender, RoutedEventArgs e)
        {
            if (globals.saveList == null)
            {
                MessageBox.Show(@"Плейлист пуст.", @"Очистка плейлиста");
                return;
            }
            MessageBoxResult resultNotification = MessageBox.Show(@"Вы хотите очистить ваш плейлист?",
                @"Очистка плейлиста", MessageBoxButton.OKCancel);
            if (resultNotification == MessageBoxResult.OK)
            {
                progressSlider.Value = 0;
                progressSlider.IsEnabled = false;
                timer.Text = null;
                megaPlayer.Source = null;
                megaPlayer.Stop();
                globals.saveList = null;
                playerListBox.Items.Clear();
                songName.Content = @"Song Name (not Sandstorm)";
                album.Content = @"Album";
                artist.Content = @"Artist";
                BitmapImage noteImage = new BitmapImage();
                setImage(noteImage,"images/note.png");
                changingButtonsClass changingButtons = new changingButtonsClass();
                changingButtons.playButtonVision(pauseButton, playButton);
            }
        }

        private string outputPath { get; set; }
    }
}

