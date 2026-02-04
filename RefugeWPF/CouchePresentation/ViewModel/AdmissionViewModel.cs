using RefugeWPF.CoucheMetiers.Helper;
using RefugeWPF.CoucheMetiers.Model.Entities;
using RefugeWPF.CoucheMetiers.Model.Enums;
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

        /**
         * <summary>
         *  Titre de la page
         * </summary>
         */
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /**
         * <summary>
         *  Titre du formulaire
         * </summary>
         */
        public string TitleForm
        {
            get;
            set;
        }

        /**
         * <summary>
         *  Liste des admissions
         * </summary>
         */
        public ObservableCollection<Admission> Admissions
        {
            get { return _admissions; }

            private set
            {
                _admissions = value;

            }
        }

        /**
         * <summary>
         *  
         * </summary>
         */
        public Admission? Selection
        {
            get;
            set { field = value; }
        }

        /**
         * <summary>
         *  
         * </summary>
         */
        public ObservableCollection<Color> Colors
        {
            get { return _colors; }

            set { _colors = value; }
        }

        /**
         * <summary>
         *  
         * </summary>
         */
        public ObservableCollection<Color> SelectedAnimalColors { get; set; }

        /**
         * <summary>
         *  
         * </summary>
         */
        public ObservableCollection<Compatibility> Compatibilities
        {
            get { return _compatibilities; }

            set { _compatibilities = value; }
        }

        /**
         * <summary>
         *  
         * </summary>
         */
        public ObservableCollection<AnimalCompatibilityDTO> AddedAnimalCompatibilities
        {
            get { return _addedAnimalCompatibilities; }
        }

        /**
         * <summary>
         *  
         * </summary>
         */
        public Contact? ContactFound
        {
            get;
            set
            {
                field = value;
                NotifyPropertyChanged();
            }
        }

        /**
         * <summary>
         *  Liste des raisons d'admission dans le formulaire d'admission
         * </summary>
         *  
         */
        public List<string> AdmissionReasons { get; set; } = MyEnumHelper.GetEnumDescriptions<AdmissionType>().ToList();

        /**
         * <summary>
         *  Raison d'admission sélectionné dans le  ComboBox "AdmissionReason_ComboBox"
         * </summary>
         * 
         */
        public string SelectedAdmissionReason { 
            get;
            set
            {
                field = value;

                switch (value)
                {
                    case "errant":
                    case "deces_proprietaire":
                    case "saisie":
                        AnimalAlreadyRegistered = false;
                        break;
                    case "retour_adoption":
                    case "retour_famille_accueil":
                        AnimalAlreadyRegistered = true;
                        break;
                    default:
                        AnimalAlreadyRegistered = false;
                        break;
                }

                this.NotifyPropertyChanged();
            } 
        }

        public bool AnimalAlreadyRegistered { 
            get;
            set {
                field = value;
                this.NotifyPropertyChanged();
            }
        } = false;

        public ObservableCollection<Animal> AnimalsFound { 
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


        public AdmissionViewModel()
        {
            _title = "Admissions";
            TitleForm = "Ajouter une admission";
            animalDataService = new AnimalDataService();
            contactDataService = new ContactDataService();
            refugeDataService = new RefugeDataService();

            // Récupération des admissions
            HashSet<Admission> admissions = this.refugeDataService.GetAdmissions();
            // Ajout des couleurs des animaux
            foreach (Admission ad in admissions)
            {
                this.animalDataService.GetAnimalColors(ad.Animal);
                Debug.WriteLine($"Admissions : \n{ad.Animal}");
            }

            _admissions = new ObservableCollection<Admission>(admissions);
            _colors = new ObservableCollection<Color>(this.animalDataService.GetColors());
            _compatibilities = new ObservableCollection<Compatibility>(this.animalDataService.GetCompatibilities());
            _addedAnimalCompatibilities = new ObservableCollection<AnimalCompatibilityDTO>();
            SelectedAnimalColors = new ObservableCollection<Color>();
            SelectedAdmissionReason = "inconnu";

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
         *  Search existing animal  
         * </summary>
         */
        internal void SearchAnimal(string name)
        {
            try
            {

                AnimalsFound = new ObservableCollection<Animal>(this.animalDataService.GetAnimalByName(name));

                if (AnimalsFound.Count == 1) { 
                    AnimalFound = AnimalsFound.First();
                }
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
        internal void CreateAdmission(Animal animal)
        {
            Animal? savedAnimal = null;

            if(ContactFound == null)
            {
                MessageBox.Show($"Aucun contact sélectionné pour l'ajout d'un animal!");
                return;
            }

            try
            {
                if(SelectedAdmissionReason == MyEnumHelper.GetEnumDescription<AdmissionType>(AdmissionType.ReturnAdoption)
                    || SelectedAdmissionReason == MyEnumHelper.GetEnumDescription<AdmissionType>(AdmissionType.ReturnFosterFamily)
                ) {
                    if (AnimalFound == null) {
                        MessageBox.Show("Aucun animal sélectionné pour un retour d'adoption ou de famille d'accueil.");
                        return;
                    }

                    savedAnimal = AnimalFound;
                }
                else
                {
                    /* Sauvegarde du nouvel animal, ses couleurs et compatibilités */

                    // Sauvegarde du nouveau animal
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
                        // Vider la liste temporaire des compatibilités pour l'animal
                        AddedAnimalCompatibilities.Clear();
                    }
                }

                

                // Création de l'admission
                Admission ad = new Admission(
                    SelectedAdmissionReason,
                    DateTime.Now,
                    ContactFound,
                    savedAnimal
                );

                // Sauvegarde de la nouvelle admission
                this.refugeDataService.HandleCreateAdmission(ad);

                // Mise à jour de la liste des admissions.
                Admissions.Add(ad);


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors l'ajout d'un animal.\nMessage : {ex.Message}.\nErreur : {ex}");

            }
        }



    }
}
