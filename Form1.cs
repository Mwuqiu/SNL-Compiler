using System.Reflection;
using System.Windows.Forms;
using Windows.ApplicationModel.Resources.Core;

namespace CompilationPrinciple {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            CodeBox.KeyPress += Codebox_KeyPress;
            CodeBox.KeyUp += CodeBox_KeyUp;
            CodeBox.MouseDown += CodeBox_MouseDown;
            CodeBox.Text = Properties.Resources.bubble.Replace("\t", new string(' ', 4));
            comboBox.DrawItem += comboBox_DrawItem;
            List<string> examples = new List<string> {
                "bubble.txt", "C1.TXT", "C2.TXT",  "C6.TXT", "C7.TXT",
                "CONST.TXT", "ECCEXAM1.TXT","EXP.TXT","LOOP2.TXT",
                "LOOP3.TXT", "SCAN.TXT"
            };
            foreach (string example in examples) {
                comboBox.Items.Add(example);
            }
            comboBox.SelectedIndex = 0;
            On_form_resize(null, null);
            Resize += On_form_resize;

        }
        void comboBox_DrawItem(object? sender, DrawItemEventArgs e) {
            float size = comboBox.ItemHeight;

            // Draw the background of the item.
            e.DrawBackground();

            // Draw each string in the items
            if (e.Index >= 0)
                e.Graphics.DrawString(comboBox.Items[e.Index].ToString(),
            this.comboBox.Font,
            System.Drawing.Brushes.Black,
            new RectangleF(e.Bounds.X, e.Bounds.Y + 15, e.Bounds.Width, e.Bounds.Height));

            // Draw the focus rectangle if the mouse hovers over an item.
            e.DrawFocusRectangle();

        }
        protected override bool ProcessDialogKey(Keys keycode) {
            switch (keycode) {
                case Keys.Left:
                case Keys.Up:
                case Keys.Right:
                case Keys.Down:
                case Keys.Enter:
                    return false;
            }
            return true;
        }
        void CodeBox_KeyUp(object? sender, KeyEventArgs e) {
            CodeBox_MouseDown(null, null);
        }
        void Codebox_KeyPress(object? sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char)Keys.Tab) {
                e.Handled = true;
                CodeBox.SelectedText = new string(' ', 4);
            }
            CodeBox_MouseDown(null, null);
        }
        private void On_form_resize(object? sender, EventArgs? e) {
            CodeBox.Width = (int)(0.55 * Width);
            MessageBox.Location = new Point(CodeBox.Width, MessageBox.Location.Y);
            MessageBox.Width = Width - CodeBox.Width - 30;
            CodeBox.Height = Height - CodeBox.Location.Y - 50;
            MessageBox.Height = CodeBox.Height;
            comboBox.ItemHeight = button1.Height - 6;
            comboBox.Location = new Point(MessageBox.Location.X, comboBox.Location.Y);
            button1.Location = new Point(comboBox.Location.X + comboBox.Width + 30, comboBox.Location.Y);
            button2.Location = new Point(button1.Location.X + button1.Width + 30, button1.Location.Y);
            //Console.WriteLine("!!");
        }
        private void button1_Click(object sender, EventArgs e) {
            try {
                Console.Write(CodeBox.Text);
                Scanner scanner = new Scanner(CodeBox.Text);
                scanner.DirectSteeringMethod();
                String errStr = "";
                if (scanner.getWorng) {
                    errStr += "[LEXICAL ERROR!] \r\n";
                    foreach (String str in scanner.errorList) {
                        errStr += str + "\r\n";
                    }
                    MessageBox.Text = errStr;
                    return;
                }

                errStr += "[LEXICAL NO ERROR.] \r\n\r\n";

                String tokenList = scanner.outPutTokenList();

                SyntaxAnalysiser s = new SyntaxAnalysiser(scanner.tokenList);
                SyntaxClass.SyntaxTreeNode? syntaxTreeNode = s.Parse();
                String syntaxTreeRD = "";
                if (syntaxTreeNode != null) {
                    errStr += "[SYNTAX NO ERROR.] \r\n\r\n";
                    syntaxTreeRD = syntaxTreeNode.PrintTree(0).Replace("\t", new string(' ', 4));
                } else {
                    errStr += "[SYNTAX ERROR!] \r\n";
                    foreach (string str in s.errorList) {
                        errStr += str + "\r\n";
                    }
                    MessageBox.Text = errStr;
                    return;
                }

                String syntaxTreeLL = "";
                LL1SyntaxAnalysis ll1 = new LL1SyntaxAnalysis(scanner.tokenList);
                ll1.parse();
                syntaxTreeLL = ll1.root.PrintTree(0).Replace("\t", new string(' ', 4));



                SemanticAnalysiser semanticAnalysiser = new SemanticAnalysiser();
                semanticAnalysiser.analyze(ll1.root);
                List<List<String>> symbTable = semanticAnalysiser.PrintSymbTable();

                if (semanticAnalysiser.errorList.Count != 0) {
                    errStr += "[SEMANTIC ERROR!]\r\n";
                    foreach (String str in semanticAnalysiser.errorList) {
                        errStr += str + "\r\n";
                    }
                    MessageBox.Text = errStr;
                    return;
                }

                errStr += "[SEMANTIC NO ERROR.]";
                MessageBox.Text = errStr;

                ResultForm resultForm = new ResultForm(tokenList, syntaxTreeRD, syntaxTreeLL, symbTable);
                resultForm.Show();
            } catch (Exception ex) {

            }
            
        }

        private void button2_Click(object sender, EventArgs e) {

        }

        private void CodeBox_TextChanged(object sender, EventArgs e) {

        }

        private void button2_Click_1(object sender, EventArgs e) {
            Console.Write(CodeBox.Text);
            Scanner scanner = new Scanner(CodeBox.Text);
            scanner.DirectSteeringMethod();
            String tokenList = scanner.outPutTokenList();

            SyntaxAnalysiser s = new SyntaxAnalysiser(scanner.tokenList);
            SyntaxClass.SyntaxTreeNode? syntaxTreeNode = s.Parse();
            if (syntaxTreeNode != null) {
                String formatCode = syntaxTreeNode.GenerateCode(0, null);
                CodeBox.Text = formatCode;
            }

        }

        private void Form1_Load(object sender, EventArgs e) {

        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e) {
            Console.WriteLine(comboBox.Text);
            string txt = Properties.Resources.ResourceManager.GetObject(comboBox.Text.Split('.')[0]) as string;
            if (txt != null) {
                txt = txt.Replace("\t", new string(' ', 4));
                CodeBox.Text = txt;
            }

        }
        private void CodeBox_MouseDown(object? sender, object? e) {
            /*int cur = CodeBox.SelectionStart;
            string str = CodeBox.Text.Substring(0, cur).Split("\r\n").Last();
            int len = 0;
            while (len < str.Length && str[len] == ' ' )
                len++;
            if (len == str.Length)
                len = 0;
            CodeBox.Text = CodeBox.Text.Substring(0, cur) + new string(' ', len) + CodeBox.Text.Substring(cur);
            CodeBox.SelectionStart = CodeBox.Text.Length;*/
            int cur = CodeBox.SelectionStart;
            int line = (CodeBox.Text + " ").Substring(0, cur).Split("\n").Length;
            LineLabel.Text = "当前光标所在行：" + line;
            //Console.WriteLine(line);
        }

        private void label1_Click(object sender, EventArgs e) {

        }
    }
}