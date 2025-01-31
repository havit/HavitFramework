using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.Scriptlets;

/// <summary>
/// Control extender, který umí pracovat s Repeaterem.
/// </summary>
public class RepeaterControlExtender : IControlExtender
{
	private readonly int priority;

	/// <summary>
	/// Vytvoří extender s danou prioritou.
	/// </summary>
	/// <param name="priority">Priorita extenderu.</param>
	public RepeaterControlExtender(int priority)
	{
		this.priority = priority;
	}

	/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetPriority")]/*' />
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
	public int? GetPriority(Control control)
	{
		return (control is Repeater) ? (int?)priority : null;
	}

	/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetInitializeClientSideValueScript")]/*' />
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
	public void GetInitializeClientSideValueScript(string parameterPrefix, IScriptletParameter parameter, Control control, ScriptBuilder scriptBuilder)
	{
		if (!(control is Repeater))
		{
			throw new HttpException("RepeaterControlExtender podporuje pouze controly typu Repeater.");
		}

		scriptBuilder.AppendFormat("{0}.{1} = new Array();\n", parameterPrefix, parameter.Name);
		Repeater repeater = (Repeater)control;

		int index = 0;
		foreach (RepeaterItem item in repeater.Items)
		{
			if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
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
					nestedParameter.GetInitializeClientSideValueScript(newParameterPrefix, item, scriptBuilder);
				}
				index++;
			}
		}
	}

	/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetAttachEventsScript")]/*' />
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
	public void GetAttachEventsScript(string parameterPrefix, IScriptletParameter parameter, Control control, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder)
	{
		GetEventsScript(parameterPrefix, parameter, control, scriptletFunctionCallDelegate, scriptBuilder,
			delegate (IScriptletParameter nestedParameter, string nestedParameterPrefix, Control nestedParentControl, string nestedScriptletFunctionCallDelegate, ScriptBuilder nestedScriptBuilder)
			{
				nestedParameter.GetAttachEventsScript(nestedParameterPrefix, nestedParentControl, nestedScriptletFunctionCallDelegate, nestedScriptBuilder);
			});
	}

	/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetDetachEventsScript")]/*' />
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
	public void GetDetachEventsScript(string parameterPrefix, IScriptletParameter parameter, Control control, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder)
	{
		GetEventsScript(parameterPrefix, parameter, control, scriptletFunctionCallDelegate, scriptBuilder,
			delegate (IScriptletParameter nestedParameter, string nestedParameterPrefix, Control nestedParentControl, string nestedScriptletFunctionCallDelegate, ScriptBuilder nestedScriptBuilder)
		{
			nestedParameter.GetDetachEventsScript(nestedParameterPrefix, nestedParentControl, nestedScriptletFunctionCallDelegate, nestedScriptBuilder);
		});
	}

	private void GetEventsScript(string parameterPrefix, IScriptletParameter parameter, Control control, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder, JobOnNesterParameterEventHandler jobOnNestedParameter)
	{
		Repeater repeater = (Repeater)control;

		int index = 0;
		foreach (RepeaterItem item in repeater.Items)
		{
			if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
			{
				foreach (Control nestedControl in ((Control)parameter).Controls)
				{
					IScriptletParameter nestedParameter = nestedControl as IScriptletParameter;
					if (nestedParameter == null)
					{
						continue;
					}
					string newParameterPrefix = String.Format("{0}.{1}[{2}]", parameterPrefix, parameter.Name, index);
					jobOnNestedParameter(nestedParameter, newParameterPrefix, item, scriptletFunctionCallDelegate, scriptBuilder);
				}
			}
			index++;
		}
	}

	private delegate void JobOnNesterParameterEventHandler(IScriptletParameter nestedParameter, string nestedParameterPrefix, Control nestedParentControl, string nestedScriptletFunctionCallDelegate, ScriptBuilder nestedScriptBuilder);
}