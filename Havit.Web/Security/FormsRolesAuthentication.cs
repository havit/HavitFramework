using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Configuration;
using System.Configuration;
using System.Web.Security;
using System.Web;
using System.Security.Principal;
using System.Diagnostics.Contracts;

namespace Havit.Web.Security
{
	/// <summary>
	/// Poskytuje statické metody pro snadnou implementaci FormAuthentication, kdy jsou do role ukládány do ticketu jako userData.
	/// </summary>
	/// <remarks>
	/// Implementováno výhradně pro cookies-authentizaci. Nepodporuje cookieless!
	/// </remarks>
	public static class FormsRolesAuthentication
	{
		#region Timeout
		/// <summary>
		/// Timeout pro authentication-ticket (web.config: system.web/authentication/forms/timeout).
		/// </summary>
		/// <remarks>
		/// Jako jedna z mála konfiguračních parametrů není přístupné přes <see cref="System.Web.Security.FormsAuthentication"/>.
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
		/// Aplikuje autentizační ticket, tj. vytáhne z něj informace o přihlášeném uživateli
		/// a jeho rolích a naplní jimi objekt User.
		/// </summary>
		/// <remarks>
		/// Vytáhne z authentication-ticketu role, vytvoří z něj identity, spojí to v principal a ten nastaví jako aktuálního uživatele.
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

			string[] roles = null;
			if (!String.IsNullOrEmpty(ticket.UserData))
			{
				roles = ticket.UserData.Split(',');
				for (int i = 0; i < roles.Length; i++)
				{
					roles[i] = roles[i].Trim();
				}
			}

			FormsIdentity identity = new FormsIdentity(ticket);
			GenericPrincipal principal = new GenericPrincipal(identity, roles);
			context.User = principal;
		}

		/// <summary>
		/// Aplikuje případný autentizační ticket, tj. vytáhne z něj informace o přihlášeném uživateli
		/// a jeho rolích a naplní jimi objekt User.
		/// </summary>
		/// <remarks>
		/// Autentizační ticket se pokouší zjistit ve formě cookie a decryptovat. V případě nalezení ho aplikuje.
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
		/// Vytvoří autentizační ticket pro forms-authentication s ukládáním rolí do userData.
		/// </summary>
		/// <param name="username">Přihlašovací jméno uživatele.</param>
		/// <param name="roles">Role, které uživateli přísluší.</param>
		/// <param name="createPersistent"><c>True</c>, pokud se má být ticket persistentní; jinak <c>false</c>.</param>
		/// <param name="cookiePath">Cookie-path pro autentizační ticket.</param>
		/// <param name="timeout">Doba platnosti autentizačního tiketu v minutách. Pro perzistentní cookie musí být hodnota null, pro neperzistentní je hodnota povinná.</param>
		/// <returns>Autentizační ticket na základě předaných argumentů.</returns>
		public static FormsAuthenticationTicket GetAuthTicket(string username, string[] roles, bool createPersistent, string cookiePath, int? timeout)
		{
			Contract.Requires((createPersistent && (timeout == null)) || (!createPersistent && (timeout != null)), "Pro perzistentní cookie nelze zadat timeout, pro neperzistentní je timeout povinný.");
			
			if (username == null)
			{
				username = String.Empty;
			}
			
			string userData = string.Empty;
			if (roles != null)
			{
				userData = String.Join(",", roles);
			}

			if (String.IsNullOrEmpty(cookiePath))
			{
				cookiePath = FormsAuthentication.FormsCookiePath;
			}
	
			FormsAuthenticationTicket authTicket;

			// .NET FW 2.0 obsahuje bug, kdy do persistentního ticketu nastavuje platnost jako nepersistentní
			if (createPersistent)
			{
				authTicket = new FormsAuthenticationTicket(
					2,											// version
					username,									// name
					DateTime.Now,								// issueDate
					DateTime.Now.AddYears(50),					// expiration
					true,										// isPersistent
					userData,									// userData
					cookiePath);								// cookiePath
			}
			else
			{
				authTicket = new FormsAuthenticationTicket(
					2,												// version
					username,										// name
					DateTime.Now,									// issueDate
					DateTime.Now.AddMinutes((double)timeout.Value),	// expiration
					false,											// isPersistent
					userData,										// userData
					cookiePath);									// cookiePath
			}
			return authTicket;
		}

		/// <summary>
		/// Vytvoří autentizační ticket pro forms-authentication s ukládáním rolí do userData.
		/// </summary>
		/// <param name="username">Přihlašovací jméno uživatele.</param>
		/// <param name="roles">Role, které uživateli přísluší.</param>
		/// <param name="createPersistent"><c>True</c>, pokud se má být ticket persistentní; jinak <c>false</c>.</param>
		/// <param name="cookiePath">Cookie-path pro autentizační ticket.</param>
		/// <returns>Autentizační ticket na základě předaných argumentů.</returns>
		public static FormsAuthenticationTicket GetAuthTicket(string username, string[] roles, bool createPersistent, string cookiePath)
		{
			return GetAuthTicket(username, roles, createPersistent, cookiePath, createPersistent ? (int?)null : Timeout);
		}
		#endregion

