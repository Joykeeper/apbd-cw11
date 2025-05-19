using NuGet.Protocol.Plugins;

namespace Task11.Exceptions;

public class DueDateBeforeDateException : Exception
{
    public DueDateBeforeDateException(): base("Due date before start date"){}
}