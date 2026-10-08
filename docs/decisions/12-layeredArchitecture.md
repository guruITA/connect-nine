# Decision 12: Layered architecture

Connect uses four inward-facing backend layers: Presentation, Application,
Domain, and Infrastructure. The startup project wires Presentation and
Infrastructure; Application depends on Domain; Domain depends on nothing.

See [the architecture overview](../architecture/architecture.md#layers).
