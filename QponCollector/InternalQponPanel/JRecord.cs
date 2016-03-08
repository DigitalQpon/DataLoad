using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalQponPanel
{
	/// <summary>
	/// A  root class to allow the system to keep track of items selected or rejected by the user
	/// this selection is used to update the "Azure" tables
	/// </summary>
	public class JRecord
	{
		public bool IsSelected;
		public bool IsModified;
	}
}
