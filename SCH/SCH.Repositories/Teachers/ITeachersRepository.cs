namespace SCH.Repositories.Teachers
{
    using SCH.Models.Teachers.Entities;

    public interface ITeachersRepository: IRepository
    {
        Task<List<Teacher>> GetTeachersAsync();

        Task<Teacher?> GetTeacherAsync(int id);

        Task InsertTeacherAsync(Teacher teacher);

        /// <summary>
        /// Updates a teacher with optimistic concurrency check
        /// </summary>
        void UpdateAsync(Teacher teacher);

        Task DeleteTeacherAsync(int id);
    }
}


