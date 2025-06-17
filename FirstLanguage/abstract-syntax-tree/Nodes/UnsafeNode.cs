namespace FirstLanguage.abstract_syntax_tree.Nodes;

public class UnsafeNode : IAstNode, IBlockNode
{
    public IAstNode? Parent { get; set; }
    public IAstNode Clone()
    {
        return new UnsafeNode();
    }

    public List<IAstNode> Children { get; } = [];
}