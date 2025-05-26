using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using MySqlConnector;
using SmartPharma5.Model;
using SmartPharma5.View.FloatingActionButton;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View
{
    public partial class FileSelectionView : ContentPage
    {
        internal DocumentViewModel _viewModel;

        public FileSelectionView(int entityId, EntityType entityType, Dictionary<string, object> navigationParams = null)
        {
            InitializeComponent();
            var modelViewEntityType = (SmartPharma5.ModelView.EntityType)entityType;
            _viewModel = new DocumentViewModel(entityId, entityType);
            string entityTypenote = CurrentData.CurrentNoteModule;
            string entityTypeactivity = CurrentData.CurrentActivityModule;

            BindingContext = _viewModel;
        }
        /* private void OpenDrawer_Clicked(object sender, EventArgs e)
         {
             if (NavigationDrawer != null)
             {
                 NavigationDrawer.ToggleDrawer();
             }
         }*/

        private async void OpenNewPage_Clicked(object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Loading...");
                await Task.Delay(200); // Simuler une t�che asynchrone
                await Navigation.PushAsync(new SimpleNavigationView());
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"Une erreur s'est produite : {ex.Message}", "Erreur");
            }
            finally
                {
                UserDialogs.Instance.HideLoading();
            }
        }

        private void OnActionButtonClicked(object sender, EventArgs e)
        {
            // Identifier quel bouton a �t� cliqu�
            var button = sender as ImageButton;

            // Afficher une alerte avec le nom de l'image cliqu�e
            string buttonName = button.Source.ToString().Replace("File: ", ""); // Extraire le nom de l'image
            DisplayAlert("Action", $"Vous avez cliqu� sur {buttonName}", "OK");
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var tappedFrame = sender as Frame;
            var document = tappedFrame?.BindingContext as Document;

            if (document != null)
            {
                await _viewModel.OpenDocumentAsync(document);
            }
        }

        private async void OnAddFileClicked(object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Loading...");
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Please select a file",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "image/*", "application/pdf" } }
                })
                });

                if (result != null)
                {
                    string filePath = result.FullPath;
                    string fileName = result.FileName;
                    await _viewModel.ProcessFile(filePath, fileName);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error has occurred: {ex.Message}", "OK");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private async void OnTakePhotoClicked(object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Loading...");
                var photo = await MediaPicker.CapturePhotoAsync();

                if (photo != null)
                {
                    string filePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                    using (var stream = await photo.OpenReadAsync())
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await stream.CopyToAsync(fileStream);
                    }

                    string fileName = photo.FileName;
                    await _viewModel.ProcessFile(filePath, fileName);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error has occurred: {ex.Message}", "OK");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }
        private async void OnDrawingClicked(object sender, EventArgs e)
        {
            try
            {
                // S'assurer que nous sommes abonnés au message avant d'ouvrir la page de dessin
                MessagingCenter.Unsubscribe<DrawingPdfView, Tuple<string, string>>(this, "NewPdfCreated");
                MessagingCenter.Subscribe<DrawingPdfView, Tuple<string, string>>(this, "NewPdfCreated", async (sender, args) =>
                {
                    var (filePath, fileName) = args;
                    await _viewModel.ProcessFile(filePath, fileName);
                });

                UserDialogs.Instance.ShowLoading("Ouverture du dessin...");
                await Task.Delay(100); // facultatif pour montrer le loading

                await Navigation.PushAsync(new FloatingActionButton.DrawingPdfView());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            var document = button?.BindingContext as Document;

            if (document != null)
            {
                bool confirm = await DisplayAlert("Confirmation", "Are you sure you want to delete this document?", "Yes", "No");
                if (confirm)
                {
                    try
                    {
                        bool isDeleted = await Document.DeleteDocumentAsync(document.Id);

                        if (isDeleted)
                        {
                            _viewModel.Documents.Remove(document);
                            await DisplayAlert("Success", "Document deleted successfully.", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Error", "Failed to delete the document.", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                    }
                }
            }
        }


    }
}
/*  private async Task ProcessFile(string filePath, string fileName)
  {
      var documentTypes = await Document.GetDocumentTypesAsync();
      if (documentTypes == null || !documentTypes.Any())
      {
          await DisplayAlert("Error", "Unable to retrieve document types.", "OK");
          return;
      }

      var popup = new CustomPopup(documentTypes, fileName);
      var result = await this.ShowPopupAsync(popup);

      if (result == null)
      {
          await DisplayAlert("Cancelation", "The selection process has been cancelled.", "OK");
          return;
      }

      // R�cup�rer les donn�es du popup
      var data = (dynamic)result;
      var newFileName = data.FileName;
      var memo = data.Memo;
      var description = data.Description;
      var selectedTypeId = data.TypeId;

      // Cr�er un document temporaire
      var temporaryDocument = new Document
      {
          name = newFileName, // Utiliser le nouveau nom du fichier
          extension = Path.GetExtension(fileName),
          content = await File.ReadAllBytesAsync(filePath),
          create_date = DateTime.Now,
          date = DateTime.Now,
          memo = memo,
          description = description,
          type_document = (uint)selectedTypeId
      };
      temporaryDocument.size = temporaryDocument.content?.LongLength ?? 0;

      try
      {
          // R�cup�rer l'ID de l'opportunit�
          int opportunityId = Opportunity.Id;
          bool isSaved = await Document.SaveToDatabase(temporaryDocument, opportunityId);

          if (isSaved)
          {
              // Ajouter le document temporaire � la liste dans le ViewModel
              if (BindingContext is OpportunityViewModel viewModel)
              {
                  viewModel.TemporaryDocuments.Add(temporaryDocument);
              }

              // Afficher un message de confirmation
              await DisplayAlert("Success", $"File added: {newFileName}", "OK");

              // Recharger les documents apr�s l'ajout
              await LoadDocumentsAsync();
          }
          else
          {
              await DisplayAlert("Error", "Failed to save the document to the database.", "OK");
          }
      }
      catch (Exception ex)
      {
          await DisplayAlert("Error", $"An error occurred while saving the document: {ex.Message}", "OK");
      }
  }
*/

/*
 
 using System;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View
{
    public partial class FileSelectionView : ContentPage
    {
        public FileSelectionView(int opportunityId)
        {
            InitializeComponent();
            this.BindingContext = new FileSelectionViewModel(opportunityId);
        }

private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
{
    var tappedFrame = sender as Frame;
    var document = tappedFrame?.BindingContext as Document;

    if (document != null)
    {
        await OpenDocumentAsync(document);
    }
}

private async Task OpenDocumentAsync(Document document)
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
            await DisplayAlert("Error", "Document is empty or invalid.", "OK");
            return;
        }

        string filePath = System.IO.Path.Combine(FileSystem.CacheDirectory, document.name + document.extension);
        await System.IO.File.WriteAllBytesAsync(filePath, document.content);
        await Launcher.OpenAsync(new OpenFileRequest
        {
            File = new ReadOnlyFile(filePath)
        });
    }
    catch (Exception ex)
    {
        await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
    }
    finally
    {
        UserDialogs.Instance.HideLoading();
    }
}

private async void OnAddFileClicked(object sender, EventArgs e)
{
    try
    {
        UserDialogs.Instance.ShowLoading("Loading...");
        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Please select a file",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.Android, new[] { "image/*", "application/pdf" } }
                    })
        });

        if (result != null)
        {
            string filePath = result.FullPath;
            string fileName = result.FileName;
            var documentTypes = await Document.GetDocumentTypesAsync();

            if (documentTypes == null || !documentTypes.Any())
            {
                await DisplayAlert("Error", "Unable to retrieve document types.", "OK");
                return;
            }

            var popup = new CustomPopup(documentTypes, fileName);
            var resultPopup = await this.ShowPopupAsync(popup);

            if (resultPopup == null)
            {
                await DisplayAlert("Cancelation", "The selection process has been cancelled.", "OK");
                return;
            }

            var data = (dynamic)resultPopup;
            var newFileName = data.FileName;
            var memo = data.Memo;
            var description = data.Description;
            var selectedTypeId = data.TypeId;

            bool isSaved = await (BindingContext as FileSelectionViewModel).SaveDocumentAsync(filePath, newFileName, memo, description, selectedTypeId);

            if (isSaved)
            {
                await DisplayAlert("Success", $"File added: {newFileName}", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Failed to save the document to the database.", "OK");
            }
        }
    }
    catch (Exception ex)
    {
        await DisplayAlert("Error", $"An error has occurred: {ex.Message}", "OK");
    }
    finally
    {
        UserDialogs.Instance.HideLoading();
    }
}
    }
}

 
 */