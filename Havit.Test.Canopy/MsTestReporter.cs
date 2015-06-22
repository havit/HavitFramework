using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using canopy;
using Microsoft.FSharp.Collections;

namespace Havit.Test.Canopy
{
    public class MsTestReporter : reporters.IReporter
    {
	    public void testStart(string id)
	    {            
		    writeOut("Test " + id);
	    }

	    public void pass()
	    {
			// NOOP - test prošel, nic jiného nedělám
	    }

	    public void fail(Exception ex, string id1, byte[] s1, string s2)
	    {
		    writeOut("Error: ");
		    writeOut(ex.Message);
		    writeOut("Stack: ");

		    string[] traceLines = ex.StackTrace.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

		    foreach (string trace in traceLines)
		    {
			    writeOut(trace);
		    }
			
		    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail(ex.Message);
	    }

	    public void todo()
	    {
			// NOOP
	    }

	    public void skip()
	    {
			// NOOP
	    }

	    public void testEnd(string obj0)
	    {
			// NOOP
	    }

	    public void describe(string d)
	    {
		    writeOut(d);
	    }

	    public void contextStart(string context)
	    {
		    writeOut(String.Format("context: {0}", context));
	    }

	    public void contextEnd(string obj0)
	    {
			// NOOP
		}

	    public void summary(int minutes, int seconds, int passed, int failed)
	    {
		    writeOut("");
		    writeOut(System.String.Format("{0} minutes {1} seconds to execute", minutes, seconds));
            if (failed == 0)
            {
	            Console.ForegroundColor = ConsoleColor.Green;
            }
		    writeOut(System.String.Format("{0} passed", passed));
		    writeOut(System.String.Format("{0} failed", failed));
	    }

	    public void write(string message)
	    {
		    writeOut(message);
	    }

	    public void suggestSelectors(string selector, FSharpList<string> suggestions)
	    {
		    writeOut(System.String.Format("Couldn't find any elements with selector '{0}', did you mean:", selector));
			foreach (string suggestion in suggestions)
		    {
			    writeOut("\t" + suggestion);
		    }
	    }

	    public void quit()
	    {
			// NOOP
		}

	    public void suiteBegin()
	    {
			// NOOP
	    }

	    public void suiteEnd()
	    {
			// NOOP
	    }

	    public void coverage(string obj0, byte[] obj1)
	    {
			// NOOP
		}

	    private void writeOut(string message)
	    {
		    System.Diagnostics.Debug.WriteLine(message);
	    }
    }
}
