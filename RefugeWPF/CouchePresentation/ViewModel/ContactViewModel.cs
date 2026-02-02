using RefugeWPF.CoucheAccesDB;
using RefugeWPF.CoucheMetiers.Model.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RefugeWPF.CouchePresentation.ViewModel
{
    class ContactViewModel: INotifyPropertyChanged
    {
        private readonly IContactDataService contactDataService;
        public string Title { get; set; }

        public string TitleForm { get; set; }

        public ObservableCollection<Contact> Contacts { get; set; }
        /**
         * <summary>
         *  Personne de contact sélectionné dans le DataGrid
         * </summary>
         */
        public Contact? DataGridSelectedItem { get; set; }

        /**
         * <summary>
         *  Personne de contact sélectionné lorsqu'on clique sur le bouton "Modifier"
         * </summary>
         */
        public Contact? SelectedContact { get; set; }

        /**
         * <summary>
         *  Liste des rôles des personnes de contact
         * </summary>
         */
        public ObservableCollection<Role> Roles { get; set; }

        /* Propriété du formulaire de personne de contact */
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;

        public string RegistryNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber {  get; set; } = string.Empty;

        public string MobileNumber {  get; set; } = string.Empty;

        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string ZipCode { get; set; } = string.Empty;

        public string Country { 
            get; 
            set; 
        } = string.Empty;

        public ObservableCollection<Role> SelectedContactRoles { get; set; } = new ObservableCollection<Role>();


        public ContactViewModel()
        {
            // Data service
            contactDataService = new ContactDataService();

            Title = "Contacts";
            TitleForm = "Ajouter un contact";
            Contacts = new ObservableCollection<Contact>(this.contactDataService.GetContacts());
            Roles = new ObservableCollection<Role>(this.contactDataService.GetRoles());
            Country = "Belgique";
        }

        /**
         * <summary>
         *  Ajouter une personne de contact
         * </summary>
         */ 
        public void CreateContact()
        {
            try
            {
                Address address = new Address(
                    Street,
                    City,
                    State,
                    ZipCode,
                    Country
                );

                Contact contact = new Contact(
                    Guid.NewGuid(),
                    Firstname,
                    Lastname,
                    RegistryNumber,
                    Email,
                    PhoneNumber,
                    MobileNumber,
                    address
                );

                foreach (Role role in SelectedContactRoles)
                {
                    ContactRole cr = new ContactRole(Guid.NewGuid(), contact, role);
                    contact.ContactRoles.Add(cr);
                }



                Contact result = this.contactDataService.HandleCreateContact(contact);

                Contacts.Add(result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors de la création d'une personne de contact.\nMessage : {ex.Message}.\nErreur : {ex}");
                throw new Exception("Erreur lors de la création d'une personne de contact.");
            }
        }

        /**
         * <summary>
         *  Modifier une personne de contact
         * </summary>
         */
        public void UpdateContact()
        {
            if(SelectedContact == null)
            {
                MessageBox.Show("Aucun contact sélectionné pour être mis à jour.");
                return;
            }

            try
            {
                Address address = new Address(
                    SelectedContact.Address.Id,
                    Street,
                    City,
                    State,
                    ZipCode,
                    Country
                );

                Contact contact = new Contact(
                    SelectedContact.Id,
                    Firstname,
                    Lastname,
                    RegistryNumber,
                    Email,
                    PhoneNumber,
                    MobileNumber,
                    address
                );


                Debug.WriteLine($"Contact : {contact} \n Address : {address}");

                Contact result = this.contactDataService.HandleUpdateContact(contact);

                Contacts[Contacts.IndexOf(SelectedContact)] = result;

                // Effacer le contact sélectionné en cliquant le bouton "mise à jour"
                SelectedContact = null;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors de la création d'une personne de contact.\nMessage : {ex.Message}.\nErreur : {ex}");
                throw new Exception("Erreur lors de la création d'une personne de contact.");
            }
        }

        /**
         * <summary>
         *  Supprimer un contact
         * </summary>
         */
        public void DeleteContact()
        {
            try
            {
                if (SelectedContact == null)
                {
                    MessageBox.Show("Sélectionnez une personne de contact dans la liste.");
                    return; 
                }                   


                Contact selectedContact = SelectedContact;
                
                this.contactDataService.DeleteContact(selectedContact);

                this.Contacts.Remove(selectedContact);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors de la suppression d'un contact.\nMessage : {ex.Message}.\nError : {ex}");
                throw new Exception("Erreur lors de la suppression d'un contact.");
            }
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
