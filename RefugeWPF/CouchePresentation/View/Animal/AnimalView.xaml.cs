
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RefugeWPF.CoucheMetiers.Helper;
using RefugeWPF.CoucheMetiers.Model.Entities;
using RefugeWPF.CoucheMetiers.Model.Enums;
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

        

        private void UpdateAnimal(object sender, RoutedEventArgs e)
        {
            AnimalViewModel vm = (AnimalViewModel)this.DataContext;
            
            try
            {
                RefugeWPF.CoucheMetiers.Model.Entities.Animal animal = new RefugeWPF.CoucheMetiers.Model.Entities.Animal(
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
         *  Evénement click sur le bouton "Modifier" qui permet d'ouvrir le formulaire de mise à jour
         *  d'un animal
         * </summary>
         */
        private void EnableUpdateAnimal(object sender, RoutedEventArgs e)
        {
            AnimalViewModel vm = (AnimalViewModel)this.DataContext;

            // Sauvegarde de la sélection de l'animal à mettre à jour
            vm.AnimalToUpdate = vm.Animals_DatagridSelection;

            RefugeWPF.CoucheMetiers.Model.Entities.Animal? animalToUpdate = vm.Animals_DatagridSelection;

            if(animalToUpdate == null)
            {
                MessageBox.Show("Sélectionner un animal pour le modifier");
                return;
            }

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


        /**
         * <summary>
         *  Evénement "Click" sur le bouton "Supprimer" un animal sélectionné dans la liste
         * </summary>
         */
        private void DeleteAnimal(object sender, RoutedEventArgs e)
        {
            AnimalViewModel vm = (AnimalViewModel)this.DataContext;

            int index = Animals_DataGrid.SelectedIndex;
            RefugeWPF.CoucheMetiers.Model.Entities.Animal animal = (RefugeWPF.CoucheMetiers.Model.Entities.Animal) Animals_DataGrid.SelectedItem;

            Debug.WriteLine($"Animal selected : {animal}");

            if(index < 0)
            {
                MessageBox.Show("Veuillez sélectionner une ligne à supprimer.");
                return;
            }

            try
            {
                vm.DeleteAnimal(animal);
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
         *  Evénement "Click" sur le bouton de soummission du formulaire pour la mise à jour de l'animal
         * </summary>
         */ 
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            RefugeWPF.CoucheMetiers.Model.Entities.Animal? animal = null;
            AnimalViewModel vm = (AnimalViewModel)this.DataContext;

            try
            {
                // Si l'ID de l'animal existe, on met à jour l'animal, sinon une erreur est retourné
                if (AnimalId.Text != "")
                {
                    Debug.WriteLine($"""
                        Animal ID : {AnimalId.Text}
                        AnimalName.Text : {AnimalName.Text}
                        TypeCat.IsChecked : {TypeCat.IsChecked}
                        TypeDog.IsChecked : {TypeDog.IsChecked}
                        GenderMale.IsChecked : {GenderMale.IsChecked}
                        GenderFemale.IsChecked : {GenderFemale.IsChecked}
                        AnimalBirthDate.SelectedDate : {AnimalBirthDate.SelectedDate}
                        AnimalDeathDate.SelectedDate : {AnimalDeathDate.SelectedDate}
                        AnimalIsSterilized.IsChecked : {AnimalIsSterilized.IsChecked}
                        AnimalDateSterilization.SelectedDate : {AnimalDateSterilization.SelectedDate}
                        AnimalParticularity.Text : {AnimalParticularity.Text}
                        AnimalDescription.Text : {AnimalDescription.Text}
                    """);
                        
                    animal = new CoucheMetiers.Model.Entities.Animal(
                        AnimalId.Text,
                        AnimalName.Text,
                        (TypeCat.IsChecked == null || (bool) TypeCat.IsChecked) ? AnimalType.Cat : AnimalType.Dog,
                        (GenderMale.IsChecked == null || (bool)GenderMale.IsChecked) ? GenderType.Male : GenderType.Female,
                        AnimalBirthDate.SelectedDate != null ? DateOnly.FromDateTime((DateTime) AnimalBirthDate.SelectedDate!) : null,
                        AnimalDeathDate.SelectedDate != null ? DateOnly.FromDateTime((DateTime)AnimalDeathDate.SelectedDate!) : null,
                        (bool) AnimalIsSterilized.IsChecked!,
                        AnimalDateSterilization.SelectedDate != null ? DateOnly.FromDateTime((DateTime) AnimalDateSterilization.SelectedDate!) : null,
                        AnimalParticularity.Text,
                        AnimalDescription.Text
                    );


                    // Mettre à jour l'animal
                    vm.UpdateAnimal(animal);

                    this.ClearForm();

                    this.CloseForm();


                }
                else
                {
                    throw new Exception($"Aucun identifiant d'animal n'est fourni!");
                }

                // Clean form
                this.ClearForm();

                // Close Form
                this.CloseForm();
                
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors l'ajout d'un animal.\nMessage : {ex.Message}.\nErreur : {ex}");
                MessageBox.Show(ex.Message);

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
         *  Efface les valeurs contenues dans le formulaire
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

            // Effacer les valeurs du formulaire de compatibilité pour l'animal
            this.ClearFormAnimalCompatibility();

            // Vider la collection contenant les objets AnimalCompatibilityDTO
            vm.AddedAnimalCompatibilities.Clear();

            // Vider la collection des couleurs de l'animal
            vm.SelectedAnimalColors.Clear();

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
