namespace zogo.Application.DTOs.Admin;

public sealed class AdminDashboardStatsResponse
{
    public int TotalProperties { get; set; }
    public int PendingApprovalsCount { get; set; }
    public int UnderReviewCount { get; set; }
    public int ChangesRequestedCount { get; set; }
    public int PublishedCount { get; set; }
    public int RejectedCount { get; set; }
    public int SoldCount { get; set; }

    public int TotalUsers { get; set; }
    public int TotalBuyers { get; set; }
    public int TotalSellers { get; set; }
    public int TotalAgents { get; set; }
    public int ActiveAgents { get; set; }

    public int TotalDistrictsCovered { get; set; }
}
