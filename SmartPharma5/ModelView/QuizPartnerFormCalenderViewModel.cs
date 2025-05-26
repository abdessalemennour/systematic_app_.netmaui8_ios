
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using SmartPharma5.Model;

namespace SmartPharma5.ViewModel
{
    public class QuizPartnerFormCalenderViewModel:BaseViewModel
    {
        public ObservableRangeCollection<Partner_Form.Collection> PartnerForms { get; set; } = new ObservableRangeCollection<Partner_Form.Collection>();
        public QuizPartnerFormCalenderViewModel() { }
        public QuizPartnerFormCalenderViewModel(ObservableRangeCollection<Partner_Form.Collection> partner_forms) 
        {
            PartnerForms = partner_forms;
        }
    }
}
