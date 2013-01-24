﻿using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace StructuralTyping.Tests
{
    public class MethodTests
    {
        public interface IInterfaceWithMethod
        {
            void SimpleMethod();
            string MethodWithResult();
            string MethodWithParameters(string a, int b);
        }

        [TestCase]
        public void Can_call_a_method()
        {
            var obj = A.New<IInterfaceWithMethod>(methods: new Method("MethodWithResult", new Func<string>(() => "HelloWorld")));

            Assert.AreEqual("HelloWorld", obj.MethodWithResult());
        }

        [TestCase]
        public void Can_call_method_with_parameters()
        {
            var obj = A.New<IInterfaceWithMethod>(methods: new Method("MethodWithParameters", new Func<string, int, string>((s, i) => s + i.ToString())));

            Assert.AreEqual("Answer=42", obj.MethodWithParameters("Answer=", 42));
        }

        [TestCase]
        public void Can_setup_two_methods()
        {
            var obj = A.New<IInterfaceWithMethod>(null,
                                                  new Method("MethodWithResult", new Func<string>(() => "Method1")),
                                                  new Method("MethodWithParameters", new Func<string, int, string>((s, i) => s + i.ToString())));

            Assert.AreEqual("Method1", obj.MethodWithResult());
            Assert.AreEqual("Answer=42", obj.MethodWithParameters("Answer=", 42));
        }

    }
}