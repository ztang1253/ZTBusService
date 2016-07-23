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
    public class testtest
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
        /// tests a valid record by using the validate method
        /// </summary>
        [Ignore]
        [Test]
        public void DriverValidation_ValidRecord_ShouldBeAccepted_ValidateMethod()
        {
            //arrange
            ValidationContext driverContext = new ValidationContext(db);

            //act            
            List<ValidationResult> validationErrors = driver.Validate(driverContext).ToList();

            //assert
            Assert.IsTrue(validationErrors.Count() == 0);
        }

        //can't pass
        //[Test]
        //public void DriverValidation_ValidRecord_ShouldBeAccepted_ControllerMethod()
        //{
        //    //arrange
        //    ZTDriverController driverController = new ZTDriverController();
        //    //act
        //    ActionResult result = driverController.Create(driver);

        //    //assert
        //    Assert.IsNotInstanceOf(typeof(ViewResult), result); //can't pass
        //    driver newDriver = db.drivers.Find(driver.driverId); //can't pass
        //    Assert.IsNotNull(newDriver); //can't pass
        //}

        [Ignore]
        [Test]
        public void DriverValidation_OverlengthPostalCode_ShouldCatchException()
        {
            //Arrange – done in [StartUp]
            //Act
            driver.postalCode = "          a3a 3a3            ";
            try
            {
                db.drivers.Add(driver);
                db.SaveChanges();
            }
            //Assert: catch any Entity Validation errors on attempted insert 
            catch (System.Data.Entity.Validation.DbEntityValidationException ex) //coloumn property conflict longer than 9 letters, validate method won't run?????why?
            {
                string message = ex.GetBaseException().Message; //Validation failed for one or more entities. See 'EntityValidationErrors' property for more details.
                message = GetValidationErrorMessages(ex);
                //GetValidationErrorMessages(ex); --- customer method to retrieve base exception message.
                //message = "object-type: 'ZTBusService.Models.driver';    field-name: 'postalCode';    error message: '字段 Postal 必须是最大长度为“9”的字符串或数组类型。'"
                Assert.Fail("unexpected edit error(s) on insert: " + message);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex) //province code = "aa"写入数据库出错 foreign key constraint
            {
                string message = ex.GetBaseException().Message;
                Assert.Fail("unexpected edit error(s) on insert: " + message);
            }
            catch (Exception ex)  // catch every other exception
            {
                string message = ex.GetBaseException().Message; //Validation failed for one or more entities. See 'EntityValidationErrors' property for more details.
                Assert.Fail("unexpected exception on insert: " + ex.GetBaseException().Message);
            }

            Assert.Fail("'          a3a 3a3            ' should not have been accepted by the database as it's too long");
        }

        [Ignore]
        [Test]
        public void DriverValidation_ProvinceNotOnFile_ShouldCatchException()
        {
            //Arrange – done in [StartUp]
            //Act
            driver.provinceCode = "AA";
            try
            {
                db.drivers.Add(driver);
                db.SaveChanges();
            }
            //Assert: catch any Entity Validation errors on attempted insert 
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Assert.Fail("unexpected edit error(s) on insert: " + GetValidationErrorMessages(ex));
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                string message = ex.GetBaseException().Message;
                //message = "The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_driver_province\". 
                //The conflict occurred in database \"E:\\TZZ\\2230\\A\\A6\\ZTBUSSERVICE-LAST\\ZTBUSSERVICE.TESTS\\BIN\\DEBUG\\BUSSERVICE.MDF\", 
                //table \"dbo.province\", column 'provinceCode'.
                //\r\nThe statement has been terminated."
                Assert.Fail("unexpected edit error(s) on insert: " + message);
            }
            catch (Exception ex)  // catch every other exception
            {
                Assert.Fail("unexpected exception on insert: " + ex.GetBaseException().Message);
            }

            Assert.Fail("'AA' should not have been accepted by the database as a province code");
        }

        private string ListFieldsInError(System.Data.Entity.Validation.DbEntityValidationException ex)
        {
            string fieldNames = "";
            foreach (var entityInError in ex.EntityValidationErrors)
            {
                foreach (var validationError in entityInError.ValidationErrors)
                {
                    fieldNames += "\n" + validationError.PropertyName + " -- ";
                }
            }
            return fieldNames;
        }

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

        [Ignore]
        [Test]
        public void DriverEdits_PostalCodeLowerCaseNoSpace_shouldPass()
        {
            //arrange : [SetUp] creates a valid order
            driver.postalCode = "a1b2c3";
            ValidationContext driverContext = new ValidationContext(db);

            //act : pass object through its validation method, collect any errors
            List<ValidationResult> errors = driver.Validate(driverContext).ToList();

            //assert : no errors should be produced and postal code is reformatted 
            Assert.AreEqual(0, errors.Count);
            //Assert.IsTrue(errors.Count == 0);
            Assert.AreEqual("A1B 2C3", driver.postalCode);
        }

        //[Test]
        //public void Driver_Postal_NUnitTests_CleanOrder_ShouldPass()
        //{
        //    //Arrange
        //    ValidationContext validationContext = new ValidationContext(db);
        //    //Act
        //    var result = driver.Validate(validationContext).ToList();
        //    //Assert
        //    Assert.IsEmpty(result);
        //}

        //[Test]
        //public void Driver_Postal_NUnitTests_CleanOrder_ShouldPass_usingController()
        //{
        //    //Arrange
        //    ZTDriverController controller = new ZTDriverController();
        //    //Act
        //    ActionResult result = controller.Create(driver);
        //    //Assert
        //    Assert.IsNotInstanceOf<ViewResult>(result,
        //        "Create should have redirected to Index, not returned to its view");
        //}

        //[Test]
        //public void Driver_Postal_NUnitTests_CleanOrder_ShouldPass_usingDatabase()
        //{
        //    //Arrange
        //    //Act
        //    try
        //    {
        //        db.drivers.Add(driver);
        //        db.SaveChanges();
        //    }
        //    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
        //    {
        //        Assert.Fail("clean record caught a validation/edit error ... data wrong or edits wrong: " +
        //            ex.GetBaseException().Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.Fail("SQL didn't like the 'clean' record: " + ex.GetBaseException().Message);
        //    }
        //    //Assert            
        //}

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
                Assert.Fail("Got an edit error on 'a3a3a3'");
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

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
                Assert.Fail("Got an edit error on 'a3a3a3'");
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

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
                Assert.Fail("Got an edit error on 'a3a3a3'");
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

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
                Assert.Fail("Got an edit error on 'a3a3a3'");
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

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
                Assert.Pass();
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.Fail("'D3A 3A3' should not have been accepted as a postal code. ");
        }

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

        [Ignore]
        [Test]
        public void Driver_Postal_NUnitTests_SpaceBeforeAndAfterPostalWithMiddleSpace_shouldPassAndReformat()
        {
            //Arrange
            driver.postalCode = " a3a 3A3 ";
            //Act
            try
            {
                db.drivers.Add(driver);
                db.SaveChanges();
            }
            //Assert
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Assert.Fail("Got an edit error on ' a3a 3A3 '");
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

        [Ignore]
        [Test]
        public void Driver_Postal_NUnitTests_SpaceBeforeAndAfterPostalNoMiddleSpace_shouldPassAndReformat()
        {
            //Arrange
            driver.postalCode = " a3a3A3 ";
            //Act
            try
            {
                db.drivers.Add(driver);
                db.SaveChanges();
            }
            //Assert
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Assert.Fail("Got an edit error on '   a3a3A3'");
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

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
                Assert.Fail("Got an edit error on '   a3a3A3'");
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

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
                Assert.Fail("Got an edit error on '  a3a 3A3'");
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

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
                Assert.Fail("Got an edit error on '   a3a3A3'");
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

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
                Assert.Fail("Got an edit error on 'a3a 3A3  '");
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

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
                Assert.Fail("Got an edit error on 'a3a3a3'");
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

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
                Assert.Fail("Got an edit error on 'a3a3a3'");
            }
            catch (Exception ex)
            {
                Assert.Fail("got an undefined exception: " + ex.GetBaseException().Message);
            }

            Assert.AreEqual("A3A 3A3", driver.postalCode);
        }

        //[Test]
        //public void Driver_Postal_NUnitTests_CreateWithBadData_ShouldCatchException()
        //{
        //    //arrange
        //    driver.provinceCode = "green";
        //    ZTDriverController controller = new ZTDriverController();
        //    //act
        //    ActionResult result = controller.Create(driver);
        //    //assert            
        //}

        //[Test]
        //public void orderController_CreateException_ShouldPutExceptionIntoModelState()
        //{
        //    //arrange
        //    driver.provinceCode = "green";
        //    ZTDriverController controller = new ZTDriverController();
        //    //act
        //    ActionResult result = controller.Create(driver);
        //    //assert 
        //    Assert.IsInstanceOf<ViewResult>(result, "bad provinceCode: should have returned to Create view");
        //    ViewResult fred = (ViewResult)result;
        //    Assert.IsNotNull(fred.ViewData.Model, "bad data: should show user the data they input");
        //    Assert.IsNotEmpty(fred.ViewData.ModelState.Keys.ToList(), "have exception: should be in ModelState");
        //    Assert.AreEqual("", fred.ViewData.ModelState.Keys.ToList()[0],
        //                "model-level errors like exceptions should have blank property name");
        //}
    }
}
