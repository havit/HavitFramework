﻿//////////////////////////////////////////////////////////////////////////////////////////////
//	Turkish Translation by Nuri AKMAN
//	Location: Ankara/TURKEY
//	e-mail	: nuriakman@hotmail.com
//	Date	: April, 9 2003
//
//	Note: if Turkish Characters does not shown on you screen
//		  please include falowing line your html code:
//
//		  <meta http-equiv="Content-Type" content="text/html; charset=windows-1254">
//
//////////////////////////////////////////////////////////////////////////////////////////////

// ** I18N
Calendar._DN = new Array("Pazar", "Pazartesi", "Salı", "Çarsamba", "Persembe", "Cuma", "Cumartesi", "Pazar");
Calendar._SDN = new Array("Paz", "Paz", "Sal", "Çar", "Per", "Cum", "Cum", "Paz");
Calendar._FD = 1;
Calendar._MN = new Array("Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık");
Calendar._SMN = new Array("Oca", "Şub", "Mar", "Nis", "May", "Haz", "Tem", "Ağu", "Eyl", "Eki", "Kas", "Ara");
// tooltips
Calendar._TT = {};
Calendar._TT["INFO"] = "About the calendar";
Calendar._TT["ABOUT"] =
	"DHTML Date/Time Selector\n" +
	"(c) dynarch.com 2002-2005 / Author: Mihai Bazon\n" + // don't translate this this ;-)
	"For latest version visit: http://www.dynarch.com/projects/calendar/\n" +
	"Distributed under GNU LGPL.  See http://gnu.org/licenses/lgpl.html for details." +
	"\n\n" +
	"Date selection:\n" +
	"- Use the \xab, \xbb buttons to select year\n" +
	"- Use the " + String.fromCharCode(0x2039) + ", " + String.fromCharCode(0x203a) + " buttons to select month\n" +
	"- Hold mouse button on any of the above buttons for faster selection.";
Calendar._TT["ABOUT_TIME"] = "\n\n" +
	"Time selection:\n" +
	"- Click on any of the time parts to increase it\n" +
	"- or Shift-click to decrease it\n" +
	"- or click and drag for faster selection.";
Calendar._TT["TOGGLE"] = "Haftanýn ilk gününü kaydýr";
Calendar._TT["PREV_YEAR"] = "Önceki Yýl (Menü için basýlý tutunuz)";
Calendar._TT["PREV_MONTH"] = "Önceki Ay (Menü için basýlý tutunuz)";
Calendar._TT["GO_TODAY"] = "Bugün'e git";
Calendar._TT["NEXT_MONTH"] = "Sonraki Ay (Menü için basýlý tutunuz)";
Calendar._TT["NEXT_YEAR"] = "Sonraki Yýl (Menü için basýlý tutunuz)";
Calendar._TT["SEL_DATE"] = "Tarih seçiniz";
Calendar._TT["DRAG_TO_MOVE"] = "Taþýmak için sürükleyiniz";
Calendar._TT["PART_TODAY"] = " (bugün)";
Calendar._TT["MON_FIRST"] = "Takvim Pazartesi gününden baþlasýn";
Calendar._TT["SUN_FIRST"] = "Takvim Pazar gününden baþlasýn";
Calendar._TT["CLOSE"] = "Kapat";
Calendar._TT["TODAY"] = "Bugün";
Calendar._TT["WEEKEND"] = "0,6";
Calendar._TT["DAY_FIRST"] = "İlk önce %s günü göster";
// date formats
Calendar._TT["DEF_DATE_FORMAT"] = "%d-%m-%y";
Calendar._TT["TT_DATE_FORMAT"] = "%A, %d-%m-%y";
Calendar._TT["WK"] = "Hafta";
Calendar._TT["TIME"] = "Zaman:";