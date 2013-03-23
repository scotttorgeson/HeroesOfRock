namespace MoveCreator {
    partial class MoveCreatorForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent () {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MoveCreatorForm));
            this.moveListContainer = new System.Windows.Forms.Panel();
            this.statusList = new System.Windows.Forms.ListBox();
            this.nameList = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.statusSorter = new System.Windows.Forms.LinkLabel();
            this.nameSorter = new System.Windows.Forms.LinkLabel();
            this.newMoveBtn = new System.Windows.Forms.Button();
            this.saveExit_btn = new System.Windows.Forms.Button();
            this.moveListContainer.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // moveListContainer
            // 
            this.moveListContainer.AutoScroll = true;
            this.moveListContainer.BackColor = System.Drawing.Color.White;
            this.moveListContainer.Controls.Add(this.statusList);
            this.moveListContainer.Controls.Add(this.nameList);
            this.moveListContainer.Location = new System.Drawing.Point(6, 60);
            this.moveListContainer.Name = "moveListContainer";
            this.moveListContainer.Size = new System.Drawing.Size(472, 392);
            this.moveListContainer.TabIndex = 0;
            // 
            // statusList
            // 
            this.statusList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.statusList.Enabled = false;
            this.statusList.FormattingEnabled = true;
            this.statusList.Location = new System.Drawing.Point(264, 4);
            this.statusList.Name = "statusList";
            this.statusList.Size = new System.Drawing.Size(202, 377);
            this.statusList.TabIndex = 1;
            // 
            // nameList
            // 
            this.nameList.FormattingEnabled = true;
            this.nameList.Location = new System.Drawing.Point(6, 3);
            this.nameList.Name = "nameList";
            this.nameList.Size = new System.Drawing.Size(460, 381);
            this.nameList.TabIndex = 0;
            this.nameList.DoubleClick += new System.EventHandler(this.listBox_DoubleClick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Teal;
            this.panel1.Controls.Add(this.saveExit_btn);
            this.panel1.Controls.Add(this.statusSorter);
            this.panel1.Controls.Add(this.nameSorter);
            this.panel1.Controls.Add(this.newMoveBtn);
            this.panel1.Location = new System.Drawing.Point(1, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(485, 60);
            this.panel1.TabIndex = 1;
            // 
            // statusSorter
            // 
            this.statusSorter.AutoSize = true;
            this.statusSorter.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusSorter.LinkColor = System.Drawing.Color.White;
            this.statusSorter.Location = new System.Drawing.Point(266, 40);
            this.statusSorter.Name = "statusSorter";
            this.statusSorter.Size = new System.Drawing.Size(42, 14);
            this.statusSorter.TabIndex = 2;
            this.statusSorter.TabStop = true;
            this.statusSorter.Text = "Status";
            this.statusSorter.VisitedLinkColor = System.Drawing.Color.White;
            // 
            // nameSorter
            // 
            this.nameSorter.AutoSize = true;
            this.nameSorter.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameSorter.LinkColor = System.Drawing.Color.White;
            this.nameSorter.Location = new System.Drawing.Point(31, 40);
            this.nameSorter.Name = "nameSorter";
            this.nameSorter.Size = new System.Drawing.Size(38, 14);
            this.nameSorter.TabIndex = 1;
            this.nameSorter.TabStop = true;
            this.nameSorter.Text = "Name";
            this.nameSorter.VisitedLinkColor = System.Drawing.Color.White;
            // 
            // newMoveBtn
            // 
            this.newMoveBtn.BackColor = System.Drawing.Color.White;
            this.newMoveBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.newMoveBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.newMoveBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.newMoveBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.newMoveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.newMoveBtn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newMoveBtn.Location = new System.Drawing.Point(402, 31);
            this.newMoveBtn.Name = "newMoveBtn";
            this.newMoveBtn.Size = new System.Drawing.Size(75, 23);
            this.newMoveBtn.TabIndex = 0;
            this.newMoveBtn.Text = "New Move";
            this.newMoveBtn.UseVisualStyleBackColor = false;
            this.newMoveBtn.Click += new System.EventHandler(this.newMoveBtn_Click);
            // 
            // saveExit_btn
            // 
            this.saveExit_btn.BackColor = System.Drawing.Color.Teal;
            this.saveExit_btn.CausesValidation = false;
            this.saveExit_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.saveExit_btn.FlatAppearance.BorderColor = System.Drawing.Color.Teal;
            this.saveExit_btn.FlatAppearance.BorderSize = 0;
            this.saveExit_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DimGray;
            this.saveExit_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkCyan;
            this.saveExit_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveExit_btn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveExit_btn.ForeColor = System.Drawing.Color.White;
            this.saveExit_btn.Location = new System.Drawing.Point(391, 5);
            this.saveExit_btn.Name = "saveExit_btn";
            this.saveExit_btn.Size = new System.Drawing.Size(86, 20);
            this.saveExit_btn.TabIndex = 3;
            this.saveExit_btn.Text = "Save and Exit";
            this.saveExit_btn.UseVisualStyleBackColor = false;
            this.saveExit_btn.Click += new System.EventHandler(this.saveExit_btn_Click);
            // 
            // MoveCreatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(484, 462);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.moveListContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "MoveCreatorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Move Creator";
            this.TopMost = true;
            this.moveListContainer.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel moveListContainer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button newMoveBtn;
        private System.Windows.Forms.LinkLabel statusSorter;
        private System.Windows.Forms.LinkLabel nameSorter;
        private System.Windows.Forms.ListBox nameList;
        private System.Windows.Forms.ListBox statusList;
        private System.Windows.Forms.Button saveExit_btn;
    }
}

