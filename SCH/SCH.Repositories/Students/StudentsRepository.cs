namespace SCH.Repositories.Students
{
    using AutoMapper;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using SCH.Models.Common.GridEntities;
    using SCH.Models.Students.ClientDtos;
    using SCH.Models.Students.DbDtos;
    using SCH.Models.Students.Entities;
    using SCH.Repositories.Common;
    using SCH.Repositories.DbContexts;

    internal class StudentsRepository : BaseRepository<Student, SCHContext>, IStudentsRepository
    {
        private readonly IMapper mapper;

        public StudentsRepository(SCHContext context, IMapper mapper) : base(context)
        {
            this.mapper = mapper;
        }

        public async Task<List<Student>> GetStudentsAsync(bool? isActive)
        {

            List<Student> students = await Context
                .Student
                .AsNoTracking()
                .Where(s => !isActive.HasValue || s.IsActive == isActive)
                .ToListAsync();

            return students;
        }

        public async Task<Student?> GetStudentAsync(int id)
        {
            Student? student = await Context
                .Student
                .AsNoTracking()
                .Include(s => s.StudentCourseMaps)
                .ThenInclude(sc => sc.Course)
                .SingleOrDefaultAsync(s => s.Id == id);

            return student;
        }

        public async Task InsertStudentAsync(Student student)
        {                                     
            await Context.Student.AddAsync(student);
        }

        public void UpdateAsync(Student student)
        {
            UpdateWithConcurrency(student);
        }

        public async Task DeleteStudentAsync(int id)
        {

            Student? studentEntity = await Context
                .Student.SingleOrDefaultAsync(s => s.Id == id);

            if (studentEntity != null)
            {
                Context.Student.Remove(studentEntity);
            }
        }

        public async Task<PagedResult<Student>> GetStudentGridAsync(StudentGridRequest request)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new("@PageNumber",          request.PageNumber),
                new("@PageSize",            request.PageSize),
                new("@SortBy",              (object?)request.SortBy              ?? DBNull.Value),
                new("@SortByOperator",      (object?)request.SortByOperator      ?? DBNull.Value),
                new("@FirstName",           (object?)request.FirstName           ?? DBNull.Value),
                new("@FirstNameOperator",   (object?)request.FirstNameOperator   ?? DBNull.Value),
                new("@LastName",            (object?)request.LastName            ?? DBNull.Value),
                new("@LastNameOperator",    (object?)request.LastNameOperator    ?? DBNull.Value),
                new("@Email",               (object?)request.Email               ?? DBNull.Value),
                new("@EmailOperator",       (object?)request.EmailOperator       ?? DBNull.Value),
                new("@PhoneNumber",         (object?)request.PhoneNumber         ?? DBNull.Value),
                new("@PhoneNumberOperator", (object?)request.PhoneNumberOperator ?? DBNull.Value),
                new("@SSN",                 (object?)request.SSN                 ?? DBNull.Value),
                new("@SSNOperator",         (object?)request.SSNOperator         ?? DBNull.Value),
                new("@StartDate",           (object?)request.StartDate           ?? DBNull.Value),
                new("@StartDateOperator",   (object?)request.StartDateOperator   ?? DBNull.Value),
                new("@IsActive",            (object?)request.IsActive            ?? DBNull.Value),
            };

            List<StudentGridResult> rows = await Context.Database
                .SqlQueryRaw<StudentGridResult>(
                    "EXEC dbo.GetStudentGrid @PageNumber, @PageSize, @SortBy, @SortByOperator," +
                    " @FirstName, @FirstNameOperator, @LastName, @LastNameOperator," +
                    " @Email, @EmailOperator, @PhoneNumber, @PhoneNumberOperator," +
                    " @SSN, @SSNOperator, @StartDate, @StartDateOperator, @IsActive",
                    parameters)
                .ToListAsync();

            List<Student> students = mapper.Map<List<Student>>(rows);

            return new PagedResult<Student>
            {
                Items      = students,
                TotalCount = rows.FirstOrDefault()?.TotalCount ?? 0,
                PageNumber = request.PageNumber,
                PageSize   = request.PageSize,
            };
        }
    }
}
