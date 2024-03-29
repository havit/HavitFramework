﻿// ** I18N
Calendar._DN = new Array('Neděle', 'Pondělí', 'Úterý', 'Středa', 'Čtvrtek', 'Pátek', 'Sobota', 'Neděle');
Calendar._SDN = new Array('Ne', 'Po', 'Út', 'St', 'Čt', 'Pá', 'So', 'Ne');
Calendar._FD = 1;
Calendar._MN = new Array('Leden', 'Únor', 'Březen', 'Duben', 'Květen', 'Červen', 'Červenec', 'Srpen', 'Září', 'Říjen', 'Listopad', 'Prosinec');
Calendar._SMN = new Array('Led', 'Úno', 'Bře', 'Dub', 'Kvě', 'Črv', 'Čvc', 'Srp', 'Zář', 'Říj', 'Lis', 'Pro');

// tooltips
Calendar._TT = {};
Calendar._TT["INFO"] = "O komponentě kalendář";
Calendar._TT["TOGGLE"] = "Změna prvního dne v týdnu";
Calendar._TT["PREV_YEAR"] = "Předchozí rok (přidrž pro menu)";
Calendar._TT["PREV_MONTH"] = "Předchozí měsíc (přidrž pro menu)";
Calendar._TT["GO_TODAY"] = "Dnešní datum";
Calendar._TT["NEXT_MONTH"] = "Další měsíc (přidrž pro menu)";
Calendar._TT["NEXT_YEAR"] = "Další rok (přidrž pro menu)";
Calendar._TT["SEL_DATE"] = "Vyber datum";
Calendar._TT["DRAG_TO_MOVE"] = "Chyť a táhni, pro přesun";
Calendar._TT["PART_TODAY"] = " (dnes)";
Calendar._TT["MON_FIRST"] = "Ukaž jako první Pondělí";
//Calendar._TT["SUN_FIRST"] = "Ukaž jako první Neděli";

Calendar._TT["ABOUT"] =
	"DHTML Date/Time Selector\n" +
	"(c) dynarch.com 2002-2005 / Author: Mihai Bazon\n" +
	"(c) HAVIT, s.r.o. 2005 / ASP.NET Integration & Patches\n" +
	"\n\n" +
	"Výběr data:\n" +
	"- Použijte tlačítka \xab, \xbb pro výběr roku\n" +
	"- Použijte tlačítka " + String.fromCharCode(0x2039) + ", " + String.fromCharCode(0x203a) + " k výběru měsíce\n" +
	"- Podržte tlačítko myši na jakémkoliv z těch tlačítek pro rychlejší výběr.";

Calendar._TT["ABOUT_TIME"] = "\n\n" +
	"Výběr času:\n" +
	"- Klikněte na jakoukoliv z částí výběru času pro zvýšení.\n" +
	"- nebo Shift-click pro snížení\n" +
	"- nebo klikněte a táhněte pro rychlejší výběr.";

// the following is to inform that "%s" is to be the first day of week
// %s will be replaced with the day name.
Calendar._TT["DAY_FIRST"] = "Zobraz %s první";

// This may be locale-dependent.  It specifies the week-end days, as an array
// of comma-separated numbers.  The numbers are from 0 to 6: 0 means Sunday, 1
// means Monday, etc.
Calendar._TT["WEEKEND"] = "0,6";

Calendar._TT["CLOSE"] = "Zavřít";
Calendar._TT["TODAY"] = "Dnes";
Calendar._TT["TIME_PART"] = "(Shift-)Klikni nebo táhni pro změnu hodnoty";

// date formats
Calendar._TT["DEF_DATE_FORMAT"] = "%d.%m.%Y";
Calendar._TT["TT_DATE_FORMAT"] = "%a, %b %e";

Calendar._TT["WK"] = "td";
Calendar._TT["TIME"] = "Čas:";
