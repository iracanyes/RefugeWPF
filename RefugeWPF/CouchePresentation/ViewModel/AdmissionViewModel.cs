using RefugeWPF.ClassesMetiers.Model.Entities;
using RefugeWPF.CoucheAccesDB;
using RefugeWPF.CoucheMetiers.Model.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace RefugeWPF.CouchePresentation.ViewModel
{
    class AdmissionViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly IRefugeDataService refugeDataService;
        private readonly IAnimalDataService animalDataService;
        private readonly IContactDataService contactDataService;
        private string _title;
        private ObservableCollection<Admission> _admissions;
        private ObservableCollection<Color> _colors;
        private ObservableCollection<Compatibility> _compatibilities;
        private ObservableCollection<AnimalCompatibilityDTO> _addedAnimalCompatibilities;

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string TitleForm
        {
            get;
            set;
        }


        public ObservableCollection<Admission> Animals
        {
            get { return _admissions; }

            private set
            {
                _admissions = value;

            }
        }

        public Animal? Selection
        {
            get;
            set { field = value; }
        }

        public ObservableCollection<Color> Colors
        {
            get { return _colors; }

            set { _colors = value; }
        }

        public ObservableCollection<Color> SelectedAnimalColors { get; set; }

        public ObservableCollection<Compatibility> Compatibilities
        {
            get { return _compatibilities; }

            set { _compatibilities = value; }
        }

        public ObservableCollection<AnimalCompatibilityDTO> AddedAnimalCompatibilities
        {
            get { return _addedAnimalCompatibilities; }
        }

        public Contact? ContactFound
        {
            get;
            set
            {
                field = value;
                NotifyPropertyChanged();
            }
        }



        public AdmissionViewModel()
        {
            _title = "Admissions";
            TitleForm = "Ajouter une admission";
            animalDataService = new AnimalDataService();
            contactDataService = new ContactDataService();
            refugeDataService = new RefugeDataService();

            _admissions = new ObservableCollection<Admission>(this.refugeDataService.GetAdmissions());
            _colors = new ObservableCollection<Color>(this.animalDataService.GetColors());
            _compatibilities = new ObservableCollection<Compatibility>(this.animalDataService.GetCompatibilities());
            _addedAnimalCompatibilities = new ObservableCollection<AnimalCompatibilityDTO>();
            SelectedAnimalColors = new ObservableCollection<Color>();

        }

        /**
         * <summary>
         *  Search existing contact  
         * </summary>
         */
        internal void SearchContact(string registryNumber)
        {
            try
            {

                ContactFound = this.contactDataService.GetContactByRegistryNumber(registryNumber);

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors de la recherche de la personne de contact. Message: {ex.Message}. Erreur {ex}");
                throw;
            }

        }

        /**
         * <summary>
         *  
         * </summary>
         */
        internal void CreateAnimal(Animal animal)
        {
            Animal? savedAnimal = null;

            try
            {
                // Sauvegarde de l'animal
                savedAnimal = this.animalDataService.CreateAnimal(animal);

                /* Sauvegarde des couleurs de l'animal */
                // si la liste des couleurs de l'animal est vide
                if (SelectedAnimalColors.Count == 0)
                    throw new Exception("Veuillez définir les couleurs de l'animal");

                foreach (Color color in SelectedAnimalColors)
                {
                    AnimalColor animalColor = new AnimalColor(savedAnimal, color);

                    this.animalDataService.CreateAnimalColor(animalColor);


                }

                // Vider la liste des couleurs pour l'animal
                this.SelectedAnimalColors.Clear();


                // Sauvegarde  des compatibilités pour l'animal
                if (AddedAnimalCompatibilities.Count > 0)
                {
                    foreach (AnimalCompatibilityDTO acdto in AddedAnimalCompatibilities)
                    {
                        AnimalCompatibility ac = new AnimalCompatibility(
                            savedAnimal,
                            acdto.Compatibility,
                            acdto.Value,
                            acdto.Description

                        );

                        this.animalDataService.CreateAnimalCompatibility(ac);


                    }

                    AddedAnimalCompatibilities.Clear();
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors l'ajout d'un animal.\nMessage : {ex.Message}.\nErreur : {ex}");

            }
        }



    }
}
