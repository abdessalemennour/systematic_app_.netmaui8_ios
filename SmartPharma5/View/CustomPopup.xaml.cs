using CommunityToolkit.Maui.Views;
using SmartPharma5.ViewModel;

namespace SmartPharma5.View
{
    public partial class CustomPopup : Popup
    {
        public CustomPopup(Dictionary<int, string> documentTypes, string fileName)
        {
            InitializeComponent();

            // Charger les types de documents dans le Picker
            //TypePicker.ItemsSource = documentTypes.Values.ToList();
            TypeComboBox.ItemsSource = documentTypes.Values.ToList();
            DocumentTypes = documentTypes;
            // Afficher le nom du fichier dans le champ FileNameEntry
            FileNameEntry.Text = Path.GetFileNameWithoutExtension(fileName);
            // Optionnellement, sélectionne le premier type par défaut
            // TypePicker.SelectedIndex = 0;
            TypeComboBox.SelectedItem = documentTypes.Values.FirstOrDefault(); // Sélectionner le premier élément
        }

        private Dictionary<int, string> DocumentTypes { get; }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            // Récupérer les valeurs des champs
            var fileName = FileNameEntry.Text;
            var memo = MemoEntry.Text;
            var description = DescriptionEntry.Text;
            // var selectedType = TypePicker.SelectedItem?.ToString();
            var selectedType = TypeComboBox.SelectedItem?.ToString();


            /*  if (string.IsNullOrWhiteSpace(selectedType))
              {
                  throw new InvalidOperationException("Please select a document type.");
              }
              var selectedTypeId = DocumentTypes.FirstOrDefault(x => x.Value == selectedType).Key;*/

            var selectedTypeId = DocumentTypes.Any() ? DocumentTypes.FirstOrDefault(x => x.Value == selectedType).Key : (int?)null; 
            

            Close(new { FileName = fileName, Memo = memo, Description = description, TypeId = selectedTypeId });
        }
        private async void TypePicker_Focused(object sender, FocusEventArgs e)
        {
            await (BindingContext as DocumentViewModel)?.LoadDocumentTypesAsync();

            if ((BindingContext as DocumentViewModel)?.DocumentTypes.Count == 0)
            {
                (BindingContext as DocumentViewModel).SelectedDocumentType = null;
            }
        }


        private void OnCancelClicked(object sender, EventArgs e)
        {
            Close(null);
        }
    }
}