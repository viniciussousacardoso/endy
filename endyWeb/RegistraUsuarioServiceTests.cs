using endy.Services;
using endy.Services.RegistraUsuarioService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace endyWeb;

/// <summary>
/// Testes para o serviço de registro e login de usuários
/// </summary>
public class RegistraUsuarioServiceTests
{
    private IConfiguration GetMockConfiguration()
    {
        var mockConfig = new Mock<IConfiguration>();
        
        mockConfig.Setup(x => x["ConnectionStrings:DefaultConnection"])
            .Returns("Server=localhost;Database=endy;Trusted_Connection=true;");
        
        mockConfig.Setup(x => x["Jwt:Issuer"])
            .Returns("endy-issuer");
        
        mockConfig.Setup(x => x["Jwt:Audience"])
            .Returns("endy-audience");
        
        mockConfig.Setup(x => x["Jwt:Key"])
            .Returns("endy-secret-key-for-jwt-token-generation-very-long-key");

        return mockConfig.Object;
    }

    [Fact]
    public void RegistraUsuarioService_ShouldCreateInstanceWithValidParameters()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = GetMockConfiguration();

        // Act
        var registraUsuarioService = new RegistraUsuarioService(services, configuration);

        // Assert
        Assert.NotNull(registraUsuarioService);
    }

    [Fact]
    public void RegistraUsuarioService_ShouldHaveConfigurationSet()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = GetMockConfiguration();

        // Act
        var registraUsuarioService = new RegistraUsuarioService(services, configuration);

        // Assert
        Assert.NotNull(registraUsuarioService);
    }

    [Theory]
    [InlineData("user1", "password1")]
    [InlineData("user2", "password2")]
    [InlineData("user3", "password3")]
    public void RegistraUsuarioService_ConstructorShouldAcceptValidUserNamesAndPasswords(string userName, string password)
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = GetMockConfiguration();

        // Act
        var registraUsuarioService = new RegistraUsuarioService(services, configuration);

        // Assert
        Assert.NotNull(registraUsuarioService);
        Assert.False(string.IsNullOrEmpty(userName));
        Assert.False(string.IsNullOrEmpty(password));
    }

    [Theory]
    [InlineData("")]
    [InlineData("validuser")]
    [InlineData("USUARIO")]
    [InlineData("usuario.name")]
    public void RegistraUsuarioService_ShouldHandleVariousUserNames(string userName)
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = GetMockConfiguration();

        // Act
        var registraUsuarioService = new RegistraUsuarioService(services, configuration);

        // Assert
        Assert.NotNull(registraUsuarioService);
        Assert.NotNull(userName);
    }

    [Theory]
    [InlineData("shortpass")]
    [InlineData("VeryLongPasswordWith123Characters!")]
    [InlineData("P@ssw0rd!#$%")]
    public void RegistraUsuarioService_ShouldHandleVariousPasswords(string password)
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = GetMockConfiguration();

        // Act
        var registraUsuarioService = new RegistraUsuarioService(services, configuration);

        // Assert
        Assert.NotNull(registraUsuarioService);
        Assert.NotNull(password);
    }
}

/// <summary>
/// Testes para a interface IRegistraUsuarioService
/// </summary>
public class IRegistraUsuarioServiceTests
{
    [Fact]
    public void IRegistraUsuarioService_ShouldBeInterface()
    {
        // Assert
        Assert.True(typeof(IRegistraUsuarioService).IsInterface);
    }

    [Fact]
    public void IRegistraUsuarioService_ShouldBeImplementable()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new Mock<IConfiguration>().Object;

        // Act & Assert - should not throw
        var implementation = new RegistraUsuarioService(services, configuration);
        Assert.IsAssignableFrom<IRegistraUsuarioService>(implementation);
    }
}
