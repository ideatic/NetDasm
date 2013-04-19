using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mono.Cecil.Cil;
using System.Reflection;

namespace NetDasm
{
    public partial class InsertInstruction : Form
    {
        public InsertInstruction(int maxIndex)
        {
            InitializeComponent();

            foreach (MemberInfo miembro in typeof(OpCodes).GetMembers())
            {
                if(miembro.MemberType.ToString()=="Field")
                comboBox1.Items.Add(miembro.Name);
            }
            comboBox1.SelectedIndex = 0;
            
            numericUpDown1.Maximum = maxIndex;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MsilInstruction ins=MsilInstruction.GetInstruction(comboBox1.Text);

            if(ins!=null)
            {
            label4.Text = string.Format(@"Instruction info:
ByteCode: {0}
Format: {1}
Description: {2}
", ins.ByteCode,ins.Format,ins.Description);
            }
            else
                 label4.Text = "Instruction info:";
        }
    }
}