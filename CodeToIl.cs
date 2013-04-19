using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Microsoft.CSharp;

namespace NetDasm
{
    public partial class CodeToIl : Form
    {
        public CodeToIl()
        {
            InitializeComponent();

            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            textBox1.Document.Parser.Init(Fireball.CodeEditor.SyntaxFiles.CodeEditorSyntaxLoader.GetLanguageFrom(Fireball.CodeEditor.SyntaxFiles.SyntaxLanguage.CSharp));
            button2_Click(null, null);

            splitContainer2.Panel2Collapsed=true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string archivoResultado = System.IO.Path.GetTempFileName();
            CompilerResults resultado = CompilacionDinamica(textBox1.Document.Text, false, false, archivoResultado, true);

            if (resultado.Errors.HasErrors)
            {
                splitContainer2.Panel2Collapsed = false;
                listBox1.Items.Clear();

                foreach (CompilerError error in resultado.Errors)
                {
                    listBox1.Items.Add(error);
                }
            }
            else
            {
                splitContainer2.Panel2Collapsed = true;

                AssemblyDefinition assembly = AssemblyFactory.GetAssembly(archivoResultado);

                //Gets all types of the MainModule of the assembly
                foreach (TypeDefinition type in assembly.MainModule.Types)
                {
                    if (type.Name == "Main")
                    {
                        //Gets all methods of the current type
                        foreach (MethodDefinition method in type.Methods)
                        {
                            if (method.Name == "Function" && method.HasBody)
                            {
                                methodInstructions1.assembly = assembly;
                                methodInstructions1.method = method;
                                methodInstructions1.labelInfo = label4;
                                methodInstructions1.Load();
                            }
                        }
                    }
                }
            }

            //Borrar resultado
            System.IO.File.Delete(archivoResultado);
        }

        public static CompilerResults CompilacionDinamica(string Codigo, bool CodigoesRuta, bool CompilarEnMemoria, string ArchivoResultado, bool CompilarCS)
        {
            //Creamos una instancia de la clase VBCodeProvider o CSharpCodeProvider
            //que usaremos para obtener una referencia a una interfaz ICodeCompiler
            System.CodeDom.Compiler.CodeDomProvider proveedor;
            if (CompilarCS == true)
            {
                proveedor = new CSharpCodeProvider();
            }
            else
            {
                proveedor = new Microsoft.VisualBasic.VBCodeProvider();
            }

            //Usamos la clase CompilerParameters para pasar parámetros al compilador
            //En particular, definimos que el assembly sea compilado en memoria.
            CompilerParameters ParametrosCompilacion = new CompilerParameters();
            if (CompilarEnMemoria == true)
            {
                ParametrosCompilacion.GenerateInMemory = true;
                ParametrosCompilacion.GenerateExecutable = false;
            }
            else
            {
                ParametrosCompilacion.GenerateExecutable = true;
                ParametrosCompilacion.GenerateInMemory = false;
                ParametrosCompilacion.OutputAssembly = ArchivoResultado;
            }
            ParametrosCompilacion.CompilerOptions = "/target:library";
            ParametrosCompilacion.ReferencedAssemblies.Add("System.dll");
            ParametrosCompilacion.ReferencedAssemblies.Add("System.Drawing.dll");
            ParametrosCompilacion.ReferencedAssemblies.Add("System.Windows.Forms.dll");

            //Creamos un objeto CompilerResult que obtendrá los resultados de la compilación
            CompilerResults ResultadosCompilacion;
            if (CodigoesRuta == true)
            {
                ResultadosCompilacion = proveedor.CompileAssemblyFromFile(ParametrosCompilacion, Codigo);
            }
            else
            {
                ResultadosCompilacion = proveedor.CompileAssemblyFromSource(ParametrosCompilacion, Codigo);
            }
            return ResultadosCompilacion;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            textBox1.Document.Text = @"using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace System
{
    class Main
    {
        public static void Function()
        {

        }
    }
}";
        }
	
    }
}