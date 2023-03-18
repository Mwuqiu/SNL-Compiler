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
            SyntaxAnalysiser s = new SyntaxAnalysiser(scanner.tokenList);
            SyntaxClass.SyntaxTreeNode? syntaxTreeNode = s.Parse();
            if(syntaxTreeNode != null) syntaxTreeNode.PrintTree(0);
            else {
                List<string> errors = s.errorList;
                foreach(string error in errors) {
                    Console.WriteLine(error);
                }
            }
        }
    }
}