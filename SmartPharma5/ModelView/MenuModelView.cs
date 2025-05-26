using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.ModelView
{
    public  class MenuModelView : BaseViewModel
    {
        private bool menuVisisble;
        public bool MenuVisisble { get => menuVisisble; set => SetProperty(ref menuVisisble, value); }

        public MenuModelView()
        {
            MenuVisisble = false;

        }
    }
}
