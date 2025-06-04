using FirstLanguage.abstract_syntax_tree.Nodes;

namespace FirstLanguage.abstract_syntax_tree.Core.logic;

public class GTNode : IAstNode
{
    public IAstNode? Parent { get; set; }
    public IAstNode Clone()
    {
        return new GTNode();
    }
}