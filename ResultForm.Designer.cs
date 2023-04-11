namespace CompilationPrinciple {
    partial class ResultForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
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
        private void InitializeComponent() {
            this.TokenBox = new System.Windows.Forms.TextBox();
            this.SyntaxBoxRD = new System.Windows.Forms.TextBox();
            this.SyntaxBoxLL = new System.Windows.Forms.TextBox();
            this.SemTable = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.RDLabel = new System.Windows.Forms.Label();
            this.LLLabel = new System.Windows.Forms.Label();
            this.SemLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TokenBox
            // 
            this.TokenBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TokenBox.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TokenBox.Location = new System.Drawing.Point(12, 55);
            this.TokenBox.Multiline = true;
            this.TokenBox.Name = "TokenBox";
            this.TokenBox.ReadOnly = true;
            this.TokenBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TokenBox.Size = new System.Drawing.Size(466, 886);
            this.TokenBox.TabIndex = 0;
            this.TokenBox.TabStop = false;
            this.TokenBox.WordWrap = false;
            // 
            // SyntaxBoxRD
            // 
            this.SyntaxBoxRD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SyntaxBoxRD.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.SyntaxBoxRD.Location = new System.Drawing.Point(484, 55);
            this.SyntaxBoxRD.Multiline = true;
            this.SyntaxBoxRD.Name = "SyntaxBoxRD";
            this.SyntaxBoxRD.ReadOnly = true;
            this.SyntaxBoxRD.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.SyntaxBoxRD.Size = new System.Drawing.Size(541, 480);
            this.SyntaxBoxRD.TabIndex = 1;
            this.SyntaxBoxRD.TabStop = false;
            this.SyntaxBoxRD.WordWrap = false;
            // 
            // SyntaxBoxLL
            // 
            this.SyntaxBoxLL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SyntaxBoxLL.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.SyntaxBoxLL.Location = new System.Drawing.Point(1030, 55);
            this.SyntaxBoxLL.Multiline = true;
            this.SyntaxBoxLL.Name = "SyntaxBoxLL";
            this.SyntaxBoxLL.ReadOnly = true;
            this.SyntaxBoxLL.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.SyntaxBoxLL.Size = new System.Drawing.Size(541, 480);
            this.SyntaxBoxLL.TabIndex = 2;
            this.SyntaxBoxLL.TabStop = false;
            this.SyntaxBoxLL.WordWrap = false;
            // 
            // SemTable
            // 
            this.SemTable.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SemTable.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.SemTable.Location = new System.Drawing.Point(484, 593);
            this.SemTable.Multiline = true;
            this.SemTable.Name = "SemTable";
            this.SemTable.ReadOnly = true;
            this.SemTable.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.SemTable.Size = new System.Drawing.Size(1087, 348);
            this.SemTable.TabIndex = 3;
            this.SemTable.TabStop = false;
            this.SemTable.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 22.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(199, 43);
            this.label1.TabIndex = 4;
            this.label1.Text = "TokenList";
            // 
            // RDLabel
            // 
            this.RDLabel.AutoSize = true;
            this.RDLabel.Font = new System.Drawing.Font("Consolas", 22.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.RDLabel.Location = new System.Drawing.Point(484, 9);
            this.RDLabel.Name = "RDLabel";
            this.RDLabel.Size = new System.Drawing.Size(479, 43);
            this.RDLabel.TabIndex = 5;
            this.RDLabel.Text = "SyntaxTree [ 递归下降 ]";
            // 
            // LLLabel
            // 
            this.LLLabel.AutoSize = true;
            this.LLLabel.Font = new System.Drawing.Font("Consolas", 22.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LLLabel.Location = new System.Drawing.Point(1030, 9);
            this.LLLabel.Name = "LLLabel";
            this.LLLabel.Size = new System.Drawing.Size(419, 43);
            this.LLLabel.TabIndex = 6;
            this.LLLabel.Text = "SyntaxTree [ LL(1) ]";
            // 
            // SemLabel
            // 
            this.SemLabel.AutoSize = true;
            this.SemLabel.Font = new System.Drawing.Font("Consolas", 22.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.SemLabel.Location = new System.Drawing.Point(484, 547);
            this.SemLabel.Name = "SemLabel";
            this.SemLabel.Size = new System.Drawing.Size(179, 43);
            this.SemLabel.TabIndex = 7;
            this.SemLabel.Text = "SemTable";
            // 
            // ResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1582, 953);
            this.Controls.Add(this.SemLabel);
            this.Controls.Add(this.LLLabel);
            this.Controls.Add(this.RDLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SemTable);
            this.Controls.Add(this.SyntaxBoxLL);
            this.Controls.Add(this.SyntaxBoxRD);
            this.Controls.Add(this.TokenBox);
            this.MinimumSize = new System.Drawing.Size(1600, 1000);
            this.Name = "ResultForm";
            this.Text = "ResultForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox TokenBox;
        private TextBox SyntaxBoxRD;
        private TextBox SyntaxBoxLL;
        private TextBox SemTable;
        private Label label1;
        private Label RDLabel;
        private Label LLLabel;
        private Label SemLabel;
    }
}