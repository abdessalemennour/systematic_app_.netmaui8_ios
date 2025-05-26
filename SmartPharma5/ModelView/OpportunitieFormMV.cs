using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
using SmartPharma5.Model;
using SmartPharma5.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.ModelView
{
    public class OpportunitieFormMV : BaseViewModel
    {
        public AsyncCommand GoToContact { get; }
        public Partner partner;
        public int opp;
        public OpportunitieFormMV(Partner partner,int opp) {
            this.partner = partner;
            this.opp = opp;
            this.GoToContact = new AsyncCommand(fun);
        }

        public async Task fun()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                await Task.Delay(500);
                await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new ContactPartnerPage(this.partner, this.opp)));
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                await DbConnection.ErrorConnection();
            }

        }
    }
}
