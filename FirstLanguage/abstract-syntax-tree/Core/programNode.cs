namespace FirstLanguage.abstract_syntax_tree.Core;

public class ProgramNode : IBlockNode
{
    public List<IAstNode> Statements { get; } = [];
}