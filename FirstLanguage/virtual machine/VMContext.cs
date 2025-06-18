using FirstLanguage.abstract_syntax_tree.Core.logic;
using FirstLanguage.abstract_syntax_tree.Nodes.Core;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.arithmetic;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.logic;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.manipulation;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.Misc;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.variables;
using System.Collections.Generic;
using FirstLanguage.abstract_syntax_tree.Nodes;

namespace FirstLanguage.virtual_machine;

public class VmContext
{
    private readonly byte[] _instructions;

    private readonly Dictionary<int, Stack<long>> _registers = new();
    private readonly Stack<long> _stack;

    private int _instructionIndex = 0;
    private bool _executing = false;


    public VmContext(ProgramNode program)
    {
        _instructions = CompileBytecode(program);

        _stack = new Stack<long>();
        _registers.Add(0, _stack);
    }

    public void Reset()
    {
        _stack.Clear();
        _registers.Clear();
        _registers.Add(0, _stack);
        _instructionIndex = 0;
        _executing = false;
    }

    private long Pop()
    {
        try
        {
            return _stack.Pop();
        }
        catch (InvalidOperationException e)
        {
            throw new VMException(
                $"Stack underflow with instruction {_instructionIndex}: {(OpCode)_instructions[_instructionIndex]}", e);
        }
    }

    private long Peek()
    {
        try
        {
            return _stack.Peek();
        }
        catch (InvalidOperationException e)
        {
            throw new VMException(
                $"Stack underflow with instruction {_instructionIndex}: {(OpCode)_instructions[_instructionIndex]}", e);
        }
    }


