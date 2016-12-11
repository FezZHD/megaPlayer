using System.Windows.Controls;

namespace Player.ViewModel
{
    public class ViewModel:ViewModelBase
    {

        #region ctor
        public ViewModel()
        {
            MediaElement = new MediaElement();
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

        #endregion
    }
}