using System;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace BuildPlanExample
{
    public class FooBuildPlanPolicy : IBuildPlanPolicy
    {
        public void BuildUp<T>(ref T context) where T : IBuilderContext
        {
            // Resolve requested type
            var argument = context.BuildKey.Type.GetTypeInfo().GenericTypeArguments[0];
            var service = context.Container.Resolve(argument, context.BuildKey.Name);


            // Create Foo
            var typeToBuild = typeof(Foo<>).GetTypeInfo().MakeGenericType(argument);
            context.Existing = Activator.CreateInstance(typeToBuild, service);

            // Note: This example does not optimize implementation in any way for
            // simplicity.
        }
    }
}
