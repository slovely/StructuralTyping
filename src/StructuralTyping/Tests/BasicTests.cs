using NUnit.Framework;

namespace StructuralTyping.Tests
{
    public class BasicTests
    {
        [TestCase]
        public void New_object_should_be_correct_instance()
        {
            var obj = A.New<IEmptyInterface>();

            Assert.IsInstanceOf<IEmptyInterface>(obj);
        }

        public interface IEmptyInterface
        {
        }
    }
}