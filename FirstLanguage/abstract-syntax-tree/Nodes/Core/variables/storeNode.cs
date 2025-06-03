namespace FirstLanguage.abstract_syntax_tree.Nodes.Core.variables;

public class StoreNode(string label) : ILabelledNode
{
    public string Label { get; set; } = label;
    public IAstNode? Parent { get; set; }
    public IAstNode Clone()
    {
        return new StoreNode(Label);
    }
}