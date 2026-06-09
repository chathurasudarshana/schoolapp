namespace SCH.Repositories.Students
{
    using SCH.Models.Common.GridEntities;
    using SCH.Models.Students.ClientDtos;
    using SCH.Models.Students.Entities;

    public interface IStudentsRepository: IRepository
    {
        Task<List<Student>> GetStudentsAsync(bool? isActive);

        Task<Student?> GetStudentAsync(int id);

        Task<Student?> GetStudentByUserIdAsync(int userId);

        Task InsertStudentAsync(Student student);

        /// <summary>
        /// Updates a student with optimistic concurrency check
        /// </summary>
        void UpdateAsync(Student student);

        Task DeleteStudentAsync(int id);

        Task<PagedResult<Student>> GetStudentGridAsync(StudentGridRequest request);
    }
}
