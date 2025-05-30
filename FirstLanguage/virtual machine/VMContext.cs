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


        foreach (var b in instructions)
        {
            Console.Write(b);
        }

        Console.WriteLine();
    }


    private static byte[] GenerateBytecode(ProgramNode program)
    {
        List<byte> instructions = [];

        var labelsDict = new Dictionary<string, int>();
        List<string> registers = [];


        List<(string label, int position)> unresolvedLabels = [];

        foreach (var node in program.Statements)
        {
            switch (node)
            {
                case AddNode:
                    instructions.Add((byte)OpCode.Add);
                    break;
                case SubNode:
                    instructions.Add((byte)OpCode.Sub);
                    break;
                case JumpzNode jumpzNode:
                {
                    var label = jumpzNode.Label;
                    instructions.Add((byte)OpCode.Jump);


                    if (labelsDict.TryGetValue(label, out var jumpIndex))
                    {
                        instructions.Add((byte)jumpIndex);
                    }
                    else
                    {
                        instructions.Add(0);
                        unresolvedLabels.Add((label, Position()));
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
                        instructions[unresolvedLabels[i].position] = (byte)Position();
                        unresolvedLabels.RemoveAt(i);
                    }
                    
                    instructions.Add((byte)OpCode.Label);

                    break;
                }

                case PopNode:
                {
                    instructions.Add((byte)OpCode.Pop);
                    break;
                }

                case PushNode pushNode:
                {
                    instructions.Add((byte)OpCode.Push);
                    var bytes = LongToBytes(pushNode.Value);
                    instructions.AddRange(bytes);

                    break;
                }


                case HaltNode:
                {
                    instructions.Add((byte)OpCode.Halt);
                    break;
                }

                case PrintNode:
                {
                    instructions.Add((byte)OpCode.Print);
                    break;
                }

                case LoadNode loadNode:
                {
                    var label = loadNode.Label;

                    if (registers.Contains(label))
                    {
                        instructions.Add((byte)OpCode.Load);

                        var register = registers.IndexOf(label);
                        instructions.Add((byte)register);
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
                    instructions.Add((byte)OpCode.Store);
                    instructions.Add((byte)register);

                    break;
                }
            }
        }

        return instructions.ToArray();

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