using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using MvvmHelpers;
using MvvmHelpers.Commands;
using SmartPharma5.Model;
using Acr.UserDialogs;
using CommunityToolkit.Maui.Views;
using SmartPharma5.View;

//using static Android.Renderscripts.FileA3D;

namespace SmartPharma5.ViewModel
{
    public class DocumentViewModel : BaseViewModel
    {   

        public ICommand SaveDocumentCommand { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        private int _entityId;
        private EntityType _entityType;
        private bool testLoad;
        public bool TestLoad { get => testLoad; set => SetProperty(ref testLoad, value); }
        public bool actpopup = true;
        public bool ActPopup { get => actpopup; set => SetProperty(ref actpopup, value); }
        /******************/
        public string EntityDocumentType { get; private set; }

        private ObservableCollection<Document> _documents = new ObservableCollection<Document>();
        public ObservableCollection<Document> Documents
        {
            get => _documents;
            set
            {
                _documents = value;
                OnPropertyChanged(nameof(Documents));
            }
        }
        private Dictionary<int, string> _documentTypes;
        public Dictionary<int, string> DocumentTypes
        {
            get => _documentTypes;
            set
            {
                _documentTypes = value;
                OnPropertyChanged(nameof(DocumentTypes));
            }
        }

        private KeyValuePair<int, string>? _selectedDocumentType;
        public KeyValuePair<int, string>? SelectedDocumentType
        {
            get => _selectedDocumentType;
            set
            {
                _selectedDocumentType = value;
                OnPropertyChanged(nameof(SelectedDocumentType));
            }
        }

        /**************/
        #region Constructors

        public DocumentViewModel(int entityId, EntityType entityType)
        {
            _entityId = entityId;
            _entityType = entityType;
            LoadDocumentsAsync();
        }

        public DocumentViewModel()
        {
            SaveDocumentCommand = new AsyncCommand<int>(SaveDocumentAsync); // <-- Utiliser AsyncCommand<int>
            LoadDocumentTypesCommand = new AsyncCommand(LoadDocumentTypesAsync);
            IsIOS = DeviceInfo.Platform == DevicePlatform.iOS;
        }
        #endregion
        #region Methods

        private async Task LoadDocumentsAsync()
        {            /****failed connectio******/
            bool Testcon = false;
            ActPopup = true;
            var P = Task.Run(() => DbConnection.Connecter3());
            Testcon = await P;
            if (Testcon == false)
            {


                TestLoad = true;
                //IsBusy = false;

                return;
            }
            TestLoad = false;
            /*********************/
            try
            {
                UserDialogs.Instance.ShowLoading("Loading...");
                await Task.Delay(200);

                IEnumerable<Document> documents = null;

                if (_entityType == EntityType.Opportunity)
                {
                    documents = await Document.GetDocumentsByOpportunityIdAsync(_entityId);
                }
                else if (_entityType == EntityType.Partner)
                {
                    documents = await Document.GetDocumentsByPartnerIdAsync(_entityId);
                }
                else if (_entityType == EntityType.Payment)
                {
                    documents = await Document.GetDocumentsByPaymentIdAsync(_entityId);
                }
                else if (_entityType == EntityType.Partner_Form)
                {
                    documents = await Document.GetDocumentsByPartnerFormIdAsync(_entityId);
                }

                if (documents != null && documents.Any())
                {
                    Documents.Clear();
                    foreach (var document in documents)
                    {
                        Documents.Add(document);
                    }
                }
            }
            catch (Exception ex)
            {
                TestLoad = true;
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
            ActPopup = false;
        }

        public async Task ProcessFile(string filePath, string fileName)
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Chargement...");
                await Task.Delay(100); // Petit délai pour s'assurer que l'UI est prête

                var documentTypes = await Document.GetDocumentTypesAsync() ?? new Dictionary<int, string>();

                // Assurer l'exécution sur le thread UI
                await MainThread.InvokeOnMainThreadAsync(async () => 
                {
                    UserDialogs.Instance.HideLoading();
                    
                    var popup = new CustomPopup(documentTypes, fileName);
                    var result = await Application.Current.MainPage.ShowPopupAsync(popup);

                    if (result == null)
                    {
                        await Application.Current.MainPage.DisplayAlert("Cancelation", "The selection process has been cancelled.", "OK");
                        return;
                    }

                    var data = (dynamic)result;
                    var newFileName = data.FileName;
                    var memo = data.Memo;
                    var description = data.Description;
                    var selectedTypeId = data.TypeId != 0 ? (int?)data.TypeId : null;

                    // Créer un document avec le constructeur
                    var temporaryDocument = new Document(
                        newFileName,
                        Path.GetExtension(fileName),
                        await File.ReadAllBytesAsync(filePath),
                        memo,
                        description,
                        selectedTypeId
                    );

                    try
                    {
                        bool isSaved = false;

                        if (_entityType == EntityType.Opportunity)
                            isSaved = await Document.SaveToDatabase(temporaryDocument, _entityId);
                        else if (_entityType == EntityType.Partner)
                            isSaved = await Document.SaveToDatabasePartner(temporaryDocument, _entityId);
                        else if (_entityType == EntityType.Payment)
                            isSaved = await Document.SaveToDatabasePayment(temporaryDocument, _entityId);
                        else if (_entityType == EntityType.Partner_Form)
                            isSaved = await Document.SaveToDatabasePartnerForm(temporaryDocument, _entityId);
                        
                        if (isSaved)
                        {
                            Documents.Add(temporaryDocument);
                            await Application.Current.MainPage.DisplayAlert("Success", $"File added: {newFileName}", "OK");
                            await LoadDocumentsAsync();
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Error", "Failed to save the document to the database.", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred while saving the document: {ex.Message}", "OK");
                    }
                });
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        public async Task OpenDocumentAsync(Document document)
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Opening...");

                if (document.content == null)
                {
                    document.content = await Document.GetDocumentContentAsync(document.Id);
                }

                if (document.content == null || document.content.Length == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Document is empty or invalid.", "OK");
                    return;
                }

                string filePath = Path.Combine(FileSystem.CacheDirectory, document.name + document.extension);
                await File.WriteAllBytesAsync(filePath, document.content);
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ICommand LoadDocumentTypesCommand { get; }


        public async Task LoadDocumentTypesAsync()
        {
            if (DocumentTypes == null || DocumentTypes.Count == 0)
            {
                var types = await Document.GetDocumentTypesAsync();
                DocumentTypes = types ?? new Dictionary<int, string>();
            }
        }
        public async Task SaveDocumentAsync(int opportunityId) // <-- Ajouter opportunityId en paramètre
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Veuillez sélectionner un fichier",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    var document = new Document
                    {
                        name = result.FileName,
                        create_date = DateTime.Now,
                        extension = Path.GetExtension(result.FileName),
                        content = await File.ReadAllBytesAsync(result.FullPath)
                    };

                    // Appeler SaveToDatabase avec les deux paramètres
                    bool isSaved = await Document.SaveToDatabase(document, opportunityId); // <-- Correction ici
                    if (isSaved)
                    {
                        await Application.Current.MainPage.DisplayAlert("Succès", "Document sauvegardé avec succès", "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Erreur", "Échec de la sauvegarde du document", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
            }
        }
        #endregion

        private bool _isIOS;
        public bool IsIOS
        {
            get => _isIOS;
            set
            {
                if (_isIOS != value)
                {
                    _isIOS = value;
                    OnPropertyChanged(nameof(IsIOS));
                    OnPropertyChanged(nameof(IsNotIOS));
                }
            }
        }

        public bool IsNotIOS => !IsIOS;

    }
    public enum EntityType
    {
        Opportunity,
        Partner,
        Payment,
        Partner_Form
    }
}