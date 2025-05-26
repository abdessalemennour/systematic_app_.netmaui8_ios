using SmartPharma5.Model;
using SmartPharma5.View;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
//using Xamarin.Essentials;
//using Xamarin.Forms;
//using static SmartPharma.Model.Payment;
//using static SmartPharma.Model.Transfer;

namespace SmartPharma5.ViewModel
{
    internal class TransferVewModel:BaseViewModel
    {
        public int iduser;
        private CashDesk cash_desk_out;
        public CashDesk Cash_desk_out { get => cash_desk_out; set => SetProperty(ref cash_desk_out, value); }
        private CashDesk cash_desk_in;
        public CashDesk Cash_desk_in { get => cash_desk_in; set => SetProperty(ref cash_desk_in, value); }
        private Transfer transfer;
        public Transfer Transfer { get => transfer; set => SetProperty(ref transfer, value); }
        private decimal amount;
        public decimal Amount { get => amount; set => SetProperty(ref amount, value); }
        private bool actpopup = false;
        public bool ActPopup { get => actpopup; set => SetProperty(ref actpopup, value); }
        private string successpopupmessage;
        public string SuccessPopupMessage { get => successpopupmessage; set => SetProperty(ref successpopupmessage, value); }

        private bool successpopup = false;
        private bool fieldpopup = false;
        public bool FieldPopup { get => fieldpopup; set => SetProperty(ref fieldpopup, value); }
        public bool SuccessPopup { get => successpopup; set => SetProperty(ref successpopup, value); }
        private bool verifPopup = false;
        public bool VerifPopup { get => verifPopup; set => SetProperty(ref verifPopup, value); }
        public AsyncCommand ExitCommand { get; set; }
        public AsyncCommand LogoutCommand { get; set; }

