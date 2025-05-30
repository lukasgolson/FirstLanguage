using FirstLanguage.abstract_syntax_tree.Core;
using FirstLanguage.abstract_syntax_tree.Core.logic;
using FirstLanguage.abstract_syntax_tree.Core.manipulation;
using FirstLanguage.abstract_syntax_tree.Core.Misc;

namespace FirstLanguage.virtual_machine;

public class VmContext
{
    public VmContext(ProgramNode root)
    {
        var instructions = GenerateBytecode(root);

        
        
        
        var data_section_length = (int) BytesToLong(instructions[..sizeof(long)]);
        var data = instructions[sizeof(long)..data_section_length];
        var program = instructions[data_section_length..];



        foreach (var b in data)
        {
            Console.Write(b + ",");
        }
        Console.Write(" ");

        foreach (var b in program)
        {
            Console.Write((OpCode)b + " ");
        }

       

        Console.WriteLine();
    }


    private static byte[] GenerateBytecode(ProgramNode program)
    {
        List<OpCode> instructions = [];
        List<byte> data = [];

        var labelsDict = new Dictionary<string, int>();
        List<string> registers = [];


        List<(string label, int position)> unresolvedLabels = [];

        foreach (var node in program.Statements)
        {
            switch (node)
            {
                case AddNode:
                    instructions.Add(OpCode.Add);
                    break;
                case SubNode:
                    instructions.Add(OpCode.Sub);
                    break;
                case JumpzNode jumpzNode:
                {
                    var label = jumpzNode.Label;
                    instructions.Add(OpCode.Jump);


                    if (labelsDict.TryGetValue(label, out var jumpIndex))
                    {
                        data.Add((byte)jumpIndex);
                    }
                    else
                    {
                        data.Add(0);
                        unresolvedLabels.Add((label, data.Count - 1));
                    }

                    break;
                }
                case LabelNode labelNode:
                {
                    var label = labelNode.Label;

                    if (!labelsDict.ContainsKey(label))
                    {
                        labelsDict.Add(label, Position());
                    }
                    else
                    {
                        throw new Exception($"Label [{label}] already defined");
                    }

                    for (var i = unresolvedLabels.Count - 1; i >= 0; i--)
                    {
                        if (unresolvedLabels[i].label != label) continue;
                        data[unresolvedLabels[i].position] = (byte)Position();
                        unresolvedLabels.RemoveAt(i);
                    }
                    
                    instructions.Add(OpCode.Label);

                    break;
                }

                case PopNode:
                {
                    instructions.Add(OpCode.Pop);
                    break;
                }

                case PushNode pushNode:
                {
                    instructions.Add(OpCode.Push);
                    var bytes = LongToBytes(pushNode.Value);
                    data.AddRange(bytes);

                    break;
                }


                case HaltNode:
                {
                    instructions.Add(OpCode.Halt);
                    break;
                }

                case PrintNode:
                {
                    instructions.Add(OpCode.Print);
                    break;
                }

                case LoadNode loadNode:
                {
                    var label = loadNode.Label;

                    if (registers.Contains(label))
                    {
                        instructions.Add(OpCode.Load);

                        var register = registers.IndexOf(label);
                        data.Add((byte)register);
                    }
                    else
                    {
                        throw new Exception("Register not found");
                    }

                    break;
                }

                case StoreNode storeNode:
                {
                    var label = storeNode.Label;

                    if (!registers.Contains(label))
                    {
                        registers.Add(label);
                    }

                    var register = registers.IndexOf(label);
                    instructions.Add(OpCode.Store);
                    data.Add((byte)register);

                    break;
                }
            }
        }
        
        // To create our final byte code, our final instructions will look like: data, start op code, program
        
        List<byte> bytecode = [];
        
        bytecode.AddRange(LongToBytes(data.Count + sizeof(long)));
        
        bytecode.AddRange(data);

        bytecode.AddRange(instructions.Select(instruction => (byte)instruction));

        return bytecode.ToArray();

        int Position() => instructions.Count - 1;
    }

    public static byte[] LongToBytes(long value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes); // Ensure Little Endian byte order
        }

        return bytes;
    }

    public static long BytesToLong(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);

        if (bytes.Length < sizeof(long))
        {
            throw new ArgumentException($"Byte array must be at least {sizeof(long)} bytes long.", nameof(bytes));
        }

        var
            localBytes =
                (byte[])bytes.Clone(); // Work on a copy to avoid modifying the original array if it's used elsewhere
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(localBytes, 0, sizeof(long)); // Ensure Little Endian byte order before conversion
        }

        return BitConverter.ToInt64(localBytes, 0);
    }
}