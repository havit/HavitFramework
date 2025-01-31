using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;
using Havit.Data.Entity.Patterns.UnitOfWorks;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Entity.Patterns.Windsor.Installers;

/// <summary>
/// Nastavení registrace komponent installeru Havit.Data.Entity.Patterns a souvisejících služeb.
/// </summary>
public class ComponentRegistrationOptions
{
	/// <summary>
	/// Typ použitého UnitOfWork.
	/// Výchozí hodnota je DbUnitOfWork.
	/// </summary>
	public Type UnitOfWorkType
	{
		get
		{
			Contract.Assert<InvalidOperationException>(unitOfWorkType != null);
			return unitOfWorkType;
		}
		set
		{
			Contract.Assert<InvalidOperationException>(value != null);
			unitOfWorkType = value;
		}
	}
	private Type unitOfWorkType;

	/// <summary>
	/// Výchozí lifestyle, pokud konkrétní není uveden. Není-li hodnota nastavena, vyhazuje výjimku InvalidOperationException.
	/// Výchozí hodnota není definována.
	/// </summary>
	public Func<LifestyleGroup<object>, ComponentRegistration<object>> GeneralLifestyle
	{
		get
		{
			Contract.Assert<InvalidOperationException>(generalLifestyle != null);
			return generalLifestyle;
		}
		set
		{
			Contract.Requires<ArgumentNullException>(value != null);
			generalLifestyle = value;
		}
	}
	private Func<LifestyleGroup<object>, ComponentRegistration<object>> generalLifestyle;

	/// <summary>
	/// Lifestyle pro DbContext. Pokud není uveden, použije se GeneralLifestyle.
	/// </summary>
	public Func<LifestyleGroup<object>, ComponentRegistration<object>> DbContextLifestyle
	{
		get
		{
			return dbContextLifestyle ?? GeneralLifestyle;
		}
		set
		{
			Contract.Requires<ArgumentNullException>(value != null);
			dbContextLifestyle = value;
		}
	}
	private Func<LifestyleGroup<object>, ComponentRegistration<object>> dbContextLifestyle;

	/// <summary>
	/// Lifestyle pro repositories. Pokud není uveden, použije se GeneralLifestyle.
	/// </summary>
	public Func<LifestyleGroup<object>, ComponentRegistration<object>> RepositoriesLifestyle
	{
		get
		{
			return repositoriesLifestyle ?? GeneralLifestyle;
		}
		set
		{
			Contract.Requires<ArgumentNullException>(value != null);
			repositoriesLifestyle = value;
		}
	}
	private Func<LifestyleGroup<object>, ComponentRegistration<object>> repositoriesLifestyle;

	/// <summary>
	/// Lifestyle pro data entries. Pokud není uveden, použije se GeneralLifestyle.
	/// </summary>
	public Func<LifestyleGroup<object>, ComponentRegistration<object>> DataEntriesLifestyle
	{
		get
		{
			return dataEntriesLifestyle ?? GeneralLifestyle;
		}
		set
		{
			Contract.Requires<ArgumentNullException>(value != null);
			dataEntriesLifestyle = value;
		}
	}
	private Func<LifestyleGroup<object>, ComponentRegistration<object>> dataEntriesLifestyle;

	/// <summary>
	/// Lifestyle pro unit of work. Pokud není uveden, použije se GeneralLifestyle.
	/// </summary>
	public Func<LifestyleGroup<object>, ComponentRegistration<object>> UnitOfWorkLifestyle
	{
		get
		{
			return unitOfWorkLifestyle ?? GeneralLifestyle;
		}
		set
		{
			Contract.Requires<ArgumentNullException>(value != null);
			unitOfWorkLifestyle = value;
		}
	}
	private Func<LifestyleGroup<object>, ComponentRegistration<object>> unitOfWorkLifestyle;

	/// <summary>
	/// Lifestyle pro DataLoader. Pokud není uveden, použije se GeneralLifestyle.
	/// </summary>
	public Func<LifestyleGroup<object>, ComponentRegistration<object>> DataLoaderLifestyle
	{
		get
		{
			return dataLoaderLifestyle ?? GeneralLifestyle;
		}
		set
		{
			Contract.Requires<ArgumentNullException>(value != null);
			dataLoaderLifestyle = value;
		}
	}
	private Func<LifestyleGroup<object>, ComponentRegistration<object>> dataLoaderLifestyle;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public ComponentRegistrationOptions()
	{
		UnitOfWorkType = typeof(DbUnitOfWork);
	}

}
