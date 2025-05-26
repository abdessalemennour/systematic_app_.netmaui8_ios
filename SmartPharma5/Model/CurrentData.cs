using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.Model
{
    public class CurrentData
    {
        public static string CurrentNoteModule { get; set; }
        public static string CurrentActivityModule { get; set; }
        public static string CurrentFormModule { get; set; }

        // public static string CurrentDocumentModule { get; set; }

        public static int CurrentModuleId { get; set; }

        public CurrentData(string currentmodule)
        {
            CurrentNoteModule = currentmodule;
        }

    }
}
