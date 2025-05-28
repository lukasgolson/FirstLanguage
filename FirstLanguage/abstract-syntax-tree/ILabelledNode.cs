namespace FirstLanguage.abstract_syntax_tree;

public interface ILabelledNode : IAstNode
{
    public string Label { get; }
}