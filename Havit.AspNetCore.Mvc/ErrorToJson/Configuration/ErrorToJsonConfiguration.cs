using System;
using System.Collections.Generic;
using System.Linq;

namespace Havit.AspNetCore.Mvc.ErrorToJson.Configuration;

/// <summary>
/// Vnitřní konfigurace mapování výjimek na JSON odpovědi.
/// </summary>
    public class ErrorToJsonConfiguration
    {
        private readonly IList<ErrorToJsonMappingItem> mappings;

	/// <summary>
	/// Konstruktor.
	/// </summary>
        public ErrorToJsonConfiguration(IList<ErrorToJsonMappingItem> mappings)
        {
            this.mappings = mappings;
        }

	/// <summary>
	/// Vyhledá (první) mapování pro danou výjimku.
	/// Pokud není nalezeno, vrací null.
	/// </summary>
        public ErrorToJsonMappingItem FindMapping(Exception exception)
        {
            return mappings.FirstOrDefault(item => item.ExceptionPredicate(exception));
        }
    }