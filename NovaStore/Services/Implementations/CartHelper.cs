namespace NovaStore.Services.Implementations
{
    public static class CartHelper
    {
        public const string SessionKey = "CartSessionId";

        public static string GetOrCreateSessionId(HttpContext context)
        {
            var sessionId = context.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                context.Session.SetString(SessionKey, sessionId);
            }
            return sessionId;
        }
    }
}