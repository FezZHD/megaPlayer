using System.Windows;
using System.Windows.Controls;

namespace Player
{
    public class changingButtonsClass
    {
        public void playButtonVision(Button pauseButton, Button playButton)
        {
            pauseButton.Visibility = Visibility.Hidden;
            pauseButton.IsEnabled = false;

            playButton.Visibility = Visibility.Visible;
            playButton.IsEnabled = true;
        }

        public void pauseButtonVision(Button pauseButton, Button playButton)
        {
            playButton.Visibility = Visibility.Hidden;
            playButton.IsEnabled = false;

            pauseButton.Visibility = Visibility.Visible;
            pauseButton.IsEnabled = true;
        }
    }
}
