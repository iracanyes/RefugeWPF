
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using RefugeWPF.CouchePresentation.Navigation;
using RefugeWPF.CouchePresentation.Core;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;

namespace RefugeWPF.CouchePresentation.ViewModel
{
    class MainWindowViewModel: INotifyPropertyChanged
    {
        
        private object _currentViewModel;
        private readonly NavigationService _navigation;

        public ObservableCollection<MenuItemViewModel> MenuItems { get; }
            = new ObservableCollection<MenuItemViewModel>();

        [Required]
        public object CurrentViewModel
        {
            get { return _currentViewModel; } 
            set { _currentViewModel = value; OnPropertyChanged(); }
        }

        
        public ICommand NavigateCommand { get; }

        private void OnNavigate(string header)
        {
            Debug.WriteLine($"MainWindowViewModel.OnNavigate - header : {header}");

            switch (header)
            {
                case "Animaux":
                    _navigation.Navigate(new AnimalViewModel());
                    break;
                case "Contacts":
                    _navigation.Navigate(new ContactViewModel());
                    break;
                case "Admissions":
                    _navigation.Navigate(new AdmissionViewModel());
                    break;
                case "Familles d'accueil":
                    _navigation.Navigate(new FosterFamilyViewModel());
                    break;
                case "Adoptions":
                    _navigation.Navigate(new AdoptionViewModel());
                    break;
                case "Vaccinations":
                    _navigation.Navigate(new VaccinationViewModel());
                    break;
                default:
                    _navigation.Navigate(new AnimalViewModel());
                    break;

            }
        }

        public MainWindowViewModel()
        {
            _navigation = new NavigationService(vm => CurrentViewModel = vm);

            NavigateCommand = new RelayCommand<string>(OnNavigate);


            _currentViewModel = new AnimalViewModel();

        }

        

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        

        
    }
}
