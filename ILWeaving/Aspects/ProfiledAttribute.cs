using PostSharp.Aspects;
using PostSharp.Serialization;
using System.Diagnostics;

namespace AOPDemo.ILWeaving.Aspects
{
    [PSerializable]
    [LinesOfCodeAvoided(8)]
    public sealed class ProfiledAttribute : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            args.MethodExecutionTag = Stopwatch.StartNew();
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            var stp = (Stopwatch)args.MethodExecutionTag;

            Debug.WriteLine($"{DateTime.Now:HH:mm:ss:fff} {args.Method.DeclaringType.Name}.{args.Method.Name} - Elapsed: {stp.Elapsed}");
        }
    }
}
