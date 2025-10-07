using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TechChallengeUsers.Application.Users.Commands;
using TechChallengeUsers.Application.Users.Handlers;
using TechChallengeUsers.Domain.Interfaces;
using TechChallengeUsers.Domain.Models;
using TechChallengeUsers.Elasticsearch;
using TechChallengeUsers.Security;

namespace TechChallengeUsers.Application.Test.Users.Handlers;

public class UserServiceTest
{
    private static readonly IPasswordValidator<User>[] PasswordValidators = [new PasswordValidator<User>()];
    private static readonly Mock<IUserEmailStore<User>> Store = new();
    private static readonly IdentityErrorDescriber ErrorDescriber = new();
    private static readonly Mock<UserManager<User>> Manager = new(Store.Object, null!, new PasswordHasher<User>(), null!, PasswordValidators, null!, ErrorDescriber, null!, null!);
    private static readonly Mock<IHttpContextAccessor> ContextAccessor = new();
    private static readonly Mock<IUserClaimsPrincipalFactory<User>> ClaimsFactory = new();
    private static readonly IdentityOptions IdentityOptions = new() { Password = { RequireDigit = true, RequireLowercase = false, RequireNonAlphanumeric = true, RequireUppercase = false, RequiredLength = 8, RequiredUniqueChars = 0 }};
    private static readonly IOptions<IdentityOptions> Options = Microsoft.Extensions.Options.Options.Create(IdentityOptions);
    
    private readonly Mock<SignInManager<User>> _signInManager = new(Manager.Object, ContextAccessor.Object, ClaimsFactory.Object, null!, null!, null!, null!);
    private readonly Mock<UserManager<User>> _manager = new(Store.Object, Options, new PasswordHasher<User>(), null!, PasswordValidators, null!, ErrorDescriber, null!, null!);
    private readonly Mock<IJwtService> _jwtService = new();
    private readonly Mock<IElasticClient<UserLog>> _elasticClient = new();
    private readonly Mock<IUserRepository> _repository = new();
    private readonly Mock<ILogger<UserService>> _logger = new();
    private readonly UserService _handler;

    public UserServiceTest()
        => _handler = new UserService(_signInManager.Object, _manager.Object, Store.Object, _jwtService.Object, _elasticClient.Object, _repository.Object, _logger.Object);
    
    [Fact]
    public async Task Login_WhenNotFoundEmail_ShouldReturnError()
    {
        // Arrange
        var request = new LoginUserRequest { Email = "teste@exemplo.com", Password = "Passw0rd@" };
        Manager.Setup(x => x.FindByEmailAsync("teste@exemplo.com")).Returns(Task.FromResult<User?>(null));
        
        // Act
        var result = await _handler.Login(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("User not found.", result.Exception.Message);
    }
    
    [Fact]
    public async Task Login_WhenInvalidPassword_ShouldReturnError()
    {
        // Arrange
        var user = new User();
        var request = new LoginUserRequest { Email = "teste@exemplo.com", Password = "Passw0rd@" };
        Manager.Setup(x => x.FindByEmailAsync("teste@exemplo.com")).Returns(Task.FromResult<User?>(user));
        _signInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), "Passw0rd@", false, true)).ReturnsAsync(SignInResult.Failed);
        
        // Act
        var result = await _handler.Login(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("User or password is incorrect.", result.Exception.Message);
    }
    
    [Fact]
    public async Task Login_WhenValidEmailAndPassword_ShouldReturnSuccess()
    {
        // Arrange
        var user = new User();
        var request = new LoginUserRequest { Email = "teste@exemplo.com", Password = "Passw0rd@" };
        Manager.Setup(x => x.FindByEmailAsync("teste@exemplo.com")).Returns(Task.FromResult<User?>(user));
        _signInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), "Passw0rd@", false, true)).ReturnsAsync(SignInResult.Success);
        
        // Act
        var result = await _handler.Login(request);

        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task LoginJwtGenerate_WhenValidEmailAndPassword_ShouldReturnSuccess()
    {
        // Arrange
        var user = new User();
        var request = new LoginUserRequest { Email = "teste@exemplo.com", Password = "Passw0rd@" };
        Manager.Setup(x => x.FindByEmailAsync("teste@exemplo.com")).Returns(Task.FromResult<User?>(user));
        _signInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), "Passw0rd@", false, true)).ReturnsAsync(SignInResult.Success);
        _jwtService.Setup(x => x.Generate(It.IsAny<User>(), It.IsAny<IEnumerable<string>>())).Returns("jwt-token");
        
        // Act
        var result = await _handler.Login(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("jwt-token", result.Value.Token);
    }
    
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