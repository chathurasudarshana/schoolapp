namespace SCH.Shared.Exceptions
{
    using System;
    using System.Collections;

    public abstract class SCHException : Exception
    {
        public required SCHExceptionTypes SCHExceptionType { get; set; }

        protected SCHException(string message, IDictionary? data = null) : base(message)
        {
            if (data != null)
            {
                foreach (DictionaryEntry entry in data)
                {
                    Data[entry.Key] = entry.Value;
                }
            }
        }

        protected SCHException(
            string message, Exception innerException, IDictionary? data = null) 
            : base(message, innerException)
        {
            if (data != null)
            {
                foreach (DictionaryEntry entry in data)
                {
                    Data[entry.Key] = entry.Value;
                }
            }
        }
    }
}
