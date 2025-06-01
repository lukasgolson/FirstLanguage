namespace FirstLanguage.abstract_syntax_tree.Nodes.Core.Misc;

public class InputNode : IAstNode
{
   
    public IAstNode? Parent { get; set; }
    public IAstNode Clone()
    {
        return new InputNode();
    }

}