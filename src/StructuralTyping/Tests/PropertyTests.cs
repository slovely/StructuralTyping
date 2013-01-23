using NUnit.Framework;

namespace StructuralTyping.Tests
{
    public class PropertyTests
    {
        [TestCase]
        public void New_object_should_be_have_values_of_supplied_anonymous_object()
        {
            var obj = A.New<IInterfaceWithProperties>(new { Name = "Test", Age = 42 });

            Assert.AreEqual("Test", obj.Name);
            Assert.AreEqual(42, obj.Age);
        }

        [TestCase]
        public void Should_return_default_value_for_unsupplied_value_properties()
        {
            var obj = A.New<IInterfaceWithProperties>(new {Name = "Simon"});
            Assert.AreEqual("Simon", obj.Name);
            Assert.AreEqual(default(int), obj.Age);
        }

        [TestCase]
        public void Should_return_null_for_unsupplied_reference_properties()
        {
            var obj = A.New<IInterfaceWithReferenceProperties>();
            Assert.IsNull(obj.OtherObject);
        }

        [TestCase]
        public void Should_be_able_to_update_interface_properties()
        {
            var obj = A.New<IInterfaceWithProperties>(new {Name = "Dave"});
            obj.Name = "Lucy";
            Assert.AreEqual("Lucy", obj.Name);
        }


        public interface IInterfaceWithProperties
        {
            string Name { get; set; }
            int Age { get; set; }
        }

        public interface IInterfaceWithReferenceProperties
        {
            IInterfaceWithProperties OtherObject { get; set; }
        }
 
    }
}