namespace CompilationPrinciple
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LogoLabel = new System.Windows.Forms.Label();
            this.CodeBox = new System.Windows.Forms.TextBox();
            this.MessageBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.LineLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LogoLabel
            // 
            this.LogoLabel.AutoSize = true;
            this.LogoLabel.Font = new System.Drawing.Font("Segoe Print", 25.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LogoLabel.Location = new System.Drawing.Point(278, 9);
            this.LogoLabel.Name = "LogoLabel";
            this.LogoLabel.Size = new System.Drawing.Size(332, 75);
            this.LogoLabel.TabIndex = 0;
            this.LogoLabel.Text = "SNL Compiler";
            // 
            // CodeBox
            // 
            this.CodeBox.AcceptsTab = true;
            this.CodeBox.BackColor = System.Drawing.SystemColors.Control;
            this.CodeBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CodeBox.Font = new System.Drawing.Font("Consolas", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CodeBox.Location = new System.Drawing.Point(0, 89);
            this.CodeBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CodeBox.Multiline = true;
            this.CodeBox.Name = "CodeBox";
            this.CodeBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.CodeBox.Size = new System.Drawing.Size(965, 761);
            this.CodeBox.TabIndex = 1;
            this.CodeBox.TabStop = false;
            this.CodeBox.WordWrap = false;
            this.CodeBox.TextChanged += new System.EventHandler(this.CodeBox_TextChanged);
            // 
            // MessageBox
            // 
            this.MessageBox.AcceptsTab = true;
            this.MessageBox.BackColor = System.Drawing.SystemColors.Control;
            this.MessageBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MessageBox.Font = new System.Drawing.Font("Consolas", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.MessageBox.ForeColor = System.Drawing.Color.Firebrick;
            this.MessageBox.Location = new System.Drawing.Point(966, 89);
            this.MessageBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MessageBox.Multiline = true;
            this.MessageBox.Name = "MessageBox";
            this.MessageBox.ReadOnly = true;
            this.MessageBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.MessageBox.Size = new System.Drawing.Size(404, 761);
            this.MessageBox.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft YaHei UI", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.button1.Location = new System.Drawing.Point(988, 12);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(168, 65);
            this.button1.TabIndex = 3;
            this.button1.Text = "Analysis";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft YaHei UI", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.button2.Location = new System.Drawing.Point(1180, 9);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(168, 65);
            this.button2.TabIndex = 4;
            this.button2.Text = "Format";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // comboBox
            // 
            this.comboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox.Font = new System.Drawing.Font("Consolas", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.comboBox.FormattingEnabled = true;
            this.comboBox.ItemHeight = 59;
            this.comboBox.Location = new System.Drawing.Point(764, 12);
            this.comboBox.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox.MaxDropDownItems = 5;
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(195, 65);
            this.comboBox.TabIndex = 5;
            this.comboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelectedIndexChanged);
            // 
            // LineLabel
            // 
            this.LineLabel.AutoSize = true;
            this.LineLabel.Location = new System.Drawing.Point(0, 65);
            this.LineLabel.Name = "LineLabel";
            this.LineLabel.Size = new System.Drawing.Size(129, 20);
            this.LineLabel.TabIndex = 6;
            this.LineLabel.Text = "当前光标所在行：";
            this.LineLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1383, 861);
            this.Controls.Add(this.LineLabel);
            this.Controls.Add(this.comboBox);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.MessageBox);
            this.Controls.Add(this.CodeBox);
            this.Controls.Add(this.LogoLabel);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(1399, 899);
            this.Name = "Form1";
            this.Text = "SNL Compiler";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        
        private Label LogoLabel;
        private TextBox CodeBox;
        private TextBox MessageBox;
        private Button button1;
        private Button button2;
        private ComboBox comboBox;
        private Label LineLabel;
    }
}