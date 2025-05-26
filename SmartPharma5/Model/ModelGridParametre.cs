using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.Model
{
    public class ModelGridParametre :BaseViewModel
    {
        public string Name { get; set; }

        public string type { get; set; }

        public bool? BoolValue { get; set; } = null;
        public Decimal? DecimalValue { get; set; } = null;
        public string StringValue { get; set; } = null;
        public DateTime DateValue { get; set; } 
        public int? IntValue { get; set; } = null;



        public bool? IsBool { get; set; } = false;
        public bool? IsString { get; set; } = false;
        public bool? IsInt { get; set; } = false;
        public bool? IsDecimal { get; set; } = false;
        public bool? IsDate { get; set; } = false;




        public ModelGridParametre()
        {

        }

        public ModelGridParametre(string name, string type)
        {
            Name = name;
            this.type = type;

          
        }

    }
}
