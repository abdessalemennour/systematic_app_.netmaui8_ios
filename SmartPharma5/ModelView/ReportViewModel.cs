using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Acr.UserDialogs;
using MvvmHelpers;
using SmartPharma5.Model;
using Microsoft.Maui.Storage;
using static SmartPharma5.Model.Activity;

namespace SmartPharma5.ModelView
{
   public class ReportViewModel : INotifyPropertyChanged
    {

        private byte[] _pdfBytes;
        private string _statusMessage;
        private bool _isStatusVisible;
        private string _binaryContent;
        private bool _showBinaryContent; private string _editableBinaryContent;


        public string EditableBinaryContent
        {
            get => _editableBinaryContent;
            set => SetProperty(ref _editableBinaryContent, value);
        }
        public string BinaryContent
        {
            get => _binaryContent;
            set => SetProperty(ref _binaryContent, value);
        }

        public bool ShowBinaryContent
        {
            get => _showBinaryContent;
            set => SetProperty(ref _showBinaryContent, value);
        }

        protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;

            backingField = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsStatusVisible
        {
            get => _isStatusVisible;
            set => SetProperty(ref _isStatusVisible, value);
        }


        public ICommand ImportPdfCommand { get; }
        public ICommand GeneratePdfCommand { get; }

        public ReportViewModel()
        {
            ImportPdfCommand = new Command(async () => await ImportPdfAsync());
            GeneratePdfCommand = new Command(GeneratePdf);
        }

            private async Task ImportPdfAsync()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Sélectionnez un fichier PDF",
                    FileTypes = FilePickerFileType.Pdf
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    _pdfBytes = memoryStream.ToArray();

                    StatusMessage = "PDF importé avec succès!";
                    IsStatusVisible = true;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur: {ex.Message}";
                IsStatusVisible = true;
            }
        }

        private void GeneratePdf()
        {
            if (_pdfBytes == null || _pdfBytes.Length == 0)
            {
                StatusMessage = "Aucun PDF importé!";
                IsStatusVisible = true;
                ShowBinaryContent = false;
                return;
            }

            try
            {
                // Convertir les bytes en représentation hexadécimale
                EditableBinaryContent = BitConverter.ToString(_pdfBytes).Replace("-", " ");
                ShowBinaryContent = true;
                StatusMessage = "PDF converti en binaire avec succès!";
                IsStatusVisible = true;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erreur de conversion: {ex.Message}";
                IsStatusVisible = true;
                ShowBinaryContent = false;
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
