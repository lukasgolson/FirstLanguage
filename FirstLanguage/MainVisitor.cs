using FirstLanguage.abstract_syntax_tree.Core.logic;
using FirstLanguage.abstract_syntax_tree.Nodes;
using FirstLanguage.abstract_syntax_tree.Nodes.Core;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.arithmetic;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.logic;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.manipulation;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.Misc;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.variables;
using FirstLanguage.abstract_syntax_tree.Nodes.Macro;

namespace FirstLanguage;

public class MainVisitor : EduLangBaseVisitor<IAstNode>
{
    public override IAstNode VisitProgram(EduLangParser.ProgramContext context)
    {
        var programNode = new ProgramNode();

        foreach (var statementContext in context.statement())
        {
            var stmtNode = Visit(statementContext);
            if (stmtNode != null)
            {
                programNode.Children.Add(stmtNode);
            }
        }

        return programNode;
    }


    public override IAstNode VisitStatement(EduLangParser.StatementContext context)
    {
        if (context.instruction() != null)
        {
            return Visit(context.instruction());
        }

        if (context.macro_def() != null)
        {
            return Visit(context.macro_def());
        }

        if (context.macro_call() != null)
        {
            return Visit(context.macro_call());
        }


        throw new NotImplementedException("Unknown statement");
    }

    
    public override IAstNode VisitMacro_call(EduLangParser.Macro_callContext context)
    {
        var label = context.name.Text;
        var arguments = context._args.Select(parameterContext => parameterContext.Text).ToList();

        return new MacroCallNode(label, arguments.ToArray());
    }

    public override IAstNode VisitMacro_def(EduLangParser.Macro_defContext context)
    {
        var label = context.name.Text;
        var argumentNames = context._args.Select(parameterContext => parameterContext.Text).ToList();

        var macroDef = new MacroDefNode(label, argumentNames.ToArray());


        foreach (var statementContext in context.statement())
        {
            var stmtNode = Visit(statementContext);
            if (stmtNode != null)
            {
                macroDef.Children.Add(stmtNode);
            }
        }

        return macroDef;
    }

    public override IAstNode VisitPush_instr(EduLangParser.Push_instrContext context)
    {
        var text = context.INTEGER_LITERAL().GetText();

        var value = Convert.ToInt64(text);

        var pushNode = new PushNode(value);

        return pushNode;
    }

    public override IAstNode VisitPop_instr(EduLangParser.Pop_instrContext context)
    {
        
        var label = context.IDENTIFIER()?.GetText();
        
        
        return new PopNode(label);
    }

    public override IAstNode VisitLoad_instr(EduLangParser.Load_instrContext context)
    {
        var label = context.IDENTIFIER().GetText();

        return new LoadNode(label);
    }

    public override IAstNode VisitStore_instr(EduLangParser.Store_instrContext context)
    {
        var label = context.IDENTIFIER().GetText();
        return new StoreNode(label);
    }

    public override IAstNode VisitAdd_instr(EduLangParser.Add_instrContext context)
    {
        return new AddNode();
    }

    public override IAstNode VisitSub_instr(EduLangParser.Sub_instrContext context)
    {
        return new SubNode();
    }

    public override IAstNode VisitGt_instr(EduLangParser.Gt_instrContext context)
    {
        return new GTNode();
    }

    public override IAstNode VisitLabel_instr(EduLangParser.Label_instrContext context)
    {
        var label = context.IDENTIFIER().GetText();
        return new LabelNode(label);
    }

    public override IAstNode VisitJumpz_instr(EduLangParser.Jumpz_instrContext context)
    {
        var label = context.IDENTIFIER().GetText();
        return new JumpzNode(label);
    }

    public override IAstNode VisitPrint_instr(EduLangParser.Print_instrContext context)
    {
        return new PrintNode();
    }

    public override IAstNode VisitInput_instr(EduLangParser.Input_instrContext context)
    {
        return new InputNode();
    }


    public override IAstNode VisitHalt_instr(EduLangParser.Halt_instrContext context)
    {
        return new HaltNode();
    }
}