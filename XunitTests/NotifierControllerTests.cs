using AutoMapper;
using Castle.Components.DictionaryAdapter.Xml;
using EventNotifier.Controllers;
using EventNotifier.DTOs;
using EventNotifier.Models;
using EventNotifier.Repositories;
using EventNotifier.Services;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using NetTopologySuite.Geometries;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;
using Hangfire.MemoryStorage;
using EventNotifier.Profiles;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace XunitTests
{
    public class NotifierControllerTests
    {

        private readonly IMapper _mapper;
        private static readonly CreateEventDTO createEventDTO = new CreateEventDTO()
        {
            Name = "Fireworks",
            Date = DateTime.Now.AddMonths(2),
            Point = new(21, 203),
            Category = "Entertaiment",
            Description = "test description",
            MaxSubscribers = 200
        };
        private static readonly List<Event> massiveOfEvents = new List<Event>()
           {
                new Event
                {
                    Id = 1,
                    Name = "Name1",
                    Description = "Description1",
                    Date = DateTime.Now,
                    Category = "Category1",
                    MaxSubscribers = 10,
                    Point = new NetTopologySuite.Geometries.Point(new Coordinate(0, 0))
                },
                new Event
                {
                     Id = 2,
                    Name = "Name2",
                    Description = "Description2",
                    Date = DateTime.Now.AddDays(41),
                    Category = "Category2",
                    MaxSubscribers = 100,
                    Point = new NetTopologySuite.Geometries.Point(new Coordinate(143.410, 350.430))
                },
                new Event
                {
                    Id = 3,
                    Name = "Name3",
                    Description = "Desc3",
                     Date = DateTime.Now.AddDays(100),
                    Category = "Category3",
                    MaxSubscribers = null,
                    Point = new NetTopologySuite.Geometries.Point(new Coordinate(931.2014, 549.210))
                }
           };

        public NotifierControllerTests()
        {
            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new EventProfile())));


            /*.WithWebHostBuilder(builder=>
            {
                builder.ConfigureServices(services =>
                {
                    var eventRepo = services.SingleOrDefault(s => s.ServiceType == typeof(EventRepo));
                    services.Remove(eventRepo);
                    services.AddScoped<IEventRepo, EventRepoTest>();

                });
            }
            
                
                );
            */
        }



        [Fact]
        public void Get_AllEvent()
        {
           //Arrange
            Mock<IEventService> service = new();
            service.Setup(x => x.GetAllEvents()).Returns(massiveOfEvents);
            Mock<ILogger<NotifierController>> logger = new();

            var notifierController = new NotifierController(logger.Object, service.Object, _mapper);
            //Act
            var result = notifierController.GetAllEvents();

            //Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            foreach (var e in massiveOfEvents)
            {
                Assert.NotNull(
                    result.FirstOrDefault(x => x.Id == e.Id
                            && x.Name.Equals(e.Name)
                            && x.Point.Equals(e.Point)
                            && x.Category == e.Category
                            && x.Date == e.Date
                      ));
            }
            


        }

        [Fact]
        public void Get_EventByExistingId()
        {
            //Arrange
            Mock<IEventService> service = new();
            var timeNow = DateTime.Now;
            service.Setup(x => x.GetEventById(It.IsAny<int>())).Returns(() => new Event() { Id = 1, Category = "Aboa", Date = timeNow });
            Mock<ILogger<NotifierController>> logger = new();

            var notifierController = new NotifierController(logger.Object, service.Object, _mapper);

            //Act
            var result = notifierController.GetEventById(It.IsAny<int>());

            //Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<ReadEventDTO>(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Aboa", result.Category);
            Assert.Equal(timeNow, result.Date);
        }

        [Fact]
        public void Get_EventByNotExistingId()
        {
            //Arrange
            Mock<IEventService> service = new();
            service.Setup(x => x.GetEventById(It.IsAny<int>())).Returns(() => null);
            Mock<ILogger<NotifierController>> logger = new();
            var notifierController = new NotifierController(logger.Object, service.Object, _mapper);

            //Act
            var result = notifierController.GetEventById(-1);
            //Assert
            Assert.Null(result);

        }
        [Fact]
        public void Create_EventByAdminSuccessful()
        {
            //Arrange
            Mock<IEventService> service = new();
            service.Setup(x => x.CreateEvent(createEventDTO)).Verifiable();
            Mock<ILogger<NotifierController>> logger = new();
            var notifierController = new NotifierController(logger.Object, service.Object, _mapper);
            //Act
            var result = notifierController.CreateEvent(createEventDTO) as IStatusCodeActionResult;

            //Assert
            service.Verify(s => s.CreateEvent(createEventDTO), Times.Once());
            Assert.Equal(201, result?.StatusCode);
        }

        [Fact]
        public void Create_EventByAdminWithException()
        {
            //Arrange
            Mock<IEventService> service = new();
            service.Setup(x => x.CreateEvent(createEventDTO)).Throws(new Exception("Something wrong on server side"));
            Mock<ILogger<NotifierController>> logger = new();
            var notifierController = new NotifierController(logger.Object, service.Object, _mapper);
            //Act
            var result = notifierController.CreateEvent(createEventDTO) as IStatusCodeActionResult;

            //Assert
            service.Verify(s => s.CreateEvent(createEventDTO), Times.Once());
            Assert.Equal(400, result?.StatusCode);
        }



        

        //[Fact]
        //public void CreateEventTest()
        //{
        //    //Arrange
        //    Mock<IEventService> service = new Mock<IEventService>();
        //    CreateEventDTO createEventDTO = new CreateEventDTO()
        //    {
        //        Name = "Fireworks",
        //        Date = DateTime.Now.AddMonths(2),
        //        Point = new(21, 203),
        //        Category = "Entertaiment",
        //        Description = "test description",
        //        MaxSubscribers = 200
        //    };
        //    //Act
        //    service.Object.CreateEvent(createEventDTO);

        //    //Assert
        //    service.Verify(x => x.CreateEvent(createEventDTO), Times.Once());
        //}

        //[Fact]
        //public void GetAllEventsTest()
        //{
        //    //Arrange
        //    var jobStorage = new MemoryStorage();
        //    GlobalConfiguration.Configuration.UseStorage(jobStorage);
        //    Mock<IEventRepo> repo = new Mock<IEventRepo>();
        //    repo.Setup(r => r.GetEvents()).Returns(events);
        //    EventService eventService = new EventService(
        //        logger: null,
        //        emailService: null,
        //        mapper: null,
        //        userRepo: null,
        //        recommendationService: null,
        //        eventRepo: repo.Object
        //        );
        //    //Act
        //    var foundedEvents = eventService.GetEventByCoord(new Coordinate(21, 41), 60);
        //    //Assert
        //    Assert.NotNull(foundedEvents);
        //    Assert.Contains(foundedEvents, e => e.Id == 1);
        //    Assert.Contains(foundedEvents, e => e.Id == 3);
        //    Assert.DoesNotContain(foundedEvents, e => e.Id == 2);

        //}

        //public static List<Event> events = new List<Event>()
        //{
        //    new() {Id = 1, Name = "Name1", Point=new(21,41), Category="Category1"},
        //    new() {Id = 2, Name= "Name2", Point=new(410,205.03), Category="Category2"},
        //    new() {Id = 3, Name= "Name3", Point=new(41.04,52.003), Category="Category3"},
        //};



    }
}

