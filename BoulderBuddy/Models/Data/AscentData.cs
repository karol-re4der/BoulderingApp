﻿namespace BoulderBuddy.Models.DB
{
    public class AscentData(Users user, Ascents ascent)
    {
        public Ascents Ascent { get; set; } = ascent;
        public Users User { get; set; } = user;
    }
}
