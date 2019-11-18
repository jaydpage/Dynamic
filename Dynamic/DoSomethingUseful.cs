using NSubstitute;

namespace Dynamic
{
    public class DoSomethingUseful : IDoSomethingUseful
    {
        public void Execute(int value) { }
    }

    public interface IDoSomethingUseful
    {
        void Execute(int value);
    }

    public class DoSomethingUsefulDataBuilder
    {
        public IDoSomethingUseful Build()
        {
            return Substitute.For<IDoSomethingUseful>();
        }
    }
}