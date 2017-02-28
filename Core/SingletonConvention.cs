using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace Paccia
{
    public class SingletonConvention : IRegistrationConvention
    {
        public void ScanTypes(TypeSet types, Registry registry)
        {
            foreach (var type in types.AllTypes())
                registry.For(type).Singleton();
        }
    }
}