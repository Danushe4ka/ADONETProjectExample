using Lab2.Service;
using Lab2Places.Infrastracture;
using Lab2Places.Middleware;
using Lab2Places.Models;
using Lab2Places.Service;
using Lab2Places.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Lab2Places
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var services = builder.Services;

            string connectionString = builder.Configuration.GetConnectionString("RemoteSQLConnection");

            services.AddDbContext<Db8011Context>(options => options.UseSqlServer(connectionString));

            // добавление кэширования
            services.AddMemoryCache();

            // добавление поддержки сессии
            services.AddDistributedMemoryCache();
            services.AddSession();

            // внедрение зависимостей
            services.AddScoped<ICachedPlacesType, CachedPlacesTypeService>();
            services.AddScoped<ICachedPlaceService, CachedPlaceService>();
            services.AddScoped<ICachedPackService, CachedPackService>();
            services.AddScoped<ICachedPlaceInPackService, CachedPlaceInPackService>();
            services.AddScoped<ICachedReviewService, CachedReviewService>();
            services.AddScoped<ICachedUserService, CachedUserService>();

            //Использование MVC - отключено
            //services.AddControllersWithViews();
            var app = builder.Build();

            // добавляем поддержку статических файлов
            app.UseStaticFiles();

            // добавляем поддержку сессий
            app.UseSession();

            // добавляем собственный компонент middleware по инициализации базы данных и производим ее инициализацию
            app.UseDbInitializer();

            //Запоминание в Session значений, введенных в форме
            app.Map("/form", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    // Считывание из Session объекта User
                    User user = context.Session.Get<User>("user") ?? new User();

                    // Формирование строки для вывода динамической HTML формы
                    string strResponse = "<HTML><HEAD><TITLE>Пользователь</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><FORM action ='/form' / >" +
                    "Имя:<BR><INPUT type = 'text' name = 'Login' value = " + user.UserLogin + ">" +
                    "<BR>Фамилия:<BR><INPUT type = 'text' name = 'Password' value = " + user.UserPassword + " >" +
                    "<BR><BR><INPUT type ='submit' value='Сохранить в Session'><INPUT type ='submit' value='Показать'></FORM>";
                    strResponse += "<BR><A href='/'>Главная</A></BODY></HTML>";

                    // Запись в Session данных объекта User
                    user.UserLogin = context.Request.Query["Login"];
                    user.UserPassword = context.Request.Query["Password"];
                    context.Session.Set<User>("user", user);

                    // Асинхронный вывод динамической HTML формы
                    await context.Response.WriteAsync(strResponse);
                });
            });

            //Запоминание в Сookies значений, введенных в форме
            //..


            // Вывод информации о клиенте
            app.Map("/info", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    // Формирование строки для вывода 
                    string strResponse = "<HTML><HEAD><TITLE>Информация</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Информация:</H1>";
                    strResponse += "<BR> Сервер: " + context.Request.Host;
                    strResponse += "<BR> Путь: " + context.Request.PathBase;
                    strResponse += "<BR> Протокол: " + context.Request.Protocol;
                    strResponse += "<BR><A href='/'>Главная</A></BODY></HTML>";
                    // Вывод данных
                    await context.Response.WriteAsync(strResponse);
                });
            });

            // Вывод кэшированной информации из таблицы базы данных
            app.Map("/placetypes", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    //обращение к сервису
                    ICachedPlacesType cachedPlacesType = context.RequestServices.GetService<ICachedPlacesType>();
                    IEnumerable<PlacesType> placesTypes = cachedPlacesType.GetObject("20");
                    string HtmlString = "<HTML><HEAD><TITLE>Типы мест</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Список типов</H1>" +
                    "<TABLE BORDER=1>";
                    HtmlString += "<TR>";
                    HtmlString += "<TH>Код</TH>";
                    HtmlString += "<TH>Название типа</TH>";
                    HtmlString += "</TR>";
                    foreach (var pt in placesTypes)
                    {
                        HtmlString += "<TR>";
                        HtmlString += "<TD>" + pt.TypeId + "</TD>";
                        HtmlString += "<TD>" + pt.TypeName + "</TD>";
                        HtmlString += "</TR>";
                    }
                    HtmlString += "</TABLE>";
                    HtmlString += "<BR><A href='/'>Главная</A></BR>";
                    HtmlString += "<BR><A href='/placetypes'>Типы мест</A></BR>";
                    HtmlString += "<BR><A href='/form'>Данные пользователя</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    // Вывод данных
                    await context.Response.WriteAsync(HtmlString);
                });
            });

            app.Map("/places", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    //обращение к сервису
                    ICachedPlaceService cachedPlaceService = context.RequestServices.GetService<ICachedPlaceService>();
                    IEnumerable<Place> places = cachedPlaceService.GetObject("20");
                    string HtmlString = "<HTML><HEAD><TITLE>Места</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Список типов</H1>" +
                    "<TABLE BORDER=1>";
                    HtmlString += "<TR>";
                    HtmlString += "<TH>Код</TH>";
                    HtmlString += "<TH>Сев широта</TH>";
                    HtmlString += "<TH>Зап долгота</TH>";
                    HtmlString += "<TH>Код типа</TH>";
                    HtmlString += "<TH>Описание места</TH>";
                    HtmlString += "<TH>Рэйтинг</TH>";
                    HtmlString += "</TR>";
                    foreach (var pt in places)
                    {
                        HtmlString += "<TR>";
                        HtmlString += "<TD>" + pt.PlaceId + "</TD>";
                        HtmlString += "<TD>" + pt.GeolocationA + "</TD>";
                        HtmlString += "<TD>" + pt.GeolocationB + "</TD>";
                        HtmlString += "<TD>" + pt.TypeId + "</TD>";
                        HtmlString += "<TD>" + pt.PlaceDescription + "</TD>";
                        HtmlString += "<TD>" + pt.Rating + "</TD>";
                        HtmlString += "</TR>";
                    }
                    HtmlString += "</TABLE>";
                    HtmlString += "<BR><A href='/'>Главная</A></BR>";
                    HtmlString += "<BR><A href='/placetypes'>Типы мест</A></BR>";
                    HtmlString += "<BR><A href='/form'>Данные пользователя</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    // Вывод данных
                    await context.Response.WriteAsync(HtmlString);
                });
            });

            app.Map("/packs", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    //обращение к сервису
                    ICachedPackService cachedPackService = context.RequestServices.GetService<ICachedPackService>();
                    IEnumerable<Pack> packs = cachedPackService.GetObject("20");
                    string HtmlString = "<HTML><HEAD><TITLE>Паки</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Список типов</H1>" +
                    "<TABLE BORDER=1>";
                    HtmlString += "<TR>";
                    HtmlString += "<TH>Код</TH>";
                    HtmlString += "<TH>Название пака</TH>";
                    HtmlString += "<TH>Код владельца</TH>";
                    HtmlString += "</TR>";
                    foreach (var pt in packs)
                    {
                        HtmlString += "<TR>";
                        HtmlString += "<TD>" + pt.PackId + "</TD>";
                        HtmlString += "<TD>" + pt.PackName + "</TD>";
                        HtmlString += "<TD>" + pt.UserId + "</TD>";
                        HtmlString += "</TR>";
                    }
                    HtmlString += "</TABLE>";
                    HtmlString += "<BR><A href='/'>Главная</A></BR>";
                    HtmlString += "<BR><A href='/places'>Места</A></BR>";
                    HtmlString += "<BR><A href='/form'>Данные пользователя</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    // Вывод данных
                    await context.Response.WriteAsync(HtmlString);
                });
            });

            app.Map("/placesinpack", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    //обращение к сервису
                    ICachedPlaceInPackService cachedPlaceInPackService = context.RequestServices.GetService<ICachedPlaceInPackService>();
                    IEnumerable<PlaceInPack> placeInPacks = cachedPlaceInPackService.GetObject("20");
                    string HtmlString = "<HTML><HEAD><TITLE>Места в паках</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Список типов</H1>" +
                    "<TABLE BORDER=1>";
                    HtmlString += "<TR>";
                    HtmlString += "<TH>Код</TH>";
                    HtmlString += "<TH>Код места</TH>";
                    HtmlString += "<TH>Код пака</TH>";
                    HtmlString += "</TR>";
                    foreach (var pt in placeInPacks)
                    {
                        HtmlString += "<TR>";
                        HtmlString += "<TD>" + pt.PipId + "</TD>";
                        HtmlString += "<TD>" + pt.PlaceId + "</TD>";
                        HtmlString += "<TD>" + pt.PackId + "</TD>";
                        HtmlString += "</TR>";
                    }
                    HtmlString += "</TABLE>";
                    HtmlString += "<BR><A href='/'>Главная</A></BR>";
                    HtmlString += "<BR><A href='/packs'>Паки</A></BR>";
                    HtmlString += "<BR><A href='/form'>Данные пользователя</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    // Вывод данных
                    await context.Response.WriteAsync(HtmlString);
                });
            });

            app.Map("/reviews", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    //обращение к сервису
                    ICachedReviewService cachedReviewService = context.RequestServices.GetService<ICachedReviewService>();
                    IEnumerable<Review> reviews = cachedReviewService.GetObject("20");
                    string HtmlString = "<HTML><HEAD><TITLE>Обзоры</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Список типов</H1>" +
                    "<TABLE BORDER=1>";
                    HtmlString += "<TR>";
                    HtmlString += "<TH>Код</TH>";
                    HtmlString += "<TH>Код пользователя</TH>";
                    HtmlString += "<TH>Код места</TH>";
                    HtmlString += "<TH>Оценка</TH>";
                    HtmlString += "<TH>Текст обзора</TH>";
                    HtmlString += "</TR>";
                    foreach (var pt in reviews)
                    {
                        HtmlString += "<TR>";
                        HtmlString += "<TD>" + pt.ReviewId + "</TD>";
                        HtmlString += "<TD>" + pt.UserId + "</TD>";
                        HtmlString += "<TD>" + pt.PlaceId + "</TD>";
                        HtmlString += "<TD>" + pt.Grade + "</TD>";
                        HtmlString += "<TD>" + pt.ReviewText + "</TD>";
                        HtmlString += "</TR>";
                    }
                    HtmlString += "</TABLE>";
                    HtmlString += "<BR><A href='/'>Главная</A></BR>";
                    HtmlString += "<BR><A href='/places'>Места</A></BR>";
                    HtmlString += "<BR><A href='/form'>Данные пользователя</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    // Вывод данных
                    await context.Response.WriteAsync(HtmlString);
                });
            });

            app.Map("/users", (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    //обращение к сервису
                    ICachedUserService cachedUserService = context.RequestServices.GetService<ICachedUserService>();
                    IEnumerable<User> users = cachedUserService.GetObject("20");
                    string HtmlString = "<HTML><HEAD><TITLE>Пользователи</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Список типов</H1>" +
                    "<TABLE BORDER=1>";
                    HtmlString += "<TR>";
                    HtmlString += "<TH>Код</TH>";
                    HtmlString += "<TH>Логин</TH>";
                    HtmlString += "<TH>Пароль</TH>";
                    HtmlString += "<TH>Почта</TH>";
                    HtmlString += "</TR>";
                    foreach (var pt in users)
                    {
                        HtmlString += "<TR>";
                        HtmlString += "<TD>" + pt.UserId + "</TD>";
                        HtmlString += "<TD>" + pt.UserLogin + "</TD>";
                        HtmlString += "<TD>" + pt.UserPassword + "</TD>";
                        HtmlString += "<TD>" + pt.Email + "</TD>";
                        HtmlString += "</TR>";
                    }
                    HtmlString += "</TABLE>";
                    HtmlString += "<BR><A href='/'>Главная</A></BR>";
                    HtmlString += "<BR><A href='/placetypes'>Типы мест</A></BR>";
                    HtmlString += "<BR><A href='/form'>Данные пользователя</A></BR>";
                    HtmlString += "</BODY></HTML>";

                    // Вывод данных
                    await context.Response.WriteAsync(HtmlString);
                });
            });
            // Стартовая страница и кэширование данных таблицы на web-сервере
            app.Run((context) =>
            {
                //обращение к сервису
                ICachedPlacesType cachedPlacesType = context.RequestServices.GetService<ICachedPlacesType>();
                cachedPlacesType.AddObjects("20");
                ICachedPackService cachedPackService = context.RequestServices.GetService<ICachedPackService>();
                cachedPackService.AddObjects("20");
                ICachedPlaceInPackService cachedPlaceInPackService = context.RequestServices.GetService<ICachedPlaceInPackService>();
                cachedPlaceInPackService.AddObjects("20");
                ICachedPlaceService cachedPlaceService = context.RequestServices.GetService<ICachedPlaceService>();
                cachedPlaceService.AddObjects("20");
                ICachedReviewService cachedReviewService = context.RequestServices.GetService<ICachedReviewService>();
                cachedReviewService.AddObjects("20");
                ICachedUserService cachedUserService = context.RequestServices.GetService<ICachedUserService>();
                cachedUserService.AddObjects("20");
                string HtmlString = "<HTML><HEAD><TITLE>Данные</TITLE></HEAD>" +
                "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                "<BODY><H1>Главная</H1>";
                HtmlString += "<H2>Данные записаны в кэш сервера</H2>";
                HtmlString += "<BR><A href='/'>Главная</A></BR>";
                HtmlString += "<BR><A href='/placetypes'>Типы мест</A></BR>";
                HtmlString += "<BR><A href='/places'>Места</A></BR>";
                HtmlString += "<BR><A href='/packs'>Паки мест</A></BR>";
                HtmlString += "<BR><A href='/placesinpack'>Места в паках</A></BR>";
                HtmlString += "<BR><A href='/reviews'>Обзоры</A></BR>";
                HtmlString += "<BR><A href='/users'>Пользователи</A></BR>";
                HtmlString += "<BR><A href='/form'>Данные пользователя</A></BR>";
                HtmlString += "</BODY></HTML>";

                return context.Response.WriteAsync(HtmlString);

            });

            app.Run();
        }
        /*static void Main(string[] args)
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
        }*/
    }
}
