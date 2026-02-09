using RefugeWPF.CoucheAccesDB;
using RefugeWPF.CoucheMetiers.Helper;
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
    class AdoptionViewModel: INotifyPropertyChanged
    {
        // ===== Variables d'instance ===================================// 
        private readonly IAnimalDataService _animalDataService;
        private readonly IContactDataService _contactDataService;
        private readonly IRefugeDataService _refugeDataService;

        // ===== PROPRIETES ===================================// 
        public string Title { get; set; }
        public string TitleForm { get; set; }

        public ObservableCollection<Adoption> Adoptions
        {
            get;
            set
            {
                field = value;
                this.NotifyPropertyChanged();
            }
        } = new ObservableCollection<Adoption>();

        /**
         * <summary>
         *  Adoptions' Datagrid : Elément sélectionné
         * </summary>
         * 
         */ 
        public Adoption? AdoptionDataGridSelectedItem
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
         *  Adoptions Datagrid  : Sauvegarde de l'élément sélectionné au moment de cliquer le bouton "Modifier" 
         * </summary>
         * 
         */
        public Adoption? SelectedAdoption { get; set; }

        /**
         * <summary>
         *  Formulaire adoption : Liste des valeurs disponibles pour le statut de la candidature
         * </summary>
         * 
         */
        public List<string> AdoptionStatus
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
         *  Formulaire adoption :  Statut de candidature pour l'adoption sélectionné dans le ComboBox
         * </summary>
         * 
         */
        public string? SelectedAdoptionStatus { get; set; }

        /**
         * <summary>
         *  Formulaire adoption :  entrée utilisateur - Date de début
         * </summary>
         * 
         */
        public DateTime DateStart
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
         *   Formulaire adoption :  entrée utilisateur - Date de fin
         * </summary>
         * 
         */
        public DateTime? DateEnd
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

        /**
         * <summary>
         *   Formulaire adoption : entrée utilisateur numéro de registre national de la personne de contact
         * </summary>
         * 
         */
        public string? FormContactRegistryNumber { get; set; }

        /**
         * <summary>
         *   Formulaire adoption : Contact trouvé avec le numéro de registre national fourni
         * </summary>
         * 
         */
        public Contact? ContactFound
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
         *   Formulaire adoption : Indique que l'opération a réussi 
         * </summary>
         * 
         */
        public bool FormAdoptionOpSuccess
        {
            get;
            set
            {
                field = value;
                this.NotifyPropertyChanged();
            }
        }

        // ===== Constructeurs ===================================// 

        public AdoptionViewModel()
        {
            this._animalDataService = new AnimalDataService();
            this._contactDataService = new ContactDataService();
            this._refugeDataService = new RefugeDataService();
            Title = "Adoptions";
            TitleForm = "Candidature pour adoption";

            Adoptions = new ObservableCollection<Adoption>(this._refugeDataService.GetAdoptions());

            AdoptionStatus = MyEnumHelper.GetEnumDescriptions<ApplicationStatus>().ToList();

            

            DateStart = DateTime.Now;

        }

        // ===== Méthodes ===================================// 
        /**
         * <summary>
         *  Search existing contact  
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

                ContactFound = this._contactDataService.GetContactByRegistryNumber(FormContactRegistryNumber);

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
         *  Search existing animal  
         * </summary>
         */
        internal void CreateAdoption()
        {
            if(AnimalFound == null)
            {
                MessageBox.Show("Veuillez rechercher et valider l'animal.");
                return;
            }

            if(ContactFound == null)
            {
                MessageBox.Show("Veuillez rechercher et valider la personne de contact.");
                return;
            }

            try
            {
                Adoption adoption = new Adoption(
                    ApplicationStatus.Application,
                    DateTime.Now,
                    DateOnly.FromDateTime(DateStart),
                    DateEnd != null ? DateOnly.FromDateTime((DateTime) DateEnd) : null,
                    ContactFound,
                    AnimalFound
                );

                

                this._refugeDataService.CreateAdoption(adoption);

                // Ajouter à la liste des adoptions
                Adoptions.Add(adoption);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors de l'ajout d'une candidature pour adoption.\nMessage : {ex.Message}.\nErreur : {ex}");
                MessageBox.Show(ex.Message);
            }
        }

        internal void UpdateAdoption()
        {
            if(SelectedAdoption == null)
            {
                MessageBox.Show("Veuillez sélectionner une adoption dans la liste.");
                return;
            }

            try
            {
                Adoption adoptionUpdatedInfo = new Adoption(
                    SelectedAdoption.Id,
                    MyEnumHelper.GetEnumFromDescription<ApplicationStatus>(SelectedAdoptionStatus!),
                    SelectedAdoption.DateCreated,
                    DateOnly.FromDateTime(DateStart),
                    DateEnd != null ? DateOnly.FromDateTime((DateTime) DateEnd) : null,
                    SelectedAdoption.Contact,
                    SelectedAdoption.Animal
                );

                if(SelectedAdoption.Status != "acceptee" && SelectedAdoptionStatus == "acceptee")
                {
                    Release release = new Release(
                        ReleaseType.Adoption,
                        DateTime.Now,
                        SelectedAdoption.Animal,
                        SelectedAdoption.Contact
                    );

                    this._refugeDataService.CreateReleaseForAdoption(adoptionUpdatedInfo, release);

                }
                else
                {
                    this._refugeDataService.UpdateAdoption(SelectedAdoption);
                }

                // Mettre à jour l'objet dans la liste des adoptions
                Adoptions[Adoptions.IndexOf(SelectedAdoption)] = adoptionUpdatedInfo;


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur lors de la mise à jour de l'adoption.\nMessage : {ex.Message}.\n Erreur : {ex}");
                MessageBox.Show($"Erreur lors de la mise à jour de l'adoption.\nMessage : {ex.Message}");
            }
        }


        // ===== PROPRIETES ===================================// 

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
