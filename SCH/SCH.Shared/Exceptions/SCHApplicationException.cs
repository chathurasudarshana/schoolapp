namespace SCH.Shared.Exceptions
{
    using System;
    using System.Collections;

    public class SCHApplicationException : SCHException
    {
        public SCHApplicationException(string message, IDictionary? data = null) 
            : base(message, data)
        {
        }

        public SCHApplicationException(
            string message, Exception innerException, IDictionary? data = null) 
            : base(message, innerException, data)
        {
        }

        public static void Throw(
            string message, SCHExceptionTypes schExceptionType, IDictionary? data = null)
        {
            throw new SCHApplicationException(message, data) 
            { SCHExceptionType = schExceptionType };
        }

        public static SCHApplicationException InternalServerError(
            string message = "Internal Server Error", IDictionary? data = null)
        {
            return new SCHApplicationException(message, data) 
            { SCHExceptionType = SCHExceptionTypes.InternalServerError };
        }
    }
}