        public AsyncCommand AmountChangeCommand { get; }
        public AsyncCommand<Transfer.Payment> SelectedChangedCommand { get; }
        private async Task SelectedChanged(Transfer.Payment Payment)
        {
            if (Model.Transfer.Payment.IsPaymentAvailable((Payment.Id)) || Payment.Is_checked)
            {
                foreach (Transfer.Payment p in PaymentList)
                {
                    this.Transfer.PaymentList.Clear();
                    if (Payment.Id == p.Id)
                    {
                        p.Is_checked = !p.Is_checked;
                        foreach (Transfer.Payment payment in PaymentList)
                            if (payment.Is_checked == true)
                            {
                                this.Transfer.PaymentList.Add(new Transfer.Payment_transfer(0, payment.Id, 0, payment.Montant, payment.code, payment.Partner));
                            }
                        if (p.Is_checked)
                        {
                            SuccessPopupMessage = "Item has been added with success";
                            SuccessPopup = true;
                            await Task.Delay(1000);
                            SuccessPopup = false;
                        }
                        else
                        {
                            SuccessPopupMessage = "Item has been deleted with success";
                            FieldPopup = true;
                            await Task.Delay(1000);
                            FieldPopup = false;
                        }

                        break;
                    }
                }
                Transfer.SetTransferAmount();
            }
            else
               await App.Current.MainPage.DisplayAlert("Warning", "Payment already affected! ", "Ok");
        }
        #region State
        private bool isVisibleStateName = false;
        public bool IsVisibleStateName { get => isVisibleStateName; set => SetProperty(ref isVisibleStateName, value); }
        private bool isVisibleState = false;
        public bool IsVisibleState { get => isVisibleState; set => SetProperty(ref isVisibleState, value); }
        private bool isVisibleAcceptedState = false;
        public bool IsVisibleAcceptedState { get => isVisibleAcceptedState; set => SetProperty(ref isVisibleAcceptedState, value); }
        private bool isVisibleRejectedState = false;
        public bool IsVisibleRejectedState { get => isVisibleRejectedState; set => SetProperty(ref isVisibleRejectedState, value); }
        private bool isSaveVisible = false;
        public bool IsSaveVisible { get => isSaveVisible; set => SetProperty(ref isSaveVisible, value); }
        public AsyncCommand SetAcceptedCommand { get; }
        private async Task SetAccepted()
        {
            Transfer.SetState(2);
            await App.Current.MainPage.DisplayAlert("Success", "Accepted State Affected", "Ok");
            await App.Current.MainPage.Navigation.PopAsync();
            await App.Current.MainPage.Navigation.PushAsync(new View.MyTransferListView());
        }
        public AsyncCommand SetRejectedCommand { get; }
        private async Task SetRejected()
        {
            Transfer.SetState(3);
            await App.Current.MainPage.DisplayAlert("Success", "Rejected State Affected", "Ok");
            await App.Current.MainPage.Navigation.PopAsync();
            await App.Current.MainPage.Navigation.PushAsync(new View.MyTransferListView());
        } 
        #endregion
        #region SaveCommand
        public AsyncCommand SaveCommand { get; }
        private async Task Save()
        {
            SuccessPopupMessage = "data verification...";
            VerifPopup = true;
            await Task.Delay(1000);
            VerifPopup = false;
            if (Transfer.Payment.IsListPaymentAvailable(Transfer.PaymentList))
            {
                if (Transfer.amount != 0)
                {
                    var Connectivity = DbConnection.CheckConnectivity();
                    if (Connectivity)
                    {
                        var DbConnectivity = DbConnection.ConnectionIsTrue();
                        if (DbConnectivity)
                        {
                            try
                            {
                                if (SetInformation())
                                {
                                    Transfer.Insert();
                                    //await App.Current.MainPage.DisplayAlert("Success", "Added", "Ok");
                                    //SavingPopup = false;
                                    //SuccessPopupMessage = "Your opportunty has been successfully sent!";
                                    //SuccessPopup = true;
                                    //await Task.Delay(1000);
                                    //SuccessPopup = false;
                                    await App.Current.MainPage.DisplayAlert("Success", "Transfer Saved", "Ok");
                                    await App.Current.MainPage.Navigation.PopToRootAsync();
                                    await App.Current.MainPage.Navigation.PushAsync(new View.MyTransferListView());
                                }
                                else
                                    await App.Current.MainPage.DisplayAlert("Failed", "Set Cash Desk In", "Ok");
                            }
                            catch (Exception ex)
                            {
                                await App.Current.MainPage.DisplayAlert("Warning", ex.Message, "Ok");
                            }
                        }
                        else
                        {
                            //SavingPopup = false;
                            //App.Current.MainPage.DisplayAlert("Warning", "There is a problem in connecting to the server. \n Please contact our support for further assistance.", "Ok");
                            //Message = "There is a problem in connecting to the server. \n Please contact our support for further assistance.";
                            await App.Current.MainPage.DisplayAlert("Warning", "There is a problem in connecting to the server. \n Please contact our support for further assistance.", "Ok");
                            //FieldPopup = true;
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Warning", "No network connectivity.", "Ok");
                        //Message = "No network connectivity.";
                        //SavingPopup = false;
                        //await Task.Delay(1000);
                        //FieldPopup = true;
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Add Transfer before validate", "Ok");
                    //SavingPopup = false;
                }
            }
            else
                await App.Current.MainPage.DisplayAlert("Warning", "You have payment used! ", "Ok");
        } 
        #endregion
        #region PaymentList
        private ObservableRangeCollection<Transfer.Payment> paymentList;
        public ObservableRangeCollection<Transfer.Payment> PaymentList { get => paymentList; set => SetProperty(ref paymentList, value); } 
        #endregion
        #region Cash Desk
        private ObservableRangeCollection<CashDesk> cash_deskList;
        public ObservableRangeCollection<CashDesk> Cash_deskList { get => cash_deskList; set => SetProperty(ref cash_deskList, value); }
        private CashDesk cash_deskListselecteditem;
        public CashDesk Cash_deskListselecteditem { get => cash_deskListselecteditem; set => SetProperty(ref cash_deskListselecteditem, value); }
        #endregion
        #region Visibility
        private bool isVisibleAmount = false;
        public bool IsVisibleAmount { get => isVisibleAmount; set => SetProperty(ref isVisibleAmount, value); }
        private bool isVisiblePayment = false;
        public bool IsVisiblePayment { get => isVisiblePayment; set => SetProperty(ref isVisiblePayment, value); }
        #endregion
        #region Constructeur
        public TransferVewModel()
        {
        }
        public TransferVewModel(Transfer tr)
        {
            ActPopup = false;
            iduser = Preferences.Get("iduser", 0);
            Transfer = tr;
            PaymentList = new ObservableRangeCollection<Transfer.Payment>();
            Cash_deskList = new ObservableRangeCollection<CashDesk>();
            AmountChangeCommand = new AsyncCommand(AmountChange);
            SelectedChangedCommand = new AsyncCommand<Transfer.Payment>(SelectedChanged);
            SaveCommand = new AsyncCommand(Save);
            SetAcceptedCommand = new AsyncCommand(SetAccepted);
            SetRejectedCommand = new AsyncCommand(SetRejected);
            Task.Run(() => LoadListonUpdate());
            ExitCommand = new AsyncCommand(Exit);
            LogoutCommand = new AsyncCommand(Logout);
        }
        public TransferVewModel(CashDesk cd)
        {
            ActPopup = false;
            iduser = Preferences.Get("iduser", 0);
            Transfer = new Transfer();
            Cash_desk_out = cd;
            Title = "Transfer[ " + cd.Name + " ]";
            ControlVisibility();
            Transfer.cash_desk_out = cd.Id;
            PaymentList = new ObservableRangeCollection<Transfer.Payment>();
            Cash_deskList = new ObservableRangeCollection<CashDesk>();
            AmountChangeCommand = new AsyncCommand(AmountChange);
            SelectedChangedCommand = new AsyncCommand<Transfer.Payment>(SelectedChanged);
            SaveCommand = new AsyncCommand(Save);
            SetAcceptedCommand = new AsyncCommand(SetAccepted);
            SetRejectedCommand = new AsyncCommand(SetRejected);
            ExitCommand = new AsyncCommand(Exit);
            LogoutCommand = new AsyncCommand(Logout);
            LoadList();
        } 
        #endregion
        public bool SetInformation()
        {
            if (Cash_deskListselecteditem != null)
            {
                Transfer.cash_desk_in = Cash_deskListselecteditem.Id;
                return true;
            }
            else 
                return false;
        }
        private async Task AmountChange()
        {
            Transfer.amount=Amount;
        }
        public void ControlVisibility()
        {
            if (Cash_desk_out.Cash) 
            {
                IsVisibleAmount = true;
                IsVisiblePayment = false;
            }
            else
            {
                IsVisibleAmount = false;
                IsVisiblePayment = true;
            }
            if (Transfer.Id > 0)
                IsSaveVisible = false;
            else
                IsSaveVisible = true;
        }
        public void ControlState()
        {
            if(Transfer.Id>0)
            {
                if (Transfer.IsUserStateEnabled(iduser))
                {
                    if(Transfer.GetState()==1)
                    {
                        IsVisibleState = IsVisibleRejectedState = IsVisibleAcceptedState = true;
                        IsVisibleStateName = false;
                    }
                    else
                    {
                        IsVisibleState = false;
                        IsVisibleStateName = true;
                    }
                        
                }
                else
                {
                    IsVisibleState = false;
                    IsVisibleStateName = true;
                }
            }
            else
            {
                IsVisibleState = false;
                IsVisibleStateName = false;
            }
        }
        public void SetTitle()
        {
            Title = "Transfer[ " + Cash_desk_out.Name + " ]";
        }
        private  void LoadList()
        {
            Cash_deskList.Clear();
            Cash_deskList.AddRange(Transfer.getCash_deskListByPayment_method(Cash_desk_out));
            if (!Cash_desk_out.Cash)
            {
                PaymentList.Clear();
                PaymentList.AddRange(Transfer.GetPaymentByCash_desk(Cash_desk_out.Id));
            }
            ControlState();
        }
        private async void LoadListonUpdate()
        {
            ActPopup = true;
            Cash_deskList.Clear();
            var E = Task.Run(() => Transfer.getCash_deskList());
            Cash_deskList = new ObservableRangeCollection<CashDesk>(await E);

            var X = Task.Run(() => CashDesk.GetCash_deskById(Transfer.cash_desk_in));
            Cash_desk_in = await X;
            Cash_deskListselecteditem = Cash_desk_in;

            var Y = Task.Run(() => CashDesk.GetCash_deskById(Transfer.cash_desk_out));
            Cash_desk_out = await Y;
            SetTitle();
            ControlVisibility();
            ControlState();
            ActPopup = false;
        }

        private async Task Logout()
        {
            var r = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to lougout!", "Yes", "No");
            if (r)
            {
                Preferences.Set("idagent", Convert.ToUInt32(null));
                await App.Current.MainPage.Navigation.PushAsync(new LoginView());
            }
        }

        private async Task Exit()
        {
            var r = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to exit!", "Yes", "No");
            if (r)
            {
                await App.Current.MainPage.Navigation.PopToRootAsync();

            }
        }
    }
}
