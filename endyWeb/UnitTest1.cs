using System.Security.Cryptography;
using endy.Model;
using endy.Services;

namespace endyWeb;

/// <summary>
/// Testes para o serviço de criptografia
/// </summary>
public class CriptografiaServiceTests
{
    [Fact]
    public void GeraSalt_ShouldReturnByteArray_WhenCalled()
    {
        // Arrange
        var criptografiaService = new CriptografiaService();
        byte[] salt = new byte[16];

        // Act
        var result = criptografiaService.GeraSalt(salt);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(16, result.Length);
    }

    [Fact]
    public void GeraSalt_ShouldGenerateRandomBytes()
    {
        // Arrange
        var criptografiaService = new CriptografiaService();
        byte[] salt1 = new byte[16];
        byte[] salt2 = new byte[16];

        // Act
        var result1 = criptografiaService.GeraSalt(salt1);
        var result2 = criptografiaService.GeraSalt(salt2);

        // Assert - Dois salts aleatórios não devem ser idênticos
        Assert.NotEqual(result1, result2);
    }

    [Fact]
    public void CriptografarSenha_ShouldReturnEncryptedPassword()
    {
        // Arrange
        var criptografiaService = new CriptografiaService();
        string senha = "TestPassword123";
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Act
        var result = criptografiaService.CriptografarSenha(senha, salt);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void CriptografarSenha_ShouldReturnDifferentHashForDifferentPasswords()
    {
        // Arrange
        var criptografiaService = new CriptografiaService();
        string senha1 = "TestPassword123";
        string senha2 = "DifferentPassword456";
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Act
        var result1 = criptografiaService.CriptografarSenha(senha1, salt);
        var result2 = criptografiaService.CriptografarSenha(senha2, salt);

        // Assert
        Assert.NotEqual(result1, result2);
    }

    [Fact]
    public void CriptografarSenha_ShouldProduceDeterministicOutput()
    {
        // Arrange
        var criptografiaService = new CriptografiaService();
        string senha = "TestPassword123";
        byte[] salt = new byte[16] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        // Act
        var result1 = criptografiaService.CriptografarSenha(senha, salt);
        var result2 = criptografiaService.CriptografarSenha(senha, salt);

        // Assert - Mesma senha e salt devem produzir mesmo hash
        Assert.Equal(result1, result2);
    }

    [Fact]
    public void CriptografaUsuario_ShouldReturnUsuarioModel()
    {
        // Arrange
        var criptografiaService = new CriptografiaService();
        string usuario = "testuser";
        string pass = "TestPassword123";

        // Act
        var result = criptografiaService.CriptografaUsuario(usuario, pass);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<UsuarioModel>(result);
        Assert.Equal(usuario, result.Usuario);
        Assert.NotNull(result.Senha);
        Assert.NotNull(result.Salt);
    }

    [Fact]
    public void CriptografaUsuario_ShouldCreateDifferentEncryptionForSamePasswordWithDifferentCalls()
    {
        // Arrange
        var criptografiaService = new CriptografiaService();
        string usuario = "testuser";
        string pass = "TestPassword123";

        // Act
        var result1 = criptografiaService.CriptografaUsuario(usuario, pass);
        var result2 = criptografiaService.CriptografaUsuario(usuario, pass);

        // Assert - Diferentes salts devem resultar em senhas criptografadas diferentes
        Assert.NotEqual(result1.Senha, result2.Senha);
        Assert.NotEqual(result1.Salt, result2.Salt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("senha123")]
    [InlineData("SenhaComCaracteresEspeciais!@#$%")]
    public void CriptografarSenha_ShouldHandleVariousPasswords(string senha)
    {
        // Arrange
        var criptografiaService = new CriptografiaService();
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Act
        var result = criptografiaService.CriptografarSenha(senha, salt);

        // Assert
        Assert.NotNull(result);
    }

    [Theory]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(32)]
    public void GeraSalt_ShouldReturnConsistentLength(int saltLength)
    {
        // Arrange
        var criptografiaService = new CriptografiaService();
        byte[] salt = new byte[saltLength];

        // Act
        var result = criptografiaService.GeraSalt(salt);

        // Assert
        Assert.Equal(saltLength, result.Length);
    }
}

/// <summary>
/// Testes para o modelo UsuarioModel
/// </summary>
public class UsuarioModelTests
{
    [Fact]
    public void UsuarioModel_ConstructorWithTwoParams_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        string senha = "EncryptedPassword";
        byte[] salt = new byte[16] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        // Act
        var usuario = new UsuarioModel(senha, salt);

