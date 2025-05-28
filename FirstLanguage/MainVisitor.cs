using FirstLanguage.abstract_syntax_tree;

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

    public override IAstNode VisitDup_instr(EduLangParser.Dup_instrContext context)
    {
        return new DupNode();
    }

    public override IAstNode VisitSwap_instr(EduLangParser.Swap_instrContext context)
    {
        return new SwapNode();
    }

    public override IAstNode VisitLoad_instr(EduLangParser.Load_instrContext context)
    {
        var label = context.IDENTIFIER().GetText();
        
        return new LoadNode(label);
    }

    public override IAstNode VisitStore_instr(EduLangParser.Store_instrContext context)
    {
        var label  = context.IDENTIFIER().GetText();
        return new StoreNode(label);
    }


    public override IAstNode VisitHalt_instr(EduLangParser.Halt_instrContext context)
    {
        return new HaltNode();
    }

    public override IAstNode VisitAdd_instr(EduLangParser.Add_instrContext context)
    {
        return new AddNode();
    }

    public override IAstNode VisitDiv_instr(EduLangParser.Div_instrContext context)
    {
        return new DivisionNode();
    }

    public override IAstNode VisitMod_instr(EduLangParser.Mod_instrContext context)
    {
        return new ModNode();
    }

    public override IAstNode VisitMul_instr(EduLangParser.Mul_instrContext context)
    {
        return new MultiplyNode();
    }

    public override IAstNode VisitSub_instr(EduLangParser.Sub_instrContext context)
    {
        return new SubNode();
    }
}