using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using TechChallenge.Application.Users.Commands;
using TechChallenge.Application.Users.Handlers;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Models;

namespace TechChallenge.Application.Test.Users.Handlers;

public class UserServiceTest
{
    private static readonly IdentityOptions IdentityOptions = new() { Password = { RequireDigit = true, RequireLowercase = false, RequireNonAlphanumeric = true, RequireUppercase = false, RequiredLength = 8, RequiredUniqueChars = 0 }};
    private static readonly IOptions<IdentityOptions> Options = Microsoft.Extensions.Options.Options.Create(IdentityOptions);
    private static readonly IPasswordValidator<User>[] PasswordValidators = [new PasswordValidator<User>()];
    private static readonly Mock<IUserEmailStore<User>> Store = new();
    private static readonly IdentityErrorDescriber ErrorDescriber = new();
    
    private readonly Mock<UserManager<User>> _manager = new(Store.Object, Options, new PasswordHasher<User>(), null!, PasswordValidators, null!, ErrorDescriber, null!, null!);
    private readonly Mock<IUserRepository> _repository = new();
    private readonly UserService _handler;

    public UserServiceTest()
        => _handler = new UserService(_manager.Object, Store.Object, _repository.Object);
    
    [Fact]
    public async Task CreateAsync_WhenInvalidEmail_ShouldReturnError()
    {
        // Arrange
        var request = new CreateUserRequest { Email = "testeexemplo.com", Name = "Teste", Password = "Passw0rd@" };
        _manager.Setup(x => x.SupportsUserEmail).Returns(true);
        _manager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        
        // Act
        var result = await _handler.CreateAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Email 'testeexemplo.com' is invalid.", result.Exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenValidEmail_ShouldReturnSuccess()
    {
        // Arrange
        var request = new CreateUserRequest { Email = "teste@exemplo.com", Name = "Teste", Password = "Passw0rd@" };
        _manager.Setup(x => x.SupportsUserEmail).Returns(true);
        _manager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        
        // Act
        var result = await _handler.CreateAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task CreateAsync_WhenPasswordLessThanEightCharacters_ShouldReturnError()
    {
        // Arrange
        var request = new CreateUserRequest { Email = "teste@exemplo.com", Name = "Teste", Password = "Passw0@" };
        _manager.Setup(x => x.SupportsUserEmail).Returns(true);
        _manager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        
        // Act
        var result = await _handler.CreateAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Passwords must be at least 8 characters.", result.Exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenPasswordWithoutDigit_ShouldReturnError()
    {
        // Arrange
        var request = new CreateUserRequest { Email = "teste@exemplo.com", Name = "Teste", Password = "Password@" };
        _manager.Setup(x => x.SupportsUserEmail).Returns(true);
        _manager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        
        // Act
        var result = await _handler.CreateAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Passwords must have at least one digit ('0'-'9').", result.Exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenPasswordWithoutLetter_ShouldReturnError()
    {
        // Arrange
        var request = new CreateUserRequest { Email = "teste@exemplo.com", Name = "Teste", Password = "12345678" };
        _manager.Setup(x => x.SupportsUserEmail).Returns(true);
        _manager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        
        // Act
        var result = await _handler.CreateAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Passwords must have at least one non alphanumeric character.", result.Exception.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenValidPassword_ShouldReturnSuccess()
    {
        // Arrange
        var request = new CreateUserRequest { Email = "teste@exemplo.com", Name = "Teste", Password = "Passw0rd@" };
        _manager.Setup(x => x.SupportsUserEmail).Returns(true);
        _manager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        
        // Act
        var result = await _handler.CreateAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
    }
}