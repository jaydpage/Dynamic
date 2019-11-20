using NSubstitute;

namespace Dynamic
{
    public class DoSomethingUseful : IDoSomethingUseful
    {
        public object Execute(dynamic value)
        {
            return value;
        }
    }

    public interface IDoSomethingUseful
    {
        object Execute(dynamic value);
    }

    public class DoSomethingUsefulDataBuilder
    {
        public IDoSomethingUseful Build()
        {
            return Substitute.For<IDoSomethingUseful>();
        }
    }
}