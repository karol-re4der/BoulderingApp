using BoulderBuddy.Data;
using BoulderBuddy.Models;
using Microsoft.AspNetCore.Identity;

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
