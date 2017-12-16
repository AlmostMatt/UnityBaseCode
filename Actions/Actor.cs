using System;

// An actor has a private abilityMap and a public statusMap

public interface Actor {
	StatusMap statusMap { get; set; }
}
