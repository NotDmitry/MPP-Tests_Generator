using Tests_Generator.Core;

namespace Test_Generator.Tests
{
    [TestClass]
    public class CoreTests
    {
        [TestMethod]
        public void Test_Generate_Manually_Check_Class_Count_Single()
        {
            // Arrange
            string code = File.ReadAllText("C:/Dima/TestSrc/Class.cs");
            var generator = new Generator(code);
            int expected = 1;

            // Act
            var result = generator.GenerateInfo();
            int actual = result.Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Test_Generate_Manually_Check_Class_Count_2()
        {
            // Arrange
            string code = File.ReadAllText("C:/Dima/TestSrc/MyClasses.cs");
            var generator = new Generator(code);
            int expected = 2;

            // Act
            var result = generator.GenerateInfo();
            int actual = result.Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}