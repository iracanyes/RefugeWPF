using RefugeWPF.ClassesMetiers.Model.Entities;
using RefugeWPF.CoucheAccesDB;
using RefugeWPF.CoucheMetiers.Model.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace RefugeWPF.CouchePresentation.ViewModel
{
    class AnimalViewModel: INotifyPropertyChanged
    {
        private readonly AnimalDataService animalDataService;
        private string _title;
        private ObservableCollection<Animal> _animals;
        private ObservableCollection<Color> _colors;
        private ObservableCollection<Compatibility> _compatibilities;
        private ObservableCollection<AnimalCompatibility> _animalCompatibilities = new ObservableCollection<AnimalCompatibility>();
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

        public Animal? Selection
        {
            get;
            set { field = value; }
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

        

        public AnimalViewModel()
        {
            _title = "Animaux";
            TitleForm = "Ajouter un animal";
            animalDataService = new AnimalDataService();

            _animals = new ObservableCollection<Animal>(this.animalDataService.GetAnimals());
            _colors = new ObservableCollection<Color>(this.animalDataService.GetColors());
            _compatibilities = new ObservableCollection<Compatibility>(this.animalDataService.GetCompatibilities());
            _addedAnimalCompatibilities = new ObservableCollection<AnimalCompatibilityDTO>();
            
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void CreateAnimal(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal void UpdateAnimal(Animal animal)
        {
            throw new NotImplementedException();

        }

        internal void DeleteAnimal(int index)
        {
            throw new NotImplementedException();
        }
    }
}
