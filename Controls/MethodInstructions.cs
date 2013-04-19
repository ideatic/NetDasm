using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Text.RegularExpressions;
using System.Reflection;

namespace NetDasm.Controls
{
    class MethodInstructions : ListBox
    {
        public AssemblyDefinition assembly;
        public MethodDefinition method;
        public Label labelInfo;
        public string filter;

        public MethodInstructions()
        {
            this.SelectedIndexChanged += new EventHandler(MethodInstructions_SelectedIndexChanged);
        }

        public void Load()
        {
            this.Items.Clear();
            if (method.Body != null)
            {
                Regex regex = filter != null ? new Regex(filter, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled) : null;

                int contador = 0;
                foreach (Instruction inst in method.Body.Instructions)
                {
                    string text = InstructionText(inst);
                    if (filter == null || regex.Match(text).Success)
                    {
                        ListItem<Instruction> nodo = new ListItem<Instruction>();
                        nodo.Text = contador + ": " + text;
                        nodo.Tag = inst;
                        nodo.Index = contador;
                        this.Items.Add(nodo);                      
                    }
                    contador++;
                }
            }
            else MessageBox.Show("The method body is empty");
        }

        public void RemoveSelected()
        {
            object[] lista = new object[this.SelectedItems.Count];
            this.SelectedItems.CopyTo(lista, 0);

            int contador = 0;
            foreach (object item in lista)
            {
                method.Body.Instructions.RemoveAt(((ListItem<Instruction>)item).Index);
                this.Items.Remove(item);
                contador++;
            }
            assembly.MainModule.Import(method.DeclaringType);
            Load();
        }

        public Instruction SelectedInstruction
        {
            get {
                return ((NetDasm.Controls.MethodInstructions.ListItem<Instruction>)this.SelectedItem).Tag;
            }
        }

        public class ListItem<T>
        {
            public T Tag;
            public string Text;
            public int Index;

            public override string ToString()
            {
                return Text;
            }
        }

        public static string InstructionText(Instruction inst)
        {
            if (inst.Operand is Mono.Cecil.Cil.Instruction)
            {
                Mono.Cecil.Cil.Instruction instruccion = (Instruction)inst.Operand;
                return string.Format("{0} {1}", inst.OpCode.ToString(), instruccion.Offset.ToString());
            }
            else if (inst.Operand is string)
            {
                return string.Format("{0} \"{1}\"", inst.OpCode.ToString(), inst.Operand.ToString());
            }
            else if (inst.Operand is MethodReference)
            {
                MethodReference metodo = (MethodReference)inst.Operand;
                return inst.OpCode.ToString() + " " + metodo.ToString();
            }
            else if (inst.Operand != null)
            {
                return inst.OpCode.ToString() + " " + inst.Operand.ToString();
            }
            else
            {
                return inst.OpCode.ToString();
            }
        }

        public void InsertInstruction(InsertInstruction insert)
        {
            CilWorker worker = this.method.Body.CilWorker;

            OpCode opcode = OpCodes.Add;
            foreach (MemberInfo miembro in typeof(OpCodes).GetMembers())
            {
                if (miembro.MemberType.ToString() == "Field" &&
                    insert.comboBox1.SelectedItem.ToString() == miembro.Name)
                {
                    FieldInfo info = (FieldInfo)miembro;
                    opcode = (OpCode)info.GetValue(null);
                }
            }

            Instruction sentence;

            if (insert.textBox1.TextLength == 0)
            {
                sentence = worker.Create(opcode);
            }
            else
            {
                int valor;
                if (int.TryParse(insert.textBox1.Text, out valor))
                {
                    sentence = worker.Create(opcode, valor);
                }
                else
                {
                    sentence = worker.Create(opcode, insert.textBox1.Text);
                }
            }

            method.Body.CilWorker.InsertBefore(method.Body.Instructions[(int)insert.numericUpDown1.Value], sentence);

            assembly.MainModule.Import(method.DeclaringType);
            this.Load(); 
        }

        void MethodInstructions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (labelInfo != null)
            {
                if (this.SelectedItem != null)
                {
                    Instruction inst = SelectedInstruction;
                    MsilInstruction ins = MsilInstruction.GetInstruction(inst.OpCode.ToString());

                    labelInfo.Text = string.Format(@"Instruction info:
Code: {0}", MethodInstructions.InstructionText(inst));

                    if (ins != null)
                    {
                        labelInfo.Text += string.Format(@"
ByteCode: {0}
Format: {1}
Description: {2}
", ins.ByteCode, ins.Format, ins.Description);
                    }
                }
                else
                    labelInfo.Text = "Instruction info:";
            }
        }

    }
}
