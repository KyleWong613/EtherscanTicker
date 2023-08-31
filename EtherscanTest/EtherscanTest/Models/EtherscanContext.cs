using System; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EtherscanTest.Models
{
    public partial class EtherscanContext : DbContext
    {

        public EtherscanContext()
        {
        }

        public EtherscanContext(DbContextOptions<EtherscanContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TokenDetail> TokenDetails { get; set; } 
          
    }
}
