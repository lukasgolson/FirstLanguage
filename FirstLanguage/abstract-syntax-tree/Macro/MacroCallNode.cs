namespace FirstLanguage.abstract_syntax_tree.Macro;

public class MacroCallNode(string label) : ILabelledNode
{
    public string Label { get; } = label;
}