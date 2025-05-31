namespace FirstLanguage.virtual_machine;

public enum OpCode : byte
{
    Load,
    Pop,
    Push,
    Store,
    Add,
    Sub,
    Jump,
    Print,
    Halt,
    Label,
    
}


public static class OpCodeExtensions
{
    public static bool HasData(this OpCode instruction)
    {
        return instruction is OpCode.Jump or OpCode.Push or OpCode.Load or OpCode.Store;
    }
}