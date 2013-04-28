namespace Ember.Handlebars.ExampleWebAPI.Migrations
{
    using Ember.Handlebars.ExampleWebAPI.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Ember.Handlebars.ExampleWebAPI.Models.AppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Ember.Handlebars.ExampleWebAPI.Models.AppContext context)
        {
            context.Shows.AddOrUpdate(
                new Show() {
                    ShowId = 1,
                    Title = "Archer",
                    Network = "FX",
                    Category = "Comedy",
                    Year = 2009
                },
                new Show() {
                    ShowId = 2,
                    Title = "24",
                    Network = "Fox",
                    Category = "Thriller",
                    Year = 2001
                },
                new Show() {
                    ShowId = 3,
                    Title = "Breaking Bad",
                    Network = "AMC",
                    Category = "Drama",
                    Year = 2008
                },
                new Show() {
                    ShowId = 4,
                    Title = "The Walking Dead",
                    Network = "AMC",
                    Category = "Drama",
                    Year = 2010
                },
                new Show() {
                    ShowId = 5,
                    Title = "Heroes",
                    Network = "NBC",
                    Category = "Sci-Fi",
                    Year = 2006
                },
                new Show() {
                    ShowId = 6,
                    Title = "Family Guy",
                    Network = "Fox",
                    Category = "Comedy",
                    Year = 1999
                },
                new Show() {
                    ShowId = 7,
                    Title = "Vikings",
                    Network = "History Channel",
                    Category = "Drama",
                    Year = 2013
                },
                new Show() {
                    ShowId = 8,
                    Title = "Person of Interest",
                    Network = "CBS",
                    Category = "Drama",
                    Year = 2011
                },
                new Show() {
                    ShowId = 9,
                    Title = "Lost",
                    Network = "ABC",
                    Category = "Sci-Fi",
                    Year = 2004
                },
                new Show() {
                    ShowId = 10,
                    Title = "A Grande Familia",
                    Network = "Globo",
                    Category = "Comedy",
                    Year = 2001
                }
            );
        }
    }
}
