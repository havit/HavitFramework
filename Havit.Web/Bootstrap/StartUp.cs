using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using Havit.Web.Bootstrap;
using Havit.Web.Bootstrap.UI.ClientScripts;

[assembly: PreApplicationStartMethod(typeof(BootstrapClientScriptHelper), BootstrapClientScriptHelper.RegisterBootstrapScriptResourceMappingMethodName)]
