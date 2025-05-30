namespace FirstLanguage.abstract_syntax_tree.Core;

public class ProgramNode : IAstNode
{
    public List<IAstNode> Statements { get; } = [];
}