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
    JumpZ,
    Print,
    Halt,
    Label,
    
}


public static class OpCodeExtensions
{
    public static bool HasData(this OpCode instruction)
    {
        return instruction is OpCode.JumpZ or OpCode.Push or OpCode.Load or OpCode.Store;
    }
}