using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class Dependency
    {
        public Type type { get; }
        public LifeType lifeType { get; }
        public object Instance { get; set; }
        public Dependency(Type type, LifeType lifeType)
        {
            this.type = type;
            this.lifeType = lifeType;
        }
    }
}
