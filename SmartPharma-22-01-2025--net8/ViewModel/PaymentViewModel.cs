using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmartPharma.ViewModel
{
    public class PaymentViewModel : BaseViewModel
    {
        private async Task Save()
        {
            // Vérifier la devise de chaque pièce sélectionnée
            foreach (var piece in UnpaiedList)
            {
                if (piece.Is_checked && piece.IdCurrency != Currency)
                {
                    await App.Current.MainPage.DisplayAlert("Failed", $"Coin currency {piece.code} does not match the selected currency.", "Ok");
                    return; // Arrête la sauvegarde si la devise ne correspond pas
                }

                // Vérifier la société de chaque pièce sélectionnée seulement si IsVisibleSocity est true
                if (IsVisibleSocity && piece.Is_checked && piece.IdSociety != SocietySelectedItem.Id)
                {
                    await App.Current.MainPage.DisplayAlert("Failed", $"The society of the selected piece does not match the selected society.", "Ok");
                    return; // Arrête la sauvegarde si la société ne correspond pas
                }
            }

            // ... existing code ...
        }
    }
} 