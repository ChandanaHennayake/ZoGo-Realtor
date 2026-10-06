using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using zogo.Application.DTOs.Seller;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Identity;
using zogo.Domain.Entities.Master;
using zogo.Infrastructure.Persistence;

namespace zogo.IntegrationTests;

public class SellerApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SellerApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private string GenerateToken(Guid userId, string email, string role = "BYR")
    {
        using var scope = _factory.Services.CreateScope();
        var jwtService = scope.ServiceProvider.GetRequiredService<IJwtTokenService>();
        return jwtService.GenerateAccessToken(userId, email, new[] { role });
    }

    [Fact]
    public async Task ActivateSeller_ExistingUser_ReturnsSuccess()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var suffix = Guid.NewGuid().ToString("N")[..8];
        var user = User.Create("Seller", "Candidate", $"seller_cand_{suffix}@example.com");
        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();

        var token = GenerateToken(user.Id, user.Email);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/seller/activate");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<SellerActivationResponse>();
        Assert.NotNull(result);
        Assert.True(result.SellerActivated);
    }
}
