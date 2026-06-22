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
                .Include(s => s.User)
                .SingleOrDefaultAsync(s => s.Id == id);

            return student;
        }

        public async Task<Student?> GetStudentByUserIdAsync(int userId)
        {
            return await Context
                .Student
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.UserId == userId);
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
                new("@FirstNameValue1",               (object?)request.FirstNameValue1               ?? DBNull.Value),
                new("@FirstNameOperator1",             (object?)request.FirstNameOperator1             ?? DBNull.Value),
                new("@FirstNameValue2",               (object?)request.FirstNameValue2               ?? DBNull.Value),
                new("@FirstNameOperator2",             (object?)request.FirstNameOperator2             ?? DBNull.Value),
                new("@FirstNameFilterConcatOperator",  (object?)request.FirstNameFilterConcatOperator  ?? DBNull.Value),
                new("@LastNameValue1",                (object?)request.LastNameValue1                ?? DBNull.Value),
                new("@LastNameOperator1",              (object?)request.LastNameOperator1              ?? DBNull.Value),
                new("@LastNameValue2",                (object?)request.LastNameValue2                ?? DBNull.Value),
                new("@LastNameOperator2",              (object?)request.LastNameOperator2              ?? DBNull.Value),
                new("@LastNameFilterConcatOperator",   (object?)request.LastNameFilterConcatOperator   ?? DBNull.Value),
                new("@EmailValue1",                   (object?)request.EmailValue1                   ?? DBNull.Value),
                new("@EmailOperator1",                 (object?)request.EmailOperator1                 ?? DBNull.Value),
                new("@EmailValue2",                   (object?)request.EmailValue2                   ?? DBNull.Value),
                new("@EmailOperator2",                 (object?)request.EmailOperator2                 ?? DBNull.Value),
                new("@EmailFilterConcatOperator",      (object?)request.EmailFilterConcatOperator      ?? DBNull.Value),
                new("@PhoneNumberValue1",             (object?)request.PhoneNumberValue1             ?? DBNull.Value),
                new("@PhoneNumberOperator1",           (object?)request.PhoneNumberOperator1           ?? DBNull.Value),
                new("@PhoneNumberValue2",             (object?)request.PhoneNumberValue2             ?? DBNull.Value),
                new("@PhoneNumberOperator2",           (object?)request.PhoneNumberOperator2           ?? DBNull.Value),
                new("@PhoneNumberFilterConcatOperator",(object?)request.PhoneNumberFilterConcatOperator?? DBNull.Value),
                new("@SSNValue1",                     (object?)request.SSNValue1                     ?? DBNull.Value),
                new("@SSNOperator1",                   (object?)request.SSNOperator1                   ?? DBNull.Value),
                new("@SSNValue2",                     (object?)request.SSNValue2                     ?? DBNull.Value),
                new("@SSNOperator2",                   (object?)request.SSNOperator2                   ?? DBNull.Value),
                new("@SSNFilterConcatOperator",        (object?)request.SSNFilterConcatOperator        ?? DBNull.Value),
                new("@StartDateOperator1",              (object?)request.StartDateOperator1             ?? DBNull.Value),
                new("@StartDateValue1",                 (object?)request.StartDateValue1                ?? DBNull.Value),
                new("@StartDateValue2",                 (object?)request.StartDateValue2                ?? DBNull.Value),
                new("@StartDateFilterConcatOperator",   (object?)request.StartDateFilterConcatOperator  ?? DBNull.Value),
                new("@StartDateOperator2",              (object?)request.StartDateOperator2             ?? DBNull.Value),
                new("@StartDateValue3",                 (object?)request.StartDateValue3                ?? DBNull.Value),
                new("@StartDateValue4",                 (object?)request.StartDateValue4                ?? DBNull.Value),
                new("@IsActive",            (object?)request.IsActive            ?? DBNull.Value),
            };

            List<StudentGridResult> rows = await Context.Database
                .SqlQueryRaw<StudentGridResult>(
                    "EXEC dbo.GetStudentGrid @PageNumber, @PageSize, @SortBy, @SortByOperator," +
                    " @FirstNameValue1, @FirstNameOperator1, @FirstNameValue2, @FirstNameOperator2, @FirstNameFilterConcatOperator," +
                    " @LastNameValue1, @LastNameOperator1, @LastNameValue2, @LastNameOperator2, @LastNameFilterConcatOperator," +
                    " @EmailValue1, @EmailOperator1, @EmailValue2, @EmailOperator2, @EmailFilterConcatOperator," +
                    " @PhoneNumberValue1, @PhoneNumberOperator1, @PhoneNumberValue2, @PhoneNumberOperator2, @PhoneNumberFilterConcatOperator," +
                    " @SSNValue1, @SSNOperator1, @SSNValue2, @SSNOperator2, @SSNFilterConcatOperator," +
                    " @StartDateOperator1, @StartDateValue1, @StartDateValue2, @StartDateFilterConcatOperator, @StartDateOperator2, @StartDateValue3, @StartDateValue4," +
                    " @IsActive",
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
