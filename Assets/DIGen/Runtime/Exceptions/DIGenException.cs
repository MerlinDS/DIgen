namespace DIGen.Runtime.Exceptions
{
    /// <summary>
    /// Exception thrown when there is an error in the dependency injection container.
    /// </summary>
    public class DIGenException : System.Exception
    {
        public DIGenException(string message) : base(message)
        {
        }
        
        public DIGenException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}