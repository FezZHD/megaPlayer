namespace Player.ViewModel
{
    public class ViewModel:ViewModelBase
    {

        #region ctor
        public ViewModel()
        {
            
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

        #endregion
    }
}