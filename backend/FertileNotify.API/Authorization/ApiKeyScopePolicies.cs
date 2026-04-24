namespace FertileNotify.API.Authorization
{
    public static class ApiKeyScopePolicies
    {
        public const string NotificationsSend = "Scope.NotificationsSend";
        public const string NotificationsSendOrMcpUsage = "Scope.NotificationsSendOrMcpUsage";
        public const string WorkflowTrigger = "Scope.WorkflowTrigger";
        public const string McpUsage = "Scope.McpUsage";
 
        public static bool HasApiKeyScope(ClaimsPrincipal user, string scope)
        {
            var isApiKeyCaller = user.HasClaim(c => c.Type == "ApiKeyId");
            if (!isApiKeyCaller)
                return true;

            return user.HasClaim("scope", scope) || user.HasClaim("Scope", scope);
        }

        public static bool HasAnyApiKeyScope(ClaimsPrincipal user, params string[] scopes)
            => scopes.Any(scope => HasApiKeyScope(user, scope));
    }
}
