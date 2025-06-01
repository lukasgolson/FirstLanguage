namespace FirstLanguage.abstract_syntax_tree.Macro;

public class MacroCallNode(string label, string[] arguments) : ILabelledNode
{



    public string[] Arguments { get; } = arguments;

    public string Label { get; } = label;
}