namespace FirstLanguage.abstract_syntax_tree.Nodes;

public class UnsafeNode : IBlockNode
{
    public IAstNode? Parent { get; set; }
    public IAstNode Clone()
    {
        var node = new UnsafeNode();
        foreach (var clonedChild in Children.Select(child => child.Clone()))
        {
            clonedChild.Parent = this;
            node.Children.Add(clonedChild.Clone());
        }

        return node;
    }

    public List<IAstNode> Children { get; } = [];
}