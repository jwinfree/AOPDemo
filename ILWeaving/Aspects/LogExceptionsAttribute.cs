using PostSharp.Aspects;
using PostSharp.Serialization;
using System.Diagnostics;
using System.Reflection;

namespace AOPDemo.ILWeaving.Aspects
{
    [PSerializable]
    [LinesOfCodeAvoided(8)]
    public sealed class LogExceptionsAndThrowAttribute : OnMethodBoundaryAspect
    {
        public override void OnException(MethodExecutionArgs args)
        {
            Debug.WriteLine(args.Exception);

            args.FlowBehavior = FlowBehavior.ThrowException;
        }
    }

    [PSerializable]
    [LinesOfCodeAvoided(8)]
    public sealed class LogExceptionsAndReturnNullAttribute : OnMethodBoundaryAspect
    {
        public override void OnException(MethodExecutionArgs args)
        {
            Debug.WriteLine(args.Exception);

            args.ReturnValue = null;
            args.FlowBehavior = FlowBehavior.Return;
        }
    }

    [PSerializable]
    [LinesOfCodeAvoided(8)]
    public sealed class LogExceptionsAndReturnNewInstanceOfReturnTypeAttribute : OnMethodBoundaryAspect
    {
        private Type _methodReturnType;

        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            _methodReturnType = ((MethodInfo)method).ReturnType;
        }

        public override void OnException(MethodExecutionArgs args)
        {
            Debug.WriteLine(args.Exception);

            args.ReturnValue = Activator.CreateInstance(_methodReturnType);
            args.FlowBehavior = FlowBehavior.Return;
        }
    }   
}
