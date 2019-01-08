using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoTema
{
    class RegulaDeProductie
    {
        public NeTerminal _partea_stanga{get; set;}
        public List<AElementAlfabet> _partea_dreapta;
        public bool Taken { get ; set ; }


        public RegulaDeProductie(string parte_stanga)
        {
            _partea_stanga = new NeTerminal(parte_stanga);
            _partea_dreapta = new List<AElementAlfabet>();
            Taken = false;
        }

        public void AdaugaRegula(AElementAlfabet element)
        {
            _partea_dreapta.Add(element);
        }
        public bool EsteRecursiva()
        {
            //foreach (AElementAlfabet element in _partea_dreapta)
            //{
            //    if (element._nume == _partea_stanga._nume)
            //        return true;
            //}
            if (_partea_dreapta[0]._nume == _partea_stanga._nume)
                return true;

            return false;
        }

        public List<AElementAlfabet>Contine(List<AElementAlfabet> lista_elemente)
        {

            List<AElementAlfabet> return_list = new List<AElementAlfabet>();
            int i = 0;

            foreach(AElementAlfabet element in lista_elemente)
            {
                if (element._nume == _partea_dreapta[i]._nume)
                {
                    return_list.Add(element);
                }
                else
                    return return_list;

                i++;
                if (i == _partea_dreapta.Count)
                    break;
            }

            return return_list;

        }

        public bool Contine(AElementAlfabet element)
        {
            var elemente=(from i in _partea_dreapta
                         where i._nume==element._nume
                         select i).ToList();

            if (elemente.Count!=0)
                return true;
            return false;
        }

        public override bool Equals(object obj)
        {

            RegulaDeProductie regula = obj as RegulaDeProductie;

            int nr_elemente_dreapta = (Contine(regula._partea_dreapta)).Count;

            if (nr_elemente_dreapta == _partea_dreapta.Count
                && _partea_stanga._nume == regula._partea_stanga._nume)
                return true;

            return false;
        }



    }
}
