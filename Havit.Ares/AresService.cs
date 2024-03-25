using Havit.Diagnostics.Contracts;

namespace Havit.Ares;

/// <inheritdoc/>
public class AresService : IAresService
{
	internal const string AresUrl = "https://ares.gov.cz/ekonomicke-subjekty-v-be/rest";
	/// <summary>
	/// Pro hledání dle názvu - maximální počet vrácených subjektů. Může jich být více nalezených - to se dozvíš v návratové hodnotě EkonomickeSubjektyResult.PocetNalezenych. ARES vrací max 1000 subjektů - jinak vrací chybu.
	/// </summary>
	public const int DefaultMaxResults = 100;

	private AresCiselnik _ciselnikPravniForma = new AresCiselnik("res", "PravniForma");
	private AresCiselnik _ciselnikRejstrikovySoud = new AresCiselnik("vr", "SoudVr");
	private AresCiselnik _ciselnikFinancniUrad = new AresCiselnik("ufo", "FinancniUrad");

	/// <inheritdoc/>
	public EkonomickySubjektItem GetEkonomickeSubjektyDleIco(string ico)
	{
		EkonomickeSubjektyKomplexFiltr ekonomickeSubjektyKomplexFiltr = GetEkonomickeSubjektyDleIco_PrepareRequest(ico);
		using HttpClient httpClient = new HttpClient();
		AresRestApi aresClient = new AresRestApi(httpClient);
		aresClient.BaseUrl = AresUrl;
		EkonomickeSubjektySeznam resp = aresClient.VyhledejEkonomickeSubjekty(ekonomickeSubjektyKomplexFiltr);
		EkonomickeSubjektyResult ekonomickeSubjektyResponse = GetEkonomickeSubjekty_ProcessResponse(resp);

		return ekonomickeSubjektyResponse.Items.SingleOrDefault();
	}

	/// <inheritdoc/>
	public async Task<EkonomickySubjektItem> GetEkonomickeSubjektyDleIcoAsync(string ico, CancellationToken cancellationToken = default)
	{
		EkonomickeSubjektyKomplexFiltr ekonomickeSubjektyKomplexFiltr = GetEkonomickeSubjektyDleIco_PrepareRequest(ico);
		using HttpClient httpClient = new HttpClient();
		AresRestApi aresClient = new AresRestApi(httpClient);
		aresClient.BaseUrl = AresUrl;
		EkonomickeSubjektySeznam resp = await aresClient.VyhledejEkonomickeSubjektyAsync(ekonomickeSubjektyKomplexFiltr).ConfigureAwait(false);
		EkonomickeSubjektyResult ekonomickeSubjektyResponse = GetEkonomickeSubjekty_ProcessResponse(resp);

		return ekonomickeSubjektyResponse.Items.SingleOrDefault();
	}

	/// <inheritdoc/>
	public EkonomickeSubjektyResult GetEkonomickeSubjektyDleObchodnihoJmena(string obchodniJmeno, int maxResult = DefaultMaxResults)
	{
		EkonomickeSubjektyKomplexFiltr ekonomickeSubjektyKomplexFiltr = GetEkonomickeSubjektyDleObchodnihoJmena_PrepareRequest(obchodniJmeno, maxResult);
		using HttpClient httpClient = new HttpClient();
		AresRestApi aresClient = new AresRestApi(httpClient);
		aresClient.BaseUrl = AresUrl;
		EkonomickeSubjektySeznam resp = aresClient.VyhledejEkonomickeSubjekty(ekonomickeSubjektyKomplexFiltr);
		EkonomickeSubjektyResult ekonomickeSubjektyResponse = GetEkonomickeSubjekty_ProcessResponse(resp);
		return ekonomickeSubjektyResponse;
	}

	/// <inheritdoc/>
	public async Task<EkonomickeSubjektyResult> GetEkonomickeSubjektyDleObchodnihoJmenaAsync(string obchodniJmeno, int maxResults = DefaultMaxResults, CancellationToken cancellationToken = default)
	{
		EkonomickeSubjektyKomplexFiltr ekonomickeSubjektyKomplexFiltr = GetEkonomickeSubjektyDleObchodnihoJmena_PrepareRequest(obchodniJmeno, maxResults);
		using HttpClient httpClient = new HttpClient();
		AresRestApi aresClient = new AresRestApi(httpClient);
		aresClient.BaseUrl = AresUrl;
		EkonomickeSubjektySeznam resp = await aresClient.VyhledejEkonomickeSubjektyAsync(ekonomickeSubjektyKomplexFiltr).ConfigureAwait(false);
		EkonomickeSubjektyResult ekonomickeSubjektyResponse = GetEkonomickeSubjekty_ProcessResponse(resp);
		return ekonomickeSubjektyResponse;
	}

