using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;
using Shouldly;

namespace RoomBookingApp.Core
{
    public class RoomBookingRequestProcessorTest
    {
        [Test]
        public void Should_Return_Room_Booking_Response_With_Values()
        {
            //Arrange
            var bookingRequest = new RoomBookingRequest
            {
                FullName = "Test Name",
                Email = "test@test.com",
                Date = new DateTime(2022, 02, 05)
            };

            var processor = new RoomBookingRequestProcessor();

            //Act
            RoomBookingResult result = processor.BookRoom(bookingRequest);

            //Assert
            Assert.NotNull(result);
             
            Assert.That(result.FullName, Is.EqualTo(bookingRequest.FullName));
            Assert.That(result.Email, Is.EqualTo(bookingRequest.Email));  
            Assert.That(result.Date, Is.EqualTo(bookingRequest.Date));

            result.ShouldNotBeNull();
            result.FullName.ShouldBe(bookingRequest.FullName);
            result.Email.ShouldBe(bookingRequest.Email);
            result.Date.ShouldBe(bookingRequest.Date);
        }
    }
}
