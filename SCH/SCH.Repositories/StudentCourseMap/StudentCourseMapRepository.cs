
namespace SCH.Repositories.StudentCourseMap
{
    using Microsoft.EntityFrameworkCore;
    using SCH.Models.StudentCourseMap.Entities;
    using SCH.Repositories.DbContexts;
    using System.Collections.Generic;

    internal class StudentCourseMapRepository : IStudentCourseMapRepository
    {
        private readonly SCHContext context;

        public StudentCourseMapRepository(SCHContext context)
        {
            this.context = context;
        }

        public async Task<StudentCourseMap?> GetStudentCourseMapAsync(
            int studentId, int courseId)
        {
            StudentCourseMap? studentCourseMap = await context
                .StudentCourseMap
                .FirstOrDefaultAsync(
                    sc => sc.StudentId == studentId 
                        && sc.CourseId == courseId);

            return studentCourseMap;
        }

        public async Task<List<StudentCourseMap>> GetStudentCourseMapsByStudentAsync(
            int studentId)
        {
            List<StudentCourseMap> studentCourseMaps = await context
                .StudentCourseMap
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .Where(sc => sc.StudentId == studentId)
                .ToListAsync();

            return studentCourseMaps;
        }

        public async Task InsertStudentCourseMapAsync(
            StudentCourseMap studentCourseMap)
        {
            await context.StudentCourseMap
                .AddAsync(studentCourseMap);
        }

        public async Task DeleteStudentCourseMapAsync(
            int studentId, int courseId)
        {
            StudentCourseMap? studentCourseMap = await context
                .StudentCourseMap
                .SingleOrDefaultAsync(sc => sc.StudentId == studentId
                    && sc.CourseId == courseId);

            if (studentCourseMap != null)
            {
                context.StudentCourseMap.Remove(studentCourseMap);
            }
        }

    }
}
