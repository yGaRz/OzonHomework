namespace Ozon.Route256.Practice.GatewayService.Exceptions;

public sealed class NotFoundException : Exception
{
    public override string Message { get; }

    public NotFoundException(string exceptionMessage)
    {
        Message = exceptionMessage;
    }
}