using System;
using Backend.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api.Test;

public class TestingDbContext : ApplicationDbContext
{
  public TestingDbContext()
    : base(new DbContextOptionsBuilder().UseSqlite("Data Source=:memory:").Options)
  {
    Database.OpenConnection();
    Database.EnsureCreated();
  }

  public override void Dispose()
  {
    Database.CloseConnection();
    base.Dispose();
    GC.SuppressFinalize(this);
  }
}
