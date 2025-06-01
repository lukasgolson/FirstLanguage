namespace FirstLanguage.abstract_syntax_tree.Nodes;

public interface IAstNode
{
    IAstNode? Parent { get; set; }
    
    
    
    /// <summary>
    /// Method returns a shallow copy of the node, excluding its parent or any children (if applicable).
    /// </summary>
    /// <returns></returns>
    IAstNode Clone(); 

};