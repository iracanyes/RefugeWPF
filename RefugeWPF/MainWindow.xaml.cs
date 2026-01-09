using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RefugeWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /**
         * <summary>
         * Bouton réduire la fenêtre
         * </summary>
         */
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /**
         * <summary>
         * Bouton agrandir la fenêtre
         * </summary>
         */
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        /**
         * <summary>
         * Bouton fermer la fenêtre
         * </summary>
         */
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(1);

            // On peut utiliser
            Application.Current.Shutdown();
        }

        /**
         * <summary>
         *  Gère le déplacement de la fênetre via Drag/Drop
         * </summary>
         */ 
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) { 
                DragMove();

            
            }
        }
    }
}