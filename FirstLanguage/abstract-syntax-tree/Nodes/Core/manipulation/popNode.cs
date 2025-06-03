namespace FirstLanguage.abstract_syntax_tree.Nodes.Core.manipulation;

public class PopNode : IAstNode
{
    public IAstNode? Parent { get; set; }
    public IAstNode Clone()
    {
        return new PopNode();
    }
}