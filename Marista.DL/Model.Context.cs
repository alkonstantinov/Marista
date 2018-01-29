﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Marista.DL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MaristaEntities : DbContext
    {
        public MaristaEntities()
            : base("name=MaristaEntities")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Level> Levels { get; set; }
        public virtual DbSet<SiteUser> SiteUsers { get; set; }
        public virtual DbSet<Constant> Constants { get; set; }
        public virtual DbSet<HCategory> HCategories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<SOBonusSize> SOBonusSizes { get; set; }
        public virtual DbSet<VCategory> VCategories { get; set; }
        public virtual DbSet<BonusSize> BonusSizes { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<ChatItem> ChatItems { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<Pyramid> Pyramids { get; set; }
        public virtual DbSet<MarketingMaterial> MarketingMaterials { get; set; }
        public virtual DbSet<BP> BPs { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<SaleDetail> SaleDetails { get; set; }
        public virtual DbSet<vSalesTotal> vSalesTotals { get; set; }
        public virtual DbSet<vMonthSalesPerUser> vMonthSalesPerUsers { get; set; }
        public virtual DbSet<vMyTeamReport> vMyTeamReports { get; set; }
        public virtual DbSet<v3MonthSalesPerUser> v3MonthSalesPerUser { get; set; }
        public virtual DbSet<vBonus> vBonuses { get; set; }
        public virtual DbSet<v3MonthsBonuses> v3MonthsBonuses { get; set; }
    }
}
