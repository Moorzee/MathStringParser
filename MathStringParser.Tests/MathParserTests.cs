using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathStringParser.Tests
{
    [TestClass]
    public class MathParserTests
    {
        [TestMethod]
        public void GivenTheInputStringContainsNoNonNumericalCharactersThenTheResultIsTheInputValueInNumericalForm()
        {
            var result = new MathParser().ParseEntire("324");
            Assert.AreEqual(324, result);
        }

        [TestMethod]
        public void GivenTheInputStringIs2a2ThenTheOutputIsEqualToFour()
        {
            var result = new MathParser().ParseEntire("2a2");
            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void GivenTheInputStringIs3a2c4ThenTheOutputIsEqualToTwenty()
        {
            var result = new MathParser().ParseEntire("3a2c4");
            Assert.AreEqual(20, result);
        }

        [TestMethod]
        public void GivenTheInputStringIs32a2d2ThenTheOutputIsEqualToSeventeen()
        {
            var result = new MathParser().ParseEntire("32a2d2");
            Assert.AreEqual(17, result);
        }

        [TestMethod]
        public void GivenTheInputStringIs500a10b66c32ThenTheOutputIsEqualToFourteenThouTwoHundredAndEight()
        {
            var result = new MathParser().ParseEntire("500a10b66c32");
            Assert.AreEqual(14208, result);
        }

        [TestMethod]
        public void GivenTheInputStringIs3ae4c66fb32ThenTheOutputIsEqualToTwoHundredThirtyFive()
        {
            //3+(4*66)-32
            var result = new MathParser().ParseEntire("3ae4c66fb32");
            Assert.AreEqual(235, result);
        }

        [TestMethod]
        public void GivenTheInputStringIs3c4d2aee2a4c41fc4fThenTheOutputIsEqualToNineHundredNinety()
        {
            //3*4/2+((2+4*41)*4)
            var result = new MathParser().ParseEntire("3c4d2aee2a4c41fc4f");
            Assert.AreEqual(990, result);
        }
    }
}
