using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using RoomBookingApp.Core.DataServices;
using RoomBookingApp.Domain.BaseModels;
using RoomBookingApp.Core.Enums;
using RoomBookingApp.Core.Models;
using RoomBookingApp.Core.Processors;
using RoomBookingApp.Domain;
using Shouldly;

namespace RoomBookingApp.Core
{
    public class RoomBookingRequestProcessorTest
    {
        private RoomBookingRequestProcessor _processor;
        private RoomBookingRequest _bookingRequest;
        private Mock<IRoomBookingService> _roomBookingServiceMock;
        private List<Room> _availalbeRooms;
        [SetUp]
        public void Setup()
        {
            _bookingRequest = new RoomBookingRequest
            {
                FullName = "Test Name",
                Email = "test@test.com",
                Date = new DateTime(2022, 02, 05)
            };
            _availalbeRooms = new List<Room>() { new Room() { Id = 1 } };
            _roomBookingServiceMock = new Mock<IRoomBookingService>();
            _roomBookingServiceMock.Setup(q => q.GetAvailableRooms(_bookingRequest.Date))
                .Returns(_availalbeRooms);
            _processor = new RoomBookingRequestProcessor(_roomBookingServiceMock.Object);
            
        }
        public RoomBookingRequestProcessorTest()
        {
            
            
            
        }
        [Test]
        public void Should_Return_Room_Booking_Response_With_Values()
        {
            //Arrange
            

           

            //Act
            RoomBookingBase result = _processor.BookRoom(_bookingRequest);

            //Assert
            Assert.NotNull(result);
             
            Assert.That(result.FullName, Is.EqualTo(_bookingRequest.FullName));
            Assert.That(result.Email, Is.EqualTo(_bookingRequest.Email));  
            Assert.That(result.Date, Is.EqualTo(_bookingRequest.Date));

            result.ShouldNotBeNull();
            result.FullName.ShouldBe(_bookingRequest.FullName);
            result.Email.ShouldBe(_bookingRequest.Email);
            result.Date.ShouldBe(_bookingRequest.Date);
        }


        [Test]
        public void Should_Throw_Exception_For_Null_Request()
        {


            Should.Throw<ArgumentNullException>(() => _processor.BookRoom(null));
        }

        [Test]
        public void Should_Save_Room_Booking_Request()
        {
            RoomBooking savedBooking = null;
            _roomBookingServiceMock.Setup(q => q.Save(It.IsAny<RoomBooking>()))
                .Callback<RoomBooking>(booking =>
                {
                    savedBooking = booking;
                });
            _processor.BookRoom(_bookingRequest);

            _roomBookingServiceMock.Verify(q => q.Save(It.IsAny<RoomBooking>()), Times.Once);

            savedBooking.ShouldNotBeNull();
            savedBooking.FullName.ShouldBe(_bookingRequest.FullName);
            savedBooking.Email.ShouldBe(_bookingRequest.Email);
            savedBooking.Date.ShouldBe(_bookingRequest.Date);
            savedBooking.RoomId.ShouldBe(_availalbeRooms.First().Id);
        }

        [Test]
        public void Should_Not_Save_Room_Booking_Request_If_None_Available()
        {
           _availalbeRooms.Clear();
            _processor.BookRoom(_bookingRequest);

            _roomBookingServiceMock.Verify(q => q.Save(It.IsAny<RoomBooking>()), Times.Never);
        }

        [Test]
        [TestCase(BookingResultFlag.Failure, false)]
        [TestCase(BookingResultFlag.Success, true)]
        public void Should_Return_SuccessOrFailure_Flag_In_Result(BookingResultFlag bookingSuccessFlag, bool isAvailable)
        {
            if (!isAvailable)
            {
                _availalbeRooms.Clear();
            }

            var result = _processor.BookRoom(_bookingRequest);
            bookingSuccessFlag.ShouldBe(result.Flag);
        }

        [Test]
        [TestCase(1, true)]
        [TestCase(null, false)]
        public void Should_Return_RoomBookingId_In_Result(int? roomBookingId, bool isAvailable)
        {
            if (!isAvailable)
            {
                _availalbeRooms.Clear();
            }
            else
            {
                _roomBookingServiceMock.Setup(q => q.Save(It.IsAny<RoomBooking>()))
                .Callback<RoomBooking>(booking =>
                {
                    booking.Id = roomBookingId.Value;
                });
            }
            var result = _processor.BookRoom(_bookingRequest);
            result.RoomBookingId.ShouldBe(roomBookingId);
        }
    }
}
