using zogo.Domain.Common;

namespace zogo.UnitTests.Domain;

public class BaseEntityTests
{
    [Fact]
    public void AuditableEntity_ShouldImplementAllInterfaces()
    {
        var entity = new TestAuditableEntity();

        Assert.IsAssignableFrom<IEntity>(entity);
        Assert.IsAssignableFrom<IAuditableEntity>(entity);
        Assert.IsAssignableFrom<ISoftDeletable>(entity);
    }

    [Fact]
    public void AuditableEntity_DefaultSoftDeleteValues()
    {
        var entity = new TestAuditableEntity();

        Assert.False(entity.IsDeleted);
        Assert.Null(entity.DeletedAt);
        Assert.Null(entity.DeletedBy);
    }

    private sealed class TestAuditableEntity : AuditableEntity;
}
