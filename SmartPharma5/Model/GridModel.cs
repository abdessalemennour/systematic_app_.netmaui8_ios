using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.Model
{
    public class GridModel
    {
        public string Name { get; set; }
        public string Contenu { get; set; }
        public string Query_Name { get; set; }

        public GridModel(string name, string contenu, string query_Name)
        {
            Name = name;
            Contenu = contenu;
            Query_Name = query_Name;
        }
    }
}
