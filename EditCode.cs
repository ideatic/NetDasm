using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Reflection;
using NetDasm.Controls;

namespace NetDasm
{
    public partial class EditCode : Form
    {
        AssemblyDefinition assembly;
        MethodDefinition method;
        public EditCode(MethodDefinition metodo, AssemblyDefinition assembly)
        {
            InitializeComponent();
            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            this.assembly = assembly;
            this.method = metodo;

            methodInstructions1.assembly = assembly;
            methodInstructions1.method = metodo;
            methodInstructions1.labelInfo = label1;
            methodInstructions1.Load();            
        }



        private void button1_Click(object sender, EventArgs e)
        {
            methodInstructions1.RemoveSelected();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            InsertInstruction insert = new InsertInstruction(method.Body.Instructions.Count - 1);
            insert.ShowDialog();

            if (insert.DialogResult == DialogResult.OK)
            {
                methodInstructions1.InsertInstruction(insert);
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            methodInstructions1.filter = textBox3.Text;
            methodInstructions1.Load();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (methodInstructions1.SelectedItem != null)
            {
                InsertInstruction insert = new InsertInstruction(method.Body.Instructions.Count - 1);
                insert.numericUpDown1.Value = methodInstructions1.SelectedIndex;
                insert.comboBox1.SelectedItem = methodInstructions1.SelectedInstruction.OpCode.Code.ToString();
                if(methodInstructions1.SelectedInstruction.Operand!=null)
                insert.textBox1.Text = methodInstructions1.SelectedInstruction.Operand.ToString();
            insert.button1.Text = "Modify";
                insert.ShowDialog();

                if (insert.DialogResult==DialogResult.OK)
                {
                    methodInstructions1.RemoveSelected();
                    methodInstructions1.InsertInstruction(insert);
                }
            }
        }
    }
}