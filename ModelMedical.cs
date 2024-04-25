using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ScreenClient
{
    class ModelMedical
    {

        // справочник видов интревью
        public class DirSurvey
        {
            public int Id { get; set; }   // первичный ключ
            public string NameSurvey { get; set; } // название интервью

            public List<DirGroup> DirGroups { get; set; } // Навигация по коллекции
 
        }

        // справочник групп
        public class DirGroup
        {
            public int Id { get; set; } // первичный ключ
            public string NameGroup { get; set; } // название группы
            public List<DirQuestion> DirQuestions { get; set; }
            public int? DirSurveyId { get; set; }      // внешний ключ к интервью
            public DirSurvey DirSurvey { get; set; }    // навигационное свойство 
            
        }
        // справочник вопросов
        public class DirQuestion
        {
            public int Id { get; set; } // первичный ключ
            public string NameQuestion { get; set; } // название вопросa
            public List<DirАnswer> DirАnswers { get; set; }
            public int? DirGroupId { get; set; }      // внешний ключ
            public DirGroup DirGroup { get; set; }    // навигационное свойство     
            
        }
        // справочник ответов 
        public class DirАnswer
        {
            public int Id { get; set; } // первичный ключ
            public string NameAnswers { get; set; }

            public int? DirQuestionId { get; set; }      // внешний ключ
            public DirQuestion DirQuestion { get; set; }    // навигационное свойство
        }

        public class ApplicationContext : DbContext
        {
            public DbSet<DirSurvey> DirSurveys { get; set; }
            public DbSet<DirGroup> DirGroups { get; set; }
            public DbSet<DirQuestion> DirQuestions { get; set; }
            public DbSet<DirАnswer> DirАnswers { get; set; }
            public ApplicationContext()
            {
                Database.EnsureDeleted();
                Database.EnsureCreated();
            }
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseNpgsql(@"Host=localhost;Port=5432;Database=d:\ModelMedical;Username=postgres;Password=pgsql");
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<DirGroup>()
                    .HasOne(p => p.DirSurvey)
                    .WithMany(t => t.DirGroups)
                    .HasForeignKey(p => p.DirSurveyId);

                modelBuilder.Entity<DirQuestion>()
                    .HasOne(p => p.DirGroup)
                    .WithMany(t => t.DirQuestions)
                    .HasForeignKey(p => p.DirGroupId);

                modelBuilder.Entity<DirАnswer>()
                    .HasOne(p => p.DirQuestion)
                    .WithMany(t => t.DirАnswers)
                    .HasForeignKey(p => p.DirQuestionId);
            }
        }
    }
}
