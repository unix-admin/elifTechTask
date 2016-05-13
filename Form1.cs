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
        //Action when insert button was clicked
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
        //Action when form was shown
        private void Form1_Shown(object sender, EventArgs e)
        {
            paintTree();
        }
        //This megafunction is used for painting comanies tree in treeview
        private void paintTree() 
        {

            sum.Clear();
            treeView1.Nodes.Clear();
            buildTree(treeView1.Nodes.Add("Companies tree"), 0, sum);
            treeView1.ExpandAll();

        }
        //Action when delete button was clicked
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Parent != null)
            {
                DialogResult result;
                result = MessageBox.Show(this, "Are you sure to delete " + getCompanyIdByNode(treeView1.SelectedNode).name + "?", "Delete company", MessageBoxButtons.YesNo);
                
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                   database.deleteData(getCompanyIdByNode(treeView1.SelectedNode).id);
                   treeView1.SelectedNode = null;
                   paintTree();
                }
                
            }
            
        }
        //Action when edit button was clicked
        private void buttonEdit_Click(object sender, EventArgs e)
        {            
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Parent != null)
            {   
                viewEditForm formToEdit = new viewEditForm();
                formToEdit.setData(viewEditForm.formActions.UPDATE, getCompanyIdByNode(treeView1.SelectedNode).id);
                treeView1.SelectedNode = null;
                formToEdit.ShowDialog();
                paintTree();
            }
            
        }
        //Action when treeview node was doubleclicked
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            e.Node.Expand();            
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Parent != null)
            {                                                
                viewEditForm formToView = new viewEditForm();
                formToView.setData(viewEditForm.formActions.VIEW, getCompanyIdByNode(treeView1.SelectedNode).id);
                treeView1.SelectedNode = null;
                formToView.ShowDialog();
                paintTree();
            }
        }
        //This megafunction is used for geting company data by selected node in treeview
        private Database.companyData getCompanyIdByNode(TreeNode node)
        {
            string[] company = node.Text.Split('|');
            return database.getCompanyByName(company[0].ToString().TrimEnd());
        }
               
    }    

}