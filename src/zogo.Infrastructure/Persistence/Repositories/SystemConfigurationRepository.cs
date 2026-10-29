using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Master;

namespace zogo.Infrastructure.Persistence.Repositories;

public class SystemConfigurationRepository : ISystemConfigurationRepository
{
    private readonly ApplicationDbContext _context;

    public SystemConfigurationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> PeekNextPropertyReferenceNoAsync(CancellationToken cancellationToken = default)
    {
        var todayStr = DateTime.UtcNow.ToString("yyyyMMdd");

        var config = await _context.SystemConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ConfigKey == "PROPERTY_REFERENCE_SEQUENCE", cancellationToken);

        int nextSeq = 1;
        if (config != null)
        {
            var parts = config.ConfigValue.Split(':');
            if (parts.Length == 2 && parts[0] == todayStr && int.TryParse(parts[1], out var seq))
            {
                nextSeq = seq + 1;
            }
        }

        return $"ZOGO-{todayStr}-{nextSeq:D5}";
    }

    public async Task<string> GenerateAndIncrementPropertyReferenceNoAsync(CancellationToken cancellationToken = default)
    {
        var todayStr = DateTime.UtcNow.ToString("yyyyMMdd");

        var config = await _context.SystemConfigurations
            .FirstOrDefaultAsync(c => c.ConfigKey == "PROPERTY_REFERENCE_SEQUENCE", cancellationToken);

        int nextSeq = 1;
        if (config == null)
        {
            config = new SystemConfiguration
            {
                ConfigKey = "PROPERTY_REFERENCE_SEQUENCE",
                ConfigValue = $"{todayStr}:1",
                Description = "Daily auto-incrementing reference number sequence for ZOGO listings",
                UpdatedAt = DateTime.UtcNow
            };
            await _context.SystemConfigurations.AddAsync(config, cancellationToken);
        }
        else
        {
            var parts = config.ConfigValue.Split(':');
            if (parts.Length == 2 && parts[0] == todayStr && int.TryParse(parts[1], out var seq))
            {
                nextSeq = seq + 1;
            }
            else
            {
                nextSeq = 1;
            }
            config.ConfigValue = $"{todayStr}:{nextSeq}";
            config.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return $"ZOGO-{todayStr}-{nextSeq:D5}";
    }

    public Task<string> GetNextPropertyReferenceNoAsync(CancellationToken cancellationToken = default)
    {
        return GenerateAndIncrementPropertyReferenceNoAsync(cancellationToken);
    }
}
