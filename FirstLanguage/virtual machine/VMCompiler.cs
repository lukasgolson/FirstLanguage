using FirstLanguage.abstract_syntax_tree.Core.logic;
using FirstLanguage.abstract_syntax_tree.Nodes;
using FirstLanguage.abstract_syntax_tree.Nodes.Core;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.arithmetic;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.logic;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.manipulation;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.Misc;
using FirstLanguage.abstract_syntax_tree.Nodes.Core.variables;

namespace FirstLanguage.virtual_machine;

public class VmCompiler
{
    public static VmProgram CompileBytecode(ProgramNode program)
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
                    var bytes = pushNode.Value.ToBytes();
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


        return new VmProgram(finalInstructions.ToArray());

        int Position() => instructions.Count - 1;
    }
}