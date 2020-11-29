﻿using NUnit.Framework;
using System;
using System.Activities;
using UnitTestExample.Controllers;

namespace UnitTestExample.Test
{
    public class AccountControllerTestFixture
    {

        [
             Test,
             TestCase("abcd1234", false),
             TestCase("irf@uni-corvinus", false),
             TestCase("irf.uni-corvinus.hu", false),
             TestCase("irf@uni-corvinus.hu", true)
         ]
        public void TestValidateEmail(string email, bool expectedResult)
        {
            // Arrange
            var accountController = new AccountController();

            // Act
            var actualResult = accountController.ValidateEmail(email);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);

        }

        [
             Test,
             TestCase("EZnemjojelszo", false),
             TestCase("EZSEM1JOJELSZO", false),
             TestCase("ezsem1jojelszo", false),
             TestCase("Ezrov1d", false),
             TestCase("Ez1JoJelszo", true),
         ]
        public void TestValidatePassword(string password, bool expectedpassResult)
        {
            // Arrange
            var accountpassController = new AccountController();

            // Act
            var actualpassResult = accountpassController.ValidatePassword(password);

            // Assert
            Assert.AreEqual(expectedpassResult, actualpassResult);
        }

        [
            Test,
            TestCase("tesztelek@uni-corvinus.hu", "JoJelszo123"),
            TestCase("csajolesz@uni-corvinus.hu", "Ezisjo1234"),
        ]
        public void TestRegisterHappyPath(string email, string password)
        {
            // Arrange
            var accountregController = new AccountController();

            // Act
            var actualregResult = accountregController.Register(email, password);

            // Assert
            Assert.AreEqual(email, actualregResult.Email);
            Assert.AreEqual(password, actualregResult.Password);
            Assert.AreNotEqual(Guid.Empty, actualregResult.ID);
        }

        [
            Test,
            TestCase("tesztelek.uni-corvinus.hu", "JoJelszo123"),
            TestCase("csajolesz@uni-corvinus", "Ezisjo1234"),
            TestCase("tesztelek@uni-corvinus.hu", "jojelszo123"),
            TestCase("csajolesz@uni-corvinus.hu", "Ezisjojojojo"),
            TestCase("tesztelek@uni-corvinus.hu", "JOJELSZO123"),
            TestCase("csajolesz@uni-corvinus.hu", "Ezjo12"),
        ]
        public void TestRegisterValidateException(string email, string password)
        {
            // Arrange
            var accountregController = new AccountController();

            // Act + Assert
            try
            {
                var actualregResult = accountregController.Register(email, password);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ValidationException>(ex);
            }
                                 
        }
    }
}
