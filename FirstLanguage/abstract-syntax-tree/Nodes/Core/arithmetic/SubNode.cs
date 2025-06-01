namespace FirstLanguage.abstract_syntax_tree.Nodes.Core.arithmetic;

public class SubNode : IAstNode
{
    public IAstNode? Parent { get; set; }
    public IAstNode Clone()
    {
        return new SubNode();
    }
}