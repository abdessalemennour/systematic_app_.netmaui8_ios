using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.Model
{
    public class QueryModel
    {
        public string Name { get; set; }
        public string Contenu { get; set; }

        public QueryModel(string name, string contenu)
        {
            Name = name;
            Contenu = contenu;
        }
    }
}
