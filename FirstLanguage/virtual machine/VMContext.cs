using FirstLanguage.abstract_syntax_tree.Core;
using FirstLanguage.abstract_syntax_tree.Core.logic;
using FirstLanguage.abstract_syntax_tree.Core.manipulation;
using FirstLanguage.abstract_syntax_tree.Core.Misc;

namespace FirstLanguage.virtual_machine;

public class VmContext
{
    private readonly byte[] _instructions;
    private readonly Stack<long> _stack = new();
    private readonly Dictionary<int, long> _registers = new();
    
    private int _instructionIndex = 0;
    private bool _executing = false;


    public VmContext(ProgramNode program)
    {
        _instructions = GenerateBytecode(program);
    }

    public void Reset()
    {
        _stack.Clear();
        _registers.Clear();
        _instructionIndex = 0;
        _executing = false;
    }

    public bool Execute()
    {
        _executing = true;
        while (_executing)
        {
            var instruction = (OpCode) _instructions[_instructionIndex];

            byte nextInstruction = 0;
            if (_instructions.Length - 1 >= _instructionIndex + 1)
            {
                nextInstruction = _instructions[_instructionIndex + 1];
            }


            int bytesRead = 1;

            switch (instruction)
            {
                case OpCode.Push:
                    var dataBytes = _instructions[(_instructionIndex + 1)..(_instructionIndex + 1 + sizeof(long))];
                    var dataConverted = BytesToLong(dataBytes);
                    _stack.Push(dataConverted);
                    bytesRead += sizeof(long);
                    break;
                case OpCode.Pop:
                    _stack.Pop();
                    break;
                case OpCode.Add:

                    if (_stack.Count < 2)
                    {
                        throw new VMException("Stack underflow, stack does not contain enough elements to add.");
                    }
                    
                    _stack.Push(_stack.Pop() + _stack.Pop());
                    break;
                case OpCode.Sub:

                    if (_stack.Count < 2)
                    {
                        throw new VMException("Stack underflow, stack does not contain enough elements to sub.");
                    }
                    
                    _stack.Push(_stack.Pop() - _stack.Pop());
                    break;
                case OpCode.Load:
                    _stack.Push(_registers[nextInstruction]);
                    bytesRead++;
                    break;
                case OpCode.Store:
                    _registers[nextInstruction] = _stack.Pop();
                    bytesRead++;
                    break;
                case OpCode.Jump:

                    if (_stack.Pop() == 0)
                    {
                        _instructionIndex = nextInstruction; // Jump
                    }
                    else
                    {
                        bytesRead++; // Skip over address
                    }
                    
                    break;
                case OpCode.Print:
                    Console.WriteLine(_stack.Peek());
                    break;
                case OpCode.Halt:
                    _executing = false;
                    break;
                case OpCode.Label:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(instruction), instruction, null);
            }


            _instructionIndex += bytesRead;
        }


        return true;
    }

    private bool PrintInstructions(byte[] instructions)
    {
        Console.WriteLine(string.Join(" ", instructions));
        int dataCounter = 0;
        OpCode lastCode;
        for (var index = 0; index < instructions.Length; index++)
        {
            var word = instructions[index];
            if (dataCounter == 0)
            {
                var instruction = (OpCode)word;
                Console.WriteLine(instruction);

                if (instruction.HasData() && instruction != OpCode.Push)
                {
                    dataCounter = 1;
                }
                else if (instruction.HasData())
                {
                    dataCounter = sizeof(long);
                }

                lastCode = instruction;
            }
            else
            {
                var data = instructions[index..(index + dataCounter)];

                foreach (var b in data)
                {
                    Console.Write(b);
                }

                Console.WriteLine();

                index += dataCounter - 1;
                dataCounter = 0;
            }
        }


        return true;
    }


    private static byte[] GenerateBytecode(ProgramNode program)
    {
        List<byte> instructions = [];
        var labelsDict = new Dictionary<string, int>();
        List<string> registers = [];

        int stackSize = 0;


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
                    // This op-code does not do much, but it may let us validate jumps in the future.
                    instructions.Add((byte)OpCode.Label); 
                    
                    
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
                        var register = registers.IndexOf(label);
                        instructions.Add((byte)OpCode.Load);
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

        // To create our final byte code, our final instructions will look like: data, start op code, program


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