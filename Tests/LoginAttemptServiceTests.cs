using Core.Options;
using FluentAssertions;
using Infrastructure.Security;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Xunit;

namespace Tests;

public class LoginAttemptServiceTests
{
    #region Setup

    private static LoginAttemptService CreateService(LoginAttemptOptions? opts = null)
        => new(new FakeCache(), Options.Create(opts ?? DefaultOptions()));

    private static LoginAttemptOptions DefaultOptions() => new()
    {
        WindowMinutes = 60,
        Steps =
        [
            new() { AfterAttempts = 6,  LockoutSeconds = 20   },
            new() { AfterAttempts = 8,  LockoutSeconds = 60   },
            new() { AfterAttempts = 10, LockoutSeconds = 300  },
            new() { AfterAttempts = 13, LockoutSeconds = 1800 },
        ]
    };

    // Implementación in-memory de IDistributedCache para tests (sin paquetes extra).
    private sealed class FakeCache : IDistributedCache
    {
        private readonly Dictionary<string, (byte[] Value, DateTimeOffset? Expires)> _store = new();

        public byte[]? Get(string key)
        {
            if (!_store.TryGetValue(key, out var entry)) return null;
            if (entry.Expires.HasValue && entry.Expires.Value < DateTimeOffset.UtcNow)
            {
                _store.Remove(key);
                return null;
            }
            return entry.Value;
        }

        public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
            => Task.FromResult(Get(key));

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            var expires = options.AbsoluteExpirationRelativeToNow.HasValue
                ? DateTimeOffset.UtcNow.Add(options.AbsoluteExpirationRelativeToNow.Value)
                : options.AbsoluteExpiration;
            _store[key] = (value, expires);
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            Set(key, value, options);
            return Task.CompletedTask;
        }

        public void Refresh(string key) { }
        public Task RefreshAsync(string key, CancellationToken token = default) => Task.CompletedTask;
        public void Remove(string key) => _store.Remove(key);
        public Task RemoveAsync(string key, CancellationToken token = default) { Remove(key); return Task.CompletedTask; }
    }

    #endregion

    // --- IsLockedOutAsync ---

    [Fact]
    public async Task IsLockedOutAsync_SinFallos_RetornaNull()
    {
        var svc = CreateService();
        var result = await svc.IsLockedOutAsync("user:test");
        result.Should().BeNull();
    }

    [Fact]
    public async Task IsLockedOutAsync_TrasBloquearse_RetornaTiempoRestante()
    {
        var svc = CreateService();
        for (var i = 0; i < 6; i++) await svc.RegisterFailureAsync("key1");

        var remaining = await svc.IsLockedOutAsync("key1");

        remaining.Should().NotBeNull();
        remaining!.Value.TotalSeconds.Should().BeGreaterThan(0).And.BeLessThanOrEqualTo(20);
    }

    // --- Progresión de intentos ---

    [Fact]
    public async Task PrimerFallo_NoBloquea_Retorna5Restantes()
    {
        var svc = CreateService();
        var result = await svc.RegisterFailureAsync("key1");

        result.IsNowLocked.Should().BeFalse();
        result.LockoutDuration.Should().BeNull();
        result.AttemptsBeforeNextLock.Should().Be(5); // 6 - 1 = 5
    }

    [Fact]
    public async Task QuintoFallo_NoBloquea_Retorna1Restante()
    {
        var svc = CreateService();
        for (var i = 0; i < 4; i++) await svc.RegisterFailureAsync("key1");
        var result = await svc.RegisterFailureAsync("key1");

        result.IsNowLocked.Should().BeFalse();
        result.AttemptsBeforeNextLock.Should().Be(1); // 6 - 5 = 1
    }

    // --- Bloqueos progresivos ---

    [Fact]
    public async Task SextoFallo_BloquePor20Segundos()
    {
        var svc = CreateService();
        for (var i = 0; i < 5; i++) await svc.RegisterFailureAsync("key1");
        var result = await svc.RegisterFailureAsync("key1");

        result.IsNowLocked.Should().BeTrue();
        result.LockoutDuration.Should().Be(TimeSpan.FromSeconds(20));
    }

    [Fact]
    public async Task OctavoFallo_BloquePor60Segundos()
    {
        var svc = CreateService();
        for (var i = 0; i < 7; i++) await svc.RegisterFailureAsync("key1");
        var result = await svc.RegisterFailureAsync("key1");

        result.IsNowLocked.Should().BeTrue();
        result.LockoutDuration.Should().Be(TimeSpan.FromSeconds(60));
    }

    [Fact]
    public async Task DecimoFallo_BloquePor300Segundos()
    {
        var svc = CreateService();
        for (var i = 0; i < 9; i++) await svc.RegisterFailureAsync("key1");
        var result = await svc.RegisterFailureAsync("key1");

        result.IsNowLocked.Should().BeTrue();
        result.LockoutDuration.Should().Be(TimeSpan.FromSeconds(300));
    }

    [Fact]
    public async Task DecimoTercerFallo_BloquePor1800Segundos()
    {
        var svc = CreateService();
        for (var i = 0; i < 12; i++) await svc.RegisterFailureAsync("key1");
        var result = await svc.RegisterFailureAsync("key1");

        result.IsNowLocked.Should().BeTrue();
        result.LockoutDuration.Should().Be(TimeSpan.FromSeconds(1800));
    }

    // El fallo 7 (entre step6 y step8) vuelve a aplicar el bloqueo del step anterior (20s).
    // Así cada intento tras el umbral mantiene el bloqueo activo hasta alcanzar el siguiente.
    [Fact]
    public async Task SeptimoFallo_ReaplicaBloqueoDeStep6()
    {
        var svc = CreateService();
        for (var i = 0; i < 6; i++) await svc.RegisterFailureAsync("key1");
        var result = await svc.RegisterFailureAsync("key1"); // count=7, step(6) aplica

        result.IsNowLocked.Should().BeTrue();
        result.LockoutDuration.Should().Be(TimeSpan.FromSeconds(20));
    }

    // --- Reset ---

    [Fact]
    public async Task ResetAsync_LimpiaTodo_SiguienteFalloEmpiezaDesde1()
    {
        var svc = CreateService();
        for (var i = 0; i < 6; i++) await svc.RegisterFailureAsync("key1");

        await svc.ResetAsync("key1");

        var locked = await svc.IsLockedOutAsync("key1");
        locked.Should().BeNull();

        var result = await svc.RegisterFailureAsync("key1");
        result.IsNowLocked.Should().BeFalse();
        result.AttemptsBeforeNextLock.Should().Be(5); // 6 - 1 = 5
    }

    // --- Aislamiento entre claves ---

    [Fact]
    public async Task ClavesDiferentes_SonIndependientes()
    {
        var svc = CreateService();
        for (var i = 0; i < 6; i++) await svc.RegisterFailureAsync("key1");

        var result = await svc.IsLockedOutAsync("key2");
        result.Should().BeNull();
    }

    // --- Configuración sin steps (sin bloqueos definidos) ---

    [Fact]
    public async Task SinSteps_NuncaBloquea_RetornaAttemptsNulos()
    {
        var svc = CreateService(new LoginAttemptOptions { WindowMinutes = 60, Steps = [] });
        for (var i = 0; i < 20; i++) await svc.RegisterFailureAsync("key1");

        var result = await svc.RegisterFailureAsync("key1");

        result.IsNowLocked.Should().BeFalse();
        result.AttemptsBeforeNextLock.Should().BeNull();
    }
}
