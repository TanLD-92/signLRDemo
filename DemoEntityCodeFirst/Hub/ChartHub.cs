using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using DemoEntityCodeFirst.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Threading;
using Microsoft.AspNet.SignalR.Hubs;
using System.Web.Security;
using System.Collections.Concurrent;

namespace SignalRChat
{
    [HubName("chatHub")]
    public class ChatHub : Hub
    {

        ManagerDbContext manager = new ManagerDbContext();
        public static bool _isLoadInfo = false;
        public static bool _timerLoad = false;
        Thread _loadDateTime;
        Task _taskTime;
        List<User> _listUser;
        private static List<Account> _listcurrentAccounts
            = new List<Account>();

        //public void LoadInfo(List<User> listUser)
        //{
        //    Clients.All.loadInfo(listUser);
        //}
        public async Task LoadInfoDefault()
        {
            //Call the updateInfo method to update clients.
            try
            {
                _listUser = GetListUser();
                await Clients.All.loadInfo(_listUser);
            }
            catch (Exception e)
            {
                await Clients.All.loadInfo(e.Message);
            }
        }
        public async Task upDateInfo(int ID, int Experence)
        {
            if (Experence >= 0)
            {
                User user = (from user1 in manager.Users
                             where user1.ID == ID
                             select user1).First();
                user.Experencde = Experence;
                manager.SaveChanges();
                _listUser = GetListUser();
                await Clients.All.loadInfo(_listUser);
            }
        }
        public List<User> GetListUser()
        {
            using (var managerUser = new ManagerDbContext())
            {
                List<User> listUser = (from user in manager.Users select user).ToList();
                return listUser;

            }
        }
        public async Task Connect(string userName)
        {
            if(!userName.Equals(""))
            {
                using (ManagerDbContext managerUser = new ManagerDbContext())
                {
                    User userCurrentCheck = await (from user in managerUser.Users where user.Name == userName select user).FirstOrDefaultAsync();
                    if (userCurrentCheck == null)
                    {
                        User userNew = new User();
                        userNew.Name = userName;
                        managerUser.Users.Add(userNew);
                        managerUser.SaveChanges();
                    }
                    User userCurrent = await (from user in managerUser.Users where user.Name == userName select user).FirstOrDefaultAsync();
                    string IdConnected = Context.ConnectionId;
                    var accountExist = (from account1 in _listcurrentAccounts where account1.ConnectId == IdConnected && account1.UserId == userCurrent.ID select account1).FirstOrDefault();
                    if (accountExist == null)
                    {
                        Account accoutNew = new Account();
                        accoutNew.UserId = userCurrent.ID;
                        accoutNew.ConnectId = IdConnected;
                        _listcurrentAccounts.Add(accoutNew);
                    }
                    List<Account> listUserOfAccount = (from userCurrent1 in _listcurrentAccounts where userCurrent1.UserId == userCurrent.ID select userCurrent1).ToList();
                    List<int> idGroupUser = (from user1 in managerUser.UserGroups where user1.UserId == userCurrent.ID select user1.GroupId).ToList();
                    List<Group> listGroup = new List<Group>();
                    foreach (var id in idGroupUser)
                    {
                        var item2 = (from groupitem in managerUser.Groups where groupitem.ID == id select groupitem).FirstOrDefault();
                        listGroup.Add(item2);
                    }
                    foreach (var item in listUserOfAccount)
                    {
                        await Clients.Client(item.ConnectId).showUserName(userName, listGroup);
                    }
                }
            }
        }
        public async Task CreateGrouponnect(string nameGroup, string userName)
        {
            if(!nameGroup.Equals("") && !userName.Equals(""))
            {
                using (ManagerDbContext managerUser = new ManagerDbContext())
                {
                    User userCurrent = await (from user in managerUser.Users where user.Name == userName select user).FirstOrDefaultAsync();
                    if (userCurrent != null)
                    {
                        var groupExist = (from groupExist1 in managerUser.Groups where groupExist1.NameGroup == nameGroup select groupExist1).FirstOrDefault();
                        if (groupExist == null)
                        {
                            Group group = new Group();
                            group.NameGroup = nameGroup;
                            managerUser.Groups.Add(group);
                            managerUser.SaveChanges();
                            UserGroup userGroup = new UserGroup();
                            userGroup.GroupId = (from group1 in managerUser.Groups where group1.NameGroup == nameGroup select group1.ID).FirstOrDefault();
                            userGroup.UserId = userCurrent.ID;
                            managerUser.UserGroups.Add(userGroup);
                            managerUser.SaveChanges();
                        }
                        List<Account> listUserOfAccount = (from userCurrent1 in _listcurrentAccounts where userCurrent1.UserId == userCurrent.ID select userCurrent1).ToList();
                        List<int> idGroupUser = (from user1 in managerUser.UserGroups where user1.UserId == userCurrent.ID select user1.GroupId).ToList();
                        List<Group> listGroup = new List<Group>();
                        foreach (var id in idGroupUser)
                        {
                            var item2 = (from groupitem in managerUser.Groups where groupitem.ID == id select groupitem).FirstOrDefault();
                            listGroup.Add(item2);
                        }
                        foreach (var item in listUserOfAccount)
                        {
                            await Clients.Client(item.ConnectId).showUserName(userName, listGroup);
                        }
                    }
                }
            }
        }
        public async Task SendMessage(string userName, int idGroup, string message)
        {
            if (!userName.Equals("") && idGroup > 0)
            {
                //int idGroup = Int32.Parse(valueIdGroup);
                if (!message.Equals(""))
                {
                    using (var managerUser = new ManagerDbContext())
                    {
                        var text = userName + ": " + message;
                        MessageBox messageBox = new MessageBox();
                        messageBox.GroupId = idGroup;
                        messageBox.MessageContent = text;
                        managerUser.MessageBoxes.Add(messageBox);
                        managerUser.SaveChanges();
                        List<int> listIdUser = (from user1 in managerUser.UserGroups where user1.GroupId == idGroup select user1.UserId).ToList();

                        foreach (var item in listIdUser)
                        {
                            List<Account> listUserOfAccount = (from userCurrent1 in _listcurrentAccounts where userCurrent1.UserId == item select userCurrent1).ToList();
                            foreach (var item2 in listUserOfAccount)
                            {
                                await Clients.Client(item2.ConnectId).sendMessage(userName, messageBox.MessageContent);
                            }
                        }
                    }
                }
            }
        }
        public async Task AddMember(string userName, int idUser, int idGroup)
        {
            if (!userName.Equals("") && idGroup > 0 && idGroup > 0)
            {
                //int idGroup = Int32.Parse(valueIdGroup);
                using (var managerUser = new ManagerDbContext())
                {
                    var userGroupExist = (from userGroup1 in managerUser.UserGroups where userGroup1.UserId == idUser && userGroup1.GroupId == idGroup select userGroup1).FirstOrDefault();
                    if(userGroupExist == null)
                    {
                        UserGroup userGroup = new UserGroup();
                        userGroup.UserId = idUser;
                        userGroup.GroupId = idGroup;
                        managerUser.UserGroups.Add(userGroup);
                        managerUser.SaveChanges();
                    }
                    //List<int> listIdAccount = (from group1 in managerUser.UserGroups where group1.GroupId == idGroup select group1.UserId).ToList();
                    //List<User> listUser = new List<User>();
                    //foreach (var userItemId in listIdAccount)
                    //{
                    //    var user = (from userItem in managerUser.Users where userItem.ID == userItemId select userItem).FirstOrDefault();
                    //    listUser.Add(user);
                    //}
                    List<User> listUserInGroup = (from user in managerUser.Users
                                                  join useGroup in managerUser.UserGroups on user.ID equals useGroup.UserId
                                                  where useGroup.GroupId == idGroup
                                                  select user).ToList();
                    foreach (User item in listUserInGroup)
                    {
                        List<Account> listUserOfAccount = (from userCurrent1 in _listcurrentAccounts where userCurrent1.UserId == item.ID select userCurrent1).ToList();
                        List<Group> listGroup = (from group1 in managerUser.Groups
                                                 join useGroup in managerUser.UserGroups on group1.ID equals useGroup.GroupId
                                                 where useGroup.UserId == item.ID
                                                 select group1).ToList();
                        foreach (var item2 in listUserOfAccount)
                        {
                            await Clients.Client(item2.ConnectId).loadMember(listUserInGroup, listGroup);
                        }
                    }
                }
            }
        }
        public async Task LoadBoxChatGroup(string userName, int idGroup)
        {
            using (ManagerDbContext managerUser = new ManagerDbContext())
            {
                User userCurrent = await (from user in managerUser.Users where user.Name == userName select user).FirstOrDefaultAsync();
                if (userCurrent != null)
                {
                    var groupExist = (from groupExist1 in managerUser.Groups where groupExist1.ID == idGroup select groupExist1).FirstOrDefault();
                    if (groupExist != null)
                    {
                        List<MessageBox> messageBox = (from message in managerUser.MessageBoxes where message.GroupId == idGroup select message).ToList();
                        List<User> listUserInGroup = (from user in managerUser.Users
                                                      join useGroup in managerUser.UserGroups on user.ID equals useGroup.UserId
                                                      where useGroup.GroupId == idGroup
                                                      select user).ToList();
                        foreach(User userTem in listUserInGroup)
                        {
                            List<Account> accountInGroup = (from account in _listcurrentAccounts where account.UserId == userTem.ID select account).ToList();
                            foreach (var accountItem in accountInGroup)
                            {
                                await Clients.Client(accountItem.ConnectId).showBoxChatGroup(messageBox,listUserInGroup, groupExist.NameGroup);
                            }
                        }

                    }
                    
                }
            }
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            Account account = (from userCurrent1 in _listcurrentAccounts where userCurrent1.ConnectId == Context.ConnectionId select userCurrent1).FirstOrDefault();
            _listcurrentAccounts.Remove(account);
            //if (_timerLoad)
            //{
            //    _loadDateTime.Abort();
            //}
            return base.OnDisconnected(stopCalled);
        }
        public void NowDate()
        {
             Task.Run(() => GetDateRealTime());

            //_loadDateTime = new Thread(() => GetDateRealTime());
            //_timerLoad = true;
            //_loadDateTime.Start();
        }

        public async Task GetDateRealTime()
        {
            while (true)
            {
                var currentDate = DateTime.Now.ToString();
                await Clients.All.loadDate(currentDate);
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}


