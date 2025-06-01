namespace FirstLanguage.abstract_syntax_tree.Nodes;

public interface ILabelledNode : IAstNode
{
    public string Label { get; set; }
}