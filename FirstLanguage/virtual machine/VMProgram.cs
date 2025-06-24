namespace FirstLanguage.virtual_machine;

public class VmProgram(byte[] bytecode)
{
    public readonly byte[] Bytecode = bytecode;

    public byte this[int instructionIndex] => Bytecode[instructionIndex];
    
    public byte[] this[Range instructionIndex] => Bytecode[instructionIndex];


    public int Length => Bytecode.Length;
}