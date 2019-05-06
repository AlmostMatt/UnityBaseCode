using System;

namespace UnityBaseCode
{
	// TODO: consider moving Actor out of Statuses
	namespace Statuses
	{
		/** 
		 * An actor has statuses and can perform actions.
		 * 
		 * it must have a public statusMap and can optionally have a private actionMap.
		 */
		public interface Actor {
			StatusMap statusMap { get; set; }

			int playerNumber { get; }
		}
	}
}
