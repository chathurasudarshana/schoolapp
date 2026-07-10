namespace SCH.Tests.IdentityUsers
{
    using Moq;
    using SCH.Models.Auth.Entities;
    using SCH.Repositories.IdentityUsers;
    using SCH.Services.IdentityUsers;
    using Xunit;

    public class IdentityUsersServiceTests
    {
        private readonly Mock<IIdentityUsersRepository> _identityUsersRepository = new();
        private readonly IdentityUsersService _sut;

        public IdentityUsersServiceTests()
        {
            _sut = new IdentityUsersService(_identityUsersRepository.Object);
        }

        [Fact]
        public async Task GetBasicOnlyUsersAsync_WhenNoUsers_ReturnsEmptyList()
        {
            _identityUsersRepository
                .Setup(r => r.GetOnlyBasicUsersAsync())
                .ReturnsAsync(new List<ApplicationUser>());

            var result = await _sut.GetBasicOnlyUsersAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetBasicOnlyUsersAsync_ReturnsMappedUserDtos()
        {
            var users = new List<ApplicationUser>
            {
                new() { Id = 1, FirstName = "Jane", LastName = "Doe" },
                new() { Id = 2, FirstName = "Bob",  LastName = ""    }
            };
            _identityUsersRepository
                .Setup(r => r.GetOnlyBasicUsersAsync())
                .ReturnsAsync(users);

            var result = await _sut.GetBasicOnlyUsersAsync();

            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Id);
            Assert.Equal("Jane Doe", result[0].FullName);
            Assert.Equal(2, result[1].Id);
            Assert.Equal("Bob", result[1].FullName);
        }
    }
}
