using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.PayMuzo;

/// <summary>
/// Stavy objednávky v systému PayMUZO. Výsledek operace queryOrderState.
/// </summary>
public enum PayMuzoOrderState
{
	/// <summary>
	/// Neukončena. Objednávka byla úspěšně přijata do Pay MUZO eBanka – čeká se na výsledek vyplnění citlivých informací držitele karty.
	/// </summary>
	Requested = 1,

	/// <summary>
	/// Neukončena. Pokud držitel karty vyplnil citlivé informace, je zaslán dotaz do 3D systému, zda je požadována autentikace držitele karty. Čeká se na výsledek z 3D systému.
	/// </summary>
	Pending = 2,

	/// <summary>
	/// Neukončena. Jestliže držitel karty přeruší zadávání údajů karty, je to finální stav objednávky.
	/// </summary>
	Created = 3,

	/// <summary>
	/// Autorizována. Výsledek autentikace držitele karty povoluje pokračování, byl poslán požadavek na autorizaci do autorizačního centra. Výsledek autorizace objednávky je úspěšný.
	/// </summary>
	Approved = 4,

	/// <summary>
	/// Reverzována. Autorizace objednávky byla zneplatněna. Na straně držitele karty došlo k odblokování původně autorizovaných finančních prostředků.
	/// </summary>
	ApproveReversed = 5,

	/// <summary>
	/// Neautorizována. Výsledek autorizace objednávky je neúspěšný, objednávku není možné uhradit. Není možné pokračovat.
	/// </summary>
	Unapproved = 6,

	/// <summary>
	/// Uhrazena. Objednávka byla označena pro uhrazení během následného dávkového zpracování. Je možné zneplatnit úhradu objednávky do okamžiku, než proběhne uzavření dávky, ve které se daná úhrada nachází. 
	/// </summary>
	DepositedBatchOpened = 7,

	/// <summary>
	/// Zpracována. Proběhl automatický proces uzavírání dávek a přenos dat do finančních systémů.
	/// </summary>
	DepositedBatchClosed = 8,

	/// <summary>
	/// Uzvařena. Objednávka byla uzavřena. Jediná přípustná operace je vymazání.
	/// </summary>
	OrderClosed = 9,

	/// <summary>
	/// Vymazána. Objednávka byla odstraněna.
	/// </summary>
	Deleted = 10,

	/// <summary>
	/// Kreditována. Objednávka byla označena pro návrat během následného dávkového zpracování.
	/// </summary>
	CreditedBatchOpened = 11,

	/// <summary>
	/// Kreditována. Je možné zneplatnit návrat objednávky do okamžiku, než proběhne uzavření dávky, ve které se daná objednávka nachází. Po uzavření dávky zůstává v tomto stavu. 
	/// </summary>
	CreditedBatchClosed = 12,

	/// <summary>
	/// Zamítnuta. Výsledek autentikace držitele karty v 3D systému je neúspěšný. Držitel karty není autentikován – není možné pokračovat. Objednávku je možné odstranit.
	/// </summary>
	Declined = 13
}
