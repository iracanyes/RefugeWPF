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

        public DateTime DateEnd { get; set; }

        public ObservableCollection<Animal> AnimalsFound { get; set; } = new ObservableCollection<Animal>();
        public Animal? AnimalFound { get; set; }

        public Contact? ContactFound { get; set; }


        public FosterFamilyViewModel() {
            this.animalDataService = new AnimalDataService();
            this.contactDataService = new ContactDataService();
            this.refugeDataService = new RefugeDataService();
            Title = "Famille d'accueil";
            TitleForm = "Ajouter une famille d'accueil";

        }

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
                MessageBox.Show("Erreur durant la recherche de famille d'accueil par animal.");
            }
        }

        public void SearchFosterFamiliesByContact()
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
                MessageBox.Show("Erreur durant la recherche de famille d'accueil par animal.");
            }
        }

        public void CreateFosterFamily()
        {

        }
        
        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
