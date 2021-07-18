using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Altamira.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;

namespace Altamira.EfRepository
{
    public class EfContext : DbContext
    {
        public DbSet<Geo> Geo { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }

        public EfContext(DbContextOptions<EfContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var client = new HttpClient();
            var dataString = client.GetStringAsync("https://jsonplaceholder.typicode.com/users").Result;
            var users = JsonConvert.DeserializeObject<List<User>>(dataString);

            users.ForEach(x =>
            {
                x.Address.Id = x.Id;
                x.Address.User = x;
                x.Address.UserId = x.Id;

                x.Address.Geo.Id = x.Id;
                x.Address.Geo.Address = x.Address;
                x.Address.Geo.AddressId = x.Id;

                x.Company.Id = x.Id;
                x.Company.User = x;
                x.Company.UserId = x.Id;
            });

            var geoList = new List<object>();
            var addressList = new List<object>();
            var companyList = new List<object>();
            var userList = new List<object>();

            foreach (var item in users.Select(x => x.Address.Geo))
            {
                geoList.Add(new
                {
                    item.Id,
                    item.AddressId,
                    item.Lat,
                    item.Lng,
                });
            }

            foreach (var item in users.Select(x => x.Address))
            {
                addressList.Add(new
                {
                    item.Id,
                    item.City,
                    item.Street,
                    item.Suite,
                    item.Zipcode,
                    item.UserId,
                });
            }

            foreach (var item in users.Select(x => x.Company))
            {
                companyList.Add(new
                {
                    item.Id,
                    item.UserId,
                    item.Bs,
                    item.CatchPhrase,
                    item.Name,
                });
            }

            foreach (var item in users)
            {
                userList.Add(new
                {
                    item.Id,
                    item.Email,
                    item.Name,
                    item.Phone,
                    item.Username,
                    item.Website,
                    Password = "123456",
                });
            }

            modelBuilder.Entity<User>().HasData(userList);
            modelBuilder.Entity<Company>().HasData(companyList);
            modelBuilder.Entity<Address>().HasData(addressList);
            modelBuilder.Entity<Geo>().HasData(geoList);
        }
    }
}