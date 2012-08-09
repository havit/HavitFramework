using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Configuration;
using System.Configuration;
using System.Web.Security;
using System.Web;
using System.Security.Principal;

namespace Havit.Web.Security
{
	/// <summary>
	/// Poskytuje statické metody pro snadnou implementaci FormAuthentication, kdy jsou do role ukládány do ticketu jako userData.
	/// </summary>
	/// <remarks>
	/// Implementováno výhradnì pro cookies-authentizaci. Nepodporuje cookieless!
	/// </remarks>
	public static class FormsRolesAuthentication
	{
		#region Timeout
		/// <summary>
		/// Timeout pro authentication-ticket (web.config: system.web/authentication/forms/timeout).
		/// </summary>
		/// <remarks>
		/// Jako jedna z mála konfiguraèních parametrù není pøístupné pøes <see cref="System.Web.Security.FormsAuthentication"/>.
		/// </remarks>
		public static int Timeout
		{
			get
			{
				if (_timeout == null)
				{
					AuthenticationSection authenticationSection = (AuthenticationSection)ConfigurationManager.GetSection("system.web/authentication");
					_timeout = (int)authenticationSection.Forms.Timeout.TotalMinutes; // pokud není konfigurováno, vrací default
				}
				return (int)_timeout;
			}
		}
		private static int? _timeout;
		#endregion

		#region ApplyAuthenticationTicket
		/// <summary>
		/// Aplikuje autentizaèní ticket, tj. vytáhne z nìj informace o pøihlášeném uživateli
		/// a jeho rolích a naplní jimi objekt User.
		/// </summary>
		/// <remarks>
		/// Vytáhne z authentication-ticketu role, vytvoøí z nìj identity, spojí to v principal a ten nastaví jako aktuálního uživatele.
		/// </remarks>
		/// <exception cref="ArgumentNullException">pokud je <c>ticket</c> null</exception>
		/// <param name="ticket">authentication-ticket</param>
		public static void ApplyAuthenticationTicket(FormsAuthenticationTicket ticket)
		{
			if (ticket == null)
			{
				throw new ArgumentNullException("ticket");
			}
			
			HttpContext context = HttpContext.Current;
			if (context == null)
			{
				throw new InvalidOperationException("HttpContext.Current not available");
			}

			string[] roles = ticket.UserData.Split(',');
			for (int i = 0; i < roles.Length; i++)
			{
				roles[i] = roles[i].Trim();
			}

			FormsIdentity identity = new FormsIdentity(ticket);
			GenericPrincipal principal = new GenericPrincipal(identity, roles);
			context.User = principal;
		}

