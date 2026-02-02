using RefugeWPF.CoucheMetiers.Model.Entities;
using RefugeWPF.CoucheAccesDB;
using RefugeWPF.CoucheMetiers.Model.DTO;
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
    class AnimalViewModel: INotifyPropertyChanged
    {
        
        private readonly IAnimalDataService animalDataService;
        private readonly IContactDataService contactDataService;
        private string _title;
        private ObservableCollection<Animal> _animals;
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


        public ObservableCollection<Animal> Animals {
            get { return _animals; } 

            private set
            {
                _animals = value;

            }
        }

        public Animal? Animals_DatagridSelection
        {
            get;
            set { field = value; }
        }

        public Animal? AnimalToUpdate
        {
            get;
            set;
        }

        public ObservableCollection<Color> Colors
        {
            get { return _colors; }

            set { _colors = value;  }
        }

        public ObservableCollection<Compatibility> Compatibilities
        {
            get { return _compatibilities; }

            set { _compatibilities = value; }
        }

        public ObservableCollection<AnimalCompatibilityDTO> AddedAnimalCompatibilities
        {
            get { return _addedAnimalCompatibilities; }
        }

        public ObservableCollection<Color> SelectedAnimalColors { get; set; }

        public Contact? ContactFound
        {
            get;
            set
            {
                field = value;
                NotifyPropertyChanged();
            } 
        }

        

        public AnimalViewModel()
        {
            _title = "Animaux";
            TitleForm = "Modifier un animal";
            animalDataService = new AnimalDataService();
            contactDataService = new ContactDataService();

            _animals = new ObservableCollection<Animal>(this.animalDataService.GetAnimals());
            _colors = new ObservableCollection<Color>(this.animalDataService.GetColors());
            _compatibilities = new ObservableCollection<Compatibility>(this.animalDataService.GetCompatibilities());
            _addedAnimalCompatibilities = new ObservableCollection<AnimalCompatibilityDTO>();
            SelectedAnimalColors = new ObservableCollection<Color>();

            
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /**
         * <summary>
         *  Affiche la liste des couleurs de chaque animal
         * </summary>
         */



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

                foreach (Color color in SelectedAnimalColors) {
                    AnimalColor animalColor = new AnimalColor(savedAnimal, color);

                    this.animalDataService.CreateAnimalColor(animalColor);

                    
                }

                // Vider la liste des couleurs pour l'animal
                this.SelectedAnimalColors.Clear();
                

                // Sauvegarde  des compatibilités pour l'animal
                if(AddedAnimalCompatibilities.Count > 0)
                {
                    foreach(AnimalCompatibilityDTO acdto in AddedAnimalCompatibilities)
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

        /**
         * <summary>
         *  
         * </summary>
         */
        internal void UpdateAnimal(Animal animal)
        {
            Animal? savedAnimal = null;

            try
            {
                // Sauvegarde de l'animal
                savedAnimal = this.animalDataService.UpdateAnimal(animal);
                
                
                // Mettre à jour la liste d'animaux
                Debug.WriteLine($"Index of {animal.Name} : {Animals.IndexOf(AnimalToUpdate!)}");
                Animals[Animals.IndexOf(AnimalToUpdate!)] = savedAnimal;


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors la mise à jour d'un animal.\nMessage : {ex.Message}.\nErreur : {ex}");

            }

            try
            {
                if (savedAnimal == null)
                    throw new Exception($"animal is null!");

                // Sauvegarde  des compatibilités pour l'animal
                if (AddedAnimalCompatibilities.Count > 0)
                {
                    // Sauvegarde des compatibilités ajoutés 
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
                Debug.WriteLine($"Erreur l'ajout d'une compatibilité pour un animal. Message : {ex.Message}.\n Erreur : {ex}");
                MessageBox.Show($"Erreur l'ajout d'une compatibilité pour un animal.");
            }

        }

        /**
         * <summary>
         *  
         * </summary>
         */ 
        internal void DeleteAnimal(Animal animal)
        {
            try
            {
                // Delete animal in database
                this.animalDataService.RemoveAnimal(animal);

                // Delete animal inside collection
                Animals.Remove(animal);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors de la suppréssion de l'animal.\nMessage : {ex.Message}.\nError : {ex}");
            }
        }
    }
}
