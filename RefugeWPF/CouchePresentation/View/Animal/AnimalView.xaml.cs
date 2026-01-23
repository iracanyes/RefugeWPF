
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RefugeWPF.ClassesMetiers.Helper;
using RefugeWPF.ClassesMetiers.Model.Entities;
using RefugeWPF.ClassesMetiers.Model.Enums;
using RefugeWPF.CoucheMetiers.Model.DTO;
using RefugeWPF.CouchePresentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace RefugeWPF.CouchePresentation.View.Animal
{
    /// <summary>
    /// Logique d'interaction pour AnimalView.xaml
    /// </summary>
    public partial class AnimalView : UserControl
    {
        

        public AnimalView()
        {
            InitializeComponent();
            this.DataContext = new AnimalViewModel();
            
        }

        

        private void CreateAnimal(object sender, RoutedEventArgs e)
        {
            AnimalViewModel vm = (AnimalViewModel)this.DataContext;
            
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
         *  Evénement click sur le bouton "Modifier" qui permet d'ouvrir le formulaire de mise à jour
         *  d'un animal
         * </summary>
         */
        private void EnableUpdateAnimal(object sender, RoutedEventArgs e)
        {
            AnimalViewModel vm = (AnimalViewModel)this.DataContext;
            RefugeWPF.ClassesMetiers.Model.Entities.Animal? animalToUpdate = vm.Selection;

            if(animalToUpdate == null)
            {
                MessageBox.Show("Sélectionner un animal pour le modifier");
                return;
            }

            // Cache la sélection des couleurs de l'animal lors de la mise à jour
            FormAnimalColorsSection.Visibility = Visibility.Collapsed;

            try
            {
                // Ouverture du formulaire
                this.OpenForm();

                /* Pré-remplissage du formulaire */
                AnimalId.Text = animalToUpdate.Id;
                AnimalName.SelectedText = animalToUpdate.Name;
                AnimalParticularity.Text = animalToUpdate.Particularity;
                AnimalDescription.Text = animalToUpdate.Description;

                // Définit le type de l'animal
                if (animalToUpdate.Type == MyEnumHelper.GetEnumDescription(AnimalType.Cat))
                    TypeCat.IsChecked = true;
                else
                    TypeDog.IsChecked = true;

                // Définit le sexe dans le formulaire
                if (animalToUpdate.Gender == MyEnumHelper.GetEnumDescription(GenderType.Male))
                {
                    GenderMale.IsChecked = true;
                }
                else
                {
                    GenderFemale.IsChecked = true;
                }

                // Définit s'il est stérilisé
                if (animalToUpdate.IsSterilized)
                    AnimalIsSterilized.IsChecked = true;
                else
                    AnimalIsSterilized.IsChecked = false;


                // Définit la date de naissance 
                if (animalToUpdate.BirthDate != null)
                    AnimalBirthDate.SelectedDate = ((DateOnly)animalToUpdate.BirthDate).ToDateTime(TimeOnly.Parse("12:00 AM"));

                // Définit la date de décès
                if (animalToUpdate.DeathDate != null)
                    AnimalDeathDate.SelectedDate = ((DateOnly)animalToUpdate.DeathDate).ToDateTime(TimeOnly.Parse("12:00 AM"));

                // Définit les dates 
                if (animalToUpdate.DateSterilization != null)
                    AnimalDateSterilization.SelectedDate = ((DateOnly)animalToUpdate.DateSterilization).ToDateTime(TimeOnly.Parse("12:00 AM"));
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                    Debug.WriteLine($"Erreur lors du chargement du formulaire de mise à jour d'un animal. Message: {ex.Message}. Error : {ex.ToString}");

                MessageBox.Show(ex.Message);
            }
        }



        private void DeleteAnimal(object sender, RoutedEventArgs e)
        {
            AnimalViewModel vm = (AnimalViewModel)this.DataContext;

            int index = Animals_DataGrid.SelectedIndex;
            RefugeWPF.ClassesMetiers.Model.Entities.Animal animal = (RefugeWPF.ClassesMetiers.Model.Entities.Animal) Animals_DataGrid.SelectedItem;

            Debug.WriteLine($"Animal selected : {animal}");

            if(index < 0)
            {
                MessageBox.Show("Veuillez sélectionner une ligne à supprimer.");
                return;
            }

            try
            {
                vm.DeleteAnimal(index);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                    Debug.WriteLine($"Erreur lors de la suppressioin d'un animal. Message: {ex.Message}. Error : {ex.ToString}");

                MessageBox.Show(ex.Message);
            }

            
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            RefugeWPF.ClassesMetiers.Model.Entities.Animal? animal = null;
            AnimalViewModel vm = (AnimalViewModel)this.DataContext;

            try
            {
                // Si l'ID de l'animal existe, on met à jour l'animal, sinon on crée un nouvel animal
                if (AnimalId.Text != "")
                {
                    Debug.WriteLine($"Animal ID : {AnimalId.Text}");
                    animal = new RefugeWPF.ClassesMetiers.Model.Entities.Animal(
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


                    // Mettre à jour l'animal
                    vm.UpdateAnimal(animal);


                }
                else
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

                    // Créer l'animal
                    vm.CreateAnimal(animal);
                }

                // Clean form
                this.ClearForm();

                // Close Form
                this.CloseForm();
                
                
            }
            catch (Exception ex)
            {

                throw;
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
            AnimalViewModel vm = (AnimalViewModel) this.DataContext;

            if(AnimalCompatibilitySelection.SelectedItem == null)
            {
                MessageBox.Show("Sélectionner une compatibilité");
                return;
            }

            AnimalCompatibilityDTO ac = new AnimalCompatibilityDTO(
                (Compatibility) AnimalCompatibilitySelection.SelectedItem,
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
            AnimalViewModel vm = (AnimalViewModel)this.DataContext;

            

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
        private void ClearListAnimalCompatibility(object sender, RoutedEventArgs e){
            AnimalViewModel vm = (AnimalViewModel)this.DataContext;

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
            RowListAnimal.Height = (GridLength) glConverter.ConvertFrom("300px")!;
            RowFormAnimal.Height = (GridLength) glConverter.ConvertFrom("600px")!;



        }

        private void CloseForm()
        {
            Animals_DataGrid.Height = 550;

            GridLengthConverter glConverter = new GridLengthConverter();
            RowListAnimal.Height = (GridLength)glConverter.ConvertFrom("600px")!;
            RowFormAnimal.Height = (GridLength)glConverter.ConvertFrom("300px")!;

        }

    }
}
