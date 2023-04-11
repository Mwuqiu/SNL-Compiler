using System.Windows.Forms;

namespace CompilationPrinciple
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CodeBox.KeyPress += Codebox_KeyPress;
            CodeBox.Text = Properties.Resources.bubble.Replace("\t", new string(' ', 4));
            On_form_resize(null, null);
            Resize += On_form_resize;
        }
        void Codebox_KeyPress(object? sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char)Keys.Tab) {
                e.Handled = true;
                CodeBox.SelectedText = new string(' ', 4);
            }
        }
        private void On_form_resize(object? sender, EventArgs? e) {
            CodeBox.Width = (int)(0.65 * Width);
            MessageBox.Location = new Point(CodeBox.Width, MessageBox.Location.Y);
            MessageBox.Width = Width - CodeBox.Width - 30;
            CodeBox.Height = Height - CodeBox.Location.Y - 50;
            MessageBox.Height = CodeBox.Height;
            button1.Location = new Point(MessageBox.Location.X, button1.Location.Y);
            button2.Location = new Point(button1.Location.X + button1.Width + 30, button1.Location.Y);
            //Console.WriteLine("!!");
        }
        private void button1_Click(object sender, EventArgs e) {
            Console.Write(CodeBox.Text);
            Scanner scanner = new Scanner(CodeBox.Text);
            scanner.DirectSteeringMethod();
            String tokenList = scanner.outPutTokenList();

            SyntaxAnalysiser s = new SyntaxAnalysiser(scanner.tokenList);
            SyntaxClass.SyntaxTreeNode? syntaxTreeNode = s.Parse();
            String syntaxTreeRD = "";
            if (syntaxTreeNode != null)
                syntaxTreeRD = syntaxTreeNode.PrintTree(0);

            String syntaxTreeLL = "";
            LL1SyntaxAnalysis ll1 = new LL1SyntaxAnalysis(scanner.tokenList);
            ll1.parse();
            syntaxTreeLL = ll1.root.PrintTree(0);

            ResultForm resultForm = new ResultForm(tokenList, syntaxTreeRD, syntaxTreeLL);
            resultForm.Show();
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
    }
}