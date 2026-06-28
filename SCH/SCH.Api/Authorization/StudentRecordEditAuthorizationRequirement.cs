namespace SCH.API.Authorization
{
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Requirement: the editing user must be the owner of the student record.
    /// Checked by StudentRecordEditAuthorizationHandler using the JWT own_student_id claim vs the route {id}.
    /// </summary>
    public class StudentRecordEditAuthorizationRequirement : IAuthorizationRequirement 
    {

    }

}
