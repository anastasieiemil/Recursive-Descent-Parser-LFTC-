using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using DemoTema;
using Microsoft.Win32;
using TemaFacultativa;
using TemaFacultativaWPF;
using TemaFacultativaWPF.GramaticaModel;
using TemaFacultativaWPF.TabelModel;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace TemaFacultativa
{
    static class CodeBehind
    {
        private static Gramatica _gramatica;
        private static GeneratorCod _generator;
        public static Compiler _compilator;
        public static int index;
        public static List<UIElement> _frames;

        public static TextBinding _generatedProgram;

        public static void Initializare()
        {
            _gramatica = new Gramatica();
            index = 0;
            _frames = new List<UIElement>();
            _generator = new GeneratorCod(_gramatica);
            
        }



        public static void Incarca_gramatica()
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.ShowDialog();
            try
            {
                _gramatica.IncarcaGramatica(openDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
                
        internal static void Ruleaza()
        {
            try
            {
                _compilator.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        internal static void RuleazaAlgormitmul(MainWindow mainWindow)
        {

            try
            {
                _gramatica.VerificaConditii();
                _gramatica.Calculeaza_Directori();
                _gramatica.VerificaConditiiLL1();
                _gramatica.Genereaza_Tabel();

                Arata_Gramatica_Modificata(mainWindow);

                Arata_Multimea_Directori(mainWindow);

                Arata_Cod_Generat(mainWindow);

                Arata_Tabel_Generat(mainWindow);


            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        internal static void Compileaza()
        {
            try
            {
                string[] referinte = { "System.Core.dll", "System.dll" };
               _compilator = new Compiler(_generatedProgram.text, referinte, "executabil.exe");
               _compilator.Compile();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #region Metode private pentru afisare de date

        private static void Arata_Gramatica_Modificata(MainWindow mainWindow)
        {
            var gramatica_view = new GramaticaView();
            
            _frames.Add(gramatica_view);
            index++;
            Seteaza_Controler( mainWindow);
            
            
            #region construieste Gramatica
            StringBuilder text =new StringBuilder();

            text.Append("\t\tGramatica Modificata\n\n\n");

            foreach(var regula in _gramatica.Reguli_de_productie)
            {
                text.Append(regula._partea_stanga._nume);

                text.Append(" => ");

                foreach (var element in regula._partea_dreapta)
                {
                    text.Append(" ");
                    text.Append(element._nume);
                }

                text.Append("\n");
            }

            #endregion
            
            gramatica_view.DataContext = new TextBinding(text.ToString());

        }

        private static void Arata_Multimea_Directori(MainWindow mainWindow)
        {
            var directori_view = new GramaticaView();

            _frames.Add(directori_view);
            index++;
            Seteaza_Controler( mainWindow);

            StringBuilder text = new StringBuilder();

            #region Text
            text.Append("\t\tMultime Directori\n\n\n");

            foreach(var director in _gramatica._directori)
            {
                if (director._tip_director == "FOLLOW")
                {
                    text.Append("FOLLOW( ");
                    text.Append(_gramatica.Reguli_de_productie[director._index]._partea_stanga._nume);                    
                }
                else
                {
                    text.Append("FIRST( ");
                    text.Append(_gramatica.Reguli_de_productie[director._index].ToString());            
                }
                text.Append(") =>{ ");
                text.Append(director.ToString());
                text.Append(" }\n");
                
            }

            #endregion

            directori_view.DataContext = new TextBinding(text.ToString());
        }

        private static void Arata_Tabel_Generat(MainWindow mainWindow)
        {
            var view_table = new TabelView();

            _frames.Add(view_table);
            index++;
            Seteaza_Controler(mainWindow);

            ScrollViewer scroll = new ScrollViewer();
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            StackPanel Header = new StackPanel
            {
                Orientation = Orientation.Horizontal               
            };       


            #region Aseaza Date

            #region Header
            Header.Children.Add(Get_Text_Box_Element("Net + Ter"));
            for (int i = 1; i < _gramatica.Terminale.Count + 2; i++)
            {
                Header.Children.Add(Get_Text_Box_Element(_gramatica.Matrice[0, i]._element_alfabet._nume));
            }
            view_table.DataStackPanel.Children.Add(Header);
            
            #endregion

            #region Body

            for (int i=1;i<_gramatica.Terminale.Count+_gramatica.Neterminale.Count+2;i++)
            {
                var row = new StackPanel() { Orientation= Orientation.Horizontal};
                row.Children.Add(Get_Text_Box_Element(_gramatica.Matrice[i,0]._element_alfabet._nume));

                for (int j=1;j<_gramatica.Terminale.Count+2;j++)
                {
                    string nume = "";
                    if(_gramatica.Matrice[i, j]._regula!=null)
                        nume ="R" + _gramatica.Reguli_de_productie.IndexOf(_gramatica.Matrice[i,j]._regula).ToString();

                    if(_gramatica.Matrice[i,0]._element_alfabet!=null&& _gramatica.Matrice[0, j]._element_alfabet!=null)
                    {
                        if (_gramatica.Matrice[i, 0]._element_alfabet._nume == _gramatica.Matrice[0, j]._element_alfabet._nume)
                        {
                            if (_gramatica.Matrice[i, 0]._element_alfabet._nume == "$")
                                nume = "A";
                            else
                                nume = "P";
                        }
                    }
                    row.Children.Add(Get_Text_Box_Element(nume));

                }

                view_table.DataStackPanel.Children.Add(row);


            }
            #endregion 


            #endregion

            scroll.Content = view_table.DataStackPanel;

            view_table.Content = scroll;
           
        }

        private static void Arata_Cod_Generat(MainWindow mainWindow)
        {
            var cod_view = new GramaticaView();

            _frames.Add(cod_view);
            index++;
            Seteaza_Controler(mainWindow);

           // _generatedProgram = _generator.TransformText();
           string program_generat = _generator.TransformText();
            cod_view.TextBox.IsReadOnly = false;

            _generatedProgram = new TextBinding(program_generat);
            cod_view.DataContext = _generatedProgram;

        }

        public static void  Seteaza_Controler( MainWindow mainWindow )
        {
            mainWindow.GridOrientation.Children.RemoveRange(2, mainWindow.GridOrientation.Children.Count);  //sterg tot din mainframe
            Grid.SetRow(_frames[index-1], 1);
            Grid.SetColumn(_frames[index-1], 0);

            mainWindow.GridOrientation.Children.Add(_frames[index-1]);

            
        }

        private static TextBox Get_Text_Box_Element(string text)
        {
           
            return new TextBox()
            {
                Margin = new Thickness(10, 10, 10, 10),
                Text = text,
                IsReadOnly = true,
                FontWeight = FontWeights.DemiBold,
                FontSize = 16,
                BorderBrush = Brushes.Black,
                Width=70,
                HorizontalAlignment=HorizontalAlignment.Center
                
            };
        }
               
        #endregion

    }

    class TextBinding: INotifyPropertyChanged
    {

        private string Text;
        public string text
        {
            get
            {
                return Text;
            }
            set
            {
                Text = value;

                OnPropertyChanged();
            }
        }
        public TextBinding( string sir)
        {
            text = sir;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("text"));
        }
    };


    }
