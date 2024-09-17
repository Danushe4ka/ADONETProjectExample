using Lab2Places;
using Lab2Places.Models;
using Lab2Places.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lab2.Service
{
    public class DBInitializer
    {
        public static string Initialize(Db8011Context db)
        {
            if(db.PlacesTypes.Any())
            {
                return "Database is already initialized";
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    Random rnd = new Random();
                    string descriptionvariety = "abcdefghijklmnpqrstuvxwyz";
                    string description;

                    //заполнение Types
                    string[] types_names = ["Cafe/Restaraunt", "Bar", "Night club", "Gaming club", "Landmark", "Cinema/Theatre/Circus", "Shop", "Other"];
                    for (int i = 0; i < types_names.Length; i++)
                        db.PlacesTypes.Add(new PlacesType { TypeName = types_names[i] });
                    db.SaveChanges();

                    //заполнение Places

                    decimal geolocationA;
                    decimal geolocationB;
                    int rating;
                    int typesIdMin = db.PlacesTypes.FirstOrDefault().TypeId;
                    int typeId;
                    for (int i = 0; i < 40; i++)
                    {
                        StringBuilder key = new();
                        for (int j = 0; j < 20; j++)
                        {
                            key.Append(descriptionvariety[rnd.Next(0, descriptionvariety.Length)]);

                        }
                        description = key.ToString();
                        geolocationA = Convert.ToDecimal((180 * rnd.NextDouble()));
                        geolocationB = Convert.ToDecimal((180 * rnd.NextDouble()));
                        rating = rnd.Next(0, 101);
                        typeId = rnd.Next(typesIdMin, typesIdMin + types_names.Length-1);
                        db.Places.Add(new Place { GeolocationA = geolocationA, GeolocationB = geolocationB, TypeId = typeId, Rating = Convert.ToInt16(rating), PlaceDescription = description });
                        db.SaveChanges();
                    }

                    //заполнение Users
                    db.Users.Add(new User { UserLogin = "Admin", UserPassword = "admin", Email = "admin@mail.ru" });
                    db.Users.Add(new User { UserLogin = "User1", UserPassword = "iasdh", Email = "fasdad@mail.ru" });
                    db.Users.Add(new User { UserLogin = "User2", UserPassword = "4827gdq", Email = "n@mail.ru" });
                    db.Users.Add(new User { UserLogin = "User3", UserPassword = "asdniua_3", Email = "dqwd7ydha@mail.ru" });
                    db.SaveChanges();

                    //заполнение Reviews
                    int usersIdMin = db.Users.FirstOrDefault().UserId;
                    int placesIdMin = db.Places.FirstOrDefault().PlaceId;
                    for (int i = 0; i < 100; i++)
                    {
                        StringBuilder key = new();
                        for (int j = 0; j < 100; j++)
                        {
                            key.Append(descriptionvariety[rnd.Next(0, descriptionvariety.Length)]);

                        }
                        description = key.ToString();
                        db.Reviews.Add(new Review { ReviewText = description, Grade = Convert.ToByte(rnd.Next(0, 6)), UserId = rnd.Next(usersIdMin, usersIdMin + 4), PlaceId = rnd.Next(placesIdMin, placesIdMin + 40) });
                    }
                    db.SaveChanges();

                    //заполнение Packs
                    for (int i = 0; i < 10; i++)
                    {
                        StringBuilder key = new();
                        for (int j = 0; j < 15; j++)
                        {
                            key.Append(descriptionvariety[rnd.Next(0, descriptionvariety.Length)]);

                        }
                        description = key.ToString();
                        db.Packs.Add(new Pack { PackName = description, UserId = rnd.Next(usersIdMin, usersIdMin + 4) });
                    }
                    db.SaveChanges();

                    //заполнение PlacesInPack
                    for (int i = 0; i < 10; i++)
                    {
                        int placesCount = rnd.Next(3, 16);
                        for (int j = 0; j < placesCount; j++)
                            db.PlaceInPacks.Add(new PlaceInPack { PackId = i, PlaceId = rnd.Next(placesIdMin, placesIdMin + 40) });
                    }
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return ex.Message;
                }
            }

            return "Database initialized";
        }
    }
}
