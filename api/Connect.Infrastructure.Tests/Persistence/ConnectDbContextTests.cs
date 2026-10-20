using System.Reflection;
using Connect.Application.Common.Abstractions;
using Connect.Domain.Common;
using Connect.Domain.Users;
using Connect.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Connect.Infrastructure.Tests.Persistence;

// Assumes Account implements IEntity and IAuditable, has a public parameterless
// constructor, and exposes a settable Id.
[TestFixture]
public sealed class ConnectDbContextTests
{
    private ConnectDbContext _context;
    private IAppDbContext _appContext;

    public enum SaveMode
    {
        SyncDefault,
        SyncAccept,
        SyncDoNotAccept,
        AsyncDefault,
        AsyncAccept,
        AsyncDoNotAccept
    }

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ConnectDbContext>()
            .UseInMemoryDatabase($"ConnectTests-{Guid.NewGuid()}")
            .Options;

        _context = new ConnectDbContext(options);
        _appContext = _context;
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [TestCase("Add")]
    [TestCase("Remove")]
    public void InterfaceMethod_RequiresClassImplementingIEntity(string name)
    {
        var method = typeof(IAppDbContext).GetMethods()
            .Single(m => m.Name == name && m.IsGenericMethodDefinition);
        var parameter = method.GetGenericArguments().Single();
        var requiresReferenceType = parameter.GenericParameterAttributes.HasFlag(
            GenericParameterAttributes.ReferenceTypeConstraint);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(requiresReferenceType, Is.True);
            Assert.That(parameter.GetGenericParameterConstraints(), Does.Contain(typeof(IEntity)));
        }
    }

    [Test]
    public void Add_ThroughInterface_TracksEntityAsAdded()
    {
        var account = NewAccount();

        _appContext.Add(account);

        Assert.That(_context.Entry(account).State, Is.EqualTo(EntityState.Added));
    }

    [Test]
    public void Remove_ThroughInterface_TracksExistingEntityAsDeleted()
    {
        var account = SeedAccount();

        _appContext.Remove(account);

        Assert.That(_context.Entry(account).State, Is.EqualTo(EntityState.Deleted));
    }

    [Test]
    public void Remove_UnsavedAddedEntity_DetachesEntity()
    {
        var account = NewAccount();
        _appContext.Add(account);

        _appContext.Remove(account);

        Assert.That(_context.Entry(account).State, Is.EqualTo(EntityState.Detached));
    }

    [TestCase(SaveMode.SyncDefault)]
    [TestCase(SaveMode.SyncAccept)]
    [TestCase(SaveMode.SyncDoNotAccept)]
    [TestCase(SaveMode.AsyncDefault)]
    [TestCase(SaveMode.AsyncAccept)]
    [TestCase(SaveMode.AsyncDoNotAccept)]
    public async Task SaveChanges_AddedEntity_SetsBothAuditProperties(SaveMode mode)
    {
        var account = NewAccount();
        _appContext.Add(account);

        var oldTimestamp = DateTimeOffset.UtcNow.AddDays(-1);
        SetAudit(account, nameof(IAuditable.CreatedAt), oldTimestamp);
        SetAudit(account, nameof(IAuditable.UpdatedAt), oldTimestamp);
        var before = DateTimeOffset.UtcNow;

        var affected = await Save(mode);

        var after = DateTimeOffset.UtcNow;
        var persisted = _context.Accounts.AsNoTracking().Single(a => a.Id == account.Id);
        var expectedState = AcceptsChanges(mode) ? EntityState.Unchanged : EntityState.Added;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(affected, Is.EqualTo(1));
            Assert.That(account.CreatedAt, Is.InRange(before, after));
            Assert.That(account.UpdatedAt, Is.EqualTo(account.CreatedAt));
            Assert.That(persisted.CreatedAt, Is.EqualTo(account.CreatedAt));
            Assert.That(persisted.UpdatedAt, Is.EqualTo(account.UpdatedAt));
            Assert.That(_context.Entry(account).State, Is.EqualTo(expectedState));
        }
    }

    [TestCase(SaveMode.SyncDefault)]
    [TestCase(SaveMode.SyncAccept)]
    [TestCase(SaveMode.SyncDoNotAccept)]
    [TestCase(SaveMode.AsyncDefault)]
    [TestCase(SaveMode.AsyncAccept)]
    [TestCase(SaveMode.AsyncDoNotAccept)]
    public async Task SaveChanges_ModifiedEntity_PreservesCreatedAt(SaveMode mode)
    {
        var account = SeedAccount();
        var originalCreatedAt = account.CreatedAt;
        SetAudit(account, nameof(IAuditable.UpdatedAt), DateTimeOffset.UtcNow.AddDays(-1));
        _context.Entry(account).State = EntityState.Modified;
        var before = DateTimeOffset.UtcNow;

        var affected = await Save(mode);

        var after = DateTimeOffset.UtcNow;
        var persisted = _context.Accounts.AsNoTracking().Single(a => a.Id == account.Id);
        var expectedState = AcceptsChanges(mode) ? EntityState.Unchanged : EntityState.Modified;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(affected, Is.EqualTo(1));
            Assert.That(account.CreatedAt, Is.EqualTo(originalCreatedAt));
            Assert.That(account.UpdatedAt, Is.InRange(before, after));
            Assert.That(persisted.CreatedAt, Is.EqualTo(originalCreatedAt));
            Assert.That(persisted.UpdatedAt, Is.EqualTo(account.UpdatedAt));
            Assert.That(_context.Entry(account).State, Is.EqualTo(expectedState));
        }
    }

    [TestCase(SaveMode.SyncDefault)]
    [TestCase(SaveMode.SyncAccept)]
    [TestCase(SaveMode.SyncDoNotAccept)]
    [TestCase(SaveMode.AsyncDefault)]
    [TestCase(SaveMode.AsyncAccept)]
    [TestCase(SaveMode.AsyncDoNotAccept)]
    public async Task SaveChanges_UnchangedEntity_DoesNotUpdateAuditProperties(SaveMode mode)
    {
        var account = SeedAccount();
        var createdAt = account.CreatedAt;
        var updatedAt = account.UpdatedAt;

        var affected = await Save(mode);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(affected, Is.Zero);
            Assert.That(account.CreatedAt, Is.EqualTo(createdAt));
            Assert.That(account.UpdatedAt, Is.EqualTo(updatedAt));
        }
    }

    [TestCase(SaveMode.SyncDefault)]
    [TestCase(SaveMode.SyncAccept)]
    [TestCase(SaveMode.SyncDoNotAccept)]
    [TestCase(SaveMode.AsyncDefault)]
    [TestCase(SaveMode.AsyncAccept)]
    [TestCase(SaveMode.AsyncDoNotAccept)]
    public async Task SaveChanges_DeletedEntity_DoesNotUpdateAuditProperties(SaveMode mode)
    {
        var account = SeedAccount();
        var createdAt = account.CreatedAt;
        var updatedAt = account.UpdatedAt;
        _appContext.Remove(account);

        var affected = await Save(mode);

        var exists = _context.Accounts.AsNoTracking().Any(a => a.Id == account.Id);
        var expectedState = AcceptsChanges(mode) ? EntityState.Detached : EntityState.Deleted;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(affected, Is.EqualTo(1));
            Assert.That(account.CreatedAt, Is.EqualTo(createdAt));
            Assert.That(account.UpdatedAt, Is.EqualTo(updatedAt));
            Assert.That(exists, Is.False);
            Assert.That(_context.Entry(account).State, Is.EqualTo(expectedState));
        }
    }

    [TestCase(SaveMode.AsyncDefault)]
    [TestCase(SaveMode.AsyncAccept)]
    [TestCase(SaveMode.AsyncDoNotAccept)]
    public void SaveChangesAsync_CanceledToken_DoesNotPersist(SaveMode mode)
    {
        var account = NewAccount();
        _appContext.Add(account);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                async () => { await Save(mode, cancellation.Token); },
                Throws.InstanceOf<OperationCanceledException>());
            Assert.That(_context.Entry(account).State, Is.EqualTo(EntityState.Added));
            Assert.That(_context.Accounts.AsNoTracking().Any(a => a.Id == account.Id), Is.False);
        }
    }

    private static Account NewAccount() => new() { Id = Guid.NewGuid() };

    private Account SeedAccount()
    {
        var account = NewAccount();
        _appContext.Add(account);
        _context.SaveChanges();
        return account;
    }

    private void SetAudit(Account account, string propertyName, DateTimeOffset value)
    {
        _context.Entry(account).Property<DateTimeOffset>(propertyName).CurrentValue = value;
    }

    private static bool AcceptsChanges(SaveMode mode) =>
        mode is not SaveMode.SyncDoNotAccept and not SaveMode.AsyncDoNotAccept;

    private Task<int> Save(SaveMode mode, CancellationToken cancellationToken = default)
    {
        return mode switch
        {
            SaveMode.SyncDefault => Task.FromResult(_context.SaveChanges()),
            SaveMode.SyncAccept => Task.FromResult(_context.SaveChanges(true)),
            SaveMode.SyncDoNotAccept => Task.FromResult(_context.SaveChanges(false)),
            SaveMode.AsyncDefault => _context.SaveChangesAsync(cancellationToken),
            SaveMode.AsyncAccept => _context.SaveChangesAsync(true, cancellationToken),
            SaveMode.AsyncDoNotAccept => _context.SaveChangesAsync(false, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(mode))
        };
    }
}
