using MvvmHelpers;
using SmartPharma5.Model;

namespace SmartPharma5.ModelView
{
    public class ShowTestMV : BaseViewModel
    {
        private List<humain> humais;
        public List<humain> Humains { get => humais; set => SetProperty(ref humais, value); }
        public ShowTestMV()
        {
            humais = new List<humain>();
            Humains.Add(new humain("A", "B"));
            Humains.Add(new humain("A", "B"));


        }
    }
}
