﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IEProject1.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class IEProjectEntities : DbContext
    {
        public IEProjectEntities()
            : base("name=IEProjectEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AgeSex> AgeSex { get; set; }
        public virtual DbSet<AgeState> AgeState { get; set; }
        public virtual DbSet<Content> Content { get; set; }
        public virtual DbSet<Field> Field { get; set; }
        public virtual DbSet<Place> Place { get; set; }
        public virtual DbSet<SportType> SportType { get; set; }
    }
}