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
    class VaccinationViewModel : INotifyPropertyChanged
    {
        private readonly IAnimalDataService _animalDataService;
        private readonly IContactDataService _contactDataService;
        private readonly IRefugeDataService _refugeDataService;

        public string Title { get; set; }

        public string TitleForm { get; set; }

        public ObservableCollection<Vaccination> Vaccinations {
            get;
            set{
                field = value;
                this.NotifyPropertyChanged();
            }
        }

        public DateTime? DateVaccination { 
            get; 
            set
            {
                field = value;
                this.NotifyPropertyChanged();
            }
        }

        public bool Done
        {
            get;
            set
            {
                field = value;
                this.NotifyPropertyChanged();
            }
        }

        public ObservableCollection<Vaccine> Vaccines
        {
            get;
            set
            {
                field = value;
                this.NotifyPropertyChanged();
            }
        } = new ObservableCollection<Vaccine>();

        public Vaccine? SelectedVaccine
        {
            get;
            set
            {
                field = value;
                this.NotifyPropertyChanged();
            }
        }

        public Vaccine? VaccineFound
        {
            get;
            set
            {
                field = value;
                this.NotifyPropertyChanged();
            }
        }

        /**
         * <summary>
         *  Adoption's form : search animal textbox input
         * </summary>
         * 
         */
        public string? FormAnimalName { get; set; }

        /**
         * <summary>
         *   Formulaire adoption : liste des animaux ayant le même nom que celui fourni
         * </summary>
         * 
         */
        public ObservableCollection<Animal> AnimalsFound
        {
            get;
            set
            {
                field = value;
                this.NotifyPropertyChanged();
            }
        } = new ObservableCollection<Animal>();

        /**
         * <summary>
         *   Formulaire adoption : animal sélectionné dans la liste des animaux ayant le même nom
         * </summary>
         * 
         */
        public Animal? AnimalFound
        {
            get;
            set
            {
                field = value;
                this.NotifyPropertyChanged();
            }
        }



        public VaccinationViewModel()
        {
            this._animalDataService = new AnimalDataService();
            this._contactDataService = new ContactDataService();
            this._refugeDataService = new RefugeDataService();

            Title = "Vaccinations";
            TitleForm = "Ajouter une vaccination";

            Vaccinations = new ObservableCollection<Vaccination>(this._refugeDataService.GetVaccinations());
            Vaccines = new ObservableCollection<Vaccine>(this._refugeDataService.GetVaccines());
        }

        // ========== Méthodes =======================================================//
        /**
         * <summary>
         *  Recherche d'un vaccin disponible  
         * </summary>
         */
        internal void SearchVaccine()
        {
            try
            {
                if (SelectedVaccine == null)
                {
                    MessageBox.Show("Veuillez choisir le vaccin à administrer à l'animal.");
                    return;
                }


                VaccineFound = this._refugeDataService.GetVaccine(SelectedVaccine.Name);


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors de la recherche d'un vaccin. Message: {ex.Message}. Erreur {ex}");
                MessageBox.Show($"Erreur lors de la recherche d'un vaccin. Message: {ex.Message}. Erreur {ex}");
            }

        }

        /**
         * <summary>
         *  Recherche d'un animal existant  
         * </summary>
         */
        internal void SearchAnimal()
        {
            try
            {
                if (FormAnimalName == null)
                {
                    MessageBox.Show("Veuillez saisir le nom de l'animal.");
                    return;
                }


                AnimalsFound = new ObservableCollection<Animal>(this._animalDataService.GetAnimalByName(FormAnimalName));



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
         *  Créer une vaccination pour un animal
         * </summary>
         * 
         */
        internal void CreateVaccination()
        {
            if(AnimalFound == null)
            {
                MessageBox.Show("Veuillez sélectionner un animal pour la vaccination.");
                return;
            }

            if(VaccineFound == null)
            {
                MessageBox.Show("Veuillez sélectionner un vaccin à administrer lors de la vaccination.");
                return;
            }

            try
            {
                Vaccination vaccination = new Vaccination(
                    DateVaccination != null ? DateOnly.FromDateTime((DateTime)DateVaccination) : null,
                    Done,
                    AnimalFound,
                    VaccineFound
                );

                this._refugeDataService.CreateVaccination(vaccination);

                Vaccinations.Add(vaccination);
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Erreur lors de la création de la vaccination.\nMessage: {ex.Message}.\nErreur : {ex}");
                
            }

        }


        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
