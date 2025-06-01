namespace FirstLanguage.virtual_machine;

[Serializable]
public class CompilerException : Exception
{
    public CompilerException()
    {
    }

    public CompilerException(string message)
        : base(message)
    {
    }

    public CompilerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}