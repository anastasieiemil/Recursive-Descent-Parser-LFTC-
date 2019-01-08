using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoTema
{

    class Program
    {
        static void Main(string[] args)
        {
            Gramatica test = new Gramatica();
            GeneratorCod generator = new GeneratorCod(test);


            try
            {
                test.IncarcaGramatica("Gramatica.txt");

                test.VerificaConditii();

             //   test.Print();

                test.Calculeaza_Directori();

                test.VerificaConditiiLL1();

                test.Genereaza_Tabel();

               // Console.WriteLine(generator.TransformText().ToString());

                string[] referinte = { "System.Core.dll", "System.dll" };
                Compiler compiler = new Compiler(generator.TransformText().ToString(), referinte, "superTare.exe");
                compiler.Compile();
                compiler.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Debug");
        }
    }
 }
