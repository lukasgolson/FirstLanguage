namespace FirstLanguage.abstract_syntax_tree.Core.manipulation;

public class PushNode(long value) : IAstNode
{
    public long Value { get; init; } = value;
    public override string ToString()
    {
        var name = "Push: " + Value;
        return name;
    }
}