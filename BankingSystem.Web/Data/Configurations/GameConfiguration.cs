using GameZone.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameZone.Data.Configurations
{
    public class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.HasKey(g => g.Id);

            builder.Property(g => g.Title)
                   .IsRequired();

            builder.Property(g => g.Description)
                   .IsRequired();

            builder.Property(g => g.PublisherName)
                   .IsRequired();

            builder.HasData(
                new Game
                {
                    Id = 1,
                    Title = "StarCraft",
                    Description = "A legendary real-time strategy game that defined competitive esports.",
                    ImageUrl = "https://www.pcguide.com/wp-content/uploads/2022/07/Starcraft-2-system-requirements.jpg",
                    PublisherName = "Blizzard Entertainment",
                    ReleasedOn = new DateTime(1998, 3, 31),
                    GenreId = 6
                },
                new Game
                {
                    Id = 2,
                    Title = "Grand Theft Auto V",
                    Description = "An open-world action game offering freedom, crime, and unforgettable characters.",
                    ImageUrl = "https://w0.peakpx.com/wallpaper/409/681/HD-wallpaper-grand-theft-auto-grand-theft-auto-v.jpg",
                    PublisherName = "Rockstar Games",
                    ReleasedOn = new DateTime(2013, 9, 17),
                    GenreId = 1
                },
                new Game
                {
                    Id = 3,
                    Title = "Colin McRae Rally 5",
                    Description = "A realistic rally racing game focused on precision, terrain, and driving skill.",
                    ImageUrl = "https://www.motorsportmagazine.com/wp-content/uploads/2025/02/Scalextric-Colin-McRae-800x450.jpg",
                    PublisherName = "Codemasters",
                    ReleasedOn = new DateTime(2002, 9, 27),
                    GenreId = 5
                }
            );
        }
    }
}
