namespace SCH.Repositories.IdentityUsers
{
    using SCH.Models.Auth.Entities;

    /// <summary>
    /// Repository interface for identity user queries
    /// </summary>
    public interface IIdentityUsersRepository : IRepository
    {
        /// <summary>
        /// Gets users that have only the Basic role assigned (no Admin, Teacher, or Student roles)
        /// </summary>
        Task<List<ApplicationUser>> GetOnlyBasicUsersAsync();
    }
}

