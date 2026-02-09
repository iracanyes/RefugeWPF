using RefugeWPF.CouchePresentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace RefugeWPF.CouchePresentation.View.Refuge
{
    /// <summary>
    /// Logique d'interaction pour VaccinationView.xaml
    /// </summary>
    public partial class VaccinationView : UserControl
    {
        public VaccinationView()
        {
            InitializeComponent();
            this.DataContext = new VaccinationViewModel();
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
                FormVaccineSection.Visibility = Visibility.Visible;

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
         *  Recherche de l'animal  
         * </summary>
         * 
         */
        private void SearchAnimalButton_Click(object sender, RoutedEventArgs e)
        {
            VaccinationViewModel vm = (VaccinationViewModel)this.DataContext;


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
         *  Recherche de l'animal  
         * </summary>
         * 
         */
        private void SearchVaccineButton_Click(object sender, RoutedEventArgs e)
        {
            VaccinationViewModel vm = (VaccinationViewModel)this.DataContext;


            try
            {
                vm.SearchVaccine();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private bool CheckConstraints()
        {
            VaccinationViewModel vm = (VaccinationViewModel)this.DataContext;
            StringBuilder sb = new StringBuilder();
            DateOnly? dateVaccination = DateVaccination_DatePicker.SelectedDate != null ? DateOnly.FromDateTime((DateTime) DateVaccination_DatePicker.SelectedDate) : null;

            

            if (dateVaccination != null && vm.AnimalFound != null && (dateVaccination < vm.AnimalFound.BirthDate))
                sb.Append("La date de vaccination : Doit être supérieur à la date de naissance de l'animal.");

            string errorMessage = sb.ToString();
            if (errorMessage.Length > 0)
            {
                MessageBox.Show(errorMessage);
                return false;
            }

            return true;
                
        }

        /**
         * <summary>
         *  Evénement "Click" sur le bouton "Confirmer". 
         *  Permet d'ajouter une famille d'accueil pour un animal
         * </summary>
         */
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {

            VaccinationViewModel vm = (VaccinationViewModel)this.DataContext;

            if (!this.CheckConstraints())
                return;

            try
            {
                vm.CreateVaccination();
                

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
            VaccinationViewModel vm = (VaccinationViewModel)this.DataContext;



            // Efface les valeurs des champs de la section "adoption" du formulaire
            vm.DateVaccination = DateTime.Now;
            vm.Done = false;
            // Ré-initialiser la recherche de la personne de contact
            vm.SelectedVaccine = null;
            vm.VaccineFound = null;
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
            Vaccinations_DataGrid.Height = 240;

            // Changement manuelle de la hauteur des grilles 
            GridLengthConverter glConverter = new GridLengthConverter();
            RowListVaccination.Height = (GridLength)glConverter.ConvertFrom("300px")!;
            RowFormVaccination.Height = (GridLength)glConverter.ConvertFrom("600px")!;
        }

        /**
         * <summary>
         *  Ferme le formulaire, en diminuant le grille parent
         * </summary>
         */
        private void CloseForm()
        {
            Vaccinations_DataGrid.Height = 380;

            // Changement manuelle de la hauteur des grilles 
            GridLengthConverter glConverter = new GridLengthConverter();
            RowListVaccination.Height = (GridLength)glConverter.ConvertFrom("600px")!;
            RowFormVaccination.Height = (GridLength)glConverter.ConvertFrom("300px")!;

        }
    }
}
