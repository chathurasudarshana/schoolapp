namespace SCH.Mappings.Students
{
    using AutoMapper;
    using SCH.Models.StudentCourseMap.ClientDtos;
    using SCH.Models.StudentCourseMap.Entities;
    using SCH.Models.Students.ClientDtos;
    using SCH.Models.Students.DbDtos;
    using SCH.Models.Students.Entities;
    using SCH.Models.Users.ClientDtos;
    using SCH.Models.Users.Entities;

    public class StudentProfile : Profile, IProfile
    {
        public StudentProfile()
        {
            CreateMap<Student, StudentDto>()
                .ForMember(
                    dest => dest.Courses, 
                    opt => opt.MapFrom(src => src.StudentCourseMaps));

            CreateMap<User, UserDomainDto>();

            CreateMap<StudentGridResult, Student>()
                .ForMember(
                    dest => dest.StudentCourseMaps, 
                    opt => opt.MapFrom(src => new List<StudentCourseMap>()));

            CreateMap<StudentCourseMap, StudentCourseMapDto>()
                .ForMember(
                    dest => dest.CourseName, 
                    opt => opt.MapFrom(src => src.Course!.Name))
                .ForMember(
                    dest => dest.StudentFirstName, 
                    opt => opt.MapFrom(src => src.Student!.FirstName))
                .ForMember(
                    dest => dest.StudentLastName, 
                    opt => opt.MapFrom(src => src.Student!.LastName));
        }
    }
}
