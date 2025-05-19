namespace Task11.Exceptions;

public class PrescriptionDuplicateException: Exception
{
    public PrescriptionDuplicateException() : base("Such prescription already exists"){}
}