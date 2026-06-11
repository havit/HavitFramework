using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Havit.Data.EntityFrameworkCore.Tests.Infrastructure.Entity;

/// <summary>
/// DbContext, jehož SaveChanges/SaveChangesAsync vyhodí výjimku <see cref="ExceptionToThrow"/>, je-li nastaveno <see cref="ThrowOnSave"/>.
/// </summary>
public class ToggleThrowOnSaveDbContext : DbContext
{
	/// <summary>
	/// Indikuje, zda má SaveChanges/SaveChangesAsync vyhodit výjimku <see cref="ExceptionToThrow"/>.
	/// </summary>
	public bool ThrowOnSave { get; set; }

	/// <summary>
	/// Výjimka, která je vyhozena při SaveChanges/SaveChangesAsync, je-li nastaveno <see cref="ThrowOnSave"/>.
	/// </summary>
	public DbUpdateException ExceptionToThrow { get; } = new DbUpdateException("Test exception", (Exception)null);

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);

		optionsBuilder.UseInMemoryDatabase(nameof(ToggleThrowOnSaveDbContext));
		optionsBuilder.AddInterceptors(new ToggleThrowOnSaveSaveChangesInterceptor(this));
	}

	private class ToggleThrowOnSaveSaveChangesInterceptor : SaveChangesInterceptor
	{
		private readonly ToggleThrowOnSaveDbContext _dbContext;

		public ToggleThrowOnSaveSaveChangesInterceptor(ToggleThrowOnSaveDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
		{
			ThrowWhenConfigured();
			return result;
		}

		public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
		{
			ThrowWhenConfigured();
			return ValueTask.FromResult(result);
		}

		private void ThrowWhenConfigured()
		{
			if (_dbContext.ThrowOnSave)
			{
				throw _dbContext.ExceptionToThrow;
			}
		}
	}
}
