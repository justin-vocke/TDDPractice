using Microsoft.EntityFrameworkCore;
using RoomBookingApp.Domain;
using RoomBookingApp.Persistence.Repositories;
using Shouldly;

namespace RoomBookingApp.Persistence.Tests
{
    public class RoomBookingServiceTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Should_Return_Available_Rooms()
        {
            //Arrange
            var date = new DateTime(2021, 10, 01);

            var dbOptions = new DbContextOptionsBuilder<RoomBookingAppDbContext>()
                .UseInMemoryDatabase("AvailableRoomTest").Options;

            using (var context = new RoomBookingAppDbContext(dbOptions))
            {
                context.Add(new Room { Id = 1, Name = "Room 1" });
                context.Add(new Room { Id = 2, Name = "Room 2" });
                context.Add(new Room { Id = 3, Name = "Room 3" });

                context.Add(new RoomBooking { RoomId = 1, Date = date });
                context.Add(new RoomBooking { RoomId = 2, Date = date.AddDays(-1) });

                context.SaveChanges();

                var roomBookingService = new RoomBookingService(context);

                //Act
                var availableRooms = roomBookingService.GetAvailableRooms(date);

                //Assert
                Assert.That(2, Is.EqualTo(availableRooms.Count()));
                //Assert.That((System.Collections.ICollection?)availableRooms, Does.Contain());
                availableRooms.ShouldContain(x => x.Id == 2);
            }
        }

        [Test]
        public void Should_Save_Room_Booking()
        {
            var dbOptions = new DbContextOptionsBuilder<RoomBookingAppDbContext>()
                .UseInMemoryDatabase("ShouldSaveTest").Options;

            var roomBooking = new RoomBooking { RoomId = 1, Date = new DateTime(2023, 10, 10) };

            using (var context = new RoomBookingAppDbContext(dbOptions))
            {
                var roomBookingService = new RoomBookingService(context);
                roomBookingService.Save(roomBooking);

                var bookings = context.RoomBookings.ToList();
                Assert.That(bookings.Count == 1);
            }

        }
    }
}