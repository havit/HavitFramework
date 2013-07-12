using Havit.Diagnostics.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// FormView podporující extrakci hodnot do datového objektu, automatické stránkování.
	/// </summary>
	public class FormViewExt: FormView
	{
		#region RequiresDataBinding (new), SetRequiresDatabinding
		/// <summary>
		/// Zpřístupňuje pro čtení chráněnou vlastnost RequiresDataBinding.
		/// </summary>
		public new bool RequiresDataBinding
		{
			get
			{
				return base.RequiresDataBinding;
			}
			protected set
			{
				base.RequiresDataBinding = value;
			}
		}

		/// <summary>
		/// Nastaví RequiresDataBinding na true.
		/// Zajistí zavolání databindingu ještě v aktuálním requestu. Běžně v OnPreRender,
		/// pokud je ale FormView schovaný, pak se DataBind volá z Page.PreRenderComplete.
		/// </summary>
		public void SetRequiresDatabinding()
		{
			RequiresDataBinding = true;
			_currentlyRequiresDataBinding = true;
		}

		/// <summary>
		/// Příznak, zda má dojít k databindingu ještě v tomto requestu.
		/// Nastavováno (na true) v metodě SetRequiresDataBinding, vypínáno v metodě PerformDataBinding.
		/// </summary>
		private bool _currentlyRequiresDataBinding = false;
		#endregion

		#region AutoDataBind
		/// <summary>
		/// Nastavuje automatický databind na FormView.		
		/// </summary>
		public bool AutoDataBind
		{
			get
			{
				return (bool)(ViewState["AutoDataBind"] ?? true);
			}
			set
			{
				ViewState["AutoDataBind"] = value;
			}
		}
		#endregion

		#region DataSource
		/// <summary>
		/// Nastaví objekt nebo kolekci jako datový zdroj FormView.
		/// </summary>
		public override object DataSource
		{
			get
			{
				return base.DataSource;
			}
			set
			{
				if ((value != null) && !(value is IEnumerable))
				{
					base.DataSource = new object[] { value };
				}
				else
				{
					base.DataSource = value;
				}
			}
		}
		#endregion

		#region OnInit
		/// <summary>
		/// Inicializuje FormViewExt.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			// Pokud dojde k vyvolání události, který nemá obsluhu, je vyvolána výjimka.
			// Protože ale některé záležitosti řešíme sami, nastavíme "prázdné" obsluhy událostí
			// (nasměrujeme je do černé díry).			
			this.ModeChanging += FormViewExt_EventBlackHole;
			this.PageIndexChanging += FormViewExt_EventBlackHole;

			this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
		}

		private void FormViewExt_EventBlackHole(object sender, EventArgs e)
		{
			// NOOP
		}
		#endregion

		#region OnModeChanging
		/// <summary>
		/// Zajišťuje automatické přecházení mezi režimy FormView (FormViewMode).
		/// </summary>
		protected override void OnModeChanging(FormViewModeEventArgs e)
		{
			base.OnModeChanging(e);
			if (!e.Cancel)
			{
				if (e.CancelingEdit) // pokud je nastaven DefaultMode na editaci a dojde ke Cancelu, e.NewMode je stále Edit, což nechceme
				{
					ChangeMode(FormViewMode.ReadOnly);
				}
				else
				{
					ChangeMode(e.NewMode);
				}
				SetRequiresDatabinding();
			}
		}
		#endregion

		#region OnPageIndexChanging
		/// <summary>
		/// Zajišťuje automatické stránkování - přechod mezi záznamy.
		/// </summary>
		protected override void OnPageIndexChanging(FormViewPageEventArgs e)
		{
			base.OnPageIndexChanging(e);
			if (!e.Cancel)
			{
				PageIndex = e.NewPageIndex;
				SetRequiresDatabinding();
			}
		}
		#endregion

		#region PerformDataBinding
		/// <summary>
		/// Zajišťuje data-binding dat na FormView.
		/// </summary>
		protected override void PerformDataBinding(System.Collections.IEnumerable data)
		{
			base.PerformDataBinding(data);

			if (data != null)
			{
				this.RequiresDataBinding = false;
				_currentlyRequiresDataBinding = false;
			}
		}
		#endregion

		#region OnPreRender
		/// <summary>
		/// Zajistíme DataBinding, pokud mají vlastnosti AutoDataBind a RequiresDataBinding hodnotu true.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			if (AutoDataBind && RequiresDataBinding)
			{
				DataBind();
			}

			base.OnPreRender(e);
		}
		#endregion

		#region Page_PreRenderComplete
		private void Page_PreRenderComplete(object sender, EventArgs e)
		{
			// pokud je control schovaný (není visible), nevolá se jeho OnPreRender.
			// Pokud ale byla zavolána metoda SetRequiresDatabinding, určitě chceme, aby k databindingu došlo ještě v tomto requestu.
			// Proto se něvěsíme na Page.PreRenderComplete jako nouzové řešení.
			if (_currentlyRequiresDataBinding && AutoDataBind && RequiresDataBinding)
			{
				DataBind();
			}
		}
		#endregion

		#region ExtractValues
		/// <summary>
		/// Vyzvedne hodnoty, které jsou nabidnované způsobem pro two-way databinding.
		/// Hodnoty nastaví jako vlastnosti předanému datovému objektu.
		/// </summary>
		/// <param name="dataObject">Datová objekt, jehož hodnoty jsou nastaveny.</param>
		public void ExtractValues(object dataObject)
		{
			Contract.Requires(dataObject != null);

			System.Collections.Specialized.IOrderedDictionary fieldValues = new System.Collections.Specialized.OrderedDictionary();
			this.ExtractRowValues(fieldValues, false);
			DataBinderExt.SetValues(dataObject, fieldValues);
		}
		#endregion
	}
}
