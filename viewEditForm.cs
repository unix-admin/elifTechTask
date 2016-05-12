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
    public partial class viewEditForm : Form
    {
        public enum formActions { 
            INSERT = 0,
            UPDATE,
            VIEW

        }
        private formActions action;
        private Database database = new Database();
        private Database.companyData companyData;
        public viewEditForm()
        {
            InitializeComponent();
        }



        public void setData(formActions _action, int companyId)
        {
            action = _action;
            companyData = database.getCompany(companyId);
        }

        private void viewEditForm_Shown(object sender, EventArgs e)
        {
            parentCompanies.Items.Add("");
            List<Database.companyData> companies = database.getAllCompanies();
            companies.ForEach(delegate(Database.companyData data)
            {
                parentCompanies.Items.Add(data.name);
            });

            switch (action)
            {
                case formActions.UPDATE:
                    companyName.Text = companyData.name.ToString();
                    companyEstimate.Text = companyData.estimate.ToString();
                    if (companyData.parentId !=0)
                        parentCompanies.SelectedIndex = parentCompanies.Items.IndexOf(database.getCompany(companyData.parentId).name);
                    buttonAction.Text = "Update";
                    this.Text = "Edit company";
                    break;                   
                case formActions.VIEW:
                    companyName.Text = companyData.name.ToString();
                    companyEstimate.Text = companyData.estimate.ToString();
                    
                    if (companyData.parentId !=0)
                        parentCompanies.SelectedIndex = parentCompanies.Items.IndexOf(database.getCompany(companyData.parentId).name);
                    companyName.Enabled = false;
                    companyEstimate.Enabled = false;
                    parentCompanies.Enabled = false;
                    buttonAction.Text = "Close";
                    this.Text = "View company";
                    break;
                
            }
              
        }

        private void buttonAction_Click(object sender, EventArgs e)
        {
            if (validateCompanyName(companyName.Text))
            {
                switch (action)
                {
                    case formActions.UPDATE:
                        updateData();
                        break;
                    case formActions.VIEW:
                        Close();
                        break;
                    default:
                        insertData();
                        break;
                }

                Close();
            }
            else
                MessageBox.Show("Company with name "+companyName.Text+" is already exist!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
        }

        private bool validateCompanyName(string _companyName) {
            bool result = true;
            switch(action)
            {
                   
                case formActions.INSERT:
                    if (database.getCompanyByName(_companyName.TrimEnd()).id != 0)
                        result = false;
                    break;
                case formActions.UPDATE:
                    if (database.getCompanyByName(_companyName.TrimEnd()).id != companyData.id)
                        result = false;
                    break;
            }
            return result;

        }

        private void insertData()
        {            
            Database.companyData newCompany= new Database.companyData();
            newCompany.name = companyName.Text;
            if (companyEstimate.Text != "")
                newCompany.estimate = Convert.ToDouble(companyEstimate.Text);
            else
                newCompany.estimate = 0;
            if (parentCompanies.Text == "")
            {
                newCompany.parentId = 0;
            }
            else
            {
                newCompany.parentId = database.getParentId(parentCompanies.Text);
            }
            database.insertData(newCompany);            
        }

        private void updateData()
        {

            companyData.name = companyName.Text;
            if (companyEstimate.Text != "")
                companyData.estimate = Convert.ToDouble(companyEstimate.Text);
            else
                companyData.estimate = 0;
            if (parentCompanies.Text == "")
            {
                companyData.parentId = 0;
            }
            else
            {
                companyData.parentId = database.getParentId(parentCompanies.Text);
            }
            database.updateData(companyData);
        }

        private void companyEstimate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8 && e.KeyChar !=44)
            e.Handled = true;   
        }

        
    }
}
