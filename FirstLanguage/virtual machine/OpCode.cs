namespace FirstLanguage.virtual_machine;

public enum OpCode : byte
{
    Null,
    Load,
    Pop,
    Push,
    Store,
    Add,
    Sub,
    Gt,
    JumpZf,
    JumpZb,
    Print,
    Input,
    Halt,
    Label,
    Init
}


public static class OpCodeExtensions
{
    public static bool HasData(this OpCode instruction)
    {
        return instruction is OpCode.JumpZf or OpCode.JumpZb or OpCode.Push or OpCode.Load or OpCode.Store or OpCode.Pop or OpCode.Init;
    }
}