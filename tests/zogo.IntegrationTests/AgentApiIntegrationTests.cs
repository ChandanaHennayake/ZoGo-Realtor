using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using zogo.Application.Common;
using zogo.Application.DTOs.Agent;
using zogo.Application.DTOs.Buyer;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Agent;
using zogo.Domain.Entities.Identity;
using zogo.Domain.Entities.Master;
using zogo.Infrastructure.Persistence;

namespace zogo.IntegrationTests;

public class AgentApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AgentApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private string GenerateToken(Guid userId, string email, string role)
    {
        using var scope = _factory.Services.CreateScope();
        var jwtService = scope.ServiceProvider.GetRequiredService<IJwtTokenService>();
        return jwtService.GenerateAccessToken(userId, email, new[] { role });
    }

    private async Task<(User admin, string adminToken)> CreateAdminUserAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var suffix = Guid.NewGuid().ToString("N")[..8];
        var admin = User.Create("Admin", "User", $"admin_{suffix}@example.com");
        await db.Users.AddAsync(admin);

        var adminRole = await db.Roles.FirstOrDefaultAsync(r => r.Code == "ADM");
        if (adminRole is null)
        {
            adminRole = Role.Create("ADM", "Admin", true);
            await db.Roles.AddAsync(adminRole);
            await db.SaveChangesAsync();
        }

        var userRole = UserRole.Create(admin.Id, adminRole.Id, null);
        admin.AddRole(userRole);
        await db.SaveChangesAsync();

        var token = GenerateToken(admin.Id, admin.Email, "ADM");
        return (admin, token);
    }

    [Fact]
    public async Task Admin_AgentCrud_Flow_Succeeds()
    {
        var (_, adminToken) = await CreateAdminUserAsync();
        var suffix = Guid.NewGuid().ToString("N")[..8];

        // 1. Create Agent
        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/admin/agents")
        {
            Content = JsonContent.Create(new
            {
                firstName = "John",
                lastName = "Perera",
                email = $"agent_{suffix}@example.com",
                phoneNumber = "0771234567",
                cityId = 1,
                districtIds = new[] { 1, 2 }
            })
        };
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var createResponse = await _client.SendAsync(createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<AgentResponse>>();
        Assert.NotNull(createResult?.Data);
        var agentId = createResult.Data.AgentId;
        Assert.Equal("John Perera", createResult.Data.Name);
        Assert.True(createResult.Data.IsActive);
        Assert.Equal(2, createResult.Data.Districts.Count);

        // 2. List Agents
        var listRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/admin/agents");
        listRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var listResponse = await _client.SendAsync(listRequest);
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var listResult = await listResponse.Content.ReadFromJsonAsync<ApiResponse<List<AgentResponse>>>();
        Assert.NotNull(listResult?.Data);
        Assert.Contains(listResult.Data, a => a.AgentId == agentId);

        // 3. Get Agent By Id
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/admin/agents/{agentId}");
        getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var getResponse = await _client.SendAsync(getRequest);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var getResult = await getResponse.Content.ReadFromJsonAsync<ApiResponse<AgentResponse>>();
        Assert.NotNull(getResult?.Data);
        Assert.Equal(agentId, getResult.Data.AgentId);

        // 4. Update Agent
        var updateRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/admin/agents/{agentId}")
        {
            Content = JsonContent.Create(new
            {
                firstName = "Johnny",
                lastName = "Perera",
                phoneNumber = "0779998888",
                cityId = 1,
                isActive = true
            })
        };
        updateRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var updateResponse = await _client.SendAsync(updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updateResult = await updateResponse.Content.ReadFromJsonAsync<ApiResponse<AgentResponse>>();
        Assert.NotNull(updateResult?.Data);
        Assert.Equal("Johnny Perera", updateResult.Data.Name);
        Assert.Equal("0779998888", updateResult.Data.PhoneNumber);

        // 5. Assign Districts
        var assignDistrictsReq = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/admin/agents/{agentId}/districts")
        {
            Content = JsonContent.Create(new
            {
                districtIds = new[] { 1 }
            })
        };
        assignDistrictsReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var assignResponse = await _client.SendAsync(assignDistrictsReq);
        Assert.Equal(HttpStatusCode.OK, assignResponse.StatusCode);

        var assignResult = await assignResponse.Content.ReadFromJsonAsync<ApiResponse<AgentResponse>>();
        Assert.NotNull(assignResult?.Data);
        Assert.Single(assignResult.Data.Districts);

        // 6. Deactivate Agent
        var deactivateReq = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/admin/agents/{agentId}/deactivate");
        deactivateReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var deactResponse = await _client.SendAsync(deactivateReq);
        Assert.Equal(HttpStatusCode.OK, deactResponse.StatusCode);

        var deactResult = await deactResponse.Content.ReadFromJsonAsync<ApiResponse<AgentResponse>>();
        Assert.NotNull(deactResult?.Data);
        Assert.False(deactResult.Data.IsActive);

        // 7. Activate Agent
        var activateReq = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/admin/agents/{agentId}/activate");
        activateReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var actResponse = await _client.SendAsync(activateReq);
        Assert.Equal(HttpStatusCode.OK, actResponse.StatusCode);

        var actResult = await actResponse.Content.ReadFromJsonAsync<ApiResponse<AgentResponse>>();
        Assert.NotNull(actResult?.Data);
        Assert.True(actResult.Data.IsActive);
    }

    [Fact]
    public async Task Security_EnforcesRoleAndDistrictAuthorization()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var buyer = User.Create("Buyer", "Sec", $"sec_buyer_{suffix}@example.com");
        var seller = User.Create("Seller", "Sec", $"sec_seller_{suffix}@example.com");
        var agent = User.Create("Agent", "Sec", $"sec_agent_{suffix}@example.com");

        var agentProfile = AgentProfile.Create(agent.Id, 1, true);
        var agentDistrict = AgentDistrict.Create(agentProfile.Id, 1); // Operating in District 1 only

        await db.Users.AddRangeAsync(buyer, seller, agent);
        await db.AgentProfiles.AddAsync(agentProfile);
        await db.AgentDistricts.AddAsync(agentDistrict);
        await db.SaveChangesAsync();

        var buyerToken = GenerateToken(buyer.Id, buyer.Email, "BYR");
        var sellerToken = GenerateToken(seller.Id, seller.Email, "SEL");
        var agentToken = GenerateToken(agent.Id, agent.Email, "AGT");

        // 1. Unauthenticated Agent endpoint -> 401
        var unauthReq = new HttpRequestMessage(HttpMethod.Get, "/api/v1/agent/dashboard");
        var unauthRes = await _client.SendAsync(unauthReq);
        Assert.Equal(HttpStatusCode.Unauthorized, unauthRes.StatusCode);

        // 2. Buyer accessing Agent API -> 403
        var buyerReq = new HttpRequestMessage(HttpMethod.Get, "/api/v1/agent/dashboard");
        buyerReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", buyerToken);
        var buyerRes = await _client.SendAsync(buyerReq);
        Assert.Equal(HttpStatusCode.Forbidden, buyerRes.StatusCode);

        // 3. Seller accessing Agent API -> 403
        var sellerReq = new HttpRequestMessage(HttpMethod.Get, "/api/v1/agent/dashboard");
        sellerReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sellerToken);
        var sellerRes = await _client.SendAsync(sellerReq);
        Assert.Equal(HttpStatusCode.Forbidden, sellerRes.StatusCode);

        // 4. Agent accessing property outside assigned district -> 403
        var propertyInDist2 = Property.CreateDraft(
            $"REF-SEC-{suffix}",
            seller.Id,
            1,
            1,
            "Outside District Property",
            "In district 2",
            1,
            2, // District 2 (Agent only has District 1)
            null,
            null,
            1,
            "Address 2",
            null,
            "10000",
            null,
            null,
            10000000m,
            false,
            seller.Id);
        propertyInDist2.ChangeStatus(2, seller.Id);
        await db.Properties.AddAsync(propertyInDist2);
        await db.SaveChangesAsync();

        var outsideDistrictReq = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/agent/properties/{propertyInDist2.Id}/visit");
        outsideDistrictReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", agentToken);
        var outsideDistrictRes = await _client.SendAsync(outsideDistrictReq);
        Assert.Equal(HttpStatusCode.Forbidden, outsideDistrictRes.StatusCode);
    }

    [Fact]
    public async Task CompleteEndToEnd_Workflow_BuyerToAgentProgression()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var buyer = User.Create("Buyer", "Flow", $"flow_buyer_{suffix}@example.com");
        var seller = User.Create("Seller", "Flow", $"flow_seller_{suffix}@example.com");
        var agent = User.Create("Agent", "Flow", $"flow_agent_{suffix}@example.com");

        var agentProfile = AgentProfile.Create(agent.Id, 1, true);
        var agentDistrict = AgentDistrict.Create(agentProfile.Id, 1); // District 1

        var property = Property.CreateDraft(
            $"REF-FLW-{suffix}",
            seller.Id,
            1,
            1,
            "Flow Villa Colombo",
            "A luxury property",
            1,
            1, // District 1
            null,
            null,
            1,
            "Colombo Central",
            null,
            "10000",
            6.92m,
            79.86m,
            45000000m,
            true,
            seller.Id);
        property.ChangeStatus(2, seller.Id); // 2 = PUBLISHED

        await db.Users.AddRangeAsync(buyer, seller, agent);
        await db.AgentProfiles.AddAsync(agentProfile);
        await db.AgentDistricts.AddAsync(agentDistrict);
        await db.Properties.AddAsync(property);
        await db.SaveChangesAsync();

        var buyerToken = GenerateToken(buyer.Id, buyer.Email, "BYR");
        var agentToken = GenerateToken(agent.Id, agent.Email, "AGT");

        // 1. Buyer shows interest (Status 1)
        var interestReq = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/buyer/property-interests/{property.Id}")
        {
            Content = JsonContent.Create(new { status = 1 })
        };
        interestReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", buyerToken);
        var interestRes = await _client.SendAsync(interestReq);
        Assert.Equal(HttpStatusCode.OK, interestRes.StatusCode);

        // Verify Agent Notification was created in DB for District 1 agent
        var notifExists = await db.AgentNotifications.AnyAsync(n => n.AgentProfileId == agentProfile.Id && n.PropertyId == property.Id);
        Assert.True(notifExists);

        // Buyer initializes interaction as 1 (INTERESTED)
        var initInteractionReq = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/buyer/property-interactions/{property.Id}")
        {
            Content = JsonContent.Create(new { status = 1 })
        };
        initInteractionReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", buyerToken);
        var initRes = await _client.SendAsync(initInteractionReq);
        Assert.Equal(HttpStatusCode.OK, initRes.StatusCode);

        // 2. Agent checks interested properties
        var agentInterestedReq = new HttpRequestMessage(HttpMethod.Get, "/api/v1/agent/properties/interested");
        agentInterestedReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", agentToken);
        var agentInterestedRes = await _client.SendAsync(agentInterestedReq);
        Assert.Equal(HttpStatusCode.OK, agentInterestedRes.StatusCode);

        var agentInterestedList = await agentInterestedRes.Content.ReadFromJsonAsync<ApiResponse<List<AgentInterestedPropertyResponse>>>();
        Assert.NotNull(agentInterestedList?.Data);
        Assert.Contains(agentInterestedList.Data, p => p.PropertyId == property.Id);

        // 3. Buyer cannot jump directly to DOCUMENTS_REQUESTED (4) before visit
        var prematureDocReq = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/buyer/property-interactions/{property.Id}/documents-request");
        prematureDocReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", buyerToken);
        var prematureDocRes = await _client.SendAsync(prematureDocReq);
        Assert.Equal(HttpStatusCode.BadRequest, prematureDocRes.StatusCode);

        // 4. Buyer requests visit (Status 2)
        var visitReq = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/buyer/property-interactions/{property.Id}")
        {
            Content = JsonContent.Create(new { status = 2 })
        };
        visitReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", buyerToken);
        var visitRes = await _client.SendAsync(visitReq);
        Assert.Equal(HttpStatusCode.OK, visitRes.StatusCode);

        // 5. Agent checks pending visits
        var agentPendingVisitsReq = new HttpRequestMessage(HttpMethod.Get, "/api/v1/agent/visits/pending");
        agentPendingVisitsReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", agentToken);
        var agentPendingVisitsRes = await _client.SendAsync(agentPendingVisitsReq);
        Assert.Equal(HttpStatusCode.OK, agentPendingVisitsRes.StatusCode);

        var pendingVisits = await agentPendingVisitsRes.Content.ReadFromJsonAsync<ApiResponse<List<AgentPendingVisitResponse>>>();
        Assert.NotNull(pendingVisits?.Data);
        Assert.Contains(pendingVisits.Data, v => v.PropertyId == property.Id);

        // 6. Agent Dashboard shows metrics
        var dashReq = new HttpRequestMessage(HttpMethod.Get, "/api/v1/agent/dashboard");
        dashReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", agentToken);
        var dashRes = await _client.SendAsync(dashReq);
        Assert.Equal(HttpStatusCode.OK, dashRes.StatusCode);

        var dashData = await dashRes.Content.ReadFromJsonAsync<ApiResponse<AgentDashboardResponse>>();
        Assert.NotNull(dashData?.Data);
        Assert.True(dashData.Data.InterestedProperties >= 1);
        Assert.True(dashData.Data.PendingVisits >= 1);

        // 7. Buyer cannot mark VISITED (3)
        var buyerVisitedReq = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/buyer/property-interactions/{property.Id}")
        {
            Content = JsonContent.Create(new { status = 3 })
        };
        buyerVisitedReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", buyerToken);
        var buyerVisitedRes = await _client.SendAsync(buyerVisitedReq);
        Assert.Equal(HttpStatusCode.BadRequest, buyerVisitedRes.StatusCode);

        // 8. Agent marks property as VISITED (3)
        var agentMarkVisitedReq = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/agent/properties/{property.Id}/visit");
        agentMarkVisitedReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", agentToken);
        var agentMarkVisitedRes = await _client.SendAsync(agentMarkVisitedReq);
        Assert.Equal(HttpStatusCode.OK, agentMarkVisitedRes.StatusCode);

        var markVisitedResult = await agentMarkVisitedRes.Content.ReadFromJsonAsync<ApiResponse<BuyerPropertyInteractionResponse>>();
        Assert.NotNull(markVisitedResult?.Data);
        Assert.Equal(3, markVisitedResult.Data.CurrentStatus);
        Assert.Equal("VISITED", markVisitedResult.Data.CurrentStatusName);

        // 9. Now Buyer can request documents (Status 4)
        var docReq = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/buyer/property-interactions/{property.Id}/documents-request");
        docReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", buyerToken);
        var docRes = await _client.SendAsync(docReq);
        Assert.Equal(HttpStatusCode.OK, docRes.StatusCode);

        var docResult = await docRes.Content.ReadFromJsonAsync<ApiResponse<BuyerPropertyInteractionResponse>>();
        Assert.NotNull(docResult?.Data);
        Assert.Equal(4, docResult.Data.CurrentStatus);
        Assert.Equal("DOCUMENTS_REQUESTED", docResult.Data.CurrentStatusName);

        // 10. Agent sees pending documents
        var agentPendingDocsReq = new HttpRequestMessage(HttpMethod.Get, "/api/v1/agent/documents/pending");
        agentPendingDocsReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", agentToken);
        var agentPendingDocsRes = await _client.SendAsync(agentPendingDocsReq);
        Assert.Equal(HttpStatusCode.OK, agentPendingDocsRes.StatusCode);

        var pendingDocs = await agentPendingDocsRes.Content.ReadFromJsonAsync<ApiResponse<List<AgentPendingDocumentResponse>>>();
        Assert.NotNull(pendingDocs?.Data);
        Assert.Contains(pendingDocs.Data, d => d.PropertyId == property.Id);
    }
}
