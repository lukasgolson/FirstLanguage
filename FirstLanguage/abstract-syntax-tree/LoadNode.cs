namespace FirstLanguage.abstract_syntax_tree;

public class LoadNode(string label) : ILabelledNode
{
    public string Label { get; } = label;
}