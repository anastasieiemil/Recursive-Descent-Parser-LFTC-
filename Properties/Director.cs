using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoTema
{
    class Director
    {
        public List<Terminal> _elemente_directoare;
        public string _tip_director;
        public int _index;

        public Director(string tip)
        {
            _tip_director = tip;
            _elemente_directoare = new List<Terminal>();
        }
    }
}
