namespace SCH.Shared.Exceptions
{
    using System.Collections;

    public class SCHDomainException : SCHException
    {
        public SCHDomainException(string message, IDictionary? data = null) 
            : base(message, data)
        {
        }

        public static void Throw(
            string message, SCHExceptionTypes schExceptionType, IDictionary? data = null)
        {
            throw new SCHDomainException(message, data) 
            { SCHExceptionType = schExceptionType };
        }

        public static SCHDomainException NotFound(
            string message = "Record Not Found", IDictionary? data = null)
        {
            return new SCHDomainException(message, data) 
            { SCHExceptionType = SCHExceptionTypes.NotFound };
        }

        public static SCHDomainException BadRequest(
            string message = "Bad Request", IDictionary? data = null)
        {
            return new SCHDomainException(message, data) 
            { SCHExceptionType = SCHExceptionTypes.BadRequest };
        }

        public static SCHDomainException Conflict(string message, IDictionary? data = null)
        {
            return new SCHDomainException(message, data) 
            { SCHExceptionType = SCHExceptionTypes.Conflict };
        }

        public static SCHDomainException Forbidden(
            string message = "Forbidden", IDictionary? data = null)
        {
            return new SCHDomainException(message, data)
            { SCHExceptionType = SCHExceptionTypes.Forbidden };
        }

        public static SCHDomainException Unauthorized(
            string message = "Unauthorized", IDictionary? data = null)
        {
            return new SCHDomainException(message, data)
            { SCHExceptionType = SCHExceptionTypes.Unauthorized };
        }
    }
}
