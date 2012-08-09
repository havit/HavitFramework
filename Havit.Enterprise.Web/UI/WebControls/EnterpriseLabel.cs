using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// EnterpriseLabel je label, který:
	/// a) umí brát text z resources (na základě ItemPropertyInfo),
	/// b) umí renderovat hvězdičku (nebo jiný symbol) pro povinná pole (na základě IsRequired nebo ItemPropertyInfo).
	/// </summary>
	[Themeable(true)]
	public class EnterpriseLabel: Label
	{
		#region ItemPropertyInfo
		/// <summary>
		/// ItemPropertyInfo. Na základě něj se určuje povinnost pole nebo text labelu z resourců.
		/// </summary>
		public Havit.Business.PropertyInfo ItemPropertyInfo { get; set; } 
		#endregion

		#region IsRequired
		/// <summary>
		/// Indikuje, zda bude pole renderováno jako povinné.
		/// Pokud není explicitně nastavena hodnota, vrací se hodnota dle ItemPropertyInfo (s výjimkou dle SupressIsRequiresForBooleanType).
		/// Pokud není vlastnost ItemPropertyInfo nastavena, vyhazuje výjimku.
		/// </summary>
		public bool IsRequired
		{
			get
			{
				bool? isRequired = (bool?)ViewState["IsRequired"];
				if (isRequired != null)
				{
					return isRequired.Value;
				}
				else
				{
					if (ItemPropertyInfo != null)
					{
						Havit.Business.FieldPropertyInfo fpi = ItemPropertyInfo as Havit.Business.FieldPropertyInfo;
						if (fpi != null)
						{
							// pokud je nastaveno, potlačíme povinnost datového typu boolean
							if (SupressIsRequiresForBooleanType && (fpi.FieldType == System.Data.SqlDbType.Bit))
							{
								return false;
							}
							return !fpi.Nullable;
						}
						else
						{
							// tohle vlastně říká (při současném stavu implementace), že kolekce nejsou povinné
							return false;
						}
					}
					else
					{
						throw new InvalidOperationException("Nelze číst hodnotu IsRequired, není nastaveno ani IsRequired, ani ItemPropertyInfo.");
					}
				}
			}
			set
			{
				ViewState["IsRequired"] = value;
			}
		} 
		#endregion

		#region SupressIsRequiresForBooleanType
		/// <summary>
		/// Potlačuje povinnost datového typu boolean pro UI.
		/// (Boolean je sice typicky povinný, ale edituje se obvykle checkboxem, který se nepovažuje za nutný z hlediska uživatele vyplňovat - uživatel by mohl povinnost chápat jako nutnost zaškrtnutí.)
		/// </summary>
		public bool SupressIsRequiresForBooleanType
		{
			get { return (bool)(ViewState["SupressIsRequiresForBooleanType"] ?? true); }
			set { ViewState["SupressIsRequiresForBooleanType"] = value; }
		}
		#endregion

		#region RequiredCssClass
		/// <summary>
		/// CssClass pro povinné pole (použito na celém labelu).
		/// </summary>
		public string RequiredCssClass
		{
			get { return (string)ViewState["RequiredCssClass"] ?? String.Empty; }
			set { ViewState["RequiredCssClass"] = value; }
		} 
		#endregion

		#region ShowRequired
		/// <summary>
		/// Indikuje, zda se má pro povinná pole renderovat symbol povinného pole a zda se má nastavovat css třída pro povinné pole.
		/// </summary>
		public bool ShowRequired
		{
			get { return (bool)(ViewState["ShowRequired"] ?? false); }
			set { ViewState["ShowRequired"] = value; }
		}
		#endregion

		#region RequiredSignCssClass
		/// <summary>
		/// CssClass použitý pro symbol povinného pole.
		/// </summary>
		public string RequiredSignCssClass
		{
			get { return (string)ViewState["RequiredSignCssClass"] ?? String.Empty; }
			set { ViewState["RequiredSignCssClass"] = value; }
		} 
		#endregion

		#region RequiredSignText
		/// <summary>
		/// Text (symbol) označující povinná pole.
		/// </summary>
		public string RequiredSignText
		{
			get { return (string)ViewState["RequiredSignText"] ?? String.Empty; }
			set { ViewState["RequiredSignText"] = value; }
		} 
		#endregion

		#region Render, RenderContents
		protected override void Render(HtmlTextWriter writer)
		{
			// pokud je pole povinné, nastavíme CssClass
			// nastavení hodnoty v Render zajišťuje, že se nám hodnota neserializuje do ViewState
			if (ShowRequired && IsRequired)
			{
				CssClass = RequiredCssClass;
			}

			// máme-li ItemPropertyInfo, nastavíme text (dle resources).
			if (ItemPropertyInfo != null)
			{
				// nastavení hodnoty v Render zajišťuje, že se nám hodnota neserializuje do ViewState
				Text = HttpUtilityExt.GetResourceString(String.Format(Text, ItemPropertyInfo.PropertyName, ItemPropertyInfo.Owner.ClassName, ItemPropertyInfo.Owner.Namespace));
			}

			base.Render(writer);
		}

		protected override void RenderContents(HtmlTextWriter writer)
		{
			base.RenderContents(writer);

			// pro povinná pole vyrenderujeme symbol povinného pole, pokud je to nastaveno.
			if (ShowRequired && IsRequired)
			{
				if (!String.IsNullOrEmpty(RequiredSignCssClass))
				{
					writer.AddAttribute(HtmlTextWriterAttribute.Class, RequiredSignCssClass);
				}
				writer.RenderBeginTag(HtmlTextWriterTag.Span);
				writer.Write(RequiredSignText);
				writer.RenderEndTag();
			}
		}
		#endregion

	}
}
