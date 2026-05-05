namespace SCH.Mappings.Students
{
    using AutoMapper;
    using SCH.Models.StudentCourseMap.ClientDtos;
    using SCH.Models.StudentCourseMap.Entities;
    using SCH.Models.Students.ClientDtos;
    using SCH.Models.Students.Entities;

    public class StudentProfile : Profile, IProfile
    {
        public StudentProfile()
        {
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.Courses, opt => opt.MapFrom(src => src.StudentCourseMaps));

            CreateMap<StudentCourseMap, StudentCourseMapDto>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course!.Name))
                .ForMember(dest => dest.StudentFirstName, opt => opt.MapFrom(src => src.Student!.FirstName))
                .ForMember(dest => dest.StudentLastName, opt => opt.MapFrom(src => src.Student!.LastName));
        }
    }
}
