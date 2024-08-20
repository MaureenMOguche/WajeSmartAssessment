using WajeSmartAssessment.Domain.Common;

namespace WajeSmartAssessment.Domain
{
    public class Like : AuditableEntity
    {
        protected Like() { }
        private Like(string postId, string userId)
        {
            PostId = Guid.Parse(postId);
            UserId = userId;
        }
        public Guid Id { get; private set; }
        public Guid PostId { get; private set; }
        public string UserId { get; private set; } = string.Empty;

        public static Like LikePost(string postId, string userId) =>
            new Like(postId, userId);
    }
}
