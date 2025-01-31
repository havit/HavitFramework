using Havit.Diagnostics.Contracts;

namespace Havit.Business;

/// <summary>
/// Předek generického typu <see cref="PropertyHolder{T}"/>. 
/// Potřebujeme kolekci PropertyHolderů a kolekci generických typů nelze udělat.
/// </summary>
public abstract class PropertyHolderBase
{
	/// <summary>
	/// Objekt, kterému property patří.
	/// </summary>
	protected BusinessObjectBase Owner
	{
		get { return _owner; }
	}
	private readonly BusinessObjectBase _owner;

	/// <summary>
	/// Indikuje, zda došlo ke změně hodnoty.
	/// </summary>
	public bool IsDirty
	{
		get
		{
			return _isDirty;
		}
		set
		{
			_isDirty = value;
			if (_isDirty)
			{
				Owner.IsDirty = true;
			}
		}
	}
	private bool _isDirty = false;

	/// <summary>
	/// Indikuje, zda je hodnota property nastavena.
	/// </summary>
	public bool IsInitialized
	{
		get
		{
			return _isInitialized;
		}
		protected set
		{
			_isInitialized = value;
		}
	}
	private bool _isInitialized = false;

	/// <summary>
	/// Pokud nebyla hodnota PropertyHolderu nastavena, vyhodí InvalidOperationException.
	/// Pokud byla hodnota PropertyHolderu nastavena, neudělá nic (projde).
	/// </summary>
	protected void CheckInitialization()
	{
		if (!_isInitialized)
		{
			throw new InvalidOperationException("Hodnota nebyla inicializována.");
		}
	}

	/// <summary>
	/// Založí instanci PropertyHolderu.
	/// </summary>
	/// <param name="owner">objekt, kterému PropertyHolder patří</param>
	protected PropertyHolderBase(BusinessObjectBase owner)
	{
		Contract.Requires<ArgumentNullException>(owner != null, nameof(owner));

		this._owner = owner;
	}
}
