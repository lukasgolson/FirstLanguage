namespace FirstLanguage.abstract_syntax_tree.Core.logic;

public class LabelNode(string label) : ILabelledNode
{
    public string Label { get; } = label;
}