using Airports.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Airports.Tests
{
    [TestClass]
    public class CsvHelperTests
    {
        [TestMethod]
        public void ValidatePropertyIsNull()
        {
            // Arrange
            var airline = new Airline
            {
                IATACode = null
            };
            var validationContext = new ValidationContext(airline, null, null);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(airline, validationContext, validationResults, true);

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void ValidatePropertyNotNull()
        {
            // Arrange
            var airline = new Airline
            {
                IATACode = "iata"
            };
            var validationContext = new ValidationContext(airline, null, null);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(airline, validationContext, validationResults, true);

            // Assert
            Assert.IsTrue(isValid);
        }
    }
}
