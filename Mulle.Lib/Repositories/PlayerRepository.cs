using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mulle.Lib.DataAccess.DbContexts;
using Mulle.Lib.Entities;

namespace Mulle.Lib.Repositories
{
    public interface IPlayerRepository : IRepository<Player>
    {
        Player FindByEmail(string email);
    }

    public class PlayerRepository : RepositoryBase<MulleContext>, IPlayerRepository
    {

        public Player FindByEmail(string email)
        {
            return Context.Players.FirstOrDefault(p => p.Email == email);
        }

        public void Add(Player player)
        {
            //var existingPlayer = Context.Players.FirstOrDefault(p => p.Email == player.Email);
            //if(existingPlayer == null)
            Context.Players.Add(player);
        }

        public void Remove(Player player)
        {
            Context.Players.Remove(player);
        }

        public void Save()
        {
            Context.SaveChanges();
        }

        public Player Get(int id)
        {
            return Context.Players.Find(id);
        }

        public IQueryable<Player> Items { get { return Context.Players; } }
        
    }
}
