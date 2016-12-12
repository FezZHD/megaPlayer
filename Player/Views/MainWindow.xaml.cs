using System.Windows;
using System.Windows.Controls.Primitives;


namespace Player.Views
{

    public partial class MainWindow
    {

        private readonly ViewModel.ViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = DataContext as ViewModel.ViewModel;
        }


        private void ProgressSlider_OnDragStarted(object sender, DragStartedEventArgs e)
        { 
           viewModel.OnStartDrag();
        }


        private void ProgressSlider_OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
           viewModel.OnCompleteDrag();
        }
    }
}

