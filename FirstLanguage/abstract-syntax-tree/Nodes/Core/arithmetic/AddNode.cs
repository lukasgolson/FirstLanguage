namespace FirstLanguage.abstract_syntax_tree.Nodes.Core.arithmetic;

public class AddNode : IAstNode
{
    public IAstNode? Parent { get; set; }
    public IAstNode Clone()
    {
        return new AddNode();
    }
}