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
    Label
}