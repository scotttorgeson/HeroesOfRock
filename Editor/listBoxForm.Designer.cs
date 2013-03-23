namespace Editor
{
    partial class listBoxForm
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
            this.listBox = new System.Windows.Forms.ListBox();
            this.listboxButtonSave = new System.Windows.Forms.Button();
            this.listboxButtonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(0, 0);
            this.listBox.Name = "listBox";
            this.listBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBox.Size = new System.Drawing.Size(284, 225);
            this.listBox.TabIndex = 0;
            // 
            // listboxButtonSave
            // 
            this.listboxButtonSave.Location = new System.Drawing.Point(12, 231);
            this.listboxButtonSave.Name = "listboxButtonSave";
            this.listboxButtonSave.Size = new System.Drawing.Size(75, 23);
            this.listboxButtonSave.TabIndex = 1;
            this.listboxButtonSave.Text = "Save";
            this.listboxButtonSave.UseVisualStyleBackColor = true;
            this.listboxButtonSave.Click += new System.EventHandler(this.listboxButtonSave_Click);
            // 
            // listboxButtonCancel
            // 
            this.listboxButtonCancel.Location = new System.Drawing.Point(197, 231);
            this.listboxButtonCancel.Name = "listboxButtonCancel";
            this.listboxButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.listboxButtonCancel.TabIndex = 2;
            this.listboxButtonCancel.Text = "Close";
            this.listboxButtonCancel.UseVisualStyleBackColor = true;
            this.listboxButtonCancel.Click += new System.EventHandler(this.listboxButtonCancel_Click);
            // 
            // listBoxForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.listboxButtonCancel);
            this.Controls.Add(this.listboxButtonSave);
            this.Controls.Add(this.listBox);
            this.Name = "listBoxForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "listBoxForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.listBoxForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Button listboxButtonSave;
        private System.Windows.Forms.Button listboxButtonCancel;
    }
}