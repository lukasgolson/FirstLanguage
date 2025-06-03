namespace FirstLanguage.abstract_syntax_tree.Nodes;

public interface IBlockNode : IAstNode
{
    public List<IAstNode> Children { get; }
    
    
}