using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.ModelView
{

    class FileSelectionViewModel
    {
    }
}

/*
 using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using MvvmHelpers;
using SmartPharma5.Model;

namespace SmartPharma5.ViewModel
{
    public class FileSelectionViewModel : BaseViewModel
    {
        public ObservableCollection<Document> Documents { get; set; } = new ObservableCollection<Document>();
        private int _opportunityId;

        public FileSelectionViewModel(int opportunityId)
        {
            _opportunityId = opportunityId;
            LoadDocumentsAsync();
        }

        private async Task LoadDocumentsAsync()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Loading...");
                var documents = await Document.GetDocumentsByOpportunityIdAsync(_opportunityId);

                if (documents != null && documents.Any())
                {
                    Documents.Clear();
                    foreach (var document in documents)
                    {
                        Documents.Add(document);
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Info", "No documents found for this opportunity.", "OK");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        public async Task<bool> SaveDocumentAsync(string filePath, string fileName, string memo, string description, int selectedTypeId)
        {
            var content = await System.IO.File.ReadAllBytesAsync(filePath);
            var temporaryDocument = new Document
            {
                name = fileName,
                extension = System.IO.Path.GetExtension(fileName),
                content = content,
                create_date = DateTime.Now,
                date = DateTime.Now,
                memo = memo,
                description = description,
                type_document = (uint)selectedTypeId,
                size = content.Length
            };

            try
            {
                bool isSaved = await Document.SaveToDatabase(temporaryDocument, _opportunityId);
                return isSaved;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

 */







/*
 using MvvmHelpers;
using MvvmHelpers.Commands;

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
using SmartPharma5.ViewModel;
using SmartPharma5.View;
using System.ComponentModel;

namespace SmartPharma5.ModelView
{

    public class FileSelectionViewModel : BaseViewModel
    {
        public ObservableCollection<Document> Documents { get; set; } = new ObservableCollection<Document>();

        private uint? _opportunityId;
        public uint? OpportunityId
        {
            get => _opportunityId;
            set
            {
                SetProperty(ref _opportunityId, value);
                LoadDocumentsAsync();
            }
        }

        private uint? _partnerId;
        public uint? PartnerId
        {
            get => _partnerId;
            set
            {
                SetProperty(ref _partnerId, value);
                LoadDocumentsAsync();
            }
        }

        public FileSelectionViewModel()
        {
            // Initialisation si nécessaire
        }

        public async Task LoadDocumentsAsync()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Loading...");
                await Task.Delay(200);

                if (OpportunityId.HasValue)
                {
                    var documents = await Document.GetDocumentsByOpportunityIdAsync((int)OpportunityId);
                    if (documents != null && documents.Any())
                    {
                        Documents.Clear();
                        foreach (var document in documents)
                        {
                            Documents.Add(document);
                        }
                    }
                    else
                    {
                      // await DisplayAlert("Info", "No documents found for this opportunity.", "OK");
                    }
                }
                else if (PartnerId.HasValue)
                {
                    var documents = await Document.GetDocumentsByPartnerIdAsync((int)PartnerId);
                    if (documents != null && documents.Any())
                    {
                        Documents.Clear();
                        foreach (var document in documents)
                        {
                            Documents.Add(document);
                        }
                    }
                    else
                    {
                       // await DisplayAlert("Info", "No documents found for this partner.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
              //  await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        // Ajouter ici les méthodes pour gérer l'ajout, l'ouverture et la suppression des fichiers, comme dans ton code original.
    }

}

 
 */