		/// <summary>
		/// Aplikuje pøípadný autentizaèní ticket, tj. vytáhne z nìj informace o pøihlášeném uživateli
		/// a jeho rolích a naplní jimi objekt User.
		/// </summary>
		/// <remarks>
		/// Autentizaèní ticket se pokouší zjistit ve formì cookie a decryptovat. V pøípadì nalezení ho aplikuje.
		/// </remarks>
		public static void ApplyAuthenticationTicket()
		{
			HttpContext context = HttpContext.Current;
			if (context == null)
			{
				throw new InvalidOperationException("HttpContext.Current not available");
			}

			HttpCookie authenticationCookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];
			if (authenticationCookie != null)
			{
				FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authenticationCookie.Value);
				ApplyAuthenticationTicket(ticket);
			}
		}
		#endregion

		#region GetAuthTicket
		/// <summary>
		/// Vytvoøí autentizaèní ticket pro forms-authentication s ukládáním rolí do userData.
		/// </summary>
		/// <param name="username">pøihlašovací jméno uživatele</param>
		/// <param name="roles">role, které uživateli pøísluší</param>
		/// <param name="createPersistent"><c>true</c>, pokud se má být ticket persistentní; jinak <c>false</c></param>
		/// <param name="cookiePath">cookie-path pro autentizaèní ticket</param>
		/// <returns>autentizaèní ticket na základì pøedaných argumentù</returns>
		public static FormsAuthenticationTicket GetAuthTicket(string username, string[] roles, bool createPersistent, string cookiePath)
		{
			FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
				2,											// version
				username,									// name
				DateTime.Now,								// issueDate
				DateTime.Now.AddMinutes((double)Timeout),	// expiration
				createPersistent,						// isPersistent
				String.Join(",", roles),					// userData
				cookiePath);								// cookiePath
			return authTicket;
		}
		#endregion

		#region GetAuthCookie
		/// <summary>
		/// Vytvoøí authentizaèní cookie pro forms-authentication s ukládáním rolí do userData.
		/// </summary>
		/// <param name="username">pøihlašovací jméno uživatele</param>
		/// <param name="roles">role, které uživateli pøísluší</param>
		/// <param name="createPersistentCookie"><c>true</c>, pokud se má vytvoøit trvalá cookie, která pøežije session browseru; jinak <c>false</c></param>
		/// <param name="cookiePath">cookie-path pro autentizaèní ticket</param>
		/// <returns></returns>
		public static HttpCookie GetAuthCookie(string username, string[] roles, bool createPersistentCookie, string cookiePath)
		{
			HttpContext context = HttpContext.Current;
			if (context == null)
			{
				throw new InvalidOperationException("HttpContext.Current not available");
			}

			if (username == null)
			{
				username = String.Empty;
			}

			if (String.IsNullOrEmpty(cookiePath))
			{
				cookiePath = FormsAuthentication.FormsCookiePath;
			}

			FormsAuthenticationTicket authTicket = GetAuthTicket(username, roles, createPersistentCookie, cookiePath);

			string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
			if (String.IsNullOrEmpty(encryptedTicket))
			{
				throw new HttpException("Unable to encrypt cookie for authentication ticket");
			}

			HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
			authCookie.HttpOnly = true;
			authCookie.Path = cookiePath;
			authCookie.Secure = FormsAuthentication.RequireSSL;
			if (FormsAuthentication.CookieDomain != null)
			{
				authCookie.Domain = FormsAuthentication.CookieDomain;
			}
			if (authTicket.IsPersistent)
			{
				authCookie.Expires = authTicket.Expiration;
			}

			return authCookie;
		}
		#endregion

		#region RedirectFromLoginPage
		/// <summary>
		/// Redirektuje autentizovaného uživatele zpìt na pùvodní URL (nebo default URL).
		/// Souèástí response je autentizaèní cookie s pøíslušným autentizaèním ticketem.
		/// </summary>
		/// <param name="username">pøihlašovací jméno uživatele</param>
		/// <param name="roles">role, které uživateli pøísluší</param>
		/// <param name="createPersistentCookie"><c>true</c>, pokud se má vytvoøit trvalá cookie, která pøežije session browseru; jinak <c>false</c></param>
		/// <param name="cookiePath">cookie-path pro autentizaèní ticket</param>
		/// <param name="redirectUrl">URL, na které má být provedeno pøesmìrování</param>
		public static void RedirectFromLoginPage(string username, string[] roles, bool createPersistentCookie, string cookiePath, string redirectUrl)
		{
			if (username != null)
			{
				HttpContext context = HttpContext.Current;
				if (context == null)
				{
					throw new InvalidOperationException("HttpContext.Current not available");
				}

				if (String.IsNullOrEmpty(redirectUrl))
				{
					redirectUrl = FormsAuthentication.GetRedirectUrl(username, createPersistentCookie);
				}

				AddAuthCookie(username, roles, createPersistentCookie, cookiePath);
				context.Response.Redirect(redirectUrl, false);
			}
		}

		/// <summary>
		/// Redirektuje autentizovaného uživatele zpìt na pùvodní URL (nebo default URL).
		/// Souèástí response je autentizaèní cookie s pøíslušným autentizaèním ticketem.
		/// </summary>
		/// <param name="username">pøihlašovací jméno uživatele</param>
		/// <param name="roles">role, které uživateli pøísluší</param>
		/// <param name="createPersistentCookie"><c>true</c>, pokud se má vytvoøit trvalá cookie, která pøežije session browseru; jinak <c>false</c></param>
		public static void RedirectFromLoginPage(string username, string[] roles, bool createPersistentCookie)
		{
			RedirectFromLoginPage(username, roles, createPersistentCookie, null, null);
		}

		/// <summary>
		/// Redirektuje autentizovaného uživatele zpìt na pùvodní URL (nebo default URL).
		/// Souèástí response je autentizaèní cookie s pøíslušným autentizaèním ticketem, bez persistence.
		/// </summary>
		/// <param name="username">pøihlašovací jméno uživatele</param>
		/// <param name="roles">role, které uživateli pøísluší</param>
		public static void RedirectFromLoginPage(string username, string[] roles)
		{
			RedirectFromLoginPage(username, roles, false, null, null);
		}
		#endregion

		#region AddAuthCookie
		/// <summary>
		/// Pøidá do Response autentizaèní cookie s pøíslušným autentizaèním ticketem.
		/// </summary>
		/// <param name="username">pøihlašovací jméno uživatele</param>
		/// <param name="roles">role, které uživateli pøísluší</param>
		/// <param name="createPersistentCookie"><c>true</c>, pokud se má vytvoøit trvalá cookie, která pøežije session browseru; jinak <c>false</c></param>
		/// <param name="cookiePath">cookie-path pro autentizaèní ticket</param>
		/// <returns>autnetizaèní cookie, která byla vytvoøena a pøidána do Response</returns>
		public static HttpCookie AddAuthCookie(string username, string[] roles, bool createPersistentCookie, string cookiePath)
		{
			if (username != null)
			{
				HttpContext context = HttpContext.Current;
				if (context == null)
				{
					throw new InvalidOperationException("HttpContext.Current not available");
				}

				HttpCookie authCookie = GetAuthCookie(username, roles, createPersistentCookie, cookiePath);
				context.Response.Cookies.Add(authCookie);
				
				return authCookie;
			}
			return null;
		}

		/// <summary>
		/// Pøidá do Response autentizaèní cookie s pøíslušným autentizaèním ticketem. Cookie není persistentní.
		/// </summary>
		/// <param name="username">pøihlašovací jméno uživatele</param>
		/// <param name="roles">role, které uživateli pøísluší</param>
		/// <returns>autentizaèní cookie, která byla vytvoøena a pøidána do Response</returns>
		public static HttpCookie AddAuthCookie(string username, string[] roles)
		{
			return AddAuthCookie(username, roles, false, null);
		}

		/// <summary>
		/// Pøidá do Response autentizaèní cookie s pøíslušným autentizaèním ticketem.
		/// </summary>
		/// <param name="username">pøihlašovací jméno uživatele</param>
		/// <param name="roles">role, které uživateli pøísluší</param>
		/// <param name="createPersistentCookie"><c>true</c>, pokud se má vytvoøit trvalá cookie, která pøežije session browseru; jinak <c>false</c></param>
		/// <returns>autnetizaèní cookie, která byla vytvoøena a pøidána do Response</returns>
		public static HttpCookie AddAuthCookie(string username, string[] roles, bool createPersistentCookie)
		{
			return AddAuthCookie(username, roles, createPersistentCookie, null);
		}
		#endregion
	}
}
