using RefugeWPF.CoucheMetiers.Helper;
using RefugeWPF.CoucheMetiers.Model.Entities;
using RefugeWPF.CoucheMetiers.Model.Enums;
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
using System.Text.RegularExpressions;

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
            Regex regex = new Regex("^(\\d{2})\\.(0[1-9]|1[0-2])\\.(0[1-9]|[1-2]\\d|3[0-1])-(\\d{3})\\.(\\d{2})$");

            if (!regex.IsMatch(ContactRegistryNumber_Textbox.Text))
            {
                MessageBox.Show("Le numéro de registre national doit être au format yy.mm.dd-999,99");
                return;
            }


            try
            {
                vm.SearchContact(ContactRegistryNumber_Textbox.Text);
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
        private void SearchAnimalButton_Click(object sender, RoutedEventArgs e)
        {
            AdmissionViewModel vm = (AdmissionViewModel)this.DataContext;
            
            try
            {
                vm.SearchAnimal(AnimalSearchByName_Textbox.Text);
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
        private void EnableCreateAdmission(object sender, RoutedEventArgs e)
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
            RefugeWPF.CoucheMetiers.Model.Entities.Animal? animal = null;
            AdmissionViewModel vm = (AdmissionViewModel) this.DataContext;
            string reason = (string) AdmissionReason_ComboBox.SelectedItem;

            if (reason == "inconnu")
            {
                MessageBox.Show("Sélectionner une raison d'admission");
                return;
            }

            try
            {
                
                if(reason == MyEnumHelper.GetEnumDescription<AdmissionType>(AdmissionType.ReturnFosterFamily)
                    || reason == MyEnumHelper.GetEnumDescription<AdmissionType>(AdmissionType.ReturnAdoption))
                {
                    if(vm.AnimalFound == null)
                    {
                        MessageBox.Show("Aucun animal n'est sélectionné");
                        return;
                    }

                    animal = vm.AnimalFound;
                }
                else
                {
                    animal = new RefugeWPF.CoucheMetiers.Model.Entities.Animal(
                        AnimalName.Text,
                        (TypeCat.IsChecked == null || (bool)TypeCat.IsChecked) ? AnimalType.Cat : AnimalType.Dog,
                        (GenderMale.IsChecked == null || (bool)GenderMale.IsChecked) ? GenderType.Male : GenderType.Female,
                        AnimalBirthDate.SelectedDate != null ? DateOnly.FromDateTime((DateTime)AnimalBirthDate.SelectedDate!) : null,
                        AnimalDeathDate.SelectedDate != null ? DateOnly.FromDateTime((DateTime)AnimalDeathDate.SelectedDate!) : null,
                        (bool)AnimalIsSterilized.IsChecked!,
                        AnimalDateSterilization.SelectedDate != null ? DateOnly.FromDateTime((DateTime)AnimalDateSterilization.SelectedDate!) : null,
                        AnimalParticularity.Text,
                        AnimalDescription.Text
                    );

                    // Transfert vers le ViewModel des couleurs de l'animal
                    foreach (CoucheMetiers.Model.Entities.Color c in AnimalColorsSelect.SelectedItems)
                    {
                        vm.SelectedAnimalColors.Add(c);
                    }
                }         


                // Créer l'animal
                vm.CreateAdmission(animal);

                // vider les champs du formulaire
                this.ClearForm();

                // Fermer le Formulaire
                this.CloseForm();


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors l'ajout d'un animal.\nMessage : {ex.Message}.\nErreur : {ex}");
                MessageBox.Show("Erreur lors l'ajout d'un animal.\nMessage : {ex.Message}");
            }

        }

        /**
         * <summary>
         *  Evénement Click sur le bouton "Ajouter" du formulaire de compatibilité pour l'animal.
         *  Les compatibilités, leurs valeurs et descriptions seront stockés dans une collection temporaire.         *  
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

            // Ré-initialiser la raison d'admission
            vm.SelectedAdmissionReason = "";

            // Ré-initialiser la recherche de la personne de contact
            ContactRegistryNumber_Textbox.Text = "";
            vm.ContactFound = null;

            // Ré-initialiser la recherche de l'animal
            AnimalSearchByName_Textbox.Text = "";
            vm.AnimalFound = null;

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
            Admissions_DataGrid.Height = 250;

            // Changement manuelle de la hauteur des grilles 
            GridLengthConverter glConverter = new GridLengthConverter();
            RowListAdmission.Height = (GridLength)glConverter.ConvertFrom("300px")!;
            RowFormAdmission.Height = (GridLength)glConverter.ConvertFrom("600px")!;


        }

        /**
         * <summary>
         *  Ferme le formulaire, en diminuant le grille parent
         * </summary>
         */
        private void CloseForm()
        {
            Admissions_DataGrid.Height = 280;
            
            // Changement manuelle de la hauteur des grilles 
            GridLengthConverter glConverter = new GridLengthConverter();
            RowListAdmission.Height = (GridLength)glConverter.ConvertFrom("400px")!;
            RowFormAdmission.Height = (GridLength)glConverter.ConvertFrom("400px")!;

        }


    }
}
