using RefugeWPF.CouchePresentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RefugeWPF.CouchePresentation.View.Refuge
{
    /// <summary>
    /// Logique d'interaction pour AdoptionView.xaml
    /// </summary>
    public partial class AdoptionView : UserControl
    {
        public AdoptionView()
        {
            InitializeComponent();
            this.DataContext = new AdoptionViewModel();
        }

        /**
         * <summary>
         *  Evénement click sur le bouton "Ajouter" qui permet d'ouvrir le formulaire d'ajout
         *  d'un animal
         * </summary>
         */
        private void EnableCreateAdoption(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ouverture du formulaire
                this.OpenForm();

                FormAnimalSection.Visibility = Visibility.Visible;
                FormContactSection.Visibility = Visibility.Visible;

                // Nettoyage du formulaire
                this.ClearForm();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        /**
         * <summary>
         *  Evénement click sur le bouton "Ajouter" qui permet d'ouvrir le formulaire d'ajout
         *  d'un animal
         * </summary>
         */
        private void EnableUpdateAdoption(object sender, RoutedEventArgs e)
        {
            AdoptionViewModel vm = (AdoptionViewModel) this.DataContext;
            try
            {
                // Ouverture du formulaire
                this.OpenForm();

                if (vm.AdoptionDataGridSelectedItem == null) {
                    MessageBox.Show($"Sélectionner une candidature pour adoption pour continuer.");
                    return;
                }

                // Sauvegarde de l'adoption sélectionné dans le DataGrid
                vm.SelectedAdoption = vm.AdoptionDataGridSelectedItem;

                AdoptionStatus_ComboBox.SelectedValue = vm.SelectedAdoption.Status;
                vm.DateStart = ((DateOnly) vm.SelectedAdoption.DateStart).ToDateTime(TimeOnly.Parse("11:00:00 AM"));
                vm.DateEnd = vm.SelectedAdoption.DateEnd != null ? ((DateOnly) vm.SelectedAdoption.DateEnd).ToDateTime(TimeOnly.Parse("11:00:00 AM")) : null;

                FormAnimalSection.Visibility = Visibility.Collapsed;
                FormContactSection.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        /**
         * <summary>
         *  Recherche de la personne de contact  
         * </summary>
         * 
         */
        private void SearchContactButton_Click(object sender, RoutedEventArgs e)
        {
            AdoptionViewModel vm = (AdoptionViewModel)this.DataContext;
            Regex regex = new Regex("^(\\d{2})\\.(0[1-9]|1[0-2])\\.(0[1-9]|[1-2]\\d|3[0-1])-(\\d{3})\\.(\\d{2})$");

            if (!regex.IsMatch(ContactRegistryNumber_Textbox.Text))
            {
                MessageBox.Show("Le numéro de registre national doit être au format yy.mm.dd-999.99");
                return;
            }

            try
            {
                vm.SearchContact();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

        /**
         * <summary>
         *  Recherche de l'animal  
         * </summary>
         * 
         */
        private void SearchAnimalButton_Click(object sender, RoutedEventArgs e)
        {
            AdoptionViewModel vm = (AdoptionViewModel)this.DataContext;


            try
            {
                vm.SearchAnimal();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /**
         * <summary>
         *  Evénement "Click" sur le bouton "Confirmer". 
         *  Permet d'ajouter une famille d'accueil pour un animal
         * </summary>
         */
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {

            AdoptionViewModel vm = (AdoptionViewModel)this.DataContext;


            try
            {

                if(vm.SelectedAdoption == null)
                {
                    // Créer l'animal
                    vm.CreateAdoption();
                }
                else
                {
                    vm.UpdateAdoption();
                }



                // vider les champs du formulaire
                this.ClearForm();

                // Fermer le Formulaire
                this.CloseForm();


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors l'ajout d'un animal.\nMessage : {ex.Message}.\nErreur : {ex}");
                MessageBox.Show($"Erreur lors l'ajout d'un animal.\nMessage : {ex.Message}.\nErreur : {ex}");
            }

        }

        

        /**
         * <summary>
         *  Efface les valeurs contenues dans les formulaires  
         * </summary>
         * 
         */
        private void ClearForm()
        {
            AdoptionViewModel vm = (AdoptionViewModel)this.DataContext;



            // Efface les valeurs des champs de la section "adoption" du formulaire
            vm.SelectedAdoption = null;            
            AdoptionStatus_ComboBox.SelectedIndex = 2;
            vm.DateStart = DateTime.Now; 
            vm.DateEnd = null;
            // Ré-initialiser la recherche de la personne de contact
            ContactRegistryNumber_Textbox.Text = "";
            vm.ContactFound = null;
            // Ré-initialiser la recherche de l'animal
            AnimalSearchByName_Textbox.Text = "";
            vm.AnimalsFound.Clear();

        }

        /**
         * <summary>
         *  Ouvre le formulaire et redimensionne les grilles (grid) et la liste des animaux
         * </summary>
         * 
         */
        private void OpenForm()
        {
            Adoptions_DataGrid.Height = 250;

            // Changement manuelle de la hauteur des grilles 
            GridLengthConverter glConverter = new GridLengthConverter();
            RowListAdoption.Height = (GridLength)glConverter.ConvertFrom("300px")!;
            RowFormAdoption.Height = (GridLength)glConverter.ConvertFrom("600px")!;



        }

        /**
         * <summary>
         *  Ferme le formulaire, en diminuant le grille parent
         * </summary>
         */
        private void CloseForm()
        {
            Adoptions_DataGrid.Height = 550;

            // Changement manuelle de la hauteur des grilles 
            GridLengthConverter glConverter = new GridLengthConverter();
            RowListAdoption.Height = (GridLength)glConverter.ConvertFrom("600px")!;
            RowFormAdoption.Height = (GridLength)glConverter.ConvertFrom("300px")!;

        }
    }
}
