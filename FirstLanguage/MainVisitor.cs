using FirstLanguage.abstract_syntax_tree;
using FirstLanguage.abstract_syntax_tree.Core.logic;
using FirstLanguage.abstract_syntax_tree.Core.manipulation;
using FirstLanguage.abstract_syntax_tree.Core.Misc;

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
                programNode.Statements.Add(stmtNode);
            }
        }

        return programNode;
    }

    public override IAstNode VisitStatement(EduLangParser.StatementContext context)
    {
        return Visit(context.instruction());
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
        return new PopNode();
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


    public override IAstNode VisitHalt_instr(EduLangParser.Halt_instrContext context)
    {
        return new HaltNode();
    }


}