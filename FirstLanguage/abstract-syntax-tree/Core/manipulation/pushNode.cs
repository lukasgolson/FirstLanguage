namespace FirstLanguage.abstract_syntax_tree.Core.manipulation;

public class PushNode(long value) : IAstNode
{
    public byte Value { get; init; } = (byte) value;
    public override string ToString()
    {
        var name = "Push: " + value;
        return name;
    }
}