	private EkonomickeSubjektyKomplexFiltr GetEkonomickeSubjektyDleIco_PrepareRequest(string ico)
	{
		Contract.Requires<ArgumentNullException>(ico != null);
		Contract.Requires<ArgumentException>(ico.Length == 8, $"Argument '{nameof(ico)}' musí mít předepsanou délku 8 znaků.");
		EkonomickeSubjektyKomplexFiltr ekonomickeSubjektyKomplexFiltr = new EkonomickeSubjektyKomplexFiltr
		{
			Start = 0,
			Pocet = 1,
			Ico = new List<string>() { ico }
		};
		return ekonomickeSubjektyKomplexFiltr;
	}

	private EkonomickeSubjektyKomplexFiltr GetEkonomickeSubjektyDleObchodnihoJmena_PrepareRequest(string obchodniJmeno, int maxResults)
	{
		Contract.Requires<ArgumentNullException>(obchodniJmeno != null);
		Contract.Requires<ArgumentException>(obchodniJmeno.Length != 0, $"Argument '{nameof(obchodniJmeno)}' nesmí obsahovat prázdný řetězec.");
		EkonomickeSubjektyKomplexFiltr ekonomickeSubjektyKomplexFiltr = new EkonomickeSubjektyKomplexFiltr
		{
			Start = 0,
			Pocet = maxResults,
			ObchodniJmeno = obchodniJmeno
		};
		return ekonomickeSubjektyKomplexFiltr;
	}

	private EkonomickeSubjektyResult GetEkonomickeSubjekty_ProcessResponse(EkonomickeSubjektySeznam ekonomickeSubjektySeznam)
	{
		return new EkonomickeSubjektyResult
		{
			PocetCelkem = ekonomickeSubjektySeznam.PocetCelkem,
			Items = ekonomickeSubjektySeznam.EkonomickeSubjekty
				.Select(ekonomickySubjekt => new EkonomickySubjektItem
				{
					EkonomickySubjektAres = ekonomickySubjekt,
					EkonomickySubjektExtension = GetEkonomickeSubjekty_GetExtension(ekonomickySubjekt)
				})
				.ToList()
		};
	}

	private EkonomickySubjektExtension GetEkonomickeSubjekty_GetExtension(EkonomickySubjekt ekonomickySubjekt)
	{
		EkonomickySubjektExtension aresExtension = new EkonomickySubjektExtension();
		aresExtension.PravniFormaText = _ciselnikPravniForma.GetValue(ekonomickySubjekt.PravniForma);
		aresExtension.FinancniUradText = _ciselnikFinancniUrad.GetValue(ekonomickySubjekt.FinancniUrad);
		var subjectvr = ekonomickySubjekt.DalsiUdaje.FirstOrDefault(x => x.DatovyZdroj == "vr" && x.SpisovaZnacka?.Length > 5);
		if (subjectvr != null)
		{
			string[] spisovaZnackaSplit = subjectvr.SpisovaZnacka.Split('/');
			aresExtension.RejstrikovySoudText = _ciselnikRejstrikovySoud.GetValue(spisovaZnackaSplit[spisovaZnackaSplit.Count() - 1]);
			aresExtension.SpisovaZnackaFull = subjectvr.SpisovaZnacka + " " + aresExtension.RejstrikovySoudText;
		}
		else
		{
			aresExtension.RejstrikovySoudText = "";
			aresExtension.SpisovaZnackaFull = "";
		}
		aresExtension.SidloPscText = ekonomickySubjekt.Sidlo?.Psc.ToString("### ##");
		aresExtension.SidloAddressLines = ekonomickySubjekt.Sidlo?.TextovaAdresa?.Split(',');
		aresExtension.IsDorucovaciAdresaStejna = IsAddressEqual(ekonomickySubjekt.Sidlo?.TextovaAdresa, ekonomickySubjekt.AdresaDorucovaci?.RadekAdresy1 + ekonomickySubjekt.AdresaDorucovaci?.RadekAdresy2 + ekonomickySubjekt.AdresaDorucovaci?.RadekAdresy3);
		string DorucAds = ekonomickySubjekt.AdresaDorucovaci?.RadekAdresy1 + ekonomickySubjekt.AdresaDorucovaci?.RadekAdresy2 + ekonomickySubjekt.AdresaDorucovaci?.RadekAdresy3;
		aresExtension.IsPlatceDph = ekonomickySubjekt.SeznamRegistraci.StavZdrojeDph == "AKTIVNI";
		return aresExtension;
	}


	/// <summary>
	/// IsAddressEqual - porovná 2 adresy Sidelni a Dorucovaci. Ignoruje nepodstatné znaky jako blank znaky, čárky.
	/// </summary>
	/// <param name="AdresaDorucovaci"></param>
	/// <param name="AdresaSidlo"></param>
	/// <returns>true/false.</returns>
	internal static bool IsAddressEqual(string AdresaSidlo, string AdresaDorucovaci)
	{
		var SidloAdsArray = (AdresaSidlo ?? "").ToCharArray().Where(c => !char.IsWhiteSpace(c) && c != ',').ToArray();
		var DorucAdsArray = (AdresaDorucovaci ?? "").ToCharArray().Where(c => !char.IsWhiteSpace(c) && c != ',').ToArray();
		return new string(SidloAdsArray) == new string(DorucAdsArray);
	}
}
