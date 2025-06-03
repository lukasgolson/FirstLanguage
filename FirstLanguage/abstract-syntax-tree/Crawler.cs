using FirstLanguage.abstract_syntax_tree.Nodes;
using FirstLanguage.abstract_syntax_tree.Nodes.Core;
using FirstLanguage.abstract_syntax_tree.Nodes.Macro;
using FirstLanguage.virtual_machine;

namespace FirstLanguage.abstract_syntax_tree;

public class Crawler
{
    public ProgramNode ResolveMacros(ProgramNode program)
    {
        var clone = program.Clone() as ProgramNode;
        if (clone == null)
        {
            throw new InvalidOperationException("The cloned program is not a valid ProgramNode.");
        }
        
        var nodesToVisit = new Stack<IAstNode>();

        var macros = new Dictionary<string, MacroDefNode>();
        var macroCounter = new Dictionary<string, int>();
        var callSites = new List<MacroCallNode>();


        nodesToVisit.Push(clone);

        while (nodesToVisit.Count > 0)
        {
            var node = nodesToVisit.Pop();

            if (node is IBlockNode blockNode)
            {
                foreach (var childNode in blockNode.Children)
                {
                    childNode.Parent = node;
                    nodesToVisit.Push(childNode);
                }
            }

            if (node is MacroDefNode macroDefNode)
            {
                if (!macros.TryAdd(macroDefNode.Label, macroDefNode))
                {
                    throw new CompilerException("Macro definition could not be added. Has it been already defined?");
                }

                if (node.Parent is IBlockNode blockNodeParent)
                {
                    blockNodeParent.Children.Remove(node); // Unlink the node from its parent
                }
            }

            if (node is MacroCallNode macroCallNode)
            {
                callSites.Add(macroCallNode);
            }
        }


        // Resolve the calls
        foreach (var callNode in callSites)
        {
            var label = callNode.Label;

            if (!macros.TryGetValue(label, out var macro))
            {
                throw new CompilerException($"Macro definition, {label}, not found. Has it been defined?");
            }


            if (macroCounter.TryGetValue(label, out var count))
            {
                macroCounter[label] = count + 1;
            }
            else
            {
                count = 0;
                macroCounter[label] = 1;
            }


            if (callNode.Parent is IBlockNode parent)
            {
                var index = parent.Children.IndexOf(callNode);
            
                var expanded = ExpandMacro(macro, callNode, count);
            
                parent.Children.Remove(callNode);
                parent.Children.InsertRange(index, expanded);
            }
        }


        return clone;
    }


    /// <summary>
    /// Clones and traverses the macro, replacing labels with the caller labels.
    /// </summary>
    /// <param name="macroDefinition"></param>
    /// <param name="caller"></param>
    /// <returns></returns>
    private List<IAstNode> ExpandMacro(MacroDefNode macroDefinition, MacroCallNode caller, int instance = 0)
    {
        var macro = macroDefinition.Clone() as MacroDefNode;
        

        if (macroDefinition.Arguments.Length != caller.Arguments.Length)
        {
            throw new CompilerException($"{macroDefinition.Label} called with incorrect number of arguments");
        }

        // Build a mapping between definitions in the macro and what its called with
        Dictionary<string, string> labelReplacements = new Dictionary<string, string>();
        for (int i = 0; i < macroDefinition.Arguments.Length; i++)
        {
            labelReplacements.Add(macroDefinition.Arguments[i], caller.Arguments[i]);
        }


        var nodesToVisit = new Stack<IAstNode>();

        foreach (var child in macro.Children)
        {
            nodesToVisit.Push(child);
        }

        while (nodesToVisit.Count > 0)
        {
            var node = nodesToVisit.Pop();

            if (node is IBlockNode blockNode)
            {
                foreach (var child in blockNode.Children) nodesToVisit.Push(child);
            }

            if (node is ILabelledNode labelledNode)
            {
                var label = labelledNode.Label;

                if (!labelReplacements.TryGetValue(label, out var replacement))
                {
                    replacement = macroDefinition.Label + "_" + label + "_" + instance;
                }
                
                labelledNode.Label = replacement;
            }
            
        }

        
        return macro.Children;
    }


}