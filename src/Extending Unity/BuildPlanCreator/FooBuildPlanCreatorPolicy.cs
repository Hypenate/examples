﻿using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Storage;
using Unity.Strategies.Legacy;

namespace BuildPlanCreatorExample
{
    public class FooBuildPlanCreatorPolicy : IBuildPlanCreatorPolicy
    {
        private readonly IPolicyList _policies;

        private readonly MethodInfo _factoryMethod = 
            typeof(FooBuildPlanCreatorPolicy).GetTypeInfo().GetDeclaredMethod(nameof(FactoryMethod));

        /// <summary>
        /// Factory plan to build [I]Foo type
        /// </summary>
        /// <param name="policies">Container policy list to store created plans</param>
        public FooBuildPlanCreatorPolicy(IPolicyList policies)
        {
            _policies = policies;
        }

        public IBuildPlanPolicy CreatePlan<T>(ref T context, INamedType buildKey) where T : IBuilderContext
        {
            // Make generic factory method for the type
            var typeToBuild = buildKey.Type.GetTypeInfo().GenericTypeArguments;
            var factoryMethod =
                _factoryMethod.MakeGenericMethod(typeToBuild)
                              .CreateDelegate(typeof(DynamicBuildPlanMethod));
            // Create policy
            var creatorPlan = new DynamicMethodBuildPlan((DynamicBuildPlanMethod)factoryMethod);

            // Register BuildPlan policy with the container to optimize performance
            _policies.Set(buildKey.Type, string.Empty, typeof(IBuildPlanPolicy), creatorPlan);

            return creatorPlan;
        }

        private static void FactoryMethod<TResult>(IBuilderContext context)
        {
            // Resolve requested type
            var service = (TResult)context.Container.Resolve(typeof(TResult), context.BuildKey.Name);

            // Create Foo
            context.Existing = new Foo<TResult>(service);
        }
    }
}
