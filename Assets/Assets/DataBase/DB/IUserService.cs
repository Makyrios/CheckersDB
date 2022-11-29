using GameClasses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DB
{
    public interface IUserService
    {
        public void CreateUser(BaseGameAccount user);
        public List<BaseGameAccount> SelectAllUsers();
        public List<BaseGameAccount> SelectBonusAccounts();
        public List<BaseGameAccount> SelectStreakAccounts();
        public List<int> GetIds();
    }
}
