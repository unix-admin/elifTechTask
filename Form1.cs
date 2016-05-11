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
        private List<decimal> sum = new List<decimal>();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sum.Clear();
            treeView1.Nodes.Clear();                                                  
            buildTree(treeView1.Nodes.Add("Companies tree"),0,sum);
            treeView1.ExpandAll();
        }
        //This megafunction is used for build tree
        private void buildTree(TreeNode parent, int parent_id, List<decimal> sum)
        {
            List<Database.companyData> root = database.getRoot(parent_id);
            if (root.Count > 0)
                root.ForEach(delegate(Database.companyData data)
                {
                    TreeNode node = parent.Nodes.Add(data.name + " | $" + data.sumEstimate.ToString());
                    node.Tag = data.sumEstimate.ToString();                    
                    buildTree(node, data.id, sum);
                    decimal sumEstimatePlusChild = 0;
                    sum.Add(data.sumEstimate);
                    foreach (decimal sumEstimate in sum)
                    {
                        sumEstimatePlusChild += sumEstimate;
                    }
                    if (database.getCount(data.id) != 0)
                        node.Text += " | $" + sumEstimatePlusChild.ToString();                     
                });

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            buildTree(treeView1.Nodes.Add("Companies tree"), 0, sum);
            treeView1.ExpandAll();
        }

        
    }

}