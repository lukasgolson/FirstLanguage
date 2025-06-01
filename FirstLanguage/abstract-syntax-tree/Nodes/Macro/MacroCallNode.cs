namespace FirstLanguage.abstract_syntax_tree.Nodes.Macro;

public class MacroCallNode : ILabelledNode
{
    public MacroCallNode(string label, string[] arguments)
    {
        Arguments = arguments;
        Label = label;
    }

    public string[] Arguments { get; }

    public string Label { get; set; }
    public IAstNode? Parent { get; set; }
    public IAstNode Clone()
    {
        return new MacroCallNode(Label, Arguments);
    }
}