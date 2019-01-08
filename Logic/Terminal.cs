using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoTema
{
    public class Terminal:AElementAlfabet
    {
        public Terminal(string numeTerminal)
            :base(numeTerminal){}
        public Terminal()
        {
        }
        


        public void Set(string nume)
        {
            _nume = nume;
        }
        public override bool Equals(object obj)
        {

            Terminal objPart = obj as Terminal;
            if (objPart._nume == _nume)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
