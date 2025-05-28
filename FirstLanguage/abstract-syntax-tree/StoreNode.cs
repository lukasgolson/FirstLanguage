namespace FirstLanguage.abstract_syntax_tree;

public class StoreNode(string label) : ILabelledNode
{
    public string Label { get; } = label;
}