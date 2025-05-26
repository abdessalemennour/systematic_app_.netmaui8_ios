//using Acr.UserDialogs;
using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
using SmartPharma5.Model;

namespace SmartPharma5.ViewModel
{
    public class PartnerTempMV : BaseViewModel
    {

        private List<PartnerTemp> listPartnerTemp;
        public List<PartnerTemp> ListPartnerTemp { get => listPartnerTemp; set => SetProperty(ref listPartnerTemp, value); }

        public AsyncCommand HistoriqueCommand { get; set; }
        public AsyncCommand RefusedHistoriqueCommand { get; set; }
        public AsyncCommand AcceptedHistoriqueCommand { get; set; }

        public AsyncCommand CurrentCommand { get; set; }

        public AsyncCommand RefreshCommand { get; set; }

        
        private bool btnCurrentNotif;
        public bool BtnCurrentNotif { get => btnCurrentNotif; set => SetProperty(ref btnCurrentNotif, value); }


        private bool btnHitstoricNotif;
        public bool BtnHitstoricNotif { get => btnHitstoricNotif; set => SetProperty(ref btnHitstoricNotif, value); }

        public AsyncCommand ShowAttributes { get; set; }

        public PartnerTempMV()
        {
            RefreshCommand = new AsyncCommand(Refresh);
            HistoriqueCommand = new AsyncCommand(history);
            CurrentCommand = new AsyncCommand(current);
            ShowAttributes = new AsyncCommand(showAttributes);

            try
            {
                BtnHitstoricNotif = true;
                btnCurrentNotif = false;


                ListPartnerTemp = new List<PartnerTemp>(PartnerTemp.GetAllPartnerTemp().Result);

            }
            catch (Exception ex)
            {

            }
        }
        public PartnerTempMV(string name)
        {
            RefreshCommand = new AsyncCommand(MyRefresh);
            HistoriqueCommand = new AsyncCommand(Myhistory);
            RefusedHistoriqueCommand = new AsyncCommand(MyRefusedhistory);
            AcceptedHistoriqueCommand = new AsyncCommand(MyAcceptedhistory);
            CurrentCommand = new AsyncCommand(Mycurrent);
            ShowAttributes = new AsyncCommand(showAttributes);

            try
            {
                BtnHitstoricNotif = true;
                btnCurrentNotif = false;


                ListPartnerTemp = new List<PartnerTemp>(PartnerTemp.GetMyPartnerTemp().Result);

            }
            catch (Exception ex)
            {

            }
        }
        public async Task MyRefresh()
        {
            ListPartnerTemp = new List<PartnerTemp>();
            try
            {
                BtnHitstoricNotif = true;
                btnCurrentNotif = false;


                ListPartnerTemp = new List<PartnerTemp>(PartnerTemp.GetMyPartnerTemp().Result);

            }
            catch (Exception ex)
            {

            }
            IsBusy = false;
        }
        public async Task Refresh()
        {
            ListPartnerTemp = new List<PartnerTemp>();
            try
            {
                BtnHitstoricNotif = true;
                btnCurrentNotif = false;


                ListPartnerTemp = new List<PartnerTemp>(PartnerTemp.GetAllPartnerTemp().Result);

            }
            catch(Exception ex)
            {

            }
            IsBusy= false;
        }
        private async Task showAttributes()
        {

            try
            {
                //UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
                // await Task.Delay(500);
                //await App.Current.MainPage.Navigation.PushAsync(new  );
                // UserDialogs.Instance.HideLoading();

            }
            catch (Exception ex)
            {

            }

        }
        

        public async Task MyRefusedhistory()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            BtnCurrentNotif = false;
            BtnHitstoricNotif = true;
            ListPartnerTemp = PartnerTemp.GetMyPartnerHistoryRefusedTemp().Result;
            UserDialogs.Instance.HideLoading();
        }
        public async Task MyAcceptedhistory()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            BtnCurrentNotif = false;
            BtnHitstoricNotif = true;
            ListPartnerTemp = PartnerTemp.GetMyPartnerHistoryAcceptedTemp().Result;
            UserDialogs.Instance.HideLoading();
        }
        public async Task current()
        {
           UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
             await Task.Delay(500);
            BtnCurrentNotif = false;
            BtnHitstoricNotif = true;
            ListPartnerTemp = PartnerTemp.GetAllPartnerTemp().Result;
             UserDialogs.Instance.HideLoading();
        }
        public async Task history()
        {
             UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
             await Task.Delay(500);
            BtnCurrentNotif = true;
            BtnHitstoricNotif = false;
            ListPartnerTemp = PartnerTemp.GetAllPartnerHistoryTemp().Result;
            UserDialogs.Instance.HideLoading();

        }

        public async Task Mycurrent()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            BtnCurrentNotif = false;
            BtnHitstoricNotif = true;
            ListPartnerTemp = PartnerTemp.GetMyPartnerTemp().Result;
            UserDialogs.Instance.HideLoading();
        }
        public async Task Myhistory()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            BtnCurrentNotif = true;
            BtnHitstoricNotif = false;
            ListPartnerTemp = PartnerTemp.GetMyPartnerHistoryTemp().Result;
            UserDialogs.Instance.HideLoading();

        }



    }
}
