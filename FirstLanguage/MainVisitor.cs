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
}