namespace SCH.Mappings.Teachers
{
    using AutoMapper;
    using SCH.Models.Teachers.ClientDtos;
    using SCH.Models.Teachers.Entities;

    public class TeacherProfile : Profile, IProfile
    {
        public TeacherProfile()
        {
            CreateMap<Teacher, TeacherDto>();
        }
    }
}
