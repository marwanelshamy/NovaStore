namespace NovaStore.Services.Implementations
{
    public static class CartHelper
    {
        public const string SessionKey = "CartSessionId";
        public const string CouponKey = "AppliedCouponCode";

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

        public static void SetCoupon(HttpContext context, string? code)
        {
            if (string.IsNullOrEmpty(code))
                context.Session.Remove(CouponKey);
            else
                context.Session.SetString(CouponKey, code);
        }

        public static string? GetCoupon(HttpContext context)
        {
            return context.Session.GetString(CouponKey);
        }
    }
}