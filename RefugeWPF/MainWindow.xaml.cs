
using RefugeWPF.CouchePresentation.ViewModel;
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
            this.DataContext = new MainWindowViewModel();
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
         *  Gère le déplacement de la fênetre via Drag/Drop en cliquant dans la zone d'en-tête
         * </summary>
         */ 
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) { 
                DragMove();            
            }
        }

        /**
         * <summary>
         *  Gère l'événement click sur le menu à gauche
         * </summary>
         */ 
        private void MySideMenu_SelectionChanged(object sender, HandyControl.Data.FunctionEventArgs<object> e) {
            // 
            var selectedItem = e.Info as HandyControl.Controls.SideMenuItem;

            // Si aucun élément sélectionné, retour
            if (selectedItem == null) return;

            string header = selectedItem.Header?.ToString() ?? "";

            switch (header)
            {
                case "Consulter un animal":
                    MainContent.Content = null;
                    break;
                default:
                    MainContent.Content = null;
                    break;
            }
        }
    }
}