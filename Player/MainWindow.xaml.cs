using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows;
using TagLib;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Threading;
using System.Windows.Threading;
using Application = System.Windows.Application;
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
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerTick;
            timer.Start();
            playerListBox.SelectionMode = SelectionMode.Single;
        }


        private void TimerTick(object sender, EventArgs e)
        {
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
            pauseButton.Visibility = Visibility.Hidden;
            pauseButton.IsEnabled = false;

            playButton.Visibility = Visibility.Visible;
            playButton.IsEnabled = true;


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
            if (!globals.mediaPlayerIsPlaying && megaPlayer.Source == null)
            {
                return;
            }
            megaPlayer.Play();
            playButton.Visibility = Visibility.Hidden;
            playButton.IsEnabled = false;

            pauseButton.Visibility = Visibility.Visible;
            pauseButton.IsEnabled = true;
            globals.mediaPlayerIsPlaying = true;

        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!globals.mediaPlayerIsPlaying)
            {
                return;
            }
            globals.mediaPlayerIsPlaying = false;
            megaPlayer.Pause();
            pauseButton.Visibility = Visibility.Hidden;
            pauseButton.IsEnabled = false;

            playButton.Visibility = Visibility.Visible;
            playButton.IsEnabled = true;
            
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

        private void chooseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            globals filePathsString = new globals();
            if (globals.mediaPlayerIsPlaying)
            {
                globals.mediaPlayerIsPlaying = false;
                megaPlayer.Stop();

                pauseButton.Visibility = Visibility.Hidden;
                pauseButton.IsEnabled = false;

                playButton.Visibility = Visibility.Visible;
                playButton.IsEnabled = true;
            }
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = @"Выберите папку с вашей музыкальной библиотекой.";
            DialogResult dialogResult = folderDialog.ShowDialog();
            if (globals.saveList != null)
            {
                if (dialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    filePathsString.filesPath = Directory.GetFiles(folderDialog.SelectedPath, "*.mp3",
                        SearchOption.AllDirectories);
                    addingMusicFiles(filePathsString.filesPath,globals.saveList);
                    playerListBox.Items.Clear();
                    globals.saveList.Sort((x, y) => String.Compare(x.SongName, y.SongName, StringComparison.Ordinal));
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
                    return;
                }
            }

            List<PlayerList> playerList = new List<PlayerList>();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                filePathsString.filesPath = Directory.GetFiles(folderDialog.SelectedPath, "*.mp3",
                    SearchOption.AllDirectories);
                    addingMusicFiles(filePathsString.filesPath,playerList);
                playerListBox.Items.Clear();
                playerList.Sort((first, second) => String.Compare(first.SongName, second.SongName, StringComparison.Ordinal));
                globals.saveList = playerList;
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
        }

        private void addFile_Click(object sender, RoutedEventArgs e)
        {
            if (globals.mediaPlayerIsPlaying)
            {
                globals.mediaPlayerIsPlaying = false;
                megaPlayer.Stop();
                pauseButton.Visibility = Visibility.Hidden;
                pauseButton.IsEnabled = false;

                playButton.Visibility = Visibility.Visible;
                playButton.IsEnabled = true;
            }
            OpenFileDialog openMusicFileDialog = new OpenFileDialog();
            openMusicFileDialog.Multiselect = true;
            openMusicFileDialog.Filter = "Mp3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (openMusicFileDialog.ShowDialog() == true)
            {
                    addingMusicFiles(openMusicFileDialog.FileNames, globals.saveList);
                    globals.saveList.Sort((x, y) => String.Compare(x.SongName, y.SongName, StringComparison.Ordinal));
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

            }
                
        }

        private void playerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            megaPlayer.Stop();
            globals.mediaPlayerIsPlaying = true;
            globals.clickedItemIndex = playerListBox.SelectedIndex;
            if (globals.clickedItemIndex == -1)
            {
                pauseButton.Visibility = Visibility.Hidden;
                pauseButton.IsEnabled = false;

                playButton.Visibility = Visibility.Visible;
                playButton.IsEnabled = true;
                return;
            }

            if (File.Exists(globals.saveList[globals.clickedItemIndex].FilePath))
            {   
                megaPlayer.Source = new Uri(globals.saveList[globals.clickedItemIndex].FilePath);
                songName.Content = globals.saveList[globals.clickedItemIndex].SongName;
                artist.Content = globals.saveList[globals.clickedItemIndex].Artist;
                album.Content = globals.saveList[globals.clickedItemIndex].Album;
                TagLib.File musicFile = TagLib.File.Create(globals.saveList[globals.clickedItemIndex].FilePath);
                if (musicFile.Tag.Pictures.Length > 0)
                {
                    TagLib.IPicture albumArt = musicFile.Tag.Pictures[0];
                    MemoryStream pictureMemoryStream = new MemoryStream(albumArt.Data.Data);
                    pictureMemoryStream.Seek(0, SeekOrigin.Begin);

                    BitmapImage albumArtImage = new BitmapImage();
                    albumArtImage.BeginInit();
                    albumArtImage.StreamSource = pictureMemoryStream;
                    albumArtImage.EndInit();
                    albumPic.Source = albumArtImage;
                    albumPic.Width = globals.albumPicWidth;
                    albumPic.Height = globals.albumPicHeight;
                }
                else
                {
                    BitmapImage noteImage = new BitmapImage();
                    noteImage.BeginInit();
                    noteImage.UriSource = new Uri("images/note.png", UriKind.Relative);

                    noteImage.EndInit();
                    albumPic.Stretch = Stretch.Fill;
                    albumPic.Source = noteImage;
                    albumPic.Width = globals.albumPicWidth;
                    albumPic.Height = globals.albumPicHeight;
                }
                    
                megaPlayer.Play();
                progressSlider.Value = 0;
            }
            else
            {
                MessageBox.Show(@"Файл повреждём или удалён. Будет проигрываться следующий трек");
                playerListBox.SelectedIndex++;
            }    
            playButton.Visibility = Visibility.Hidden;
            playButton.IsEnabled = false;

            pauseButton.Visibility = Visibility.Visible;
            pauseButton.IsEnabled = true;
            globals.mediaPlayerIsPlaying = true;

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
                globals.clickedItemIndex++;
                return;
            }
            playerListBox.SelectedIndex = globals.clickedItemIndex;
            megaPlayer.Source = new Uri(globals.saveList[globals.clickedItemIndex].FilePath);
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

        private void mainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            globals.albumPicHeight = albumPic.ActualHeight;
            globals.albumPicWidth = albumPic.ActualWidth;

        }
    }
}
