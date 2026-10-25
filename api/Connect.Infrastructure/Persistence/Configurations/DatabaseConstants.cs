namespace Connect.Infrastructure.Persistence.Configurations;

public static class DatabaseConstants
{
    // Account constants
    public const string AccountCheckConstraintPasswordHashNotEmpty = "ck_asp_net_users_password_hash_not_empty";
    public const string AccountEmailUniqueIndex = "ux_account_email";
    public const string AccountUserNameUniqueIndex = "ux_account_username";

    //  Session constants
    public const string SessionCheckConstraintTokenHashSha256 = "ck_session_token_hash_sha256";
    public const string SessionTokenUniqueIndex = "ux_session_token_hash";
    public const string SessionAccountIdIndex = "ix_session_account_id";
}
