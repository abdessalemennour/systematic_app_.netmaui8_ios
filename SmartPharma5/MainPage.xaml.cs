namespace SmartPharma5
{
    public partial class MainPage : Shell
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void OnOpenWebButtonClicked(System.Object sender, System.EventArgs e)
        {
            await Browser.OpenAsync("https://www.devexpress.com/maui/");
        }

        private void MenuItem_Clicked(object sender, EventArgs e)
        {

        }
    }
}