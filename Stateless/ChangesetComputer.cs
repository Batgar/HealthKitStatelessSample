using System;
using System.Collections.Generic;

namespace Stateless
{
	public class ChangesetComputer {
		public static List<Change> ComputeListDifferences(List<IIdentifiable> lhs, List<IIdentifiable> rhs) {
			List<Change> changes = new List<Change> ();

			int i = 0;
			foreach (var entry in rhs) {
				if (!lhs.Contains (entry)) {
					changes.Add (new Change (){ ChangeSetType = Change.ChangeType.Insert, Index = i }); 
				}
				i++;
			}

			i = 0;
			foreach (var entry in lhs) {
				if (!rhs.Contains (entry)) {
					changes.Add (new Change () { ChangeSetType = Change.ChangeType.Delete, Index = i });
				}
				i++;
			}

			//There is no way to update a blood glucose entry, so we don't do any Update stuff.

			return changes;
		}
	}
}

