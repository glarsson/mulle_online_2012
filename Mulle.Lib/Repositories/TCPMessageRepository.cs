using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mulle.Lib.DataAccess.DbContexts;
using Mulle.Lib.Entities;
using System.Data.Entity;

namespace Mulle.Lib.Repositories
{
    public interface ITCPMessageRepository : IRepository<TCPMessage>
    {
        TCPMessage FindByAction(string action);
    }

    public class TCPMessageRepository : RepositoryBase<MulleContext>, ITCPMessageRepository
    {

        public TCPMessage FindByAction(string action)
        {
            return Context.TCPMessages.FirstOrDefault(p => p.Action == action);
        }

        public void Add(TCPMessage tcpmessage)
        {
            try
            {
                Context.TCPMessages.Add(tcpmessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Remove(TCPMessage tcpmessage)
        {
            Context.TCPMessages.Remove(tcpmessage);
        }

        public void Save()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public TCPMessage Get(int id)
        {
            return Context.TCPMessages.Find(id);
        }

        public IQueryable<TCPMessage> Items { get { return Context.TCPMessages; } }

    }
}
