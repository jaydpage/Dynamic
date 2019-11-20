using System;
using System.Dynamic;
using FluentAssertions;
using Microsoft.CSharp.RuntimeBinder;
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

        public class TheThing { }

        [Test]
        public void ShouldAllowReAssignmentOfComplexTypes()
        {
            // Arrange
            dynamic d = new DoSomethingUseful();
            // Pre-Assert
            Assert.AreEqual(typeof(DoSomethingUseful),d.GetType());
            // Act
            d = new TheThing();
            // Assert
            Assert.AreEqual(typeof(TheThing), d.GetType());
        }

        [Test]
        public void InvalidOperationOnDerivedTypeShouldThrow()
        {
            // Arrange
            dynamic d = "test";
            Action act = () => d++;
            // Act
            // Assert
            act.Should().Throw<RuntimeBinderException>()
                .WithMessage("Operator '++' cannot be applied to operand of type 'string'");
        }

        [Test]
        public void ShouldAllowImplicitConversion()
        {
            // Arrange
            dynamic d = DateTime.Now;
            // Act
            DateTime t = d;
            d += new TimeSpan(0, 2, 0, 0);
            // Assert
            Assert.AreEqual(t.AddHours(2), d);
        }

        [Test]
        public void ShouldThrowRuntimeExceptionWhenCallingMethodThatDoesNotExist()
        {
            // Arrange
            dynamic something = new DoSomethingUseful();
            // Act
            try
            {
                something.SomeMethodThatDoesNotExist();
            }
            catch (Exception e)
            {
                // Assert
                e.Message.Should()
                    .Be("'Dynamic.DoSomethingUseful' does not contain a definition for 'SomeMethodThatDoesNotExist'");
            }
        }

        [Test]
        public void ShouldAllowDynamicAsParameter()
        {
            // Arrange
            var value = 5;
            var something = new DoSomethingUseful();
            // Act
            var result = something.Execute(value);
            // Assert
            result.Should().Be(value);
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