namespace Task11.Exceptions;

public class MedicamentsOverflowException :Exception
{
    public MedicamentsOverflowException() : base("Too many medicaments for one prescription"){}
}