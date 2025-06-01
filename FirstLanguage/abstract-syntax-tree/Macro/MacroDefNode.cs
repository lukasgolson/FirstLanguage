namespace FirstLanguage.abstract_syntax_tree.Macro;

public class MacroDefNode(string label, string[] arguments) : ILabelledNode, IBlockNode
{
    public string Label { get; } = label;
    public string[] Arguments { get; } = arguments;
    public List<IAstNode> Statements { get; } = [];
}