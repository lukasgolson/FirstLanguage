namespace FirstLanguage.virtual_machine;

[Serializable]
public class VMException : Exception
{
    public VMException()
    {
    }

    public VMException(string message)
        : base(message)
    {
    }

    public VMException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}