    public bool Execute()
    {
        
        _executing = true;
        while (_executing)
        {
            var instruction = (OpCode)_instructions[_instructionIndex];

            byte nextInstruction = 0;
            if (_instructions.Length - 1 >= _instructionIndex + 1)
            {
                nextInstruction = _instructions[_instructionIndex + 1];
            }


            var bytesRead = 1;
            switch (instruction)
            {
                case OpCode.Init:
                    for (var i = 0; i < nextInstruction; i++)
                    {
                        _registers.Add(i + 1, new Stack<long>());
                    }

                    bytesRead++;
                    break;

                case OpCode.Push:
                    var dataBytes = _instructions[(_instructionIndex + 1)..(_instructionIndex + 1 + sizeof(long))];
                    var dataConverted = BytesToLong(dataBytes);
                    _stack.Push(dataConverted);
                    bytesRead += sizeof(long);
                    break;


                case OpCode.Add:

                    if (_stack.Count < 2)
                    {
                        throw new VMException("Stack underflow, stack does not contain enough elements to add.");
                    }

                    _stack.Push(Pop() + Pop());
                    break;
                case OpCode.Sub:

                    if (_stack.Count < 2)
                    {
                        throw new VMException("Stack underflow, stack does not contain enough elements to sub.");
                    }

                    _stack.Push(Pop() - Pop());
                    break;
                case OpCode.Pop:
                    _stack.Push(_registers[nextInstruction].Pop());
                    bytesRead++;

                    break;
                case OpCode.Load:
                    _stack.Push(_registers[nextInstruction].Peek());
                    bytesRead++;
                    break;
                case OpCode.Store:
                    _registers[nextInstruction].Push(Pop());
                    bytesRead++;
                    break;
                case OpCode.JumpZf:
                case OpCode.JumpZb:
                    
                    if (Pop() == 0)
                    {
                        // calculate the jump location

                        int location;
                        if (instruction == OpCode.JumpZf)
                        {
                            location = _instructionIndex + nextInstruction;
                        }
                        else
                        {
                            location = _instructionIndex - nextInstruction;
                        }
                        
                        _instructionIndex = location;
                    }
                    else
                    {
                        bytesRead++; // Skip over address
                    }
                    
                    break;
               

                case OpCode.Gt:
                {
                    if (_stack.Count < 2)
                    {
                        throw new VMException("Stack underflow, stack does not contain enough elements to add.");
                    }

                    var item1 = _stack.Pop();
                    var item2 = _stack.Pop();

                    _stack.Push(item1 > item2 ? 1 : 0);

                    break;
                }

                case OpCode.Print:
                    Console.WriteLine(Peek());
                    break;
                case OpCode.Input:

                    while (true)
                    {
                        Console.Write("Input: ");
                        var input = Console.ReadLine();
                        if (!long.TryParse(input, out var result)) continue;
                        _stack.Push(result);
                        break;
                    }


                    break;
                case OpCode.Halt:
                    _executing = false;
                    break;
                case OpCode.Label:
                    break;
                case OpCode.Null:
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


    private static byte[] CompileBytecode(ProgramNode program)
    {
        List<byte> instructions = [];
        var labelsDict = new Dictionary<string, int>();
        List<string> registers = ["main"];

        List<(string label, int position)> unresolvedJump = [];

        foreach (var node in program.Children)
        {
            string label;
            int register;

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
                    label = jumpzNode.Label;
                    instructions.Add((byte)OpCode.JumpZf);


                    instructions.Add(0);
                    unresolvedJump.Add((label, Position()));

                    break;
                }
                case LabelNode labelNode:
                {
                    // This op-code does not do much, but it may let us validate jumps in the future.
                    instructions.Add((byte)OpCode.Label);


                    label = labelNode.Label;

                    if (!labelsDict.TryAdd(label, Position()))
                    {
                        throw new CompilerException($"Label [{label}] already defined");
                    }

                    break;
                }
                case PopNode popNode:
                    label = popNode.Label;

                    if (!registers.Contains(label))
                    {
                        registers.Add(label);
                    }

                    register = registers.IndexOf(label);
                    instructions.Add((byte)OpCode.Pop);
                    instructions.Add((byte)register);
                    break;
                case PushNode pushNode:
                {
                    instructions.Add((byte)OpCode.Push);
                    var bytes = LongToBytes(pushNode.Value);
                    instructions.AddRange(bytes);
                    break;
                }

                case GTNode:
                {
                    instructions.Add((byte)OpCode.Gt);
                    break;
                }
                case HaltNode:
                    instructions.Add((byte)OpCode.Halt);
                    break;
                case PrintNode:
                    instructions.Add((byte)OpCode.Print);
                    break;
                case LoadNode loadNode:
                {
                    label = loadNode.Label;

                    if (registers.Contains(label))
                    {
                        register = registers.IndexOf(label);
                        instructions.Add((byte)OpCode.Load);
                        instructions.Add((byte)register);
                    }
                    else
                    {
                        throw new CompilerException($"Register, {label}, not found");
                    }

                    break;
                }
                case StoreNode storeNode:
                {
                    label = storeNode.Label;

                    if (!registers.Contains(label))
                    {
                        registers.Add(label);
                    }

                    register = registers.IndexOf(label);
                    instructions.Add((byte)OpCode.Store);
                    instructions.Add((byte)register);
                    break;
                }
                case InputNode inputNode:
                    instructions.Add((byte)OpCode.Input);
                    break;
                case UnsafeNode:
                    // We can ignore the unsafe Node as it just indicates manually written vm code.
                    break;
                default:
                    var type = node.GetType().Name;
                    throw new CompilerException(
                        $"Unsupported node type, {type}, insufficient lowering or unsupported language instructions.");
            }
        }

        // Resolve jumps
        for (var i = unresolvedJump.Count - 1; i >= 0; i--)
        {
            var label = unresolvedJump[i].label;

            if (labelsDict.TryGetValue(label, out var labelPosition))
            {
                var offset = labelPosition - unresolvedJump[i].position;

                if (offset < 0)
                {
                    instructions[unresolvedJump[i].position - 1] = (byte)OpCode.JumpZb;
                }
                
                instructions[unresolvedJump[i].position] = (byte)Math.Abs(offset);
                unresolvedJump.RemoveAt(i);
            }
            else
            {
                throw new CompilerException($"Label [{label}] not defined");
            }
        }

        // To create our final byte code, our final instructions will look like: init op codes to start registries, program

        var initInstructions = new List<byte>();

        int remainingRegisters = registers.Count; // The VM automatically initializes the main stack

        while (remainingRegisters > 0)
        {
            initInstructions.Add((byte)OpCode.Init);

            if (remainingRegisters > byte.MaxValue)
            {
                initInstructions.Add(byte.MaxValue);
                remainingRegisters -= byte.MaxValue;
            }
            else
            {
                initInstructions.Add((byte)remainingRegisters);
                remainingRegisters = 0;
            }
        }
        
        List<byte> finalInstructions = [];
        finalInstructions.AddRange(initInstructions);
        finalInstructions.AddRange(instructions);


        return finalInstructions.ToArray();

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