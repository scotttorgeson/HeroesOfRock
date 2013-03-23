namespace MoveCreator {
    partial class MultipleButtonInput {
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
            this.buttonList = new System.Windows.Forms.ListBox();
            this.cancel_btn = new System.Windows.Forms.Button();
            this.doneBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.yBtn = new System.Windows.Forms.Button();
            this.xBtn = new System.Windows.Forms.Button();
            this.bBtn = new System.Windows.Forms.Button();
            this.aBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonList
            // 
            this.buttonList.FormattingEnabled = true;
            this.buttonList.Location = new System.Drawing.Point(12, 53);
            this.buttonList.Name = "buttonList";
            this.buttonList.Size = new System.Drawing.Size(120, 82);
            this.buttonList.TabIndex = 28;
            this.buttonList.DoubleClick += new System.EventHandler(this.buttonList_DblClick);
            // 
            // cancel_btn
            // 
            this.cancel_btn.BackColor = System.Drawing.Color.White;
            this.cancel_btn.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.cancel_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.cancel_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.cancel_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancel_btn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancel_btn.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.cancel_btn.Location = new System.Drawing.Point(146, 196);
            this.cancel_btn.Name = "cancel_btn";
            this.cancel_btn.Size = new System.Drawing.Size(75, 23);
            this.cancel_btn.TabIndex = 27;
            this.cancel_btn.Text = "Cancel";
            this.cancel_btn.UseVisualStyleBackColor = false;
            this.cancel_btn.Click += new System.EventHandler(this.cancel_btn_Click);
            // 
            // doneBtn
            // 
            this.doneBtn.BackColor = System.Drawing.Color.White;
            this.doneBtn.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.doneBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.doneBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.doneBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.doneBtn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.doneBtn.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.doneBtn.Location = new System.Drawing.Point(44, 196);
            this.doneBtn.Name = "doneBtn";
            this.doneBtn.Size = new System.Drawing.Size(75, 23);
            this.doneBtn.TabIndex = 26;
            this.doneBtn.Text = "Done";
            this.doneBtn.UseVisualStyleBackColor = false;
            this.doneBtn.Click += new System.EventHandler(this.doneBtn_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Teal;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(-1, -2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(287, 39);
            this.panel1.TabIndex = 25;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mutli Button Input";
            // 
            // yBtn
            // 
            this.yBtn.BackColor = System.Drawing.Color.Gold;
            this.yBtn.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.yBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.yBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.yBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.yBtn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yBtn.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.yBtn.Location = new System.Drawing.Point(171, 156);
            this.yBtn.Name = "yBtn";
            this.yBtn.Size = new System.Drawing.Size(75, 23);
            this.yBtn.TabIndex = 24;
            this.yBtn.Text = "Y";
            this.yBtn.UseVisualStyleBackColor = false;
            this.yBtn.Click += new System.EventHandler(this.yBtn_Click);
            // 
            // xBtn
            // 
            this.xBtn.BackColor = System.Drawing.Color.MediumBlue;
            this.xBtn.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.xBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.xBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.xBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.xBtn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xBtn.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.xBtn.Location = new System.Drawing.Point(171, 117);
            this.xBtn.Name = "xBtn";
            this.xBtn.Size = new System.Drawing.Size(75, 23);
            this.xBtn.TabIndex = 23;
            this.xBtn.Text = "X";
            this.xBtn.UseVisualStyleBackColor = false;
            this.xBtn.Click += new System.EventHandler(this.xBtn_Click);
            // 
            // bBtn
            // 
            this.bBtn.BackColor = System.Drawing.Color.Red;
            this.bBtn.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.bBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.bBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.bBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bBtn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bBtn.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.bBtn.Location = new System.Drawing.Point(171, 77);
            this.bBtn.Name = "bBtn";
            this.bBtn.Size = new System.Drawing.Size(75, 23);
            this.bBtn.TabIndex = 22;
            this.bBtn.Text = "B";
            this.bBtn.UseVisualStyleBackColor = false;
            this.bBtn.Click += new System.EventHandler(this.bBtn_Click);
            // 
            // aBtn
            // 
            this.aBtn.BackColor = System.Drawing.Color.LimeGreen;
            this.aBtn.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.aBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.aBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.aBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.aBtn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aBtn.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.aBtn.Location = new System.Drawing.Point(171, 43);
            this.aBtn.Name = "aBtn";
            this.aBtn.Size = new System.Drawing.Size(75, 23);
            this.aBtn.TabIndex = 21;
            this.aBtn.Text = "A";
            this.aBtn.UseVisualStyleBackColor = false;
            this.aBtn.Click += new System.EventHandler(this.aBtn_Click);
            // 
            // MultipleButtonInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(258, 228);
            this.Controls.Add(this.buttonList);
            this.Controls.Add(this.cancel_btn);
            this.Controls.Add(this.doneBtn);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.yBtn);
            this.Controls.Add(this.xBtn);
            this.Controls.Add(this.bBtn);
            this.Controls.Add(this.aBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MultipleButtonInput";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MultipleButtonInput";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox buttonList;
        private System.Windows.Forms.Button cancel_btn;
        private System.Windows.Forms.Button doneBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button yBtn;
        private System.Windows.Forms.Button xBtn;
        private System.Windows.Forms.Button bBtn;
        private System.Windows.Forms.Button aBtn;
    }
}