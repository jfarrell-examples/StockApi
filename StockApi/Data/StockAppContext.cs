using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StockApi.Data
{
    public class StockAppContext : DbContext, IContext
    {
        public DbSet<StockAveragePrice> AveragePrices { get; set; }
        public DbSet<StockMaxPrice> MaxPrices { get; set; }
        public DbSet<StockMinPrice> MinPrices { get; set; }
        public DbSet<StockPriceChange> PriceChanges { get; set; }

        public StockAppContext(DbContextOptions<StockAppContext> options) : base(options)
        {
        }
    }

    public interface IContext
    {
        DbSet<StockAveragePrice> AveragePrices { get; set; }
        DbSet<StockMaxPrice> MaxPrices { get; set; }
        DbSet<StockMinPrice> MinPrices { get; set; }
        DbSet<StockPriceChange> PriceChanges { get; set; }

        Task<int> SaveChangesAsync(CancellationToken token = default);
    }

    [Table("StockAveragePrices")]
    public class StockAveragePrice
    {
        [Key]
        public Guid Id { get; set; }
        
        public string Symbol { get; set; }
        
        public int TotalEntries { get; set; }
        public decimal Total { get; set; }
        public decimal Average => Math.Round(Total / TotalEntries, 3);
        public DateTime WrittenOn { get; set; }
    }

    [Table("StockMaxPrices")]
    public class StockMaxPrice
    {
        [Key]
        public string Symbol { get; set; }
        public decimal MaxPrice { get; set; }
        public DateTime WrittenOn { get; set; }
    }

    [Table("StockMinPrices")]
    public class StockMinPrice
    {
        [Key]
        public string Symbol { get; set; }
        public decimal MinPrice { get; set; }
        public DateTime WrittenOn { get; set; }
    }

    [Table("StockPriceChanges")]
    public class StockPriceChange
    {
        [Key]
        public Guid Id { get; set; }
        
        public string Symbol { get; set; }
        public decimal NewPrice { get; set; }
        
        [Column("Change")]
        public decimal PriceChange { get; set; }

        [Column("ChangePercent")]
        public decimal PercentChange { get; set; }
        
        public DateTime? Expired { get; set; }
        public DateTime Created { get; set; }
    }
}