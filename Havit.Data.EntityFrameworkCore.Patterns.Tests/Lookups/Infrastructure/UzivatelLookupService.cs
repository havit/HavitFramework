using Havit.Data.EntityFrameworkCore.Patterns.Lookups;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Repositories;
using System.Linq.Expressions;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Lookups.Infrastructure;

public class UzivatelLookupService : LookupServiceBase<string, Uzivatel>
{

	public UzivatelLookupService(IEntityLookupDataStorage lookupStorage, IRepository<Uzivatel, int> repository, IDbContext dbContext, IEntityKeyAccessor entityKeyAccessor, ISoftDeleteManager softDeleteManager) : base(lookupStorage, repository, dbContext, entityKeyAccessor, softDeleteManager)
	{
	}

	/// <summary>
	/// Vyhledá uživatele podle emailu.
	/// </summary>
	public Uzivatel GetUzivatelByEmail(string email) => GetEntityByLookupKey(email);

	/// <summary>
	/// Vyhledá uživatele podle emailu.
	/// </summary>
	public async Task<Uzivatel> GetUzivatelByEmailAsync(string email, CancellationToken cancellationToken = default) => await GetEntityByLookupKeyAsync(email, cancellationToken);

	/// <summary>
	/// Vyhledá uživatele podle emailů.
	/// </summary>
	public List<Uzivatel> GetUzivateleByEmails(string[] emaily) => GetEntitiesByLookupKeys(emaily);

	/// <summary>
	/// Vyhledá uživatele podle emailů.
	/// </summary>
	public async Task<List<Uzivatel>> GetUzivateleByEmailsAsync(string[] emaily, CancellationToken cancellationToken = default) => await GetEntitiesByLookupKeysAsync(emaily, cancellationToken);

	internal override Task<EntityLookupData<Uzivatel, int, string>> CreateEntityLookupDataAsync(CancellationToken cancellationToken = default)
	{
		// EntityFrameworkCore poskytuje možnost použít asynchronní metody ToListAsync, apod.
		// Tyto metody však nelze (neumíme) použít v unit testech - na mockovaných dbSetech, resp. mockovaných IQueryable
		// nelze ToListAsync apod. použít.
		// Proto zde vyměníme logiku asynchronního načtení dat pro účely unit testu za synchronní variantu.
		// A ano, asynchronní způsob získání dat tak není otestován, nicméně pro podobnost se synchronní variantou
		// "snad" není nutné toto testovat.
		return Task.FromResult(CreateEntityLookupData());
	}

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
