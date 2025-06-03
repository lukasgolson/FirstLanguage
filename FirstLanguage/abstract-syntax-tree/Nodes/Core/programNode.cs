namespace FirstLanguage.abstract_syntax_tree.Nodes.Core;

public class ProgramNode : IBlockNode
{
    public List<IAstNode> Children { get; } = [];

    public IAstNode? Parent
    {
        get => null;
        set => throw new InvalidOperationException("ProgramNode parent cannot be set.");
    }

    public ProgramNode()
    {
        
    }

    private ProgramNode(ProgramNode programNode)
    {
        foreach (var clonedChild in programNode.Children.Select(child => child.Clone()))
        {
            clonedChild.Parent = this;
            Children.Add(clonedChild.Clone());
        }
    }

    public IAstNode Clone()
    {
        return new ProgramNode(this);
    }
}