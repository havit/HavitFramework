using System;
using Hangfire.States;

namespace Havit.Hangfire.Extensions.States;

/// <summary>
/// FailedState, který je zároveň finální, originální <see cref="FailedState" /> není finální.
/// Finální stavy expirují (v důsledku se na záznamech v databázi nastavuje hodnota ve sloupci ExpirateAt), ostatním je nastavována expirace na null.
/// Aby vše fungovalo (dashboard, atp.) musí se stav jmenovat (vlastnost Name) stejně jako FailedState. Proto je zvolena implementace zděděním původní třídy.
/// <seealso href="https://github.com/HangfireIO/Hangfire/issues/1211" />.
///
/// Poznámka:
/// Toto celé má smysl řešit, pokud chceme FailedState považovat za finální. Ve výchozím chování je FailedState považován za stav,
/// který je zotavitelný opakovaným zpracováním jobu. V případě opakovaného neúspěchu je job přepnut do stavu Deleted, který je finální.
/// </summary>
public class FinalFailedState : FailedState, IState
{
	/// <inheritdoc />
	bool IState.IsFinal => true; // v bázové třídě není virtuální, je však možné implementovat explicitně rozhraní IState.

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public FinalFailedState(Exception exception, string serverId) : base(exception, serverId)
	{
	}

	/// <summary>
	/// Vrátí instanci inicializovanou originálním <see cref="FailedState"/>.
	/// </summary>
	public static FinalFailedState From(FailedState failedState)
	{
		return new FinalFailedState(failedState.Exception, failedState.ServerId)
		{
			Reason = failedState.Reason,
			MaxLinesInStackTrace = failedState.MaxLinesInStackTrace,
		};
	}
}