        // Assert
        Assert.Equal(senha, usuario.Senha);
        Assert.Equal(salt, usuario.Salt);
    }

    [Fact]
    public void UsuarioModel_ConstructorWithThreeParams_ShouldSetAllPropertiesCorrectly()
    {
        // Arrange
        string senha = "EncryptedPassword";
        byte[] salt = new byte[16] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        string usuario = "testuser";

        // Act
        var usuarioModel = new UsuarioModel(senha, salt, usuario);

        // Assert
        Assert.Equal(senha, usuarioModel.Senha);
        Assert.Equal(salt, usuarioModel.Salt);
        Assert.Equal(usuario, usuarioModel.Usuario);
    }

    [Fact]
    public void UsuarioModel_ShouldAllowNullUsuario_WhenNotProvided()
    {
        // Arrange
        string senha = "EncryptedPassword";
        byte[] salt = new byte[16] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        // Act
        var usuario = new UsuarioModel(senha, salt);

        // Assert
        Assert.Null(usuario.Usuario);
    }

    [Fact]
    public void UsuarioModel_ShouldAllowModifyingProperties()
    {
        // Arrange
        var usuario = new UsuarioModel("inicialSenha", new byte[16]);
        string newSenha = "novaSenha";
        byte[] newSalt = new byte[16] { 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };

        // Act
        usuario.Senha = newSenha;
        usuario.Salt = newSalt;
        usuario.Usuario = "novoUsuario";

        // Assert
        Assert.Equal(newSenha, usuario.Senha);
        Assert.Equal(newSalt, usuario.Salt);
        Assert.Equal("novoUsuario", usuario.Usuario);
    }

    [Theory]
    [InlineData("user1")]
    [InlineData("user2")]
    [InlineData("USUARIO")]
    public void UsuarioModel_ShouldAcceptVariousUsernames(string username)
    {
        // Arrange & Act
        var usuario = new UsuarioModel("senha", new byte[16], username);

        // Assert
        Assert.Equal(username, usuario.Usuario);
    }
}

/// <summary>
/// Testes para o modelo ClienteModel
/// </summary>
public class ClienteModelTests
{
    [Fact]
    public void ClienteModel_ShouldAllowSettingAllProperties()
    {
        // Arrange & Act
        var cliente = new ClienteModel
        {
            IdCliente = 1,
            Email = "test@example.com",
            Nome = "Test User",
            Visualizado = false,
            telefone = "123456789",
            motivo_contato = "Test reason"
        };

        // Assert
        Assert.Equal(1, cliente.IdCliente);
        Assert.Equal("test@example.com", cliente.Email);
        Assert.Equal("Test User", cliente.Nome);
        Assert.False(cliente.Visualizado);
        Assert.Equal("123456789", cliente.telefone);
        Assert.Equal("Test reason", cliente.motivo_contato);
    }

    [Fact]
    public void ClienteModel_ShouldHaveDefaultValuesForBoolProperty()
    {
        // Arrange & Act
        var cliente = new ClienteModel
        {
            IdCliente = 1,
            Email = "test@example.com",
            Nome = "Test User"
        };

        // Assert
        Assert.False(cliente.Visualizado);
    }

    [Fact]
    public void ClienteModel_ShouldAllowNullableEmailAndNome()
    {
        // Arrange & Act
        var cliente = new ClienteModel
        {
            IdCliente = 1,
            Email = null,
            Nome = null
        };

        // Assert
        Assert.Null(cliente.Email);
        Assert.Null(cliente.Nome);
    }

