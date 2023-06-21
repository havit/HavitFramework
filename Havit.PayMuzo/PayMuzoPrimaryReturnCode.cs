#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Havit.PayMuzo;

/// <summary>
/// Třída pro primární návratový kód PayMUZO.
/// </summary>
public class PayMuzoPrimaryReturnCode : PayMuzoReturnCode
{
	public static PayMuzoPrimaryReturnCode Ok { get { return FindByValue(0); } }
	public static PayMuzoPrimaryReturnCode FieldTooLong { get { return FindByValue(1); } }
	public static PayMuzoPrimaryReturnCode FieldTooShort { get { return FindByValue(2); } }
	public static PayMuzoPrimaryReturnCode IncorrectFieldContent { get { return FindByValue(3); } }
	public static PayMuzoPrimaryReturnCode FieldNull { get { return FindByValue(4); } }
	public static PayMuzoPrimaryReturnCode RequiredFieldMissing { get { return FindByValue(5); } }
	public static PayMuzoPrimaryReturnCode UnknownMerchant { get { return FindByValue(11); } }
	public static PayMuzoPrimaryReturnCode DuplicateOrderNumber { get { return FindByValue(14); } }
	public static PayMuzoPrimaryReturnCode ObjectNotFound { get { return FindByValue(15); } }
	public static PayMuzoPrimaryReturnCode ApprovedAmmountExceeded { get { return FindByValue(17); } }
	public static PayMuzoPrimaryReturnCode DepositedAmmountExceeded { get { return FindByValue(18); } }
	public static PayMuzoPrimaryReturnCode InvalidObjectState { get { return FindByValue(20); } }
	public static PayMuzoPrimaryReturnCode AuthorizationCenterConnectionFailed { get { return FindByValue(26); } }
	public static PayMuzoPrimaryReturnCode IncorrectOrderType { get { return FindByValue(27); } }
	public static PayMuzoPrimaryReturnCode DeclinedIn3D { get { return FindByValue(28); } }
	public static PayMuzoPrimaryReturnCode DeclinedInAuthorizationCenter { get { return FindByValue(30); } }
	public static PayMuzoPrimaryReturnCode WrongDigest { get { return FindByValue(31); } }
	public static PayMuzoPrimaryReturnCode TechnicalProblem { get { return FindByValue(1000); } }

	/// <summary>
	/// Vytvoří instanci <see cref="PayMuzoPrimaryReturnCode"/>.
	/// </summary>
	/// <param name="value">numeric code</param>
	/// <param name="csText">význam česky</param>
	/// <param name="enText">význam anglicky</param>
	protected PayMuzoPrimaryReturnCode(int value, string csText, string enText)
		: base(value, csText, enText)
	{
	}

	/// <summary>
	/// Statický constructor
	/// </summary>
	static PayMuzoPrimaryReturnCode()
	{
		RegisterCode(new PayMuzoPrimaryReturnCode(0, "OK", "OK"));
		RegisterCode(new PayMuzoPrimaryReturnCode(1, "Pole příliš dlouhé", "Field too long"));
		RegisterCode(new PayMuzoPrimaryReturnCode(2, "Pole příliš krátké", "Field too short"));
		RegisterCode(new PayMuzoPrimaryReturnCode(3, "Chybný obsah pole", "Incorrect content of field"));
		RegisterCode(new PayMuzoPrimaryReturnCode(4, "Pole je prázdné", "Field is null"));
		RegisterCode(new PayMuzoPrimaryReturnCode(5, "Chybí povinné pole", "Missing required field"));
		RegisterCode(new PayMuzoPrimaryReturnCode(11, "Neznámý obchodník", "Unknown merchant"));
		RegisterCode(new PayMuzoPrimaryReturnCode(14, "Duplikátní číslo objednávky", "Duplicate order number"));
		RegisterCode(new PayMuzoPrimaryReturnCode(15, "Objekt nenalezen", "Object not found"));
		RegisterCode(new PayMuzoPrimaryReturnCode(17, "Částka k úkradě překročila autorizovanou částku", "Amount to deposit exceeds approved amount"));
		RegisterCode(new PayMuzoPrimaryReturnCode(18, "Součet kreditovaných částek překročil uhrazenou částku", "Total sum of credited amounts exceede deposited amount"));
		RegisterCode(new PayMuzoPrimaryReturnCode(20, "Objekt není ve stavu odpovídajícím této operaci", "Object not in valid state for operation"));
		RegisterCode(new PayMuzoPrimaryReturnCode(26, "Technický problém při spojení s autoriazačním centrem", "Technical problem in connection to authorization center"));
		RegisterCode(new PayMuzoPrimaryReturnCode(27, "Chybný typ objednávky", "Incorrect order type"));
		RegisterCode(new PayMuzoPrimaryReturnCode(28, "Zamítnuto v 3D", "Declined in 3D"));
		RegisterCode(new PayMuzoPrimaryReturnCode(30, "Zamítnuto v autorizačním centru", "Declined in AC"));
		RegisterCode(new PayMuzoPrimaryReturnCode(31, "Chybný podpis", "Wrong digest"));
		RegisterCode(new PayMuzoPrimaryReturnCode(1000, "Technický problém", "Technical problem"));
	}

	/// <summary>
	/// Najde instanci podle numerické hodnoty kódu. Pokud není nalezen, vrací <c>null</c>.
	/// </summary>
	/// <param name="value">numerická hodnota kódu</param>
	public static PayMuzoPrimaryReturnCode FindByValue(int value)
	{
		return PayMuzoReturnCode.FindByValueInternal<PayMuzoPrimaryReturnCode>(value);
	}
}
#pragma warning restore 1591
