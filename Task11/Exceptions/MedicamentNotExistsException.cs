namespace Task11.Exceptions;

public class MedicamentNotExistsException : Exception
{
    public MedicamentNotExistsException(string message) : base(message){}
}