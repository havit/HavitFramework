using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Havit.Web.UI.Scriptlets
{
	class ListControlExtender : IControlExtender
	{		
		private int priority;
		private Type controlType;

		public ListControlExtender(Type controlType, int priority)
		{
			this.priority = priority;
	        this.controlType = controlType;
		}
		
        public int? GetPriority(Control control)
        {
            return (this.controlType.IsAssignableFrom(control.GetType())) ? (int?)this.priority : null;
        }
        
		public void CreateParameter(string parameterPrefix, IScriptletParameter parameter, System.Web.UI.Control control, ScriptBuilder scriptBuilder)
		{
            // vytvoøíme objekt
            scriptBuilder.AppendFormat("{0}.{1} = new Array();\n", parameterPrefix, parameter.Name, control.ClientID);
                        
            ListControl listControl = control as ListControl;
            if (listControl == null)
				throw new ArgumentException("ListControlExtender podporuje pouze controly typu ListControl.");
				
			for (int i = 0; i < listControl.Items.Count; i++)
			{
				scriptBuilder.AppendFormat("{0}.{1}[{2}] = document.getElementById(\"{3}_{2}\");\n", parameterPrefix, parameter.Name, i, control.ClientID);
				if (((ControlParameter)parameter).StartOnChange)
				{
					scriptBuilder.Append(BrowserHelper.GetAttachEvent(
                        String.Format("{0}.{1}[{2}]", parameterPrefix, parameter.Name, i),
                        "onclick",
                        parameter.Scriptlet.ClientSideFunctionCall
                    ));
                    scriptBuilder.Append("\n");
				}
			}
		
		}
	}
}
