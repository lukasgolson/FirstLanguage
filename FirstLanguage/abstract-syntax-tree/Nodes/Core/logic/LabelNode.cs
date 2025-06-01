namespace FirstLanguage.abstract_syntax_tree.Nodes.Core.logic;

public class LabelNode(string label) : ILabelledNode
{
    public string Label { get; set; } = label;
    public IAstNode? Parent { get; set; }

    public IAstNode Clone()
    {
        return new LabelNode(Label);
    }
}