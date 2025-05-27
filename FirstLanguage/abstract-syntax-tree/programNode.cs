namespace FirstLanguage.abstract_syntax_tree;

public class ProgramNode : IAstNode
{
    public List<IAstNode> Statements { get; } = [];
}