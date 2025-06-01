namespace FirstLanguage.abstract_syntax_tree.Nodes.Core.logic;

public class JumpzNode(string label) : ILabelledNode
{
    public string Label { get; set; } = label;
    public IAstNode? Parent { get; set; }
    public IAstNode Clone()
    {
        return new JumpzNode(Label);
    }
}