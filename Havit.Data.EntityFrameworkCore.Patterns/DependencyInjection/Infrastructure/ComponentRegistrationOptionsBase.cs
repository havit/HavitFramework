using System;
using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection
{
	/// <summary>
	/// Nastavení registrace komponent installeru Havit.Data.Entity.Patterns a souvisejících služeb.
	/// Bázová třída, potomci k použití pro jednotlivých DI kontejnery si určují typ, který definuje lifetime registrace služeb.
	/// </summary>
	public class ComponentRegistrationOptionsBase<TLifetime>
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
		public TLifetime GeneralLifestyle
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
		private TLifetime generalLifestyle;		

		/// <summary>
		/// Lifestyle pro DbContext. Pokud není uveden, použije se GeneralLifestyle.
		/// </summary>
		public TLifetime DbContextLifestyle
		{
			get
			{
				return dbContextLifestyleSet ? dbContextLifestyle : GeneralLifestyle;
			}
			set
			{
				Contract.Requires<ArgumentNullException>(value != null);
				dbContextLifestyle = value;
			}
		}
		private TLifetime dbContextLifestyle;
		private bool dbContextLifestyleSet = false;

		/// <summary>
		/// Lifestyle pro repositories. Pokud není uveden, použije se GeneralLifestyle.
		/// </summary>
		public TLifetime RepositoriesLifestyle
		{
			get
			{
				return repositoriesLifestyleSet ? repositoriesLifestyle : GeneralLifestyle;
			}
			set
			{
				Contract.Requires<ArgumentNullException>(value != null);
				repositoriesLifestyle = value;
			}
		}
		private TLifetime repositoriesLifestyle;
		private bool repositoriesLifestyleSet = false;

		/// <summary>
		/// Lifestyle pro data entries. Pokud není uveden, použije se GeneralLifestyle.
		/// </summary>
		public TLifetime DataEntriesLifestyle
		{
			get
			{
				return dataEntriesLifestyleSet ? dataEntriesLifestyle : GeneralLifestyle;
			}
			set
			{
				Contract.Requires<ArgumentNullException>(value != null);
				dataEntriesLifestyle = value;
			}
		}
		private TLifetime dataEntriesLifestyle;
		private bool dataEntriesLifestyleSet = false;

		/// <summary>
		/// Lifestyle pro unit of work. Pokud není uveden, použije se GeneralLifestyle.
		/// </summary>
		public TLifetime UnitOfWorkLifestyle
		{
			get
			{
				return unitOfWorkLifestyleSet ? unitOfWorkLifestyle : GeneralLifestyle;
			}
			set
			{
				Contract.Requires<ArgumentNullException>(value != null);
				unitOfWorkLifestyle = value;
			}
		}
		private TLifetime unitOfWorkLifestyle;
		private bool unitOfWorkLifestyleSet = false;

		/// <summary>
		/// Lifestyle pro DataLoader. Pokud není uveden, použije se GeneralLifestyle.
		/// </summary>
		public TLifetime DataLoaderLifestyle
		{
			get
			{
				return dataLoaderLifestyleSet ? dataLoaderLifestyle : GeneralLifestyle;
			}
			set
			{
				Contract.Requires<ArgumentNullException>(value != null);
				dataLoaderLifestyle = value;
			}
		}
		private TLifetime dataLoaderLifestyle;
		private bool dataLoaderLifestyleSet = false;

		/// <summary>
		/// Installer služeb pro cachování. Výchozí hodnotou je instance DefaultCachingInstalleru.
		/// </summary>
		public ICachingInstaller<TLifetime> CachingInstaller { get; set; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public ComponentRegistrationOptionsBase()
		{
			UnitOfWorkType = typeof(DbUnitOfWork);
			CachingInstaller = new DefaultCachingInstaller<TLifetime>();
		}		
	}
}
