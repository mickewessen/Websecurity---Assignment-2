using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Websecurity___Essay_2.Models;

namespace Websecurity___Essay_2.Data
{
    public class Context : DbContext
    {
        public Context (DbContextOptions<Context> options)
            : base(options)
        {
        }

        public DbSet<UserInput> UserInput { get; set; }
        public DbSet<UserUploadFile> UserUploadFile { get; set; }
    }
}
