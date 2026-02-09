using RefugeWPF.CoucheAccesDB;
using RefugeWPF.CoucheMetiers.Model.Entities;
using RefugeWPF.CoucheMetiers.Model.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace RefugeWPF.CouchePresentation.ViewModel
{
    class FosterFamilyViewModel: INotifyPropertyChanged
    {
        private readonly IRefugeDataService refugeDataService;
        private readonly IContactDataService contactDataService;
        private readonly IAnimalDataService animalDataService;

        public string Title { get; set; }
        public string TitleForm { get; set; }

        /**
         * <summary>
         *  Liste des animaux accueillis par une famille d'accueil 
         * </summary> 
         */
        public ObservableCollection<FosterFamily> FosterFamilyAnimals
        {
            get;
            set
            {
                field = value;
                this.NotifyPropertyChanged();
            }
        } = new ObservableCollection<FosterFamily>();

        
        /**
         * <summary>
         *  Liste des familles d'accueil par l'animal est passé
         * </summary> 
         */ 
        public ObservableCollection<FosterFamily> AnimalFosterFamilies { 
            get;
            set
            {
                field = value;
                this.NotifyPropertyChanged();
            }
        } = new ObservableCollection<FosterFamily>();
        

        public string? SearchByAnimalName { 
            get;
            set
            {
                field = value;
                this.NotifyPropertyChanged();
            }
        }

        public string? SearchByContactRegistryNumber { 
            get;
            set
            {
                field = value;
                this.NotifyPropertyChanged();
            } 
        }

        public DateTime DateStart { get; set; }

        public DateTime? DateEnd { get; set; }

        public string? FormAnimalName
        {
            get;
            set
            {
                field = value;
                this.NotifyPropertyChanged();
            }
        }

        public ObservableCollection<Animal> AnimalsFound
        {
            get;
            set
            {
                field = value;
                this.NotifyPropertyChanged();
            }
        } = new ObservableCollection<Animal>();
        public Animal? AnimalFound
        {
            get;
            set
            {
                field = value;
                this.NotifyPropertyChanged();
            }
        }

        public string? FormContactRegistryNumber
        {
            get;
            set
            {
                field = value;
                this.NotifyPropertyChanged();
            }
        }

        public Contact? ContactFound { 
            get;
            set {
                field = value;
                this.NotifyPropertyChanged();
            } 
        }

        internal bool CreateFosterFamilySuccess
        {
            get;
            set
            {
                field = value;
                this.NotifyPropertyChanged();
            }
        }


        public FosterFamilyViewModel() {
            this.animalDataService = new AnimalDataService();
            this.contactDataService = new ContactDataService();
            this.refugeDataService = new RefugeDataService();
            Title = "Famille d'accueil";
            TitleForm = "Ajouter une famille d'accueil";
            DateStart = DateTime.Now;
            DateEnd = null;

        }

        /**
         * <summary>
         *  Lister les familles d’accueil par où l’animal est passé
         * </summary>
         */
        public void SearchFosterFamiliesByAnimal()
        {
            try
            {
                if(SearchByAnimalName == null)
                {
                    MessageBox.Show("Veuillez entrer le nom d'un animal.");
                    return;
                }

                Debug.WriteLine($"SearchByAnimalName : {SearchByAnimalName}");

                AnimalFosterFamilies = new ObservableCollection<FosterFamily>(this.refugeDataService.GetFosterFamiliesForAnimal(SearchByAnimalName));

                foreach (FosterFamily ff in AnimalFosterFamilies)
                    Debug.WriteLine(ff);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur durant la recherche de famille d'accueil par animal.\nMessage : {ex.Message}.\nErreur : {ex}");
                MessageBox.Show($"Erreur durant la recherche de famille d'accueil par animal.\nMessage : {ex.Message}");
            }
        }

        /**
         * <summary>
         *  Lister les animaux accueillis par une famille d’accueil
         * </summary>
         */
        internal void SearchFosterFamiliesByContact()
        {
            try
            {
                if (SearchByContactRegistryNumber == null)
                {
                    MessageBox.Show("Veuillez entrer le nom d'un animal.");
                    return;
                }

                

                FosterFamilyAnimals = new ObservableCollection<FosterFamily>(this.refugeDataService.GetFosterFamiliesForContact(SearchByContactRegistryNumber));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur durant la recherche de famille d'accueil par animal.\nMessage : {ex.Message}.\nErreur : {ex}");
                MessageBox.Show($"Erreur durant la recherche de famille d'accueil par animal.\nMessage : {ex.Message}");
            }
        }

        /**
         * <summary>
         *  Rechercher une personne de contact  
         * </summary>
         */
        internal void SearchContact()
        {
            try
            {
                if (FormContactRegistryNumber == null)
                {
                    MessageBox.Show("Veuillez saisir le numéro de registre national de la personne de contact dans la famille d'accueil.");
                    return;
                }

                ContactFound = this.contactDataService.GetContactByRegistryNumber(FormContactRegistryNumber);

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors de la recherche de la personne de contact. Message: {ex.Message}. Erreur {ex}");
                MessageBox.Show($"Erreur lors de la recherche de la personne de contact. Message: {ex.Message}. Erreur {ex}");
            }

        }

        /**
         * <summary>
         *  Rechercher un animal 
         * </summary>
         */
        internal void SearchAnimal()
        {
            try
            {
                if(FormAnimalName == null)
                {
                    MessageBox.Show("Veuillez saisir le nom de l'animal.");
                    return;
                }


                AnimalsFound = new ObservableCollection<Animal>(this.animalDataService.GetAnimalByName(FormAnimalName));



                if (AnimalsFound.Count == 1)
                {
                    AnimalFound = AnimalsFound.First();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors de la recherche de la personne de contact. Message: {ex.Message}. Erreur {ex}");
                MessageBox.Show($"Erreur lors de la recherche de la personne de contact. Message: {ex.Message}. Erreur {ex}");
            }

        }

        /**
         * <summary>
         *  Search existing animal  
         * </summary>
         */
        internal void CreateFosterFamily()
        {
            try
            {
                if(ContactFound == null) {
                    MessageBox.Show("Veuillez rechercher une personne de contact par son numéro de registre national.");
                    return;
                }

                if (AnimalFound == null) {
                    MessageBox.Show("Veuillez rechercher un animal par son nom et sélectionner un animal dans la liste si plusieurs animaux ont le même nom.");
                    return;
                }

                Release release = new Release(
                    ReleaseType.FosterFamily,
                    DateTime.Now,
                    AnimalFound,
                    ContactFound
                );

                FosterFamily ff = new FosterFamily(
                    DateOnly.FromDateTime(DateStart),
                    DateEnd != null ? DateOnly.FromDateTime((DateTime) DateEnd) : null,
                    ContactFound,
                    AnimalFound
                );

                CreateFosterFamilySuccess =  this.refugeDataService.CreateReleaseForFosterFamily(ff, release);

                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors de la création d'une sortie d'un animal en famille d'accueil.\nMessage{ex.Message}.\nErreur : {ex}");
                MessageBox.Show($"Erreur lors de la création d'une sortie d'un animal en famille d'accueil.");
            }
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
