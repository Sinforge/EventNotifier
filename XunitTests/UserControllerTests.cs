using Castle.Core.Logging;
using EventNotifier.Controllers;
using EventNotifier.DTOs;
using EventNotifier.Models;
using EventNotifier.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XunitTests
{
    public class UserControllerTests
    {
        public UserControllerTests() { 

        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Register_User_WithCreated_And_WithBadRequest(bool flag)
        {
            //Arrange
            Mock<IOptions<Audience>> settings = new();
            Mock<ILogger<UserController>> logger = new();
            Mock<IUserService> service = new();
            service.Setup(x => x.RegisterUser(It.IsAny<CreateUserDTO>())).ReturnsAsync(flag); //Returns(()=> true);

            UserController userController = new UserController(
                logger.Object,
                service.Object,
                settings.Object
                );
            //Act
            var result = await userController.Registration(new()) as IStatusCodeActionResult;

            //Assert
            if (flag) Assert.Equal(201, result?.StatusCode);
            else Assert.Equal(400, result?.StatusCode);
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Confirm_EmailWithSuccess_And_EmailWithBadRequest(bool flag)
        {
            //Arrange
            Mock<IOptions<Audience>> settings = new();
            Mock<ILogger<UserController>> logger = new();
            Mock<IUserService> service = new();
            service.Setup(x => x.ConfirmEmail(It.IsAny<string>())).Returns(flag); 

            UserController userController = new UserController(
                logger.Object,
                service.Object,
                settings.Object
                );
            //Act
            var result = userController.ConfirmEmail("test") as IStatusCodeActionResult;

            //Assert
            if (flag) Assert.Equal(200, result?.StatusCode);
            else Assert.Equal(400, result?.StatusCode);
        }
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Login_User_WithSuccess_And_WithBadRequest(bool flag)
        {
            //Arrange
            Mock<IOptions<Audience>> settings = new();
            settings.Setup(x => x.Value).Returns(new Audience()
            {
                Iss = "not_shorrrrrrrrrrrrrrrrrrrd_iss",
                Aud = "not_shorrrrrrrrrrrrrrrrrrr_aut",
                Secret = "not_shoooooooooooooooort_secret"
            });
            Mock<ILogger<UserController>> logger = new();
            Mock<IUserService> service = new();
            service.Setup(x => x.CheckUserdata(It.IsAny<string>(), It.IsAny<string>())).Returns(flag?new User():null);

            UserController userController = new UserController(
                logger.Object,
                service.Object,
                settings.Object
                );
            //Act
            IActionResult result = await userController.Login("email", "pass");

            //Assert
            if (flag)
            {
                Assert.NotNull((result as JsonResult)?.Value);
            }
            else Assert.Equal(400, ((BadRequestObjectResult)result)?.StatusCode);
        }

    }
}
