using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public static class AppDbContextSeed
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            if (!await db.Authors.AnyAsync() && !await db.Categories.AnyAsync() && !await db.Products.AnyAsync())
            {
                var author1 = new Author() { FullName = "Fyodor Dostoevsky" };
                var author2 = new Author() { FullName = "George Orwell" };
                var author3 = new Author() { FullName = "Stephen R.Covey" };
                var author4 = new Author() { FullName = "Rhonda Byrne" };
                var author5 = new Author() { FullName = "Robert Greene" };
                var author6 = new Author() { FullName = "Yuval Noah Harari" };
                await db.Authors.AddRangeAsync(author1, author2, author3, author4, author5, author6);

                var cat1 = new Category() { CategoryName = "Classics" };
                var cat2 = new Category() { CategoryName = "Literary" };
                var cat3 = new Category() { CategoryName = "Self-Help" };
                var cat4 = new Category() { CategoryName = "Politics & Social Sciences" };
                await db.Categories.AddRangeAsync(cat1, cat2, cat3, cat4);

                await db.Products.AddRangeAsync(
                    new Product() { Name = "Crime and Punishment", Price = 7.31m, PictureUri = "samples/01.jpg", Author = author1, Category = cat1 },
                    new Product() { Name = "Notes from the Underground", Price = 12.99m, PictureUri = "samples/02.jpg", Author = author1, Category = cat1 },
                    new Product() { Name = "The Idiot", Price = 32.95m, PictureUri = "samples/03.jpg", Author = author1, Category = cat1 },
                    new Product() { Name = "The Grand Inquisitor", Price = 5.99m, PictureUri = "samples/04.jpg", Author = author1, Category = cat2 },
                    new Product() { Name = "Animal Farm", Price = 9.19m, PictureUri = "samples/05.jpg", Author = author2, Category = cat1 },
                    new Product() { Name = "Politics and the English Language", Price = 39.47m, PictureUri = "samples/06.jpg", Author = author2, Category = cat1 },
                    new Product() { Name = "Homage to Catalonia", Price = 14.71m, PictureUri = "samples/07.jpg", Author = author2, Category = cat2 },
                    new Product() { Name = "The 7 Habits of Highly Effective People: Powerful Lessons in Personal Change", Price = 19.05m, PictureUri = "samples/08.jpg", Author = author3, Category = cat3 },
                    new Product() { Name = "The Secret", Price = 11.32m, PictureUri = "samples/09.jpg", Author = author4, Category = cat3 },
                    new Product() { Name = "The Magic", Price = 16.62m, PictureUri = "samples/10.jpg", Author = author4, Category = cat3 },
                    new Product() { Name = "The 48 Laws of Power", Price = 9.99m, PictureUri = "samples/11.jpg", Author = author5, Category = cat4 },
                    new Product() { Name = "Sapiens: A Brief History of Humankind", Price = 11.29m, PictureUri = "samples/12.jpg", Author = author6, Category = cat4 }
                    );
                await db.SaveChangesAsync();
            }
        }
    }
}
