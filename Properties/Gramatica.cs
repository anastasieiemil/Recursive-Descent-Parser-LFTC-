using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace DemoTema
{

    struct ElementMatrice
    {
        public AElementAlfabet _element_alfabet;
        public RegulaDeProductie _regula;
    }

    public class Gramatica
    {
        #region Membrii

        private NeTerminal _start;

        private List<NeTerminal> _neterminale;

        private List<Terminal> _terminale;

        private List<RegulaDeProductie> _reguli_de_productie;

        private List<Director> _directori;

        private int _numar_reguli;

        private ElementMatrice [,] _matrice;


        internal NeTerminal Start { get => _start; set => _start = value; }
        internal List<RegulaDeProductie> Reguli_de_productie { get => _reguli_de_productie; set => _reguli_de_productie = value; }
        internal List<Terminal> Terminale { get => _terminale; set => _terminale = value; }
        internal List<NeTerminal> Neterminale { get => _neterminale; set => _neterminale = value; }
        internal ElementMatrice[,] Matrice { get => _matrice; set => _matrice = value; }

        #endregion

        #region Metode Publice
        public void IncarcaGramatica(string fisier)
        {
            try
            {
                using (StreamReader stream = new StreamReader(fisier))
                {
                    Start = new NeTerminal(stream.ReadLine());
                    Element<NeTerminal>.Adauga_elemente_alfabet(ref _neterminale, stream.ReadLine(), ' ');      //separator
                    Element<Terminal>.Adauga_elemente_alfabet(ref _terminale, stream.ReadLine(), ' ');      //separator

                    _numar_reguli = Int32.Parse(stream.ReadLine());

                    Reguli_de_productie = new List<RegulaDeProductie>();


                    while (!stream.EndOfStream)
                    {

                        Adauga_Reguli(stream.ReadLine());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }

        public void VerificaConditii()
        {
            Conditie inceput_identic=new Conditie(Verifica_Inceput);
            Executa_Conditie(inceput_identic);

            Reset_Reguli_Productie();

            Conditie recursivitate_stanga = new Conditie(Recursivitate_Stanga);
            Executa_Conditie(recursivitate_stanga);
        }

        public void Print()
        {
            foreach(RegulaDeProductie regula in Reguli_de_productie)
            {
                Console.Write(regula._partea_stanga._nume);
                Console.Write(" => ");
                foreach (AElementAlfabet element in regula._partea_dreapta)
                    Console.Write(" {0}", element._nume);
                Console.WriteLine();

            }
        }
        #endregion


        #region Metode pentru a Verifica Conditiile pentru analizor descendent

        private void Executa_Conditie(Conditie conditie)
        {

            List<RegulaDeProductie> lista;
            for (int i = 0; i < Reguli_de_productie.Count; i++)
            {
                lista = Get_Reguli(Reguli_de_productie[i]._partea_stanga);

                if (lista.Count > 1)
                    conditie(lista);
            }
        }

        #region Inceput Identic
        private void Verifica_Inceput(List<RegulaDeProductie> lista)
        {
            List<AElementAlfabet> lista_elemente;
            List<AElementAlfabet> lista_temporara;

            List<RegulaDeProductie> reguli_cu_inceput_identic = new List<RegulaDeProductie>();

            for (int i = 0; i < lista.Count; i++)
            {
                lista_elemente = lista[i]._partea_dreapta;

                for (int j = i; j < lista.Count; j++)
                {
                    lista_temporara = lista[j].Contine(lista_elemente);
                    if (lista_temporara.Count != 0)
                    {
                        lista_elemente = lista_temporara;
                        reguli_cu_inceput_identic.Add(lista[j]);
                    }
                }

                if (reguli_cu_inceput_identic.Count > 1)
                    Rezolva_Inceput(lista_elemente, reguli_cu_inceput_identic);
                lista.Clear();


            }
        }

        private void Rezolva_Inceput(List<AElementAlfabet> elemente_comune, List<RegulaDeProductie> reguli)
        {
            NeTerminal neterminal = new NeTerminal(reguli[0]._partea_stanga._nume + "1");
            Neterminale.Add(neterminal);

            RegulaDeProductie R = new RegulaDeProductie(reguli[0]._partea_stanga._nume);

            foreach (AElementAlfabet element in elemente_comune)
                R.AdaugaRegula(element);

            R.AdaugaRegula(neterminal);
            Reguli_de_productie.Add(R);

            for (int i = 0; i < reguli.Count; i++)
            {
                RegulaDeProductie regula = reguli[i];
                regula._partea_stanga = neterminal;
                regula._partea_dreapta.RemoveRange(0, elemente_comune.Count);

                if (regula._partea_dreapta.Count() == 0)
                    regula.AdaugaRegula(new Terminal("epsilon"));
                regula.Taken = false;
            }

            
        }

        #endregion

        #region Recursivitate Stanga

        private void Recursivitate_Stanga(List<RegulaDeProductie> reguli)
        {
            var reguli_cu_recursivitate = (from i in reguli
                                           where i.EsteRecursiva()
                                           select i).ToList();

            if (reguli_cu_recursivitate.Count == 0)
                return;

            NeTerminal partea_stanga = new NeTerminal(reguli[0]._partea_stanga._nume + "2");
            Neterminale.Add(partea_stanga);

            RegulaDeProductie regula_noua = new RegulaDeProductie(partea_stanga._nume);
            regula_noua.AdaugaRegula(new Terminal("epsilon"));
            Reguli_de_productie.Add(regula_noua);


            bool schima_partea;
            

            foreach (var regula in reguli)
            {
                regula.AdaugaRegula(partea_stanga);
                schima_partea = false;

                foreach (var regula_recursiva in reguli_cu_recursivitate)
                {
                    if (regula_recursiva == regula)
                    {
                        schima_partea = true;
                        break;
                    }
                }
                if(schima_partea )
                    regula._partea_stanga = partea_stanga;
            }

            foreach (var regula in reguli_cu_recursivitate)
                regula._partea_dreapta.RemoveRange(0, 1);




        }

        #endregion


        #endregion


        #region Utilitar

        delegate void Conditie(List<RegulaDeProductie> lista);

        class Element<T> where T : new()
        {
            static public void Adauga_elemente_alfabet(ref List<T> nume_lista, string buffer, char separator)
            {
                nume_lista = new List<T>();
                string[] cuvinte = buffer.Split(separator);

                foreach (string cuvant in cuvinte)
                {
                    T element = new T();

                    MethodInfo method = typeof(T).GetMethod("Set");
                    string[] argument = new string[1];
                    argument[0] = cuvant;

                    method.Invoke(element, argument);

                    nume_lista.Add(element);
                }
            }

        }

        private void Adauga_Reguli(string buffer)
        {

            string[] elemente = buffer.Split(':');
            RegulaDeProductie regula_temporara = new RegulaDeProductie(elemente[0]);

            for(int i=2;i<elemente.Count();i++)                     //posibil sa am si :
                elemente[1] += ":" + elemente[i];

            string[] partea_dreapta = elemente[1].Split(' ');

            foreach (string elem in partea_dreapta)
            {
                if (Neterminale.Contains(new NeTerminal(elem)))
                    regula_temporara.AdaugaRegula(new NeTerminal(elem));
                else if (Terminale.Contains(new Terminal(elem)))
                    regula_temporara.AdaugaRegula(new Terminal(elem));
                else
                    throw new Exception("Sibolul nu '" + elem.ToString() + "' apartine lui Vn");
            }

            Reguli_de_productie.Add(regula_temporara);
        }

        private List<RegulaDeProductie> Get_Reguli(NeTerminal partea_stanga)
        {
            List<RegulaDeProductie> lista = new List<RegulaDeProductie>();

            for(int i=0; i<Reguli_de_productie.Count ;i++)
            {
                if(Reguli_de_productie[i]._partea_stanga._nume==partea_stanga._nume
                    && Reguli_de_productie[i].Taken == false)
                {
                    lista.Add(Reguli_de_productie[i]);
                    Reguli_de_productie[i].Taken = true;
                }
            }


            return lista;
        }

        private void Reset_Reguli_Productie()
        {
            foreach (RegulaDeProductie regula in Reguli_de_productie)
                regula.Taken = false;
        }

        private RegulaDeProductie Get_Regula(NeTerminal partea_stanga)
        {
            var regula = (from i in Reguli_de_productie
                          where i._partea_stanga == partea_stanga 
                         select i).ToList();
            return regula[0];
        }

        private bool Intersecteaza_Partial(List<Terminal>multime1,List<Terminal> multime2)
        {
            foreach(var element in multime1)
            {
                if (multime2.Contains(element))
                    return true;
            }

            return false;
        }


        private void Get_Index(Terminal element,int index_director,out int i,out int j)
        {
            i = Neterminale.IndexOf(Reguli_de_productie[index_director]._partea_stanga)+1;
            j = Terminale.IndexOf(element) + 1;

        }
        #endregion

        #region Multime Directori
        
        public void Calculeaza_Directori()
        {
            _directori = new List<Director>();

            Conditie_Directori first = new Conditie_Directori(FIRST);
            Conditie_Directori follow = new Conditie_Directori(FOLLOW);
            Conditie_Directori functie_variabila;

            int i = 0;

            foreach(RegulaDeProductie regula in Reguli_de_productie)
            {
               
                Reset_Reguli_Productie();

                if (regula.Contine(new Terminal("epsilon")))
                {
                    functie_variabila = follow;
                    _directori.Add(new Director("FOLLOW"));
                }
                else
                {
                    functie_variabila = first;
                    _directori.Add(new Director("FIRST"));

                }

                functie_variabila(regula, ref _directori[i]._elemente_directoare);

                var director_curent = _directori[i]._elemente_directoare;
                _directori[i]._elemente_directoare = director_curent.Union(director_curent).ToList();     //elimina duplicatele
                
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //foreach (var element in _directori[i]._elemente_directoare)
                //{
                //    Console.Write(" {0}", element._nume);
                //}
                //Console.WriteLine();
                //!!!!!!!!!!!!!!!!!!!!!!!!!
                _directori[i]._index = i;
                i++;
            }
        }

        private void FIRST(RegulaDeProductie regula,ref List<Terminal> terminale)
        {
            if (regula._partea_dreapta[0].GetType() == typeof(Terminal)
                && regula.Taken!=true )
            {
                terminale.Add((Terminal)regula._partea_dreapta[0]);
                return;
            }

            regula.Taken = true;

            var reguli = from i in Reguli_de_productie
                         where i._partea_stanga._nume == regula._partea_dreapta[0]._nume
                         select i;
            foreach(var regula_selectata in reguli)
                FIRST(regula_selectata, ref terminale);
        }

        private void FOLLOW(RegulaDeProductie regula,ref List<Terminal> terminale)
        {

            var reguli_pentru_follow = (from i in Reguli_de_productie           //selecteaza toate partile de productie care au in partea dreapta partea stanga a regulii data ca parametru
                         where i.Contine(regula._partea_stanga)
                            && i.Taken==false
                         select i).ToList();


            if (reguli_pentru_follow.Count == 0)
                return;

            regula.Taken = true;


            foreach (RegulaDeProductie regula_selectata in reguli_pentru_follow)
            {
                int numar_elemente = regula_selectata._partea_dreapta.Count;

                for(int i=0; i< numar_elemente; i++)
                {
                    if (regula_selectata._partea_dreapta[i]._nume == regula._partea_stanga._nume)
                    {
                        if (i < numar_elemente - 1)
                        {
                            if(regula_selectata._partea_dreapta[i + 1].GetType() == typeof(Terminal))           //cazul  cel mai bun
                                terminale.Add((Terminal)regula_selectata._partea_dreapta[i + 1]);

                            if (regula_selectata._partea_dreapta[i + 1].GetType() == typeof(NeTerminal))    //daca e la inceput sau intre 
                                Follow_FirstFollow(regula_selectata._partea_dreapta[i + 1]._nume,ref terminale);
                        }

                        if (i == numar_elemente - 1)                                    //daca e la final
                            FOLLOW(Get_Regula(regula_selectata._partea_stanga), ref terminale);

                        regula_selectata.Taken = false;
                    }
                    
                }
            }
        }
        private void Follow_FirstFollow(string partea_stanga,ref List<Terminal> terminale)
        {
            var reguli_dreapta_identica = (from r in Reguli_de_productie
                              where r._partea_stanga._nume == partea_stanga
                              select r).ToList();

            foreach(RegulaDeProductie regula in reguli_dreapta_identica)
            {
                if (regula._partea_dreapta[0]._nume == "epsilon")
                    FOLLOW(regula, ref terminale);
                else
                    FIRST(regula, ref terminale);
                regula.Taken = true;
            }
        }
        private delegate void Conditie_Directori(RegulaDeProductie regula, ref List<Terminal> terminale);


        #region Verifica Conditii LL1
            public bool VerificaConditiiLL1()
            {
                Verifica_First();

                Verifica_Follow_First();

                return true;
            }

            private bool Verifica_First()
            {
                Reset_Reguli_Productie();

                foreach (var regula in Reguli_de_productie)
                {
                    var index = (from i in Reguli_de_productie
                                             where i._partea_stanga._nume == regula._partea_stanga._nume
                                                && i.Taken == false
                                             select Reguli_de_productie.IndexOf(i)).ToList();

                    if (index.Count == 0)
                        continue;

                    for(int i=0;i< index.Count-1;i++)
                    {
                        for(int j=i+1;j< index.Count;j++)
                        {
                            if(Intersecteaza_Partial(_directori[index[i]]._elemente_directoare, _directori[index[j]]._elemente_directoare))
                                throw new Exception("Nu se respecta conditia gramaticii LL(1) First(alfa_j) intersectat FIRST(alfa_i)");
                        }

                        Reguli_de_productie[index[i]].Taken = true;
                    }
                    Reguli_de_productie[index.Last()].Taken = true;


                }
              return true;
            }

            private bool Verifica_Follow_First()
            {
               
                List<Terminal> A=new List<Terminal>();
                List<Terminal> alfa=new List<Terminal>();

                foreach(var regula in Reguli_de_productie)
                {
                    Reset_Reguli_Productie();
                    FOLLOW(regula, ref A);

                    Reset_Reguli_Productie();
                    FIRST(regula, ref alfa);

                    if (Intersecteaza_Partial(A, alfa))
                        throw new Exception("Nu se respecta conditia gramaticii LL(1) FOLLOW(A) intersectat FIRST(alfa_i)");

                    A.Clear();
                    alfa.Clear();

                }
            return true;
            }
        #endregion

        #endregion

        #region Genereaza Tabel

        public void Genereaza_Tabel()
        {
        
            Initializeaza_Tabel();

            CompleteazaTabel();

          //  Printeaza();

        }

        private void Initializeaza_Tabel()
        {
            int nr_linii = Terminale.Count + Neterminale.Count + 2;
            int nr_coloane = Terminale.Count + 2;

            Matrice = new ElementMatrice[nr_linii, nr_coloane];

            int i;
            for (i = 1; i < Neterminale.Count+1; i++)
                Matrice[i, 0]._element_alfabet = Neterminale[i-1];

            for (int j =1; j < nr_coloane-1 ; j++,i++)
            {
                Matrice[i, 0]._element_alfabet = Terminale[j-1];
                Matrice[0, j]._element_alfabet = Terminale[j-1];
            }

            Matrice[0, nr_coloane - 1]._element_alfabet=new Terminal("$");
            Matrice[nr_linii-1,0]._element_alfabet=new Terminal("$");
        }

        private void CompleteazaTabel()
        {
            int index_director = 0;
            foreach (var director in _directori)
            {
                foreach(var element in director._elemente_directoare)
                {
                    Get_Index(element, index_director, out int i, out int j);
                    Matrice[i, j]._regula = Reguli_de_productie[index_director];
                }

                index_director++;
            }

            var follow = (from d in _directori
                          where d._tip_director == "FOLLOW"
                          select d).ToList();

            foreach(var element in follow)
            {
                Get_Index(element._elemente_directoare[0], element._index, out int i, out int j);
                Matrice[i,Terminale.Count + 1]._regula = Reguli_de_productie[element._index];
            }
        }

        private void Printeaza() 
        {
            
            for (int j = 1; j < Terminale.Count +2 ; j++)
            {
                Console.Write("\t{0}", Matrice[0, j]._element_alfabet._nume);
            }
            Console.WriteLine();

            for (int i=1;i<Terminale.Count+Neterminale.Count + 2;i++)
            {
                Console.Write("{0}", Matrice[i, 0]._element_alfabet._nume);

                for (int j=1;j<Terminale.Count + 2;j++)
                {
                    if (Matrice[i, j]._regula != null)
                        Console.Write("\t{0}", Matrice[i, j]._regula._partea_stanga._nume);
                    else
                        Console.Write("\t");
                }
                Console.WriteLine();
            }
        }

        #endregion
    }
}
