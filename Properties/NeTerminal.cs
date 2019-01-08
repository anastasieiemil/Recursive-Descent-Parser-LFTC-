using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoTema
{
    class NeTerminal:AElementAlfabet
    {
       
        public NeTerminal(string numeNeTerminal)
            :base(numeNeTerminal) {}
        public NeTerminal()
        {
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            NeTerminal objPart = obj as NeTerminal;
            if (objPart._nume == _nume)
                return true;

            return false;
        }
        public void Set(string nume)
        {
            _nume = nume;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
