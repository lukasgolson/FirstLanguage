using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using DotMake.CommandLine;
using FirstLanguage;
using FirstLanguage.abstract_syntax_tree;
using FirstLanguage.abstract_syntax_tree.Nodes.Core;
using FirstLanguage.virtual_machine;

// Add this single line to run you app!
Cli.Run<RootCliCommand>(args);

// Create a simple class like this to define your root command:
[CliCommand(Description = "Compiles and runs an EduLang file.")]
public class RootCliCommand
{
    [CliOption(Description = "File to compile.")]
    public string File { get; set; }


    public static void ProcessFile(string filePath)
    {
        // 1. Load the file content
        AntlrInputStream inputStream;
        using (var fs = new FileStream(filePath, FileMode.Open))
        {
            inputStream = new AntlrInputStream(fs);
        }

        EduLangLexer lexer = new EduLangLexer(inputStream);

        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);

        EduLangParser parser = new EduLangParser(commonTokenStream);

        IParseTree tree = parser.program();

        MainVisitor mainVisitor = new MainVisitor();

        var result = (ProgramNode) mainVisitor.Visit(tree);
        
        var crawler = new Crawler();
        var program = crawler.ResolveMacros(result);

        var vm = new VmContext(program);

        try
        {
            vm.Execute();
        }
        catch (VMException e)
        {
            Console.WriteLine(e);
        }
        catch (CompilerException e)
        {
            Console.WriteLine(e);
        }


        //Console.WriteLine("Parsing and visiting complete.");
        // System.Console.WriteLine($"Result: {result}"); // If your visitor returns something
    }

    public void Run()
    {
        try
        {
            ProcessFile(File);
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine($"File {File} not found.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}