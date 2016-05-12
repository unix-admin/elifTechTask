namespace WindowsFormsApplication3
{
    partial class viewEditForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelCompanyName = new System.Windows.Forms.Label();
            this.companyName = new System.Windows.Forms.TextBox();
            this.labelCompanyEstimate = new System.Windows.Forms.Label();
            this.companyEstimate = new System.Windows.Forms.TextBox();
            this.labelParentCompany = new System.Windows.Forms.Label();
            this.parentCompanies = new System.Windows.Forms.ComboBox();
            this.buttonAction = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelCompanyName
            // 
            this.labelCompanyName.AutoSize = true;
            this.labelCompanyName.Location = new System.Drawing.Point(14, 45);
            this.labelCompanyName.Name = "labelCompanyName";
            this.labelCompanyName.Size = new System.Drawing.Size(80, 13);
            this.labelCompanyName.TabIndex = 0;
            this.labelCompanyName.Text = "Company name";
            // 
            // companyName
            // 
            this.companyName.Location = new System.Drawing.Point(113, 42);
            this.companyName.Name = "companyName";
            this.companyName.Size = new System.Drawing.Size(244, 20);
            this.companyName.TabIndex = 1;
            // 
            // labelCompanyEstimate
            // 
            this.labelCompanyEstimate.AutoSize = true;
            this.labelCompanyEstimate.Location = new System.Drawing.Point(14, 88);
            this.labelCompanyEstimate.Name = "labelCompanyEstimate";
            this.labelCompanyEstimate.Size = new System.Drawing.Size(93, 13);
            this.labelCompanyEstimate.TabIndex = 2;
            this.labelCompanyEstimate.Text = "Company estimate";
            // 
            // companyEstimate
            // 
            this.companyEstimate.Location = new System.Drawing.Point(113, 85);
            this.companyEstimate.Name = "companyEstimate";
            this.companyEstimate.Size = new System.Drawing.Size(106, 20);
            this.companyEstimate.TabIndex = 3;
            this.companyEstimate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.companyEstimate_KeyPress);
            // 
            // labelParentCompany
            // 
            this.labelParentCompany.AutoSize = true;
            this.labelParentCompany.Location = new System.Drawing.Point(14, 126);
            this.labelParentCompany.Name = "labelParentCompany";
            this.labelParentCompany.Size = new System.Drawing.Size(84, 13);
            this.labelParentCompany.TabIndex = 4;
            this.labelParentCompany.Text = "Parent company";
            // 
            // parentCompanies
            // 
            this.parentCompanies.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.parentCompanies.FormattingEnabled = true;
            this.parentCompanies.Location = new System.Drawing.Point(113, 123);
            this.parentCompanies.Name = "parentCompanies";
            this.parentCompanies.Size = new System.Drawing.Size(244, 21);
            this.parentCompanies.TabIndex = 5;
            // 
            // buttonAction
            // 
            this.buttonAction.Location = new System.Drawing.Point(114, 175);
            this.buttonAction.Name = "buttonAction";
            this.buttonAction.Size = new System.Drawing.Size(223, 23);
            this.buttonAction.TabIndex = 6;
            this.buttonAction.Text = "Insert";
            this.buttonAction.UseVisualStyleBackColor = true;
            this.buttonAction.Click += new System.EventHandler(this.buttonAction_Click);
            // 
            // viewEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 217);
            this.Controls.Add(this.buttonAction);
            this.Controls.Add(this.parentCompanies);
            this.Controls.Add(this.labelParentCompany);
            this.Controls.Add(this.companyEstimate);
            this.Controls.Add(this.labelCompanyEstimate);
            this.Controls.Add(this.companyName);
            this.Controls.Add(this.labelCompanyName);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "viewEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Insert company";
            this.Shown += new System.EventHandler(this.viewEditForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelCompanyName;
        private System.Windows.Forms.TextBox companyName;
        private System.Windows.Forms.Label labelCompanyEstimate;
        private System.Windows.Forms.TextBox companyEstimate;
        private System.Windows.Forms.Label labelParentCompany;
        private System.Windows.Forms.ComboBox parentCompanies;
        private System.Windows.Forms.Button buttonAction;
    }
}