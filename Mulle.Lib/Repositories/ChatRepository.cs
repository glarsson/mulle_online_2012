using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mulle.Lib.DataAccess.DbContexts;
using Mulle.Lib.Entities;
using System.Data.Entity;

namespace Mulle.Lib.Repositories
{
    public interface IChatRepository : IRepository<Chat>
    {
        Chat FindByAlias(string alias);
    }

    public class ChatRepository : RepositoryBase<MulleContext>, IChatRepository
    {

        public Chat FindByAlias(string alias)
        {
            return Context.Chat.FirstOrDefault(p => p.Alias == alias);
        }

        public void Add(Chat message)
        {
            try
            {
                Context.Chat.Add(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Remove(Chat tcpmessage)
        {
            Context.Chat.Remove(tcpmessage);
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

        public Chat Get(int id)
        {
            return Context.Chat.Find(id);
        }

        public IQueryable<Chat> Items { get { return Context.Chat; } }

    }
}
