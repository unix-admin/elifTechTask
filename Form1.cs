using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {

        private Database database = new Database();
        private List<double> sum = new List<double>();
        public Form1()
        {
            InitializeComponent();
        }

        private void insertButton_Click(object sender, EventArgs e)
        {
            viewEditForm insert = new viewEditForm();
            insert.ShowDialog();
            paintTree();
        }
        //This megafunction is used for build tree
        private void buildTree(TreeNode parent, int parent_id, List<double> sum)
        {
            List<Database.companyData> root = database.getRoot(parent_id);
            if (root.Count > 0)
                root.ForEach(delegate(Database.companyData data)
                {
                    if (data.parentId == 0)
                        sum.Clear();
                    TreeNode node = parent.Nodes.Add(data.name + " | $" + data.sumEstimate.ToString());
                    node.Tag = data.sumEstimate.ToString();                    
                    buildTree(node, data.id, sum);
                    double sumEstimatePlusChild = 0;
                    sum.Add(data.sumEstimate);
                    foreach (double sumEstimate in sum)
                    {
                        sumEstimatePlusChild += sumEstimate;
                    }
                    if (database.getCount(data.id) != 0)
                        node.Text += " | $" + sumEstimatePlusChild.ToString();                     
                });

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            paintTree();
        }

        private void paintTree() 
        {

            sum.Clear();
            treeView1.Nodes.Clear();
            buildTree(treeView1.Nodes.Add("Companies tree"), 0, sum);
            treeView1.ExpandAll();

        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Parent != null)
            {
                DialogResult result;
                string[] company = treeView1.SelectedNode.Text.Split('|');
                result = MessageBox.Show(this, "Are you sure to delete " + company[0] + "?", "Delete company", MessageBoxButtons.YesNo);
                treeView1.SelectedNode = null;
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                   
                   database.deleteData(database.getCompanyByName(company[0].ToString().TrimEnd()).id);
                   paintTree();
                }
            }
            
        }
       
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            Database.companyData companyToEdit = new Database.companyData();     
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Parent != null)
            {
                string[] company = treeView1.SelectedNode.Text.Split('|');
                companyToEdit = database.getCompanyByName(company[0].ToString().TrimEnd());
                treeView1.SelectedNode = null;
                viewEditForm formToEdit = new viewEditForm();
                formToEdit.setData(viewEditForm.formActions.UPDATE, companyToEdit.id);
                formToEdit.ShowDialog();
                paintTree();
            }
            
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            e.Node.Expand();
            Database.companyData companyToView = new Database.companyData();
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Parent != null)
            {
                string[] company = treeView1.SelectedNode.Text.Split('|');
                companyToView = database.getCompanyByName(company[0].ToString().TrimEnd());
                treeView1.SelectedNode = null;
                viewEditForm formToView = new viewEditForm();
                formToView.setData(viewEditForm.formActions.VIEW, companyToView.id);
                formToView.ShowDialog();
                paintTree();
            }
        }
               
    }

}