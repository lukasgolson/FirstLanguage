namespace FirstLanguage.abstract_syntax_tree.Nodes.Core.manipulation;

public class PopNode(string? label) : ILabelledNode
{
    public IAstNode? Parent { get; set; }
    public IAstNode Clone()
    {
        return new PopNode(Label);
    }

    public string Label { get; set; } = label ?? "main";
}