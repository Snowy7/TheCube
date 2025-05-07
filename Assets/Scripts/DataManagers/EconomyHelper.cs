using System;
using UnityEngine;

namespace Firebase.Game
{
    public static class EconomyHelper
    {
        public static bool HasEnoughBalance(int hours)
        {
            var ttl = UserController.Instance.UserData.TimeToLive;
            var date = DateTime.Parse(ttl);
            var diff = date - DateTime.Now;
            Debug.Log($"Comparing {diff.TotalHours} >= {hours}");
            return diff.TotalHours >= hours;
        }
        
        public static DateTime SubtractBalance(int hours)
        {
            var ttl = UserController.Instance.UserData.TimeToLive;
            var date = DateTime.Parse(ttl);
            var newDate = date.AddHours(-hours);
            return newDate;
        }
        
        public static DateTime AddBalance(int hours)
        {
            var ttl = UserController.Instance.UserData.TimeToLive;
            var date = DateTime.Parse(ttl);
            var newDate = date.AddHours(hours);
            return newDate;
        }
    }
}