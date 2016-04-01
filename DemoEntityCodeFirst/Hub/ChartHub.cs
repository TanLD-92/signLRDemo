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

namespace SignalRChat
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {

        ManagerDbContext manager = new ManagerDbContext(); 
               
        //public void LoadInfo(List<User> listUser)
        //{
        //    Clients.All.loadInfo(listUser);
        //}
        public async Task LoadInfoDefault()
        {
            // Call the updateInfo method to update clients.
           
            try
            {
                //manager.Users.Add(stud);
                //manager.SaveChanges();
                //listUser.Add(stud);
                List<User> listUser = GetListUser();
                await Clients.All.loadInfo(listUser);
            }
            catch (Exception e)
            {
                Clients.All.loadInfo("error");
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
                List<User> listUser = GetListUser();
                await Clients.All.loadInfo(listUser);
            }
        }
        public List<User> GetListUser()
        {
            List<User> list = (from user in manager.Users select user).ToList();
            return list;
        }
        public async Task NowDate()
        {
            new Thread(
                while (true)
            {
                var currentDate = DateTime.Now.ToString();
                await Clients.All.loadDate(currentDate);
                System.Threading.Thread.Sleep(1000);
            }).Start();
        }
    }

}