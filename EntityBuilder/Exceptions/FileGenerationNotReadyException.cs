namespace EntityBuilder.Exceptions
{
    internal class FileGenerationNotReadyException : Exception
    {
        public FileGenerationNotReadyException() : base() { }

        public FileGenerationNotReadyException(string message) : base(message) { }

        public FileGenerationNotReadyException(
            string message,
            Exception innerException) : base(message, innerException) { }
    }
}
