namespace DIGen.Runtime.Base
{
    /// <summary>
    /// Lifetime of an instance in the container.
    /// </summary>
    public enum Lifetime : byte
    {
        /// <summary>
        /// The instance is created every time it is requested.
        /// </summary>
        Transient,
        /// <summary>
        /// The instance is created once per scope. 
        /// </summary>
        Scoped,
        /// <summary>
        /// The instance is created once at the root container and shared across all scopes.
        /// </summary>
        Singleton,
    }
}