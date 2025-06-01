namespace FirstLanguage.abstract_syntax_tree;

public interface IBlockNode : IAstNode
{
    public List<IAstNode> Statements { get; }
}