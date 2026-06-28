namespace SCH.API.Authorization
{
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Requirement: the editing user must be the owner of the teacher record.
    /// Checked by TeacherRecordEditAuthorizationHandler using the JWT own_teacher_id claim vs the route {id}.
    /// </summary>
    public class TeacherRecordEditAuthorizationRequirement : IAuthorizationRequirement 
    { 
        
    }

}
