using Havit.Data.EntityFrameworkCore.Patterns.Lookups;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Repositories;
using System.Linq.Expressions;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Lookups.Infrastructure;

public class UzivatelLookupService : LookupServiceBase<string, Uzivatel>
{

	public UzivatelLookupService(IEntityLookupDataStorage lookupStorage, IRepository<Uzivatel> repository, IDbContext dbContext, IEntityKeyAccessor entityKeyAccessor, ISoftDeleteManager softDeleteManager) : base(lookupStorage, repository, dbContext, entityKeyAccessor, softDeleteManager)
	{
	}

	/// <summary>
	/// Vyhledá uživatele podle emailu.
	/// </summary>
	public Uzivatel GetUzivatelByEmail(string email) => GetEntityByLookupKey(email);

	/// <summary>
	/// Vyhledá uživatele podle emailu.
	/// </summary>
	public Task<Uzivatel> GetUzivatelByEmailAsync(string email, CancellationToken cancellationToken = default) => GetEntityByLookupKeyAsync(email, cancellationToken);

	/// <summary>
	/// Vyhledá uživatele podle emailů.
	/// </summary>
	public List<Uzivatel> GetUzivateleByEmails(string[] emaily) => GetEntitiesByLookupKeys(emaily);

	/// <summary>
	/// Vyhledá uživatele podle emailů.
	/// </summary>
	public Task<List<Uzivatel>> GetUzivateleByEmailsAsync(string[] emaily, CancellationToken cancellationToken = default) => GetEntitiesByLookupKeysAsync(emaily, cancellationToken);

	/// <summary>
	/// Párovací klíč je email.
	/// </summary>
	protected override Expression<Func<Uzivatel, string>> LookupKeyExpression => uzivatel => uzivatel.Email;

	protected override LookupServiceOptimizationHints OptimizationHints => LookupServiceOptimizationHints.None;

	/// <summary>
	/// IncludeDeleted nastavitelné, pro účely unit testu.
	/// </summary>
	protected override bool IncludeDeleted => includeDeleted;
	private bool includeDeleted = false;

	/// <summary>
	/// ThrowExceptionWhenNotFound nastavitelné, pro účely unit testu.
	/// </summary>
	protected override bool ThrowExceptionWhenNotFound => throwExceptionWhenNotFound;
	private bool throwExceptionWhenNotFound = true;

	/// <summary>
	/// Filter nastavitelný, pro účely unit testu.
	/// </summary>
	protected override Expression<Func<Uzivatel, bool>> Filter => filter;
	private Expression<Func<Uzivatel, bool>> filter;

	public void SetIncludeDeleted(bool value) => includeDeleted = value;
	public void SetThrowExceptionWhenNotFound(bool value) => throwExceptionWhenNotFound = value;
	public void SetFilter(Expression<Func<Uzivatel, bool>> value) => filter = value;

	public new void Invalidate(Changes changes) // chceme metodu Invalidate zveřejnit pro možnost použití v unit testu
	{
		base.Invalidate(changes);
	}

}
