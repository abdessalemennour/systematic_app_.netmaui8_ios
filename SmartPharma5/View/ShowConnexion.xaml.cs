using SmartPharma5.Model;

namespace SmartPharma5.View;

public partial class ShowConnexion : ContentPage
{
	public ShowConnexion()
	{
		InitializeComponent();
		var DB_base = DbConnection.Database;
		var DB_adress = DbConnection.Address;
		var DB_password = DbConnection.Password;
	
	}
}