using System.ComponentModel;
using System.Windows;


namespace XUpload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // The Closing event is raised when Close is called, if a windows Close button is clicked, or if the user presses ALT+F4.
            this.Closing += new CancelEventHandler(MainWindowViewModel.OnProcessExit);
        }
    }
}
