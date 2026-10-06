using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using zogo.Application.Common;
using zogo.Application.DTOs.Buyer;
using zogo.Application.DTOs.Properties;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Identity;
using zogo.Domain.Entities.Master;
using zogo.Domain.Enums;
using zogo.Infrastructure.Persistence;

namespace zogo.IntegrationTests;

public class BuyerPropertyInteractionTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public BuyerPropertyInteractionTests(WebApplicationFactory<Program> factory)
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

    private async Task<(User buyerA, User buyerB, Property publishedProperty, Property draftProperty)> SeedTestDataAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var suffix = Guid.NewGuid().ToString("N")[..8];

        var buyerA = User.Create("Buyer", "One", $"buyer1_{suffix}@example.com");
        var buyerB = User.Create("Buyer", "Two", $"buyer2_{suffix}@example.com");
        var seller = User.Create("Seller", "User", $"seller_{suffix}@example.com");

        await db.Users.AddRangeAsync(buyerA, buyerB, seller);

        var published = Property.CreateDraft(
            $"REF-PUB-{suffix}",
            seller.Id,
            1,
            1,
            "Published Luxury Villa",
            "A beautiful villa",
            1,
            1,
            null,
            null,
            1,
            "123 Ocean View",
            null,
            "10100",
            6.9271m,
            79.8612m,
            50000000m,
            true,
            seller.Id);

        published.ChangeStatus(2, seller.Id); // 2 = PUBLISHED

        var draft = Property.CreateDraft(
            $"REF-DFT-{suffix}",
            seller.Id,
            1,
            1,
            "Draft Villa",
            "Not yet published",
            1,
            1,
            null,
            null,
            1,
            "456 Mountain Road",
            null,
            "10100",
            null,
            null,
            25000000m,
            false,
            seller.Id);
        // Status remains 1 (Draft)

        await db.Properties.AddRangeAsync(published, draft);
        await db.SaveChangesAsync();

        return (buyerA, buyerB, published, draft);
    }

    [Fact]
    public async Task FavoriteFlow_Add_Get_Duplicate_Delete_Succeeds()
    {
        var (buyerA, _, published, _) = await SeedTestDataAsync();
        var token = GenerateToken(buyerA.Id, buyerA.Email);

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/buyer/favorites/{published.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 1. Add Favorite
        var addResponse = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, addResponse.StatusCode);

        var addResult = await addResponse.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(addResult);
        Assert.True(addResult.Success);
        Assert.Equal("Property added to favorites.", addResult.Message);

        // 2. Add Duplicate Favorite -> Should succeed gracefully without creating duplicate row
        var duplicateRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/buyer/favorites/{published.Id}");
        duplicateRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var duplicateResponse = await _client.SendAsync(duplicateRequest);
        Assert.Equal(HttpStatusCode.OK, duplicateResponse.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var favCount = await db.Favorites.CountAsync(f => f.UserId == buyerA.Id && f.PropertyId == published.Id);
            Assert.Equal(1, favCount);
        }

        // 3. Get My Favorites
        var getRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/buyer/favorites");
        getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var getResponse = await _client.SendAsync(getRequest);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var getResult = await getResponse.Content.ReadFromJsonAsync<ApiResponse<List<GetPropertyResponse>>>();
        Assert.NotNull(getResult);
        Assert.True(getResult.Success);
        Assert.NotNull(getResult.Data);
        Assert.Contains(getResult.Data, p => p.PropertyId == published.Id);

        // 4. Remove Favorite
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/buyer/favorites/{published.Id}");
        deleteRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var deleteResponse = await _client.SendAsync(deleteRequest);
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var favCount = await db.Favorites.CountAsync(f => f.UserId == buyerA.Id && f.PropertyId == published.Id);
            Assert.Equal(0, favCount);
        }
    }

    [Fact]
    public async Task InterestFlow_Create_Update_Get_NoDuplicateRows()
    {
        var (buyerA, _, published, _) = await SeedTestDataAsync();
        var token = GenerateToken(buyerA.Id, buyerA.Email);

        // 1. Create Interest with Status 1 (INTERESTED)
        var postRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/buyer/property-interests/{published.Id}")
        {
            Content = JsonContent.Create(new { status = 1 })
        };
        postRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var postResponse = await _client.SendAsync(postRequest);
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        var postResult = await postResponse.Content.ReadFromJsonAsync<ApiResponse<PropertyInterestResponse>>();
        Assert.NotNull(postResult);
        Assert.True(postResult.Success);
        Assert.NotNull(postResult.Data);
        Assert.Equal(1, postResult.Data.Status);
        Assert.Equal("INTERESTED", postResult.Data.StatusName);

        // 2. Change Status to 2 (CONSIDERING)
        var putRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/buyer/property-interests/{published.Id}")
        {
            Content = JsonContent.Create(new { status = 2 })
        };
        putRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var putResponse = await _client.SendAsync(putRequest);
        Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

        var putResult = await putResponse.Content.ReadFromJsonAsync<ApiResponse<PropertyInterestResponse>>();
        Assert.NotNull(putResult);
        Assert.True(putResult.Success);
        Assert.NotNull(putResult.Data);
        Assert.Equal(2, putResult.Data.Status);
        Assert.Equal("CONSIDERING", putResult.Data.StatusName);

        // Verify only 1 row exists in DB
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var count = await db.PropertyInterests.CountAsync(p => p.BuyerId == buyerA.Id && p.PropertyId == published.Id);
            Assert.Equal(1, count);
        }

        // 3. Get My Interest
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/buyer/property-interests/{published.Id}");
        getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var getResponse = await _client.SendAsync(getRequest);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var getResult = await getResponse.Content.ReadFromJsonAsync<ApiResponse<PropertyInterestResponse>>();
        Assert.NotNull(getResult);
        Assert.True(getResult.Success);
        Assert.NotNull(getResult.Data);
        Assert.Equal(2, getResult.Data.Status);
        Assert.Equal("CONSIDERING", getResult.Data.StatusName);
    }

    [Fact]
    public async Task InteractionFlow_StatusProgression_OnlyOneCurrentRecord()
    {
        var (buyerA, _, published, _) = await SeedTestDataAsync();
        var token = GenerateToken(buyerA.Id, buyerA.Email);

        // 1. Create INTERESTED (1)
        var postRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/buyer/property-interactions/{published.Id}")
        {
            Content = JsonContent.Create(new { status = 1 })
        };
        postRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var postResponse = await _client.SendAsync(postRequest);
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        var postResult = await postResponse.Content.ReadFromJsonAsync<ApiResponse<BuyerPropertyInteractionResponse>>();
        Assert.NotNull(postResult?.Data);
        Assert.Equal(1, postResult.Data.CurrentStatus);
        Assert.Equal("INTERESTED", postResult.Data.CurrentStatusName);

        // 2. Update to VISIT_REQUESTED (2)
        var updateRequest1 = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/buyer/property-interactions/{published.Id}")
        {
            Content = JsonContent.Create(new { status = 2 })
        };
        updateRequest1.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var updateResponse1 = await _client.SendAsync(updateRequest1);
        Assert.Equal(HttpStatusCode.OK, updateResponse1.StatusCode);

        var updateResult1 = await updateResponse1.Content.ReadFromJsonAsync<ApiResponse<BuyerPropertyInteractionResponse>>();
        Assert.NotNull(updateResult1?.Data);
        Assert.Equal(2, updateResult1.Data.CurrentStatus);
        Assert.Equal("VISIT_REQUESTED", updateResult1.Data.CurrentStatusName);

        // 3. Update to VISITED (3)
        var updateRequest2 = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/buyer/property-interactions/{published.Id}")
        {
            Content = JsonContent.Create(new { status = 3 })
        };
        updateRequest2.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var updateResponse2 = await _client.SendAsync(updateRequest2);
        Assert.Equal(HttpStatusCode.OK, updateResponse2.StatusCode);

        var updateResult2 = await updateResponse2.Content.ReadFromJsonAsync<ApiResponse<BuyerPropertyInteractionResponse>>();
        Assert.NotNull(updateResult2?.Data);
        Assert.Equal(3, updateResult2.Data.CurrentStatus);
        Assert.Equal("VISITED", updateResult2.Data.CurrentStatusName);

        // Verify only 1 row exists in DB
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var count = await db.BuyerPropertyInteractions.CountAsync(i => i.BuyerId == buyerA.Id && i.PropertyId == published.Id);
            Assert.Equal(1, count);
        }

        // 4. Get Interaction Status
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/buyer/property-interactions/{published.Id}");
        getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var getResponse = await _client.SendAsync(getRequest);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var getResult = await getResponse.Content.ReadFromJsonAsync<ApiResponse<BuyerPropertyInteractionResponse>>();
        Assert.NotNull(getResult?.Data);
        Assert.Equal(3, getResult.Data.CurrentStatus);
        Assert.Equal("VISITED", getResult.Data.CurrentStatusName);
    }

    [Fact]
    public async Task Security_BuyerCannotAccessOrModifyAnotherBuyerData()
    {
        var (buyerA, buyerB, published, _) = await SeedTestDataAsync();
        var tokenA = GenerateToken(buyerA.Id, buyerA.Email);
        var tokenB = GenerateToken(buyerB.Id, buyerB.Email);

        // Buyer A favorites property
        var favReqA = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/buyer/favorites/{published.Id}");
        favReqA.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenA);
        await _client.SendAsync(favReqA);

        // Buyer A sets interest
        var intReqA = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/buyer/property-interests/{published.Id}")
        {
            Content = JsonContent.Create(new { status = 1 })
        };
        intReqA.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenA);
        await _client.SendAsync(intReqA);

        // Buyer A sets interaction
        var actReqA = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/buyer/property-interactions/{published.Id}")
        {
            Content = JsonContent.Create(new { status = 1 })
        };
        actReqA.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenA);
        await _client.SendAsync(actReqA);

        // Buyer B checks favorites -> Should NOT see Buyer A's favorite
        var getFavB = new HttpRequestMessage(HttpMethod.Get, "/api/v1/buyer/favorites");
        getFavB.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenB);
        var resFavB = await _client.SendAsync(getFavB);
        var listB = await resFavB.Content.ReadFromJsonAsync<ApiResponse<List<GetPropertyResponse>>>();
        Assert.DoesNotContain(listB!.Data!, p => p.PropertyId == published.Id);

        // Buyer B checks interest -> Should return 404 (no interest for Buyer B)
        var getIntB = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/buyer/property-interests/{published.Id}");
        getIntB.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenB);
        var resIntB = await _client.SendAsync(getIntB);
        Assert.Equal(HttpStatusCode.NotFound, resIntB.StatusCode);

        // Buyer B checks interaction -> Should return 404
        var getActB = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/buyer/property-interactions/{published.Id}");
        getActB.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenB);
        var resActB = await _client.SendAsync(getActB);
        Assert.Equal(HttpStatusCode.NotFound, resActB.StatusCode);

        // Buyer B deletes favorite -> Should return 404 (cannot delete Buyer A's favorite)
        var delFavB = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/buyer/favorites/{published.Id}");
        delFavB.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenB);
        var resDelFavB = await _client.SendAsync(delFavB);
        Assert.Equal(HttpStatusCode.NotFound, resDelFavB.StatusCode);

        // Verify Buyer A's favorite is still untouched in DB
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var exists = await db.Favorites.AnyAsync(f => f.UserId == buyerA.Id && f.PropertyId == published.Id);
            Assert.True(exists);
        }
    }

    [Fact]
    public async Task VisibilityRule_NonPublishedPropertyCannotBeInteractedWith()
    {
        var (buyerA, _, _, draft) = await SeedTestDataAsync();
        var token = GenerateToken(buyerA.Id, buyerA.Email);

        // 1. Try to favorite draft property (Status != 2)
        var favReq = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/buyer/favorites/{draft.Id}");
        favReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var favRes = await _client.SendAsync(favReq);
        Assert.Equal(HttpStatusCode.BadRequest, favRes.StatusCode);

        // 2. Try to set interest on draft property (Status != 2)
        var intReq = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/buyer/property-interests/{draft.Id}")
        {
            Content = JsonContent.Create(new { status = 1 })
        };
        intReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var intRes = await _client.SendAsync(intReq);
        Assert.Equal(HttpStatusCode.BadRequest, intRes.StatusCode);

        // 3. Try to set interaction on draft property (Status != 2)
        var actReq = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/buyer/property-interactions/{draft.Id}")
        {
            Content = JsonContent.Create(new { status = 1 })
        };
        actReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var actRes = await _client.SendAsync(actReq);
        Assert.Equal(HttpStatusCode.BadRequest, actRes.StatusCode);
    }

    [Fact]
    public async Task Validation_InvalidStatusValues_ReturnBadRequest()
    {
        var (buyerA, _, published, _) = await SeedTestDataAsync();
        var token = GenerateToken(buyerA.Id, buyerA.Email);

        // Invalid interest status: 99
        var intReq = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/buyer/property-interests/{published.Id}")
        {
            Content = JsonContent.Create(new { status = 99 })
        };
        intReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var intRes = await _client.SendAsync(intReq);
        Assert.Equal(HttpStatusCode.BadRequest, intRes.StatusCode);

        // Invalid interaction status: 99
        var actReq = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/buyer/property-interactions/{published.Id}")
        {
            Content = JsonContent.Create(new { status = 99 })
        };
        actReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var actRes = await _client.SendAsync(actReq);
        Assert.Equal(HttpStatusCode.BadRequest, actRes.StatusCode);
    }
}
