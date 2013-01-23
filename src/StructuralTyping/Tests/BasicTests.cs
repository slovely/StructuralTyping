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

        [TestCase]
        public void New_object_should_be_have_values_of_supplied_anonymous_object()
        {
            var obj = A.New<ISimpleInterface>(new {Name = "Test", Age = 42});

            Assert.AreEqual("Test", obj.Name);
            Assert.AreEqual(42, obj.Age);
        }

        public interface ISimpleInterface
        {
            string Name { get; set; }
            int Age { get; set; }
        }

        public interface IEmptyInterface
        {
        }
    }
}