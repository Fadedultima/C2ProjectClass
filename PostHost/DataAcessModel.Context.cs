﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PostHost
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PostHostDBEntities : DbContext
    {
        public PostHostDBEntities()
            : base("name=PostHostDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Content> Contents { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<TagToContent> TagToContents { get; set; }
        public virtual DbSet<Like> Likes { get; set; }
    }
}
