using System;

/** 
 * An actor is something that can perform actions.
 * 
 * it usually has a private abilityMap and a public statusMap.
 */
public interface Actor {
	StatusMap statusMap { get; set; }

	int playerNumber { get; }
}
