using FirstLanguage.abstract_syntax_tree.Core.logic;
using FirstLanguage.abstract_syntax_tree.Nodes.Core;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.arithmetic;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.logic;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.manipulation;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.Misc;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.variables;
using FirstLanguage.abstract_syntax_tree.Nodes;

namespace FirstLanguage.virtual_machine;

public class VmContext
{
    private readonly VmProgram _instructions;

    private readonly Dictionary<int, Stack<long>> _registers = new();
    private readonly Stack<long> _stack;

    private int _instructionIndex = 0;
    private bool _executing = false;


    public VmContext(VmProgram program)
    {
        _instructions = program;

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
                    var dataConverted = dataBytes.ToLong();
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



  
}