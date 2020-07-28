namespace AutoTagRoomsWF
{
    partial class AutoTagRoomsForm
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
            this.levelLabel = new System.Windows.Forms.Label();
            this.tagTypeLabel = new System.Windows.Forms.Label();
            this.levelsComboBox = new System.Windows.Forms.ComboBox();
            this.tagTypesComboBox = new System.Windows.Forms.ComboBox();
            this.autoTagButton = new System.Windows.Forms.Button();
            this.roomsListView = new System.Windows.Forms.ListView();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // levelLabel
            // 
            this.levelLabel.AutoSize = true;
            this.levelLabel.Location = new System.Drawing.Point(27, 20);
            this.levelLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.levelLabel.Name = "levelLabel";
            this.levelLabel.Size = new System.Drawing.Size(55, 15);
            this.levelLabel.TabIndex = 2;
            this.levelLabel.Text = "Level:";
            // 
            // tagTypeLabel
            // 
            this.tagTypeLabel.AutoSize = true;
            this.tagTypeLabel.Location = new System.Drawing.Point(27, 63);
            this.tagTypeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.tagTypeLabel.Name = "tagTypeLabel";
            this.tagTypeLabel.Size = new System.Drawing.Size(79, 15);
            this.tagTypeLabel.TabIndex = 3;
            this.tagTypeLabel.Text = "Tag Type:";
            // 
            // levelsComboBox
            // 
            this.levelsComboBox.FormattingEnabled = true;
            this.levelsComboBox.Location = new System.Drawing.Point(143, 12);
            this.levelsComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.levelsComboBox.Name = "levelsComboBox";
            this.levelsComboBox.Size = new System.Drawing.Size(232, 23);
            this.levelsComboBox.Sorted = true;
            this.levelsComboBox.TabIndex = 4;
            // 
            // tagTypesComboBox
            // 
            this.tagTypesComboBox.FormattingEnabled = true;
            this.tagTypesComboBox.Location = new System.Drawing.Point(143, 55);
            this.tagTypesComboBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tagTypesComboBox.Name = "tagTypesComboBox";
            this.tagTypesComboBox.Size = new System.Drawing.Size(232, 23);
            this.tagTypesComboBox.TabIndex = 5;
            // 
            // autoTagButton
            // 
            this.autoTagButton.Location = new System.Drawing.Point(472, 57);
            this.autoTagButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.autoTagButton.Name = "autoTagButton";
            this.autoTagButton.Size = new System.Drawing.Size(208, 27);
            this.autoTagButton.TabIndex = 6;
            this.autoTagButton.Text = "&Auto Tag All";
            this.autoTagButton.UseVisualStyleBackColor = true;
            this.autoTagButton.Click += new System.EventHandler(this.autoTagButton_Click);
            // 
            // roomsListView
            // 
            this.roomsListView.HideSelection = false;
            this.roomsListView.Location = new System.Drawing.Point(30, 101);
            this.roomsListView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.roomsListView.Name = "roomsListView";
            this.roomsListView.Size = new System.Drawing.Size(664, 201);
            this.roomsListView.TabIndex = 7;
            this.roomsListView.UseCompatibleStateImageBehavior = false;
            this.roomsListView.View = System.Windows.Forms.View.Details;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(481, 328);
            this.okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(100, 27);
            this.okButton.TabIndex = 8;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(594, 328);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 27);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // AutoTagRoomsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 375);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.roomsListView);
            this.Controls.Add(this.autoTagButton);
            this.Controls.Add(this.tagTypesComboBox);
            this.Controls.Add(this.levelsComboBox);
            this.Controls.Add(this.tagTypeLabel);
            this.Controls.Add(this.levelLabel);
            this.Name = "AutoTagRoomsForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.AutoTagRoomsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label levelLabel;
        private System.Windows.Forms.Label tagTypeLabel;
        private System.Windows.Forms.ComboBox levelsComboBox;
        private System.Windows.Forms.ComboBox tagTypesComboBox;
        private System.Windows.Forms.Button autoTagButton;
        private System.Windows.Forms.ListView roomsListView;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}