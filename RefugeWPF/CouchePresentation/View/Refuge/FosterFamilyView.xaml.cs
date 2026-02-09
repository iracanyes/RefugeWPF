using RefugeWPF.CoucheMetiers.Helper;
using RefugeWPF.CoucheMetiers.Model.Enums;
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
    /// Logique d'interaction pour FosterFamilyView.xaml
    /// </summary>
    public partial class FosterFamilyView : UserControl
    {
        public FosterFamilyView()
        {
            InitializeComponent();
            this.DataContext = new FosterFamilyViewModel();
        }


        public void EnableAnimalFosterFamiliesList(object sender, RoutedEventArgs e)
        {
            SearchFosterFamilyAnimals.Visibility = Visibility.Collapsed;
            SearchAnimalFosterFamilies.Visibility = Visibility.Visible;
            

            FosterFamilyAnimals_DataGrid.Visibility = Visibility.Collapsed;
            AnimalFosterFamilies_DataGrid.Visibility = Visibility.Visible;
            
        }

        public void ShowFosterFamilyAnimalsList(object sender, RoutedEventArgs e)
        {
            SearchAnimalFosterFamilies.Visibility = Visibility.Collapsed;
            SearchFosterFamilyAnimals.Visibility = Visibility.Visible;

            AnimalFosterFamilies_DataGrid.Visibility = Visibility.Collapsed;
            FosterFamilyAnimals_DataGrid.Visibility = Visibility.Visible;
            
        }

        public void SearchFosterFamiliesByAnimal(object sender, RoutedEventArgs e)
        {
            FosterFamilyViewModel vm = (FosterFamilyViewModel) this.DataContext;


            try
            {
                vm.SearchFosterFamiliesByAnimal();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        public void SearchFosterFamiliesByContact(object sender, RoutedEventArgs e)
        {
            FosterFamilyViewModel vm = (FosterFamilyViewModel)this.DataContext;


            try
            {
                vm.SearchFosterFamiliesByContact();
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
            FosterFamilyViewModel vm = (FosterFamilyViewModel)this.DataContext;
            Regex regex = new Regex("^(\\d{2})\\.(0[1-9]|1[0-2])\\.(0[1-9]|[1-2]\\d|3[0-1])-(\\d{3})\\.(\\d{2})$");

            if (!regex.IsMatch(ContactRegistryNumber_Textbox.Text))
            {
                MessageBox.Show("Le numéro de registre national doit être au format yy.mm.dd-999.99");
                return;
            }

            try
            {
                vm.SearchContact();
            }catch(Exception ex)
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
            FosterFamilyViewModel vm = (FosterFamilyViewModel)this.DataContext;

            
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
         *  Evénement click sur le bouton "Ajouter" qui permet d'ouvrir le formulaire d'ajout
         *  d'un animal
         * </summary>
         */
        private void EnableCreateFosterFamily(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ouverture du formulaire
                this.OpenForm();

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
         *  Evénement "Click" sur le bouton "Confirmer". 
         *  Permet d'ajouter une famille d'accueil pour un animal
         * </summary>
         */
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            
            FosterFamilyViewModel vm = (FosterFamilyViewModel) this.DataContext;

            
            try
            {
                


                // Créer l'animal
                vm.CreateFosterFamily();

                // vider les champs du formulaire
                this.ClearForm();

                // Fermer le Formulaire
                this.CloseForm();


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors l'ajout d'un animal.\nMessage : {ex.Message}.\nErreur : {ex}");
                throw new Exception("Erreur lors l'ajout d'un animal.");
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
            FosterFamilyViewModel vm = (FosterFamilyViewModel)this.DataContext;



            // Ré-initiliaser les champs de date de début et fin de l'accueil
            vm.DateStart = DateTime.Now;
            vm.DateEnd = null;


            // Ré-initialiser la recherche de la personne de contact
            vm.FormContactRegistryNumber = "";
            vm.ContactFound = null;

            // Ré-initialiser la recherche de l'animal
            vm.FormAnimalName = "";
            vm.AnimalFound = null;
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
            AnimalFosterFamilies_DataGrid.Height = 250;
            FosterFamilyAnimals_DataGrid.Height = 250;

            // Changement manuelle de la hauteur des grilles 
            GridLengthConverter glConverter = new GridLengthConverter();
            RowListFosterFamilies.Height = (GridLength)glConverter.ConvertFrom("280px")!;
            RowFormFosterFamilies.Height = (GridLength)glConverter.ConvertFrom("720px")!;



        }

        /**
         * <summary>
         *  Ferme le formulaire, en diminuant le grille parent
         * </summary>
         */
        private void CloseForm()
        {
            
            AnimalFosterFamilies_DataGrid.Height = 420;
            FosterFamilyAnimals_DataGrid.Height = 420;

            // Changement manuelle de la hauteur des grilles 
            GridLengthConverter glConverter = new GridLengthConverter();
            RowListFosterFamilies.Height = (GridLength)glConverter.ConvertFrom("600px")!;
            RowFormFosterFamilies.Height = (GridLength)glConverter.ConvertFrom("300px")!;

        }

    }
}
