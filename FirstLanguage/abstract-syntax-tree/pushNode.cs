namespace FirstLanguage.abstract_syntax_tree;

public class PushNode(long value) : IAstNode
{
    public override string ToString()
    {
        var name = "Push: " + value;
        return name;
    }
}