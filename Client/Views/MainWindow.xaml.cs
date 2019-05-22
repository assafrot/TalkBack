using Client.Interfaces;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Closing += Window_Closing;

        }

        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IClosing context = DataContext as IClosing;
            context.OnClosing();
        }
    }
}
