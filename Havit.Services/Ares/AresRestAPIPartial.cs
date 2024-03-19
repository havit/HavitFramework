using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.Ares;
public partial class EkonomickySubjekt
{
	public Ares_Extension AresExtension { get; set; }
}

public class Ares_Extension
{
	public string RejstrikovySoudText { get; set; }
	public string SpisovaZnackaFull { get; set; }
	public string FinancniUradText { get; set; }
	public string PravniFormaText { get; set; }
	public string SidloPscText { get; set; }
	public string[] SidloAddressLine { get; set; }
	public bool IsDorucovaciAdresaStejna { get; set; }
	public bool IsPlatceDph { get; set; }
}