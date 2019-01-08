using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoTema
{
    abstract class  AElementAlfabet
    {
        public string _nume { get; set; }
        public bool Taken;


        public AElementAlfabet(string numeElement){
            _nume = numeElement;
            Taken = false;
        }
        public AElementAlfabet()
        {
            Taken = false;

        }

        public override bool Equals(object obj)
        {

            AElementAlfabet objPart = obj as AElementAlfabet;
            if (objPart._nume == _nume)
                return true;

            return false;
        }
    }
}