		#region GetAuthCookie
		/// <summary>
		/// Vytvoří authentizační cookie pro forms-authentication s ukládáním rolí do userData.
		/// </summary>
		/// <param name="username">přihlašovací jméno uživatele</param>
		/// <param name="roles">role, které uživateli přísluší</param>
		/// <param name="createPersistentCookie"><c>true</c>, pokud se má vytvořit trvalá cookie, která přežije session browseru; jinak <c>false</c></param>
		/// <param name="cookiePath">cookie-path pro autentizační ticket</param>
		/// <returns></returns>
		public static HttpCookie GetAuthCookie(string username, string[] roles, bool createPersistentCookie, string cookiePath)
		{
			HttpContext context = HttpContext.Current;
			if (context == null)
			{
				throw new InvalidOperationException("HttpContext.Current not available");
			}

			FormsAuthenticationTicket authTicket = GetAuthTicket(username, roles, createPersistentCookie, cookiePath);

			string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
			if (String.IsNullOrEmpty(encryptedTicket))
			{
				throw new HttpException("Unable to encrypt cookie for authentication ticket");
			}

			if (String.IsNullOrEmpty(cookiePath))
			{
				cookiePath = FormsAuthentication.FormsCookiePath;
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
		/// Redirektuje autentizovaného uživatele zpět na původní URL (nebo default URL).
		/// Součástí response je autentizační cookie s příslušným autentizačním ticketem.
		/// </summary>
		/// <param name="username">přihlašovací jméno uživatele</param>
		/// <param name="roles">role, které uživateli přísluší</param>
		/// <param name="createPersistentCookie"><c>true</c>, pokud se má vytvořit trvalá cookie, která přežije session browseru; jinak <c>false</c></param>
		/// <param name="cookiePath">cookie-path pro autentizační ticket</param>
		/// <param name="redirectUrl">URL, na které má být provedeno přesměrování</param>
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
		/// Redirektuje autentizovaného uživatele zpět na původní URL (nebo default URL).
		/// Součástí response je autentizační cookie s příslušným autentizačním ticketem.
		/// </summary>
		/// <param name="username">přihlašovací jméno uživatele</param>
		/// <param name="roles">role, které uživateli přísluší</param>
		/// <param name="createPersistentCookie"><c>true</c>, pokud se má vytvořit trvalá cookie, která přežije session browseru; jinak <c>false</c></param>
		public static void RedirectFromLoginPage(string username, string[] roles, bool createPersistentCookie)
		{
			RedirectFromLoginPage(username, roles, createPersistentCookie, null, null);
		}

		/// <summary>
		/// Redirektuje autentizovaného uživatele zpět na původní URL (nebo default URL).
		/// Součástí response je autentizační cookie s příslušným autentizačním ticketem, bez persistence.
		/// </summary>
		/// <param name="username">přihlašovací jméno uživatele</param>
		/// <param name="roles">role, které uživateli přísluší</param>
		public static void RedirectFromLoginPage(string username, string[] roles)
		{
			RedirectFromLoginPage(username, roles, false, null, null);
		}
		#endregion

		#region AddAuthCookie
		/// <summary>
		/// Přidá do Response autentizační cookie s příslušným autentizačním ticketem.
		/// </summary>
		/// <param name="username">přihlašovací jméno uživatele</param>
		/// <param name="roles">role, které uživateli přísluší</param>
		/// <param name="createPersistentCookie"><c>true</c>, pokud se má vytvořit trvalá cookie, která přežije session browseru; jinak <c>false</c></param>
		/// <param name="cookiePath">cookie-path pro autentizační ticket</param>
		/// <returns>autnetizační cookie, která byla vytvořena a přidána do Response</returns>
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
		/// Přidá do Response autentizační cookie s příslušným autentizačním ticketem. Cookie není persistentní.
		/// </summary>
		/// <param name="username">přihlašovací jméno uživatele</param>
		/// <param name="roles">role, které uživateli přísluší</param>
		/// <returns>autentizační cookie, která byla vytvořena a přidána do Response</returns>
		public static HttpCookie AddAuthCookie(string username, string[] roles)
		{
			return AddAuthCookie(username, roles, false, null);
		}

		/// <summary>
		/// Přidá do Response autentizační cookie s příslušným autentizačním ticketem.
		/// </summary>
		/// <param name="username">přihlašovací jméno uživatele</param>
		/// <param name="roles">role, které uživateli přísluší</param>
		/// <param name="createPersistentCookie"><c>true</c>, pokud se má vytvořit trvalá cookie, která přežije session browseru; jinak <c>false</c></param>
		/// <returns>autnetizační cookie, která byla vytvořena a přidána do Response</returns>
		public static HttpCookie AddAuthCookie(string username, string[] roles, bool createPersistentCookie)
		{
			return AddAuthCookie(username, roles, createPersistentCookie, null);
		}
		#endregion
	}
}
