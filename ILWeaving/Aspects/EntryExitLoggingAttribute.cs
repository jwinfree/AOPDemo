using PostSharp.Aspects;
using PostSharp.Serialization;
using System.Diagnostics;

namespace AOPDemo.ILWeaving.Aspects
{
    [PSerializable]
    [LinesOfCodeAvoided(8)]
    public class EntryExitLoggingAttribute : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            Debug.WriteLine($"Calling method: {args.Method.Name}");
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            Debug.WriteLine($"{args.Method.DeclaringType.Name}.{args.Method.Name} executed successfully");
        }

        public override void OnException(MethodExecutionArgs args)
        {
            Debug.WriteLine($"{args.Method.DeclaringType.Name}.{args.Method.Name} threw an exception: {args.Exception.Message}");
        }
    }
}
