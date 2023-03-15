namespace CompilationPrinciple {
    internal static class Program {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            /*            ApplicationConfiguration.Initialize();
                        Application.Run(new Form1());*/
            Scanner scanner = new Scanner();
            scanner.DirectSteeringMethod();
            scanner.outPutTokenList();
            //SyntaxAnalysiser s = new SyntaxAnalysiser(scanner.tokenList);

            LL1SyntaxAnalysis ll1 = new LL1SyntaxAnalysis(scanner.tokenList);
            ll1.parse();

            //SyntaxClass.SyntaxTreeNode? syntaxTreeNode = s.Parse();

            //if(syntaxTreeNode != null) syntaxTreeNode.PrintTree(0);
        }
    }
}