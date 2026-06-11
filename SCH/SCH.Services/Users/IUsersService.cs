namespace SCH.Services.Users
{
    using SCH.Models.Users.ClientDtos;

    public interface IUsersService: IService
    {
        Task<List<UserDomainDto>> GetBasicOnlyUsersAsync();
    }
}
