namespace Connect.Application.Profiles.AddProfileTag;

// The name is already normalised with TagRules.Normalize and checked by the validator.
public sealed record AddProfileTagCommand(Guid AccountId, string Name);
