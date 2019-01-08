using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

class Compiler
{
    private string _program;
    private string[] _referinte;
    private string _nume_program;

    public string Program { get => _program; set => _program = value; }

    #region Constructor
    public Compiler(string program, string[] referinte,string nume_program)
    {
        _program = program;

        _referinte = referinte;

        _nume_program = nume_program;
    }

    #endregion

    #region Public Methods

    public void Compile()
    {
        CompilerParameters parameters = new CompilerParameters(_referinte, _nume_program)
        {
            GenerateExecutable = true
        };

        CSharpCodeProvider provider = new CSharpCodeProvider();
        CompilerResults results= provider.CompileAssemblyFromSource(parameters, _program);

        if(results.Errors.HasErrors)
{
            StringBuilder sb = new StringBuilder();

            foreach (CompilerError error in results.Errors)
                sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));

            throw new InvalidOperationException(sb.ToString());
        }

    }

    public void Run()
    {
        Process.Start(_nume_program);
    }
}


    #endregion
