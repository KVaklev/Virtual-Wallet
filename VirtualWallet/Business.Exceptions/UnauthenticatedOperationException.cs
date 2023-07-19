namespace Business.Exceptions
{
    public class UnauthenticatedOperationException : ApplicationException
    {
        public UnauthenticatedOperationException(string message)
            : base(message)
        {
        }
    }
}
