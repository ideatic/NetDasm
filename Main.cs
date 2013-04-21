using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mono.Cecil;
using System.Text.RegularExpressions;

namespace NetDasm
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }

        AssemblyDefinition assembly;
        private void loadAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                assembly = AssemblyFactory.GetAssembly(openFileDialog.FileName);
                LoadAssembly();
            }
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                AssemblyFactory.SaveAssembly(assembly, saveFileDialog.FileName);
            }
        }

        private void filtroCambiado(object sender, EventArgs e)
        {
            LoadAssembly();
        }

        private void LoadAssembly()
        {
            Regex filter = textBox3.TextLength > 0 ? new Regex(textBox3.Text, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled) : null;
            treeView1.Nodes.Clear();
            foreach (TypeDefinition type in this.assembly.MainModule.Types)
            {
                if (type.Name != "<Module>")
                {
                    if (filter == null || filter.Match(type.Name).Success)
                    {
                        TreeNode nodo = new TreeNode();
                        nodo.Text = type.Name;
                        nodo.Tag = type;

                        treeView1.Nodes.Add(nodo);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                TypeDefinition type = (TypeDefinition)treeView1.SelectedNode.Tag;
                assembly.MainModule.Types.Remove(type);
                int index = treeView1.Nodes.IndexOf(treeView1.SelectedNode);
                treeView1.Nodes.Remove(treeView1.SelectedNode);

                if (treeView1.Nodes.Count > index)
                    treeView1.SelectedNode = treeView1.Nodes[index];
                else if (treeView1.Nodes.Count > 0)
                    treeView1.SelectedNode = treeView1.Nodes[0];
            }
        }

        private void TypeSelected(object sender, TreeViewEventArgs e)
        {
            TypeDefinition type = (TypeDefinition)treeView1.SelectedNode.Tag;

            label2.Text = string.Format(@"Type info:
Name: {0}
Namespace: {1}
Base type: {2}
Fullname: {3}
Methods: {4}
Properties: {5}", type.Name, type.Namespace, type.BaseType != null ? type.BaseType.Name : string.Empty, type.FullName, type.Methods.Count
 , type.Properties.Count);
            treeView2.Nodes.Clear();
            Regex filter = textBox4.TextLength > 0 ? new Regex(textBox4.Text, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled) : null;

            foreach (MethodDefinition method in type.Methods)
            {
                if (filter == null || filter.Match(method.Name).Success)
                {
                    TreeNode nodo = new TreeNode();
                    nodo.Text = method.Name;
                    nodo.Tag = method;

                    treeView2.Nodes.Add(nodo);
                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            TypeSelected(null, null);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView2.SelectedNode != null)
            {
                TypeDefinition type = (TypeDefinition)treeView1.SelectedNode.Tag;
                MethodDefinition metodo = (MethodDefinition)treeView2.SelectedNode.Tag;
                type.Methods.Remove(metodo);

                int index = treeView2.Nodes.IndexOf(treeView2.SelectedNode);
                treeView2.Nodes.Remove(treeView2.SelectedNode);

                if (treeView2.Nodes.Count > index)
                    treeView2.SelectedNode = treeView2.Nodes[index];
                else if (treeView2.Nodes.Count > 0)
                    treeView2.SelectedNode = treeView2.Nodes[0];
            }
        }

        private void MemberSelected(object sender, TreeViewEventArgs e)
        {
            MethodDefinition metodo = (MethodDefinition)treeView2.SelectedNode.Tag;
            label4.Text = string.Format(@"Method info:
Name: {0}
Return type: {1}
Parametros: {2}
Instrucciones: {3}", metodo.Name, metodo.ReturnType.ReturnType.Name, metodo.Parameters.Count, metodo.Body != null ? metodo.Body.Instructions.Count : 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (treeView2.SelectedNode != null)
            {
                MethodDefinition metodo = (MethodDefinition)treeView2.SelectedNode.Tag;

                EditCode edit = new EditCode(metodo, assembly);
                edit.Show();
            }
        }

        private void Principal_FormClosing(object sender, FormClosingEventArgs e)
        {
                e.Cancel = MessageBox.Show("Are you sure that want to close the application?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No;
        }

        private void lunaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            officeBackPanel1.Style = System.Windows.FutureStyle.Office2007.OfficeBackPanelStyles.Luna;
        }

        private void vistaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            officeBackPanel1.Style = System.Windows.FutureStyle.Office2007.OfficeBackPanelStyles.Vista;
        }

        private void whiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            officeBackPanel1.Style = System.Windows.FutureStyle.Office2007.OfficeBackPanelStyles.Transparent;
        }

        private void codeToILToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CodeToIl window = new CodeToIl();
            window.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }
    }
}