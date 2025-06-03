namespace FirstLanguage.abstract_syntax_tree.Nodes.Macro;

public class MacroDefNode : ILabelledNode, IBlockNode
{
    public string Label { get; set; }
    public string[] Arguments { get; }
    public List<IAstNode> Children { get; } = [];
    public IAstNode? Parent { get; set; }


    public MacroDefNode(string label, string[] arguments)
    {
        Label = label;
        Arguments = arguments;
    }

    private MacroDefNode(MacroDefNode macroDefNode)
    {
        Label = macroDefNode.Label;
        Arguments = macroDefNode.Arguments;

        foreach (var clonedChild in macroDefNode.Children.Select(child => child.Clone()))
        {
            clonedChild.Parent = this;
            Children.Add(clonedChild.Clone());
        }
    }

    public IAstNode Clone()
    {
        return new MacroDefNode(this);
    }
}