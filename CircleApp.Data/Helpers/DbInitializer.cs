using CircleApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircleApp;

namespace CircleApp.Data.Helpers
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext appDbContext)
        {
            //Preguntamos si en la BD no hay algun usuario o un post
            if (!appDbContext.Users.Any() && !appDbContext.Posts.Any())
            {
                //Creamos el nuevo usuario
                var newUser = new User()
                {
                    FullName = "Jeaustin Rojas",
                    ProfilePictureUrl = "https://yt3.ggpht.com/yti/ANjgQV_6nWAmw-oljVfZSzz4qq24q8ucCalOM3rldfsUdUoDEA=s88-c-k-c0x00ffffff-no-rj"
                };

                //Se agrega el nuevo usuario al contexto de la BD y se guardan los cambios
                await appDbContext.Users.AddAsync(newUser);
                await appDbContext.SaveChangesAsync();


                var newPostWithoutImage = new Post()
                {
                    Content = "This is going to be  our firts post which is being loaded from the database and it has been created using our test user.",
                    ImageUrl = "",
                    NrOfReports = 0,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,

                    UserId = newUser.Id
                };

                var newPostWithImage = new Post()
                {
                    Content = "This is going to be  our firts post which is being loaded from the database and it has been created using our test user. This post has a image.",
                    ImageUrl = "~/images/Promo 1.png",
                    NrOfReports = 0,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,

                    UserId = newUser.Id
                };
                await appDbContext.Posts.AddRangeAsync(newPostWithoutImage, newPostWithImage);
                await appDbContext.SaveChangesAsync();

            }
        }
    }
}
