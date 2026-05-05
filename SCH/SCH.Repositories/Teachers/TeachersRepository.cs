namespace SCH.Repositories.Teachers
{
    using Microsoft.EntityFrameworkCore;
    using SCH.Models.Teachers.Entities;
    using SCH.Repositories.Common;
    using SCH.Repositories.DbContexts;

    internal class TeachersRepository : BaseRepository<Teacher, SCHContext>, ITeachersRepository
    {
        public TeachersRepository(SCHContext context) : base(context)
        {
        }

        public async Task<List<Teacher>> GetTeachersAsync()
        {
            List<Teacher> teachers = await Context
                .Teacher
                .AsNoTracking()
                .ToListAsync();

            return teachers;
        }

        public async Task<Teacher?> GetTeacherAsync(int id)
        {
            Teacher? teacher = await Context
                .Teacher
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.Id == id);

            return teacher;
        }

        public async Task InsertTeacherAsync(Teacher teacher)
        {
            await Context.Teacher.AddAsync(teacher);
        }

        public void UpdateAsync(Teacher teacher)
        {
            UpdateWithConcurrency(teacher);
        }

        public async Task DeleteTeacherAsync(int id)
        {
            Teacher? teacherEntity = await Context
                .Teacher.SingleOrDefaultAsync(s => s.Id == id);

            if (teacherEntity != null)
            {
                Context.Teacher.Remove(teacherEntity);
            }
        }
    }
}


