/*  Driver_Postal_NUnitTests.cs
 *  Assignment 6
 * 
 *  Revision History
 *      Zhenzhen Tang, 2015.11.14: Created
 */
using System;
using System.Collections.Generic;
using NUnit.Framework;
using ZTBusService.Models;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZTBusService.Controllers;

/* 
 * To test the edits in the solution
 * 
 * Zhenzhen Tang, 2015-11-14
 */

namespace ZTBusService.Tests.NUnitTests
{
    /// <summary>
    /// tests to verify the edits on postal code
    /// </summary>
    [TestFixture]
    public class Driver_Postal_NUnitTests
    {
        driver driver;
        BusServiceContext db = new BusServiceContext();

        /// <summary>
        /// initilise creates a valid driver object for test
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            driver = new driver();
            driver.firstName = "Zhen Zhen";
            driver.lastName = "Tang";
            driver.homePhone = "222-222-2222";
            driver.postalCode = "A3A 3A3";
            driver.dateHired = DateTime.Now.AddYears(-1);
        }

        /// <summary>
        /// removes the prior order object from the Entity Framework queue
        /// </summary>
        [TearDown]
        public void CleanUp()
        {
            db.Entry(driver).State = EntityState.Detached;
        }
        
        /// <summary>
        /// test uppercase postal code with space in the middle
        /// </summary>
        [Test]
        public void Driver_Postal_NUnitTests_UpperCasePostalWithSpace_shouldPassAndReformat()
        {
            //Arrange
            driver.postalCode = "A3A 3A3";
            //Act
            try
            {
                db.drivers.Add(driver);
                db.SaveChanges();
            }
            //Assert
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Assert.Fail("Got an edit error on 'A3A 3A3' : " + GetValidationErrorMessages(ex));
            }
            catch (Exception ex)
            {
                Assert.Fail("Got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

        /// <summary>
        /// test uppercase postal code without space in the middle
        /// </summary>
        [Test]
        public void Driver_Postal_NUnitTests_UpperCasePostalNoSpace_shouldPassAndReformat()
        {
            //Arrange
            driver.postalCode = "A3A3A3";
            //Act
            try
            {
                db.drivers.Add(driver);
                db.SaveChanges();
            }
            //Assert
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Assert.Fail("Got an edit error on 'A3A3A3' : " + GetValidationErrorMessages(ex));
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

        /// <summary>
        /// test lowercase postal code with space in the middle
        /// </summary>
        [Test]
        public void Driver_Postal_NUnitTests_LowerCasePostalWithSpace_shouldPassAndReformat()
        {
            //Arrange
            driver.postalCode = "a3a 3a3";
            //Act
            try
            {
                db.drivers.Add(driver);
                db.SaveChanges();
            }
            //Assert
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Assert.Fail("Got an edit error on 'a3a 3a3' : " + GetValidationErrorMessages(ex));
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

        /// <summary>
        /// test lowercase postal code without space in the middle
        /// </summary>
        [Test]
        public void Driver_Postal_NUnitTests_LowerCasePostalNoSpace_shouldPassAndReformat()
        {
            //Arrange
            driver.postalCode = "a3a3a3";
            //Act
            try
            {
                db.drivers.Add(driver);
                db.SaveChanges();
            }
            //Assert
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Assert.Fail("Got an edit error on 'a3a3a3' : " + GetValidationErrorMessages(ex));
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

        /// <summary>
        /// test postal code with wrong first letter
        /// </summary>
        [Test]
        public void Driver_Postal_NUnitTests_FirstLetterWrongPostal_ShouldCatchException()
        {
            //Arrange
            driver.postalCode = "D3A 3A3";
            //Act
            try
            {
                db.drivers.Add(driver);
                db.SaveChanges();
            }
            //Assert
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                string message = GetValidationErrorMessages(ex);
                Assert.Pass("Entity Validation Exception has been catched: " + message);
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.Fail("'D3A 3A3' should not have been accepted as a postal code. ");
        }

        /// <summary>
        /// test lowercase postal code with spaces before and after it, and with space in the middle
        /// </summary>
        [Test]
        public void SpaceBeforeAndAfterPostalWithMiddleSpace_shouldPassAndReformat()
        {
            //arrange
            ValidationContext driverContext = new ValidationContext(db);
            driver.postalCode = "          a3a 3a3            ";
            List<ValidationResult> validationErrors;

            //act
            validationErrors = driver.Validate(driverContext).ToList();
            try
            {
                db.drivers.Add(driver);
                db.SaveChanges();
            }
            //assert            
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Assert.Fail("Got an edit error on '          a3a 3a3            ' : " + ex.GetBaseException().Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }
            Assert.IsTrue(validationErrors.Count() == 0);
            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

        /// <summary>
        /// test lowercase postal code with spaces before and after it, and without space in the middle
        /// </summary>
        [Test]
        public void SpaceBeforeAndAfterPostalNoMiddleSpace_shouldPassAndReformat()
        {
            //arrange
            ValidationContext driverContext = new ValidationContext(db);
            driver.postalCode = "          a3a3a3            ";
            List<ValidationResult> validationErrors;

            //act
            validationErrors = driver.Validate(driverContext).ToList();
            try
            {
                db.drivers.Add(driver);
                db.SaveChanges();
            }
            //assert            
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Assert.Fail("Got an edit error on '          a3a 3a3            ' : " + ex.GetBaseException().Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }
            Assert.IsTrue(validationErrors.Count() == 0);
            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }        
        
        /// <summary>
        /// test mixed uppercase and lowercase postal code with spaces before it, and without space in the middle
        /// </summary>
        [Test]
        public void Driver_Postal_NUnitTests_SpacesBeforePostalNoMiddleSpace_shouldPassAndReformat()
        {
            //Arrange
            driver.postalCode = "   a3a3A3";
            //Act
            try
            {
                db.drivers.Add(driver);
                db.SaveChanges();
            }
            //Assert
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Assert.Fail("Got an edit error on '   a3a3A3' : " + GetValidationErrorMessages(ex));
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

        /// <summary>
        /// test mixed uppercase and lowercase postal code with spaces before it, and with space in the middle
        /// </summary>
        [Test]
        public void Driver_Postal_NUnitTests_SpacesBeforePostalWithMiddleSpace_shouldPassAndReformat()
        {
            //Arrange
            driver.postalCode = "  a3a 3A3";
            //Act
            try
            {
                db.drivers.Add(driver);
                db.SaveChanges();
            }
            //Assert
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Assert.Fail("Got an edit error on '  a3a 3A3' : " + GetValidationErrorMessages(ex));
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

        /// <summary>
        /// test mixed uppercase and lowercase postal code with spaces after it, and without space in the middle
        /// </summary>
        [Test]
        public void Driver_Postal_NUnitTests_SpacesAfterPostalNoMiddleSpace_shouldPassAndReformat()
        {
            //Arrange
            driver.postalCode = "a3a3A3   ";
            //Act
            try
            {
                db.drivers.Add(driver);
                db.SaveChanges();
            }
            //Assert
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Assert.Fail("Got an edit error on '   a3a3A3' : " + GetValidationErrorMessages(ex));
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

        /// <summary>
        /// test mixed uppercase and lowercase postal code with spaces after it, and with space in the middle
        /// </summary>
        [Test]
        public void Driver_Postal_NUnitTests_SpacesAfterPostalWithMiddleSpace_shouldPassAndReformat()
        {
            //Arrange
            driver.postalCode = "a3a 3A3  ";
            //Act
            try
            {
                db.drivers.Add(driver);
                db.SaveChanges();
            }
            //Assert
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Assert.Fail("Got an edit error on 'a3a 3A3  ' : " + GetValidationErrorMessages(ex));
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

        /// <summary>
        /// test mixed uppercase and lowercase postal code and with space in the middle
        /// </summary>
        [Test]
        public void Driver_Postal_NUnitTests_MixedUpperLowerCasePostalWithSpace_shouldPassAndReformat()
        {
            //Arrange
            driver.postalCode = "A3a 3A3";
            //Act
            try
            {
                db.drivers.Add(driver);
                db.SaveChanges();
            }
            //Assert
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Assert.Fail("Got an edit error on 'a3a3a3' : " + GetValidationErrorMessages(ex));
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

        /// <summary>
        /// test mixed uppercase and lowercase postal code and without space in the middle
        /// </summary>
        [Test]
        public void Driver_Postal_NUnitTests_MixedUpperLowerCasePostalNoSpace_shouldPassAndReformat()
        {
            //Arrange
            driver.postalCode = "A3a3A3";
            //Act
            try
            {
                db.drivers.Add(driver);
                db.SaveChanges();
            }
            //Assert
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Assert.Fail("Got an edit error on 'a3a3a3' : " + GetValidationErrorMessages(ex));
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

        /// <summary>
        /// get all EntityValidationErrors' detail message from System.Data.Entity.Validation.DbEntityValidationException
        /// </summary>
        /// <param name="ex">DbEntityValidationException exception</param>
        /// <returns>exception detail message</returns>
        private string GetValidationErrorMessages(System.Data.Entity.Validation.DbEntityValidationException ex)
        {
            string message = "";
            foreach (var entityInError in ex.EntityValidationErrors)
            {
                foreach (var validationError in entityInError.ValidationErrors)
                {
                    message += string.Format
                            ("object-type: '{0}';    field-name: '{1}';    error message: '{2}'",
                            entityInError.Entry.Entity.ToString(),
                            validationError.PropertyName, validationError.ErrorMessage);
                }
            }
            return message;
        }
    }
}





























































        //private string ListFieldsInError(System.Data.Entity.Validation.DbEntityValidationException ex)
        //{
        //    string fieldNames = "";
        //    foreach (var entityInError in ex.EntityValidationErrors)
        //    {
        //        foreach (var validationError in entityInError.ValidationErrors)
        //        {
        //            fieldNames += "\n" + validationError.PropertyName + " -- ";
        //        }
        //    }
        //    return fieldNames;            
        //}
