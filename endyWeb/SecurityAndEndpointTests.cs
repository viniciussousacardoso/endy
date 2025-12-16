using endy.EndpointDiscovery;
using endy.Services;

namespace endyWeb;

/// <summary>
/// Testes para as extensões de descoberta de endpoints
/// </summary>
public class EndpointDiscoveryTests
{
    [Fact]
    public void IEndpointDefinitionExtensions_ShouldBeInterface()
    {
        // Assert
        Assert.True(typeof(IEndpointDefinitionExtensions).IsInterface);
    }

    [Fact]
    public void IEndpointDefinitionExtensions_ShouldHaveDefineServicesMethod()
    {
        // Assert
        var method = typeof(IEndpointDefinitionExtensions).GetMethod("DefineServices");
        Assert.NotNull(method);
    }

    [Fact]
    public void IEndpointDefinitionExtensions_ShouldHaveDefineEndpointsMethod()
    {
        // Assert
        var method = typeof(IEndpointDefinitionExtensions).GetMethod("DefineEndpoints");
        Assert.NotNull(method);
    }
}

/// <summary>
/// Testes para validação de segurança e criptografia
/// </summary>
public class SecurityTests
{
    [Fact]
    public void CriptografiaService_ShouldNotReturnPlainTextPassword()
    {
        // Arrange
        var service = new CriptografiaService();
        string plainPassword = "MyPassword123";
        byte[] salt = new byte[16];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Act
        var encrypted = service.CriptografarSenha(plainPassword, salt);

        // Assert
        Assert.NotEqual(plainPassword, encrypted);
    }

    [Fact]
    public void CriptografiaService_ShouldGenerateDifferentSaltsEachTime()
    {
        // Arrange
        var service = new CriptografiaService();
        var salts = new System.Collections.Generic.List<byte[]>();

        // Act
        for (int i = 0; i < 5; i++)
        {
            byte[] salt = new byte[16];
            salts.Add(service.GeraSalt(salt));
        }

        // Assert - All salts should be different
        for (int i = 0; i < salts.Count; i++)
        {
            for (int j = i + 1; j < salts.Count; j++)
            {
                Assert.NotEqual(salts[i], salts[j]);
            }
        }
    }

    [Fact]
    public void CriptografiaService_ShouldProduceConsistentHashWithSameSaltAndPassword()
    {
        // Arrange
        var service = new CriptografiaService();
        string password = "TestPassword";
        byte[] salt = new byte[16] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        // Act
        var hash1 = service.CriptografarSenha(password, salt);
        var hash2 = service.CriptografarSenha(password, salt);

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void CriptografiaService_ShouldProduceDifferentHashesWithDifferentSalts()
    {
        // Arrange
        var service = new CriptografiaService();
        string password = "TestPassword";
        byte[] salt1 = new byte[16] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        byte[] salt2 = new byte[16] { 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };

        // Act
        var hash1 = service.CriptografarSenha(password, salt1);
        var hash2 = service.CriptografarSenha(password, salt2);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }
}

/// <summary>
/// Testes de performance e escalabilidade
/// </summary>
public class PerformanceTests
{
    [Fact]
    public void CriptografiaService_EncryptionShouldCompleteInReasonableTime()
    {
        // Arrange
        var service = new CriptografiaService();
        string password = "TestPassword";
        byte[] salt = new byte[16];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        var sw = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = service.CriptografarSenha(password, salt);

        sw.Stop();

        // Assert - Encryption should complete within 5 seconds
        Assert.True(sw.ElapsedMilliseconds < 5000, $"Encryption took {sw.ElapsedMilliseconds}ms");
        Assert.NotEmpty(result);
    }

    [Fact]
    public void CriptografiaService_SaltGenerationShouldBeFast()
    {
        // Arrange
        var service = new CriptografiaService();
        byte[] salt = new byte[16];

        var sw = System.Diagnostics.Stopwatch.StartNew();

        // Act
        service.GeraSalt(salt);

        sw.Stop();

        // Assert - Salt generation should be very fast
        Assert.True(sw.ElapsedMilliseconds < 100, $"Salt generation took {sw.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void CriptografiaService_UserCryptographyShouldCompleteInReasonableTime()
    {
        // Arrange
        var service = new CriptografiaService();

        var sw = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = service.CriptografaUsuario("testuser", "TestPassword123");

        sw.Stop();

        // Assert
        Assert.True(sw.ElapsedMilliseconds < 5000, $"User cryptography took {sw.ElapsedMilliseconds}ms");
        Assert.NotNull(result);
    }
}
