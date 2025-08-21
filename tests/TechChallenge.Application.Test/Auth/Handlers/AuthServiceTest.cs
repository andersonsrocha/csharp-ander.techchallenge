using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using TechChallenge.Application.Auth.Commands;
using TechChallenge.Application.Auth.Handlers;
using TechChallenge.Domain.Models;
using TechChallenge.Security;

namespace TechChallenge.Application.Test.Auth.Handlers;

public class AuthServiceTest
{
    private static readonly IPasswordValidator<User>[] PasswordValidators = [new PasswordValidator<User>()];
    private static readonly Mock<IUserEmailStore<User>> Store = new();
    private static readonly IdentityErrorDescriber ErrorDescriber = new();
    private static readonly Mock<UserManager<User>> Manager = new(Store.Object, null!, new PasswordHasher<User>(), null!, PasswordValidators, null!, ErrorDescriber, null!, null!);
    private static readonly Mock<IHttpContextAccessor> ContextAccessor = new();
    private static readonly Mock<IUserClaimsPrincipalFactory<User>> ClaimsFactory = new();
    
    private readonly Mock<SignInManager<User>> _signInManager = new(Manager.Object, ContextAccessor.Object, ClaimsFactory.Object, null!, null!, null!, null!);
    private readonly Mock<IJwtService> _jwtService = new();
    private readonly AuthService _handler;

    public AuthServiceTest()
        => _handler = new AuthService(_signInManager.Object, _jwtService.Object);
    
    [Fact]
    public async Task Login_WhenNotFoundEmail_ShouldReturnError()
    {
        // Arrange
        var request = new LoginRequest { Email = "teste@exemplo.com", Password = "Passw0rd@" };
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
        var request = new LoginRequest { Email = "teste@exemplo.com", Password = "Passw0rd@" };
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
        var request = new LoginRequest { Email = "teste@exemplo.com", Password = "Passw0rd@" };
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
        var request = new LoginRequest { Email = "teste@exemplo.com", Password = "Passw0rd@" };
        Manager.Setup(x => x.FindByEmailAsync("teste@exemplo.com")).Returns(Task.FromResult<User?>(user));
        _signInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), "Passw0rd@", false, true)).ReturnsAsync(SignInResult.Success);
        _jwtService.Setup(x => x.Generate(It.IsAny<User>(), It.IsAny<IEnumerable<string>>())).Returns("jwt-token");
        
        // Act
        var result = await _handler.Login(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("jwt-token", result.Value.Token);
    }
}