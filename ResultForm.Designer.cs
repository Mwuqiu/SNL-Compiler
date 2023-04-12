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
            this.label1 = new System.Windows.Forms.Label();
            this.RDLabel = new System.Windows.Forms.Label();
            this.LLLabel = new System.Windows.Forms.Label();
            this.SemLabel = new System.Windows.Forms.Label();
            this.SemTable = new System.Windows.Forms.ListView();
            this.LevelHeader = new System.Windows.Forms.ColumnHeader();
            this.NameHeader = new System.Windows.Forms.ColumnHeader();
            this.KindHeader = new System.Windows.Forms.ColumnHeader();
            this.TypeHeader = new System.Windows.Forms.ColumnHeader();
            this.NoffHeader = new System.Windows.Forms.ColumnHeader();
            this.OffsetHeader = new System.Windows.Forms.ColumnHeader();
            this.DirHeader = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // TokenBox
            // 
            this.TokenBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TokenBox.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TokenBox.Location = new System.Drawing.Point(12, 55);
            this.TokenBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
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
            this.SyntaxBoxRD.Location = new System.Drawing.Point(483, 55);
            this.SyntaxBoxRD.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
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
            this.SyntaxBoxLL.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.SyntaxBoxLL.Multiline = true;
            this.SyntaxBoxLL.Name = "SyntaxBoxLL";
            this.SyntaxBoxLL.ReadOnly = true;
            this.SyntaxBoxLL.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.SyntaxBoxLL.Size = new System.Drawing.Size(541, 480);
            this.SyntaxBoxLL.TabIndex = 2;
            this.SyntaxBoxLL.TabStop = false;
            this.SyntaxBoxLL.WordWrap = false;
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
            this.RDLabel.Location = new System.Drawing.Point(483, 9);
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
            this.SemLabel.Location = new System.Drawing.Point(483, 547);
            this.SemLabel.Name = "SemLabel";
            this.SemLabel.Size = new System.Drawing.Size(179, 43);
            this.SemLabel.TabIndex = 7;
            this.SemLabel.Text = "SemTable";
            // 
            // SemTable
            // 
            this.SemTable.BackColor = System.Drawing.SystemColors.Control;
            this.SemTable.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LevelHeader,
            this.NameHeader,
            this.KindHeader,
            this.TypeHeader,
            this.NoffHeader,
            this.OffsetHeader,
            this.DirHeader});
            this.SemTable.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.SemTable.GridLines = true;
            this.SemTable.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.SemTable.Location = new System.Drawing.Point(483, 593);
            this.SemTable.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SemTable.Name = "SemTable";
            this.SemTable.Size = new System.Drawing.Size(1087, 348);
            this.SemTable.TabIndex = 8;
            this.SemTable.TabStop = false;
            this.SemTable.UseCompatibleStateImageBehavior = false;
            this.SemTable.View = System.Windows.Forms.View.Details;
            // 
            // LevelHeader
            // 
            this.LevelHeader.Text = "Level";
            this.LevelHeader.Width = 120;
            // 
            // NameHeader
            // 
            this.NameHeader.Text = "Name";
            this.NameHeader.Width = 120;
            // 
            // KindHeader
            // 
            this.KindHeader.Text = "Kind";
            this.KindHeader.Width = 120;
            // 
            // TypeHeader
            // 
            this.TypeHeader.Text = "Type";
            this.TypeHeader.Width = 120;
            // 
            // NoffHeader
            // 
            this.NoffHeader.Text = "Noff";
            this.NoffHeader.Width = 120;
            // 
            // OffsetHeader
            // 
            this.OffsetHeader.Text = "Offset";
            this.OffsetHeader.Width = 120;
            // 
            // DirHeader
            // 
            this.DirHeader.Text = "Dir";
            this.DirHeader.Width = 120;
            // 
            // ResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1584, 961);
            this.Controls.Add(this.SemTable);
            this.Controls.Add(this.SemLabel);
            this.Controls.Add(this.LLLabel);
            this.Controls.Add(this.RDLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SyntaxBoxLL);
            this.Controls.Add(this.SyntaxBoxRD);
            this.Controls.Add(this.TokenBox);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(1599, 999);
            this.Name = "ResultForm";
            this.Text = "ResultForm";
            this.Load += new System.EventHandler(this.ResultForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox TokenBox;
        private TextBox SyntaxBoxRD;
        private TextBox SyntaxBoxLL;
        private Label label1;
        private Label RDLabel;
        private Label LLLabel;
        private Label SemLabel;
        private ListView SemTable;
        private ColumnHeader LevelHeader;
        private ColumnHeader NameHeader;
        private ColumnHeader KindHeader;
        private ColumnHeader TypeHeader;
        private ColumnHeader NoffHeader;
        private ColumnHeader OffsetHeader;
        private ColumnHeader DirHeader;
    }
}