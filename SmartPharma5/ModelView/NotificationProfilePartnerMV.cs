
//using Acr.UserDialogs;
using Acr.UserDialogs;
using MvvmHelpers;
using MvvmHelpers.Commands;
using SmartPharma5.Model;
using Command = MvvmHelpers.Commands.Command;

namespace SmartPharma5.ViewModel
{
    public class NotificationProfilePartnerMV : BaseViewModel
    {
        private List<NotificationProfilePartner> listNotification;
        public List<NotificationProfilePartner> ListNotification { get => listNotification; set => SetProperty(ref listNotification, value); }

        private List<ShearchItemStatProfile> listShearshState;
        public List<ShearchItemStatProfile> ListShearshState { get => listShearshState; set => SetProperty(ref listShearshState, value); }


        private ShearchItemStatProfile selectItem;
        public ShearchItemStatProfile SelectItem { get => selectItem; set => SetProperty(ref selectItem, value); }


        public Command changeSearsh { get; set; }

        public AsyncCommand HistoriqueCommand { get; set; }
        
        public AsyncCommand HistoriqueCommandMy { get; set; }
        public AsyncCommand AccepteHistoriqueCommandMy { get; set; }
        public AsyncCommand RefuseHistoriqueCommandMy { get; set; }
        public AsyncCommand CurrentCommandMy { get; set; }

        public AsyncCommand CurrentCommand { get; set; }

        private bool btnCurrentNotif;
        public bool BtnCurrentNotif { get => btnCurrentNotif; set => SetProperty(ref btnCurrentNotif, value); }


        private bool btnHitstoricNotif;
        public bool BtnHitstoricNotif { get => btnHitstoricNotif; set => SetProperty(ref btnHitstoricNotif, value); }




        public NotificationProfilePartnerMV()
        {
            BtnHitstoricNotif = true;
            btnCurrentNotif = false;

            HistoriqueCommand = new AsyncCommand(history);
            CurrentCommand = new AsyncCommand(current);

            ListNotification = NotificationProfilePartner.getAllTempValues();
            listShearshState = ShearchItemStatProfile.getAllSheachItem();


            changeSearsh = new Command(async () =>
            {


                /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
                Avant :
                                ListNotification.Clear();



                                if (selectItem.Name.ToLower() == "all" )
                Après :
                                ListNotification.Clear();



                                if (selectItem.Name.ToLower() == "all" )
                */
                ListNotification.Clear();



                if (selectItem.Name.ToLower() == "all")
                {
                    ListNotification = NotificationProfilePartner.getAllTempValues();

                }
                else if (selectItem.Name.ToLower() == "current")
                {
                    ListNotification = NotificationProfilePartner.getAllTempValues().Where(x => x.state == 1).ToList();

                }
                else if (selectItem.Name.ToLower() == "accepted")
                {
                    ListNotification = NotificationProfilePartner.getAllTempValues().Where(x => x.state == 2).ToList();

                }
                else
                {
                    ListNotification = NotificationProfilePartner.getAllTempValues().Where(x => x.state == 3).ToList();
                }

            });
        }
        public NotificationProfilePartnerMV(string name)
        {
            BtnHitstoricNotif = true;
            btnCurrentNotif = false;

            HistoriqueCommandMy = new AsyncCommand(historyMy);
            AccepteHistoriqueCommandMy = new AsyncCommand(historyAccepteMy);
            RefuseHistoriqueCommandMy = new AsyncCommand(historyRefuseMy);
            CurrentCommandMy = new AsyncCommand(currentMy);

            ListNotification = NotificationProfilePartner.getMyTempValues();
            listShearshState = ShearchItemStatProfile.getAllSheachItem();


            changeSearsh = new Command(async () =>
            {


                /* Modification non fusionnée à partir du projet 'SmartPharma5 (net7.0-ios)'
                Avant :
                                ListNotification.Clear();



                                if (selectItem.Name.ToLower() == "all" )
                Après :
                                ListNotification.Clear();



                                if (selectItem.Name.ToLower() == "all" )
                */
                ListNotification.Clear();



                if (selectItem.Name.ToLower() == "all")
                {
                    ListNotification = NotificationProfilePartner.getMyTempValues();

                }
                else if (selectItem.Name.ToLower() == "current")
                {
                    ListNotification = NotificationProfilePartner.getMyTempValues().Where(x => x.state == 1).ToList();

                }
                else if (selectItem.Name.ToLower() == "accepted")
                {
                    ListNotification = NotificationProfilePartner.getMyTempValues().Where(x => x.state == 2).ToList();

                }
                else
                {
                    ListNotification = NotificationProfilePartner.getAllTempValues().Where(x => x.state == 3).ToList();
                }

            });
        }
        public async Task current()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
             await Task.Delay(500);
            BtnCurrentNotif = false;
            BtnHitstoricNotif = true;
            ListNotification = NotificationProfilePartner.getAllTempValues();
            UserDialogs.Instance.HideLoading();
        }
        public async Task history()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
             await Task.Delay(500);
            BtnCurrentNotif = true;
            BtnHitstoricNotif = false;
            ListNotification = NotificationProfilePartner.getAllHistoryTempValues();
            UserDialogs.Instance.HideLoading();

        }

        public async Task currentMy()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            BtnCurrentNotif = false;
            BtnHitstoricNotif = true;
            ListNotification = NotificationProfilePartner.getMyTempValues();
            UserDialogs.Instance.HideLoading();
        }
        public async Task historyMy()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            BtnCurrentNotif = true;
            BtnHitstoricNotif = false;
            ListNotification = NotificationProfilePartner.getMyHistoryTempValues();
            UserDialogs.Instance.HideLoading();

        }
        public async Task historyRefuseMy()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            BtnCurrentNotif = true;
            BtnHitstoricNotif = false;
            ListNotification = NotificationProfilePartner.getMyRefuseHistoryTempValues();
            UserDialogs.Instance.HideLoading();

        }
        public async Task historyAccepteMy()
        {
            UserDialogs.Instance.ShowLoading("Loading Pleae wait ...");
            await Task.Delay(500);
            BtnCurrentNotif = true;
            BtnHitstoricNotif = false;
            ListNotification = NotificationProfilePartner.getMyAccepteHistoryTempValues();
            UserDialogs.Instance.HideLoading();

        }

    }
}