    [Theory]
    [InlineData("email@domain.com")]
    [InlineData("test.email@example.co.uk")]
    [InlineData("user+tag@example.com")]
    public void ClienteModel_ShouldAcceptVariousEmails(string email)
    {
        // Arrange & Act
        var cliente = new ClienteModel { Email = email };

        // Assert
        Assert.Equal(email, cliente.Email);
    }

    [Theory]
    [InlineData("João Silva")]
    [InlineData("José Santos")]
    [InlineData("María García")]
    public void ClienteModel_ShouldAcceptVariousNames(string nome)
    {
        // Arrange & Act
        var cliente = new ClienteModel { Nome = nome };

        // Assert
        Assert.Equal(nome, cliente.Nome);
    }

    [Fact]
    public void ClienteModel_ShouldAllowToggleVisualization()
    {
        // Arrange
        var cliente = new ClienteModel { Visualizado = false };

        // Act
        cliente.Visualizado = true;

        // Assert
        Assert.True(cliente.Visualizado);
    }

    [Fact]
    public void ClienteModel_ShouldAllowMultipleClientsWithDifferentIds()
    {
        // Arrange & Act
        var cliente1 = new ClienteModel { IdCliente = 1, Nome = "Cliente 1" };
        var cliente2 = new ClienteModel { IdCliente = 2, Nome = "Cliente 2" };

        // Assert
        Assert.NotEqual(cliente1.IdCliente, cliente2.IdCliente);
    }
}

/// <summary>
/// Testes de integração e casos extremos
/// </summary>
public class EdgeCaseTests
{
    [Fact]
    public void CriptografarSenha_ShouldHandleEmptyPassword()
    {
        // Arrange
        var criptografiaService = new CriptografiaService();
        string senha = "";
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Act
        var result = criptografiaService.CriptografarSenha(senha, salt);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void CriptografarSenha_ShouldHandleLongPassword()
    {
        // Arrange
        var criptografiaService = new CriptografiaService();
        string senha = new string('a', 500);
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Act
        var result = criptografiaService.CriptografarSenha(senha, salt);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void CriptografarSenha_ShouldHandleSpecialCharacters()
    {
        // Arrange
        var criptografiaService = new CriptografiaService();
        string senha = "P@ssw0rd!#$%^&*()_+-=[]{}|;:',.<>?";
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Act
        var result = criptografiaService.CriptografarSenha(senha, salt);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void CriptografarSenha_ShouldHandleUnicodeCharacters()
    {
        // Arrange
        var criptografiaService = new CriptografiaService();
        string senha = "Señor123!";
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Act
        var result = criptografiaService.CriptografarSenha(senha, salt);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void CriptografaUsuario_ShouldCreateUsableEncryption()
    {
        // Arrange
        var criptografiaService = new CriptografiaService();
        string usuario = "testuser";
        string pass = "TestPassword123";

        // Act
        var result = criptografiaService.CriptografaUsuario(usuario, pass);

        // Assert
        Assert.NotNull(result.Senha);
        Assert.NotEmpty(result.Senha);
        Assert.True(result.Senha.Length > 0);
        
        // Verifica se é Base64 válido
        try
        {
            byte[] decoded = Convert.FromBase64String(result.Senha);
            Assert.True(decoded.Length > 0);
        }
        catch
        {
            Assert.Fail("Encrypted password should be valid Base64");
        }
    }

    [Fact]
    public void ClienteModel_ShouldHandleLongContactReason()
    {
        // Arrange
        var cliente = new ClienteModel();
        string longReason = new string('a', 500);

        // Act
        cliente.motivo_contato = longReason;

        // Assert
        Assert.Equal(longReason, cliente.motivo_contato);
    }

    [Fact]
    public void ClienteModel_ShouldHandleLongEmail()
    {
        // Arrange
        var cliente = new ClienteModel();
        string longEmail = new string('a', 100) + "@example.com";

        // Act
        cliente.Email = longEmail;

        // Assert
        Assert.Equal(longEmail, cliente.Email);
    }

    [Fact]
    public void UsuarioModel_ShouldHandleLongPassword()
    {
        // Arrange
        var usuario = new UsuarioModel(new string('a', 300), new byte[16]);

        // Assert
        Assert.Equal(300, usuario.Senha.Length);
    }
}