namespace SCH.Repositories.Courses
{
    using Microsoft.EntityFrameworkCore;
    using SCH.Models.Courses.Entities;
    using SCH.Repositories.Common;
    using SCH.Repositories.DbContexts;
    using System.Collections.Generic;

    internal class CoursesRepository : BaseRepository<Course, SCHContext>, ICoursesRepository
    {
        public CoursesRepository(SCHContext context) : base(context)
        {
        }

        public async Task<List<Course>> GetCoursesAsync()
        {

            List<Course> courses = await Context
                .Course
                .AsNoTracking()
                .ToListAsync();

            return courses;
        }

        public async Task<List<Course>> GetCoursesAsync(
            List<int> coursesIds)
        {

            List<Course> courses = await Context
                .Course
                .AsNoTracking()
                .Where(c => coursesIds.Contains(c.Id))
                .ToListAsync();

            return courses;
        }

        public async Task<Course?> GetCourseAsync(int id)
        {
            Course? course = await Context
                .Course
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.Id == id);

            return course;
        }

        public async Task InsertCourseAsync(Course course)
        {                                     
            await Context.Course.AddAsync(course);
        }

        public void UpdateAsync(Course course)
        {
            UpdateWithConcurrency(course);
        }

        public async Task DeleteCourseAsync(int id)
        {

            Course? courseEntity = await Context
                .Course.SingleOrDefaultAsync(s => s.Id == id);

            if (courseEntity != null)
            {
                Context.Course.Remove(courseEntity);
            }
        }
    }
}
