namespace Task11.Exceptions;

public class MedicamentNotExistsException : Exception
{
    public MedicamentNotExistsException() : base("Provided medicament doesn't exists"){}
}