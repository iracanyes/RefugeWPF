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

namespace RefugeWPF.CouchePresentation.View.Contact
{
    /// <summary>
    /// Logique d'interaction pour ContactView.xaml
    /// </summary>
    public partial class ContactView : UserControl
    {
        public ContactView()
        {
            InitializeComponent();
            this.DataContext = new ContactViewModel();
        }             



        /**
         * <summary>
         *  Evénement click sur le bouton "Ajouter" qui permet d'ouvrir le formulaire d'ajout
         *  d'un animal
         * </summary>
         */
        private void EnableCreateContact(object sender, RoutedEventArgs e)
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
                Debug.WriteLine($"Error while opening contact's form.\nMessage: {ex.Message}.\nError : {ex}");
                throw new Exception("Error while opening contact's form");
            }
        }

        /**
         * <summary>
         *  Evénement click sur le bouton "Modifier" qui permet d'ouvrir le formulaire de modification d'un contact
         *  d'un animal
         * </summary>
         */
        private void EnableUpdateContact(object sender, RoutedEventArgs e)
        {
            ContactViewModel vm = (ContactViewModel)this.DataContext;

            try
            {
                if (Contacts_DataGrid.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez sélectionner une personne de contact.");
                    return;
                }

                // Ouverture du formulaire
                this.OpenForm();

                // Cacher la liste des rôles 
                FormContactRolesSection.Visibility = Visibility.Collapsed;

                // Chargement de la personne de contact sélectionné dans la liste
                CoucheMetiers.Model.Entities.Contact selectedContact = (CoucheMetiers.Model.Entities.Contact) Contacts_DataGrid.SelectedItem;
                ContactFirstname.Text = selectedContact.Firstname;
                ContactLastname.Text = selectedContact.Lastname;
                ContactRegistryNumber_TextBox.Text = selectedContact.RegistryNumber;
                ContactRegistryNumber_TextBox.IsReadOnly = true;
                ContactEmail.Text = selectedContact.Email != null ? selectedContact.Email : "" ;
                ContactPhoneNumber_TextBox.Text = selectedContact.PhoneNumber != null ? selectedContact.PhoneNumber : "";
                ContactMobilePhone_TextBox.Text = selectedContact.MobileNumber != null ? selectedContact.MobileNumber : "";
                ContactAddressStreet.Text = selectedContact.Address.Street;
                ContactAddressCity.Text = selectedContact.Address.City;
                ContactAddressState.Text = selectedContact.Address.State;
                ContactAddressZipCode.Text = selectedContact.Address.ZipCode;
                ContactAddressCountry.Text = selectedContact.Address.Country;

                // Sauvegarde du contact sélectionné
                vm.SelectedContact = vm.DataGridSelectedItem;
                

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error while opening contact's form.\nMessage: {ex.Message}.\nError : {ex}");
                MessageBox.Show("Error while opening contact's form");
            }
        }

        /**
         * <summary>
         *  Evénement "Click" sur le bouton "Supprimer".
         *  Permet de supprimer un contact sélectionné dans la liste
         * </summary>
         */
        public void DeleteContact(object sender, RoutedEventArgs e)
        {
            ContactViewModel vm = (ContactViewModel)this.DataContext;

            Debug.WriteLine($"Contacts_DataGrid.SelectedItem : {Contacts_DataGrid.SelectedItem}");

            if (Contacts_DataGrid.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner une personne de contact.");
                return;
            }

            try
            {
                vm.SelectedContact = vm.DataGridSelectedItem;
                vm.DeleteContact(); 
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Erreur lors de la suppression du contact.\nMessage: {ex.Message}.\nError : {ex}");
                MessageBox.Show("Erreur lors de la suppression du contact");
            }
        }



        /**
         * <summary>
         *  Evénement "Click" sur le bouton "Confirmer" du formulaire d'ajout/modification de personne de contact
         * </summary>
         */
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            RefugeWPF.CoucheMetiers.Model.Entities.Animal? animal = null;
            ContactViewModel vm = (ContactViewModel)this.DataContext;

            try
            {

               

                if(vm.SelectedContact != null)
                {
                    vm.UpdateContact();
                }
                else
                {
                    // Transfert des rôles sélectionnés à la ViewModel
                    foreach (Role role in Roles_ListBox.SelectedItems)
                    {
                        vm.SelectedContactRoles.Add(role);
                    }

                    // Créer la personne de contact
                    vm.CreateContact();
                }

                    

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
            ContactViewModel vm = (ContactViewModel)this.DataContext;



            // Efface les valeurs des champs de formulaire
            ContactFirstname.Text = "";
            ContactLastname.Text = "";
            ContactRegistryNumber_TextBox.Text = "";
            ContactEmail.Text = "";
            ContactPhoneNumber_TextBox.Text =  "";
            ContactMobilePhone_TextBox.Text = "";
            ContactAddressStreet.Text = "";
            ContactAddressCity.Text = "";
            ContactAddressState.Text = "";
            ContactAddressZipCode.Text = "";
            ContactAddressCountry.Text = "";

            // Vider la collection des couleurs de l'animal
            vm.SelectedContactRoles.Clear();

            // Supprimer le contact sélectionné dans le DataGrid
            // lors d'une mise à jour
            vm.SelectedContact = null;

        }


        /**
         * <summary>
         *  Ouvre le formulaire et redimensionne les grilles (grid) et la liste des animaux
         * </summary>
         * 
         */
        private void OpenForm()
        {
            Contacts_DataGrid.Height = 270;

            // Changement manuelle de la hauteur des grilles 
            GridLengthConverter glConverter = new GridLengthConverter();
            RowListContact.Height = (GridLength)glConverter.ConvertFrom("300px")!;
            RowFormContact.Height = (GridLength)glConverter.ConvertFrom("600px")!;

            // Affichage du choix des rôles de la personne de contact
            FormContactRolesSection.Visibility = Visibility.Visible;

        }

        /**
         * <summary>
         *  Ferme le formulaire, en diminuant le grille parent
         * </summary>
         */
        private void CloseForm()
        {
            Contacts_DataGrid.Height = 550;

            // Changement manuelle de la hauteur des grilles 
            GridLengthConverter glConverter = new GridLengthConverter();
            RowListContact.Height = (GridLength)glConverter.ConvertFrom("600px")!;
            RowFormContact.Height = (GridLength)glConverter.ConvertFrom("300px")!;

        }
    }
}
