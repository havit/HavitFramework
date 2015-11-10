using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Havit.Web.UI.Scriptlets
{
	/// <summary>
	/// Control extender, který umí pracovat s Repeaterem.
	/// </summary>
    public class GridViewControlExtender : IControlExtender
    {
		#region Private fields
		private readonly int priority;		
		#endregion

		#region Constructors
		/// <summary>
		/// Vytvoří extender s danou prioritou.
		/// </summary>
		/// <param name="priority">Priorita extenderu.</param>
		public GridViewControlExtender(int priority)
		{
			this.priority = priority;
		}
		#endregion		
		
		#region GetPriority
		/// <summary>
		/// Vrátí prioritu extenderu pro daný control.
		/// Pokud je control Repeaterem, vrátí prioritu zadanou v konstruktoru,
		/// jinak vrací null.
		/// </summary>
		/// <param name="control">Control, pro který se ověřuje priorita.</param>
		/// <returns>Priorita.</returns>
		public int? GetPriority(Control control)
		{
			return (control is GridView) ? (int?)priority : null;
		}		
		#endregion

		#region GetInitializeClientSideValueScript
		/// <summary>
		/// Vytvoří klientský parametr pro předaný control.
		/// </summary>
		/// <param name="parameterPrefix">Název objektu na klientské straně.</param>
		/// <param name="parameter">Parametr předávající řízení extenderu.</param>
		/// <param name="control">Control ke zpracování.</param>
		/// <param name="scriptBuilder">Script builder.</param>
		public void GetInitializeClientSideValueScript(string parameterPrefix, IScriptletParameter parameter, Control control, ScriptBuilder scriptBuilder)
        {
			if (!(control is GridView))
		    {
				throw new HttpException("GridViewControlExtender podporuje pouze controly typu GridView.");	    
			}
			
            scriptBuilder.AppendFormat("{0}.{1} = new Array();\n", parameterPrefix, parameter.Name);
			GridView gridView = (GridView)control;

            int index = 0;
			foreach (GridViewRow row in gridView.Rows)
            {
				if (row.RowType == DataControlRowType.DataRow)
                {
					string newParameterPrefix = String.Format("{0}.{1}[{2}]", parameterPrefix, parameter.Name, index);
					scriptBuilder.AppendFormat("{0} = new Object();\n", newParameterPrefix);
					
					foreach (Control nestedControl in ((Control)parameter).Controls)
                    {
                        ParameterBase nestedParameter = nestedControl as ParameterBase;
						if (nestedParameter == null)
						{
							continue;
						}
						nestedParameter.GetInitializeClientSideValueScript(newParameterPrefix, row, scriptBuilder);
                    }
                    index++;
                }
            }
        }
		#endregion

		#region GetAttachEventsScript
		/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetAttachEventsScript")]/*' />
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
		public void GetAttachEventsScript(string parameterPrefix, IScriptletParameter parameter, Control control, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder)
		{
			GetEventsScript(parameterPrefix, parameter, control, scriptletFunctionCallDelegate, scriptBuilder,
				delegate(IScriptletParameter nestedParameter, string nestedParameterPrefix, Control nestedParentControl, string nestedScriptletFunctionCallDelegate, ScriptBuilder nestedScriptBuilder)
				{
					nestedParameter.GetAttachEventsScript(nestedParameterPrefix, nestedParentControl, nestedScriptletFunctionCallDelegate, nestedScriptBuilder);
				});
		}

		#endregion

		#region GetDetachEventsScript
		/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetDetachEventsScript")]/*' />
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
		public void GetDetachEventsScript(string parameterPrefix, IScriptletParameter parameter, Control control, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder)
		{
			GetEventsScript(parameterPrefix, parameter, control, scriptletFunctionCallDelegate, scriptBuilder,
				delegate(IScriptletParameter nestedParameter, string nestedParameterPrefix, Control nestedParentControl, string nestedScriptletFunctionCallDelegate, ScriptBuilder nestedScriptBuilder)
			{
				nestedParameter.GetDetachEventsScript(nestedParameterPrefix, nestedParentControl, nestedScriptletFunctionCallDelegate, nestedScriptBuilder);
			});
		}
		#endregion

		#region GetEventsScript
		private void GetEventsScript(string parameterPrefix, IScriptletParameter parameter, Control control, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder, JobOnNestedParameterEventHandler jobOnNestedParameter)
		{
			GridView gridView = (GridView)control;

			int index = 0;
			foreach (GridViewRow row in gridView.Rows)
            {
				if (row.RowType == DataControlRowType.DataRow)
                {
					foreach (Control nestedControl in ((Control)parameter).Controls)
                    {
                        IScriptletParameter nestedParameter = nestedControl as IScriptletParameter;
						if (nestedParameter == null)
						{
							continue;
						}
						string newParameterPrefix = String.Format("{0}.{1}[{2}]", parameterPrefix, parameter.Name, index);
						jobOnNestedParameter(nestedParameter, newParameterPrefix, row, scriptletFunctionCallDelegate, scriptBuilder);
					}
				}
				index++;
			}
		}

		private delegate void JobOnNestedParameterEventHandler(IScriptletParameter nestedParameter, string nestedParameterPrefix, Control nestedParentControl, string scriptletFunctionCallDelegate, ScriptBuilder nestedScriptBuilder);
		#endregion		
	}
}