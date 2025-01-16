using BoulderBuddy.DataAccess.Data;
using BoulderBuddy.Models.Models;

namespace BoulderBuddy.Utility
{
    public static class UserUtility
    {
        public static UserData GetUserByNetId(ApplicationDbContext db, string aspNetId)
        {
            return db.UserData.Where(x => x.AspNetUserId.Equals(aspNetId)).First();
        }
    }
}
