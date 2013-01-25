using NUnit.Framework;

namespace StructuralTyping.Tests
{
    public class BasicTests
    {
        [TestCase]
        public void New_object_should_be_correct_instance_of_interface()
        {
            var obj = A.New<IEmptyInterface>();

            Assert.IsInstanceOf<IEmptyInterface>(obj);
        }

        [TestCase]
        public void New_object_should_be_correct_instance_of_class()
        {
            var obj = A.New<EmptyClass>();

            Assert.IsInstanceOf<EmptyClass>(obj);
        }

        public interface IEmptyInterface
        {
        }

        public class EmptyClass
        {
        }
    }
}