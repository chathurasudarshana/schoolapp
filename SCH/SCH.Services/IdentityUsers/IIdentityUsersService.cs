namespace SCH.Services.IdentityUsers
{
    using SCH.Models.Users.ClientDtos;

    public interface IIdentityUsersService : IService
    {
        Task<List<UserDomainDto>> GetBasicOnlyUsersAsync();
    }
}
