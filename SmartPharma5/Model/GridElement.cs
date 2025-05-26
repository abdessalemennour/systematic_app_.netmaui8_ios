using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.Model
{
    public class GridElement
    {
        

        public string name { get; set; }

        public string type { get; set; }

        public GridElement(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
    }
}
