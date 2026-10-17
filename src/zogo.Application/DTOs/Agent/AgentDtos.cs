namespace zogo.Application.DTOs.Agent;

public class CreateAgentRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public int CityId { get; set; }
    public List<short> DistrictIds { get; set; } = new();
}

public class UpdateAgentRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public int CityId { get; set; }
    public bool? IsActive { get; set; }
}

public class AssignAgentDistrictsRequest
{
    public List<short> DistrictIds { get; set; } = new();
}

public class AgentCityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

public class AgentDistrictDto
{
    public short Id { get; set; }
    public string Name { get; set; } = null!;
}

public class AgentUserDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? PhoneNumber { get; set; }
}

public class AgentResponse
{
    public Guid AgentId { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public AgentCityDto City { get; set; } = null!;
    public List<AgentDistrictDto> Districts { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AgentDashboardResponse
{
    public int InterestedProperties { get; set; }
    public int PendingVisits { get; set; }
    public int PendingDocuments { get; set; }
}

public class AgentInterestedPropertyResponse
{
    public Guid PropertyId { get; set; }
    public string Title { get; set; } = null!;
    public decimal Price { get; set; }
    public AgentDistrictDto District { get; set; } = null!;
    public AgentUserDto Seller { get; set; } = null!;
    public AgentUserDto Buyer { get; set; } = null!;
    public short InterestStatus { get; set; }
    public short InteractionStatus { get; set; }
}

public class AgentPendingVisitResponse
{
    public Guid PropertyId { get; set; }
    public string Title { get; set; } = null!;
    public string ReferenceNo { get; set; } = null!;
    public decimal Price { get; set; }
    public string AddressLine1 { get; set; } = null!;
    public AgentDistrictDto District { get; set; } = null!;
    public AgentUserDto Seller { get; set; } = null!;
    public AgentUserDto Buyer { get; set; } = null!;
    public short InteractionStatus { get; set; }
}

public class AgentPendingDocumentResponse
{
    public Guid PropertyId { get; set; }
    public string Title { get; set; } = null!;
    public string ReferenceNo { get; set; } = null!;
    public decimal Price { get; set; }
    public string AddressLine1 { get; set; } = null!;
    public AgentDistrictDto District { get; set; } = null!;
    public AgentUserDto Seller { get; set; } = null!;
    public AgentUserDto Buyer { get; set; } = null!;
    public short InteractionStatus { get; set; }
}

public class MarkVisitedRequest
{
    public Guid? BuyerId { get; set; }
}
