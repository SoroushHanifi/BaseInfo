using Domain.Entities.Daroo;

namespace Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for Bizagi entities
    /// </summary>
    public static class BizagiEntityExtensions
    {
        /// <summary>
        /// Sets BaCreatedTime to current Unix timestamp in milliseconds (as expected by Bizagi)
        /// </summary>
        public static void SetBaCreatedTimeToNow(this Department entity)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            entity.BaCreatedTime = (long)(DateTime.UtcNow - epoch).TotalMilliseconds;
        }

        /// <summary>
        /// Sets BaCreatedTime to current Unix timestamp in milliseconds (as expected by Bizagi)
        /// </summary>
        public static void SetBaCreatedTimeToNow(this Scope entity)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            entity.BaCreatedTime = (long)(DateTime.UtcNow - epoch).TotalMilliseconds;
        }

        /// <summary>
        /// Sets BaCreatedTime to current Unix timestamp in milliseconds (as expected by Bizagi)
        /// </summary>
        public static void SetBaCreatedTimeToNow(this MainTitle entity)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            entity.BaCreatedTime = (long)(DateTime.UtcNow - epoch).TotalMilliseconds;
        }

        /// <summary>
        /// Sets BaCreatedTime to current Unix timestamp in milliseconds (as expected by Bizagi)
        /// </summary>
        public static void SetBaCreatedTimeToNow(this ProductType entity)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            entity.BaCreatedTime = (long)(DateTime.UtcNow - epoch).TotalMilliseconds;
        }

        /// <summary>
        /// Converts BaCreatedTime (Unix timestamp in milliseconds) to DateTime
        /// </summary>
        public static DateTime? BaCreatedTimeToDateTime(this long baCreatedTime)
        {
            if (baCreatedTime <= 0) return null;

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(baCreatedTime);
        }

        /// <summary>
        /// Prepares entity for creation by setting required Bizagi fields
        /// </summary>
        public static void PrepareForCreation(this Department entity, int? userId = null)
        {
            entity.SetBaCreatedTimeToNow();
            entity.BaGuid = Guid.NewGuid();
            entity.FinalEnt = 10008;
            entity.CreateDate = DateTime.Now;
            entity.ModifyDate = DateTime.Now;
            entity.IsDeleted = false;

            if (userId.HasValue)
                entity.CreateUserID = userId.Value;
        }

        /// <summary>
        /// Prepares entity for creation by setting required Bizagi fields
        /// </summary>
        public static void PrepareForCreation(this Scope entity, long? userId = null)
        {
            entity.SetBaCreatedTimeToNow();
            entity.BaGuid = Guid.NewGuid();
            entity.FinalEnt = 10009;
            entity.CreateDate = DateTime.Now;
            entity.ModifyDate = DateTime.Now;
            entity.IsDeleted = false;

            if (userId.HasValue)
                entity.CreateUserID = userId.Value;
        }

        /// <summary>
        /// Prepares entity for creation by setting required Bizagi fields
        /// </summary>
        public static void PrepareForCreation(this MainTitle entity, long? userId = null)
        {
            entity.SetBaCreatedTimeToNow();
            entity.BaGuid = Guid.NewGuid();
            entity.FinalEnt = 10012;
            entity.CreateDate = DateTime.Now;
            entity.ModifyDate = DateTime.Now;
            entity.IsDeleted = false;

            if (userId.HasValue)
                entity.CreateUserID = userId.Value;
        }

        /// <summary>
        /// Prepares entity for creation by setting required Bizagi fields
        /// </summary>
        public static void PrepareForCreation(this ProductType entity, long? userId = null)
        {
            entity.SetBaCreatedTimeToNow();
            entity.BaGuid = Guid.NewGuid();
            entity.FinalEnt = 10011;
            entity.CreateDate = DateTime.Now;
            entity.ModifyDate = DateTime.Now;
            entity.IsDeleted = false;
        }

        /// <summary>
        /// Prepares entity for update by setting ModifyDate
        /// </summary>
        public static void PrepareForUpdate(this Department entity)
        {
            entity.ModifyDate = DateTime.Now;
        }

        /// <summary>
        /// Prepares entity for update by setting ModifyDate
        /// </summary>
        public static void PrepareForUpdate(this Scope entity)
        {
            entity.ModifyDate = DateTime.Now;
        }

        /// <summary>
        /// Prepares entity for update by setting ModifyDate
        /// </summary>
        public static void PrepareForUpdate(this MainTitle entity)
        {
            entity.ModifyDate = DateTime.Now;
        }

        /// <summary>
        /// Prepares entity for update by setting ModifyDate
        /// </summary>
        public static void PrepareForUpdate(this ProductType entity)
        {
            entity.ModifyDate = DateTime.Now;
        }
    }
}