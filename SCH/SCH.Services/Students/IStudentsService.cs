namespace SCH.Services.Students
{
    using SCH.Models.StudentCourseMap.ClientDtos;
    using SCH.Models.Students.ClientDtos;

    public interface IStudentsService: IService
    {
        Task<List<StudentDto>> GetStudentsAsync(bool? isActive);

        Task<StudentDto?> GetStudentAsync(int id);

        Task<int> InsertStudentAsync(StudentDto student);

        Task UpdateStudentAsync(StudentDto student);

        Task DeleteStudentAsync(int id);

        Task<List<StudentCourseMapDto>> GetCoursesAsync(int id);

        Task InsertCourseAsync(StudentCourseMapDto studentCourseMap);

        Task DeleteCourseAsync(int id, int courseId);

    }
}
