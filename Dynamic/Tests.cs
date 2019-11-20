using System;
using System.Dynamic;
using FluentAssertions;
using Microsoft.CSharp.RuntimeBinder;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;

namespace Dynamic
{
    public class Tests
    {
        [Test]
        public void TypeIsInferredFromValueAtRuntime()
        {
            // Arrange
            dynamic d = "test";
            // Act
            // Assert
            Assert.AreEqual(typeof(string), d.GetType());
        }

        [Test]
        public void TypeShouldChangeDependingOnValueAssigned()
        {
            // Arrange 
            dynamic d = "test";
            // Pre-Assert
            Assert.AreEqual(typeof(string), d.GetType());

            // Act
            d = 100;
            // Assert
            Assert.AreEqual(typeof(int), d.GetType());
        }

        [Test]
        public void InvalidOperationForDerivedTypeShouldThrow()
        {
            // Arrange
            dynamic d = "test";
            Action act = () => d++;
            // Act
            // Assert
            act.Should().Throw<RuntimeBinderException>()
                .WithMessage("Operator '++' cannot be applied to operand of type 'string'");
        }

        public class TheThing
        {
        }

        [Test]
        public void ShouldThrowRuntimeExceptionWhenCallingMethodThatDoesNotExist()
        {
            // Arrange
            dynamic thing = new TheThing();
            // Act
            try
            {
                thing.SomeMethodThatDoesNotExist();
            }
            catch (Exception e)
            {
                // Assert
                e.Message.Should()
                    .Be("'Dynamic.Tests.TheThing' does not contain a definition for 'SomeMethodThatDoesNotExist'");
            }
        }

        [Test]
        public void ShouldAllowForExpandoObjects()
        {
            // Arrange
            var somethingUseful = new DoSomethingUsefulDataBuilder().Build();
            const int theNumber = 123;
            dynamic expando = new ExpandoObject();
            expando.SampleProperty = theNumber;
            expando.SampleMethod = (Action) (() => somethingUseful.Execute(expando.SampleProperty));

            // Act
            expando.SampleMethod();

            // Assert
            somethingUseful.Received(Quantity.Exactly(1)).Execute(theNumber);
        }
    }
}