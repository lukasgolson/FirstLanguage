namespace FirstLanguage.abstract_syntax_tree.Core.Misc;

public class LabelNode(string label) : ILabelledNode
{
    public string Label { get; } = label;
}