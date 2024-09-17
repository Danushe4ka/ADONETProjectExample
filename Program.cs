using Lab2.Service;
using Lab2Places.Models;
using Lab2Places.Service;
using Lab2Places.ViewModels;

namespace Lab2Places
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using Db8011Context db8011 = new();
            Console.WriteLine(DBInitializer.Initialize(db8011));
            int chs = -1;
            while (chs != 5)
            {
                Console.WriteLine("Choose action:\n1)Select info from views/tables\n2)Add new info in table\n3)Update tables info\n4)Delete info from tables\n5)Exit");
                chs = Convert.ToInt32(Console.ReadLine());
                switch (chs)
                {
                    case 1:
                        int chs_1 = -1;
                        while (chs_1 != 4)
                        {
                            Console.WriteLine("Choose selectable info:\n1)All Packs\n2)All user packs\n3)All user reviews\n4)Exit");
                            chs_1 = Convert.ToInt32(Console.ReadLine());
                            switch (chs_1)
                            {
                                case 1:
                                    Console.Clear();
                                    Console.WriteLine("PackId   PackName    PlaceId");
                                    foreach (var item in db8011.AllPacks.ToList())
                                        Console.WriteLine($"{item.PackId} {item.PackName} {item.PlaceId}");
                                    break;
                                case 2:
                                    Console.Clear();
                                    Console.WriteLine("UserId   UserLogin   PackName");
                                    foreach (var item in db8011.UserPacks.ToList())
                                        Console.WriteLine($"{item.UserId} {item.UserLogin} {item.PackName}");
                                    break;
                                case 3:
                                    Console.Clear();
                                    Console.WriteLine("Enter lower grade border");
                                    byte reviewGrade = Convert.ToByte(Console.ReadLine());
                                    Console.WriteLine("UserId   UserLogin   Grade   ReviewText");
                                    foreach (var item in db8011.UserReviews.Where(g => g.Grade >= reviewGrade).ToList())
                                        Console.WriteLine($"{item.UserId} {item.UserLogin} {item.Grade} {item.ReviewText}");
                                    break;
                                case 4:
                                    break;
                                default:
                                    break;
                            }
                            Console.ReadKey();
                            Console.Clear();
                        }
                        break;
                    case 2:
                        Console.Clear();
                        Console.WriteLine("Additing new review for User1 and place_id = 1...\nEnter your grade and description");
                        byte grade= 10;
                        while(grade < 0 || grade > 5)
                             grade = Convert.ToByte(Console.ReadLine());
                        string ?description = Console.ReadLine();
                        Review review = new()
                        {
                            ReviewText = description,
                            Grade = grade,
                            UserId = 1,
                            PlaceId = 1
                        };
                        db8011.Reviews.Add(review);
                        db8011.SaveChanges();
                        break;
                    case 3:
                        Console.Clear();
                        Console.WriteLine("Updating a random place for id...\nEnter place id");
                        int id = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Enter new description");
                        Place place = db8011.Places.Where(p => p.PlaceId == id).FirstOrDefault();
                        if(place != null)
                            place.PlaceDescription = Console.ReadLine();
                        db8011.SaveChanges();
                        break;
                    case 4:
                        Console.Clear();
                        Console.WriteLine("Deleting all user reviews...\nEnter user login:");
                        string login = Console.ReadLine();
                        IQueryable<Review> deletingReviews = db8011.Reviews.Where(d => d.User.UserLogin == login);
                        db8011.Reviews.RemoveRange(deletingReviews);
                        db8011.SaveChanges();
                        break;
                    case 5:
                        break;
                    default:
                        Console.WriteLine("Nothing happened(");
                        break;
                }
                Console.ReadKey();
                Console.Clear();
            }
            Console.ReadKey();
        }
    }
}
