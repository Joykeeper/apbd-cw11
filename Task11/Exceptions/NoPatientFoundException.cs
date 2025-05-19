namespace Task11.Exceptions;

public class NoPatientFoundException: Exception
{
    public NoPatientFoundException() : base("No patient found with provided ID"){}
}