using System;

namespace FifthGroup_front.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class WithoutAuthenticationAttribute : Attribute//
    {
    
    }
}
