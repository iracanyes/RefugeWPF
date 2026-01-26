using RefugeWPF.ClassesMetiers.Model.Entities;
using RefugeWPF.ClassesMetiers.Model.Enums;
using RefugeWPF.CoucheMetiers.Model.DTO;
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
    /// Logique d'interaction pour AdmissionView.xaml
    /// </summary>
    public partial class AdmissionView : UserControl
    {
        public AdmissionView()
        {
            InitializeComponent();
            this.DataContext = new AdmissionViewModel();
        }

        /**
         * <summary>
         *  Recherche de la personne de contact  
         * </summary>
         * 
         */
        private void SearchContactButton_Click(object sender, RoutedEventArgs e)
        {
            AdmissionViewModel vm = (AdmissionViewModel)this.DataContext;

            vm.SearchContact(ContactRegistryNumber_Textbox.Text);

        }

        private void CreateAnimal(object sender, RoutedEventArgs e)
        {
            AdmissionViewModel vm = (AdmissionViewModel)this.DataContext;

            try
            {
                RefugeWPF.ClassesMetiers.Model.Entities.Animal animal = new RefugeWPF.ClassesMetiers.Model.Entities.Animal(
                    AnimalId.Text,
                    AnimalName.Text,
                    (TypeCat.IsChecked == null || (bool)TypeCat.IsChecked) ? AnimalType.Cat : AnimalType.Dog,
                    (GenderMale.IsChecked == null || (bool)GenderMale.IsChecked) ? GenderType.Male : GenderType.Female,
                    DateOnly.FromDateTime((DateTime)AnimalBirthDate.SelectedDate!),
                    DateOnly.FromDateTime((DateTime)AnimalDeathDate.SelectedDate!),
                    (bool)AnimalIsSterilized.IsChecked!,
                    DateOnly.FromDateTime((DateTime)AnimalDateSterilization.SelectedDate!),
                    AnimalParticularity.Text,
                    AnimalDescription.Text
                );

                vm.CreateAnimal(animal);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                    Debug.WriteLine($"Erreur lors de la suppressioin d'un animal. Message: {ex.Message}. Error : {ex.ToString}");

                MessageBox.Show(ex.Message);
            }
        }

        /**
         * <summary>
         *  Evénement click sur le bouton "Ajouter" qui permet d'ouvrir le formulaire d'ajout
         *  d'un animal
         * </summary>
         */
        private void EnableCreateAnimal(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ouverture du formulaire
                this.OpenForm();

                // Affichage de la sélection des couleurs de l'animal lors de l'ajout
                FormAnimalColorsSection.Visibility = Visibility.Visible;

                // Nettoyage du formulaire
                this.ClearForm();

            }
            catch (Exception)
            {

                throw;
            }
        }

        

        /**
         * <summary>
         *  
         * </summary>
         */
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            RefugeWPF.ClassesMetiers.Model.Entities.Animal? animal = null;
            AdmissionViewModel vm = (AdmissionViewModel)this.DataContext;

            try
            {
                animal = new RefugeWPF.ClassesMetiers.Model.Entities.Animal(
                        AnimalName.Text,
                        (TypeCat.IsChecked == null || (bool)TypeCat.IsChecked) ? AnimalType.Cat : AnimalType.Dog,
                        (GenderMale.IsChecked == null || (bool)GenderMale.IsChecked) ? GenderType.Male : GenderType.Female,
                        DateOnly.FromDateTime((DateTime)AnimalBirthDate.SelectedDate!),
                        DateOnly.FromDateTime((DateTime)AnimalDeathDate.SelectedDate!),
                        (bool)AnimalIsSterilized.IsChecked!,
                        DateOnly.FromDateTime((DateTime)AnimalDateSterilization.SelectedDate!),
                        AnimalParticularity.Text,
                        AnimalDescription.Text
                    );

                // Transfert vers le ViewModel des couleurs de l'animal
                foreach (ClassesMetiers.Model.Entities.Color c in AnimalColorsSelect.SelectedItems)
                {
                    vm.SelectedAnimalColors.Add(c);
                }

                // Créer l'animal
                vm.CreateAnimal(animal);

                // Clean form
                this.ClearForm();

                // Close Form
                this.CloseForm();


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors l'ajout d'un animal.\nMessage : {ex.Message}.\nErreur : {ex}");
                
            }

        }

        /**
         * <summary>
         *  Evénement Click sur le bouton "Ajouter" du formulaire de compatibilité pour l'animal.
         *  La compatibilité, sa valeur et sa description seront stockés dans une collection.
         *  
         * </summary>
         */
        private void AddAnimalCompatibility(object sender, RoutedEventArgs e)
        {
            AdmissionViewModel vm = (AdmissionViewModel)this.DataContext;

            if (AnimalCompatibilitySelection.SelectedItem == null)
            {
                MessageBox.Show("Sélectionner une compatibilité");
                return;
            }

            AnimalCompatibilityDTO ac = new AnimalCompatibilityDTO(
                (Compatibility)AnimalCompatibilitySelection.SelectedItem,
                AnimalId.Text,
                AnimalCompatibilityValue.IsChecked,
                AnimalCompatibilityDescription.Text
            );

            // On stocke la compatibilité pour l'animal
            vm.AddedAnimalCompatibilities.Add(ac);

            // Effacer les valeurs contenues dans le formulaire de compatibilité pour l'animal
            this.ClearFormAnimalCompatibility();




        }

        

        /**
         * <summary>
         *  Efface les valeurs contenues dans les formulaires  
         * </summary>
         * 
         */ 
        private void ClearForm()
        {
            AdmissionViewModel vm = (AdmissionViewModel)this.DataContext;



            // Efface
            AnimalId.Text = "";
            AnimalName.Text = "";
            GenderMale.IsChecked = true;
            AnimalBirthDate.SelectedDate = null;
            AnimalDeathDate.SelectedDate = null;
            AnimalDateSterilization.SelectedDate = null;
            AnimalParticularity.Text = "";
            AnimalDescription.Text = "";
            AnimalColorsSelect.SelectedItems.Clear();

            // Effacer les valeurs du formulaire de compatibilité pour l'animal
            this.ClearFormAnimalCompatibility();

            // Vider la collection contenant les objets AnimalCompatibilityDTO
            vm.AddedAnimalCompatibilities.Clear();

            // Vider la collection des couleurs de l'animal
            vm.SelectedAnimalColors.Clear();

            // Ré-initialiser la recherche de la personne de contact
            ContactRegistryNumber_Textbox.Text = "";
            vm.ContactFound = null;

        }

        /**
         * <summary>
         *  Efface les valeurs contenues dans le formulaire de compatibilité pour l'animal
         * </summary>
         */
        private void ClearFormAnimalCompatibility()
        {
            AnimalCompatibilitySelection.SelectedItem = null;
            AnimalCompatibilityValue.IsChecked = false;
            AnimalCompatibilityDescription.Text = "";

        }

        /**
         * <summary>
         *  Evénement Click sur le bouton "Vider la liste".
         *  Lancer la ré-initialisation la liste des compatibilités pour l'animal
         * </summary>
         */
        private void ClearListAnimalCompatibility(object sender, RoutedEventArgs e)
        {
            AdmissionViewModel vm = (AdmissionViewModel)this.DataContext;

            vm.AddedAnimalCompatibilities.Clear();
        }


        /**
         * <summary>
         *  Ouvre le formulaire et redimensionne les grilles (grid) et la liste des animaux
         * </summary>
         * 
         */
        private void OpenForm()
        {
            Animals_DataGrid.Height = 270;

            GridLengthConverter glConverter = new GridLengthConverter();
            RowListAnimal.Height = (GridLength)glConverter.ConvertFrom("300px")!;
            RowFormAnimal.Height = (GridLength)glConverter.ConvertFrom("600px")!;



        }

        /**
         * <summary>
         *  Ferme le formulaire, en diminuant le grille parent
         * </summary>
         */
        private void CloseForm()
        {
            Animals_DataGrid.Height = 550;

            GridLengthConverter glConverter = new GridLengthConverter();
            RowListAnimal.Height = (GridLength)glConverter.ConvertFrom("600px")!;
            RowFormAnimal.Height = (GridLength)glConverter.ConvertFrom("300px")!;

        }


    }
}
