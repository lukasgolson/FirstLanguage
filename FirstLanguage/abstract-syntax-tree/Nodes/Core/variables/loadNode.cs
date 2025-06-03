namespace FirstLanguage.abstract_syntax_tree.Nodes.Core.variables;

public class LoadNode(string label) : ILabelledNode
{
    public string Label { get; set; } = label;
    public IAstNode? Parent { get; set; }
    public IAstNode Clone()
    {
        return new LoadNode(Label);
    }
}