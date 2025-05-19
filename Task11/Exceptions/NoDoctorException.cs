namespace Task11.Exceptions;

public class NoDoctorException : Exception
{
    public NoDoctorException() : base("No doctor found with provided ID"){}
}