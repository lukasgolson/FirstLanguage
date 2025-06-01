namespace FirstLanguage.abstract_syntax_tree.Macro;

public class MacroDefNode : ILabelledNode, IBlockNode
{
    public string Label { get; set; }
    public List<IAstNode> Statements { get; }
}