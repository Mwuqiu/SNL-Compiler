using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompilationPrinciple {
    public partial class ResultForm : Form {
        public ResultForm(String tokenList, String syntaxTreeRD, String syntaxTreeLL, List<List<String>> symbTable) {
            InitializeComponent();
            TokenBox.Text = tokenList;
            Resize += on_form_resize;
            SyntaxBoxRD.Text = syntaxTreeRD.Replace("\t", new string(' ', 4));
            SyntaxBoxLL.Text = syntaxTreeLL.Replace("\t", new string(' ', 4));

            for (int i = 0; i < symbTable.Count; i++) {
                List<String> line = symbTable[i];
                ListViewItem item = new ListViewItem(line[0], 0);
                item.Checked = true;
                for(int j = 1; j < 7; j++) {
                    if (line[j] != null) {
                        item.SubItems.Add(line[j]);
                    } else {
                        item.SubItems.Add("");
                    }
                }
                SemTable.Items.Add(item);
            }

            on_form_resize(null, null);
        }

        private void on_form_resize(object? sender, EventArgs? e) {
            TokenBox.Height = Height - TokenBox.Location.Y - 60;
            SyntaxBoxRD.Width = (Width - SyntaxBoxRD.Location.X - 35) / 2;
            SyntaxBoxLL.Width = SyntaxBoxRD.Width;
            SyntaxBoxLL.Location = new Point(SyntaxBoxRD.Location.X + SyntaxBoxRD.Width + 5, SyntaxBoxLL.Location.Y);
            LLLabel.Location = new Point(SyntaxBoxLL.Location.X, LLLabel.Location.Y);

            SyntaxBoxRD.Height = (int)(Height * 0.6);
            SyntaxBoxLL.Height = SyntaxBoxRD.Height;

            SemLabel.Location = new Point(SemLabel.Location.X, SyntaxBoxRD.Location.Y + SyntaxBoxRD.Height);

            SemTable.Location = new Point(SemTable.Location.X, SemLabel.Location.Y + SemLabel.Height);
            SemTable.Width = SyntaxBoxLL.Location.X + SyntaxBoxLL.Width - SemTable.Location.X;
            SemTable.Height = Height - SemTable.Location.Y - 60;

            if (Math.Abs(SemTable.Columns[0].Width - (SemTable.Width - 30) / 7) < 15)
                return;
            foreach (ColumnHeader column in SemTable.Columns) {
                column.Width = (SemTable.Width - 30) / 7;

            }
        }

        private void ResultForm_Load(object sender, EventArgs e) {

        }
    }
}
