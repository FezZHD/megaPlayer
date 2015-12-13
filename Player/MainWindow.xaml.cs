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


        }

        private void TimerTick(object sender, EventArgs e)
        {
            if ((megaPlayer.Source != null) && (megaPlayer.NaturalDuration.HasTimeSpan) && (!globals.userIsDraggingSlider))
            {
                progressSlider.Minimum = 0;
                progressSlider.Maximum = megaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                progressSlider.Value = megaPlayer.Position.TotalSeconds;
            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {

            playButton.Visibility = Visibility.Hidden;
            playButton.IsEnabled = false;

            pauseButton.Visibility = Visibility.Visible;
            pauseButton.IsEnabled = true;

        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            pauseButton.Visibility = Visibility.Hidden;
            pauseButton.IsEnabled = false;

            playButton.Visibility = Visibility.Visible;
            playButton.IsEnabled = true;
        }

        private void chooseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            globals filePathsString = new globals();

            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = @"Выберите папку с вашей музыкальной библиотекой.";
            DialogResult dialogResult = folderDialog.ShowDialog();
            if (globals.saveList != null)
            {
                if (dialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    filePathsString.filesPath = Directory.GetFiles(folderDialog.SelectedPath, "*.mp3", SearchOption.AllDirectories);
                    foreach (string filePath in filePathsString.filesPath)
                    {
                        TagLib.File musicFile = TagLib.File.Create(filePath);
                        if (musicFile != null)
                        {
                            if (musicFile.Tag == null)
                            {
                                globals.saveList.Add(new PlayerList(musicFile.Name, null, null, filePath));
                            }
                            else
                            {
                                globals.saveList.Add(new PlayerList(musicFile.Tag.Title, musicFile.Tag.FirstPerformer, musicFile.Tag.Album, filePath));
                            }
                        }

                    }
                    globals.saveList.Sort((x, y) => String.Compare(x.SongName, y.SongName, StringComparison.Ordinal));
                    return;
                }
           }

           List<PlayerList> playerList = new List<PlayerList>();
                if (dialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    filePathsString.filesPath = Directory.GetFiles(folderDialog.SelectedPath, "*.mp3",SearchOption.AllDirectories);
                    foreach (string filePath in filePathsString.filesPath)
                    {
                        TagLib.File musicFile = TagLib.File.Create(filePath);
                        if (musicFile != null)
                        {
                            if (musicFile.Tag == null)
                            {
                                playerList.Add(new PlayerList(musicFile.Name,null,null,filePath));
                            }
                            else
                            {
                                playerList.Add(new PlayerList(musicFile.Tag.Title, musicFile.Tag.FirstPerformer, musicFile.Tag.Album, filePath));
                            }
                        }
                    }
                    playerListBox.Items.Clear();
                    playerList.Sort((first, second) => String.Compare(first.SongName, second.SongName, StringComparison.Ordinal)); 
                    globals.saveList = playerList;
                    for (int index = 0; index < globals.saveList.Count; index++)
                    {
                        playerListBox.Items.Add(globals.saveList[index].SongName + " - " + globals.saveList[index].Artist);
                    }
                }
            }

        }
    }
