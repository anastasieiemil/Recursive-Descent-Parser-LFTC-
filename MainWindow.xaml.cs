using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TemaFacultativa;
using TemaFacultativaWPF.GramaticaModel;

namespace TemaFacultativaWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CodeBehind.Initializare();

        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            CodeBehind.Incarca_gramatica();
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            CodeBehind.RuleazaAlgormitmul(this);
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (CodeBehind.index - 1 <= 0)
                return;
            CodeBehind.index--;
            CodeBehind.Seteaza_Controler(this);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CodeBehind.index  > CodeBehind._frames.Count-1)
                return;
            CodeBehind.index++;
            CodeBehind.Seteaza_Controler(this);
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CodeBehind._compilator.Run();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void CompileButton_Click(object sender, RoutedEventArgs e)
        {
            CodeBehind.Compileaza();
        }
    }
}
