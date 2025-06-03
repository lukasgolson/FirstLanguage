namespace FirstLanguage.abstract_syntax_tree.Nodes.Core.Misc;

public class PrintNode : IAstNode
{
    public IAstNode? Parent { get; set; }
    public IAstNode Clone()
    {
        return new PrintNode();
    }
}