namespace SCH.Services.Teachers
{
    using SCH.Models.Teachers.ClientDtos;

    public interface ITeachersService: IService
    {
        Task<List<TeacherDto>> GetTeachersAsync();

        Task<TeacherDto?> GetTeacherAsync(int id);

        Task<int> InsertTeacherAsync(TeacherDto teacher);

        Task UpdateTeacherAsync(TeacherDto teacher);

        Task DeleteTeacherAsync(int id);
    }
}


