using System;

namespace Stateless
{
	public struct Change {
		public enum ChangeType {
			Delete,
			Insert,
			Update,
		}

		public ChangeType ChangeSetType { get;set;}
		public int Index {get; set;}
	}
}

