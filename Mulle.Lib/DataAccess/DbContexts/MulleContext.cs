using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Configuration;
using Mulle.Lib.Entities;


namespace Mulle.Lib.DataAccess.DbContexts
{
    public class MulleContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<TCPMessage> TCPMessages { get; set; }
        public DbSet<Chat> Chat { get; set; }

        public MulleContext()
            : base(@"Data Source=w0eaq0d4k6.database.windows.net;Initial Catalog=MulleOnline_dev;User Id=dev@w0eaq0d4k6;Password=***REMOVED***;Encrypt=true;Trusted_Connection=false;MultipleActiveResultSets=True")//ConfigurationManager.ConnectionStrings["Mulle"].ToString()
//            : base(@"Data Source=88.26.67.1;Initial Catalog=MulleOnline_dev;User Id=dev;Password=***REMOVED***;Encrypt=true;Trusted_Connection=false;MultipleActiveResultSets=True")//ConfigurationManager.ConnectionStrings["Mulle"].ToString()
        {
            Database.SetInitializer<MulleContext>(null);
        }

       
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Map(modelBuilder);
            MapTcpMessage(modelBuilder);
            MapChat(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void Map(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>()
                .HasKey(p => p.Id).ToTable("Players", "player");

            modelBuilder.Entity<Player>()
                .Property(p => p.Email).HasColumnName("Email").IsRequired();

            modelBuilder.Entity<Player>()
                .Property(p => p.Password).HasColumnName("Password").IsRequired();

            modelBuilder.Entity<Player>()
                .Property(p => p.Alias).HasColumnName("Alias").IsOptional();

            modelBuilder.Entity<Player>()
                .Property(p => p.Win).HasColumnName("Win").IsOptional();

            modelBuilder.Entity<Player>()
                .Property(p => p.Loss).HasColumnName("Loss").IsOptional();

            modelBuilder.Entity<Player>()
                .Property(p => p.Rank).HasColumnName("Rank").IsOptional();

            modelBuilder.Entity<Player>()
                .Property(p => p.Reputation).HasColumnName("Reputation").IsOptional();

            modelBuilder.Entity<Player>()
                .Property(p => p.ProfilePicture).HasColumnName("ProfilePicture").IsOptional();

            //modelBuilder.Entity<Player>()
            //    .HasMany(m => m.Messages).WithOptional();
    
        }

        private void MapTcpMessage(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TCPMessage>()
                .HasKey(p => p.Id).ToTable("TCPMessages", "tcpmessage");

            modelBuilder.Entity<TCPMessage>()
                .Property(p => p.TransmissionControl).HasColumnName("TransmissionControl").IsRequired();

            modelBuilder.Entity<TCPMessage>()
                .Property(p => p.PlayerId).HasColumnName("FKPlayerID").IsOptional();

            modelBuilder.Entity<TCPMessage>()
                .Property(p => p.Action).HasColumnName("Action").IsRequired();

            modelBuilder.Entity<TCPMessage>()
                .Property(p => p.Message).HasColumnName("Message").IsRequired();
        }

        private void MapChat(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>()
                .HasKey(p => p.Id).ToTable("Chat", "dbo");

            modelBuilder.Entity<Chat>()
                .Property(p => p.DateTime).HasColumnName("DateTime").IsRequired();

            modelBuilder.Entity<Chat>()
                .Property(p => p.Alias).HasColumnName("Alias").IsOptional();

            modelBuilder.Entity<Chat>()
                .Property(p => p.Message).HasColumnName("Message").IsRequired();
        }

    
    }
}
