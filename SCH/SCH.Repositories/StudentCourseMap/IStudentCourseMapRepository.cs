namespace SCH.Repositories.StudentCourseMap
{
    using SCH.Models.StudentCourseMap.Entities;
    using System.Collections.Generic;

    public interface IStudentCourseMapRepository: IRepository
    {
        Task<StudentCourseMap?> GetStudentCourseMapAsync(int studentId, int courseId);

        Task<List<StudentCourseMap>> GetStudentCourseMapsByStudentAsync(int studentId);

        Task InsertStudentCourseMapAsync(StudentCourseMap studentCourseMap);

        Task DeleteStudentCourseMapAsync(int studentId, int courseId);
    }
}
