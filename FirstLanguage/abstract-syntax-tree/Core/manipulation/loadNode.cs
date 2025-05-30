namespace FirstLanguage.abstract_syntax_tree.Core.manipulation;

public class LoadNode(string label) : ILabelledNode
{
    public string Label { get; } = label;
}