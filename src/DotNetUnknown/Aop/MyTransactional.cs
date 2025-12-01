using System.Reflection;
using Castle.DynamicProxy;

namespace DotNetUnknown.Aop;

[AttributeUsage(AttributeTargets.Method)]
public class MyTransactional : Attribute
{
}

public class MyTransactionalInterceptor(MyTransactionSupport myTransactionSupport) : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        var method = invocation.Method;
        var myTransactional = method.GetCustomAttribute<MyTransactional>();

        if (myTransactional == null) invocation.Proceed();
        try
        {
            myTransactionSupport.BeginTransaction(method);
            invocation.Proceed();
            myTransactionSupport.CommitTransaction(method);
        }
        catch (System.Exception)
        {
            myTransactionSupport.RollbackTransaction(method);
            throw;
        }
    }
}

public class MyTransactionSupport
{
    public virtual void BeginTransaction(MethodInfo method)
    {
    }

    public virtual void CommitTransaction(MethodInfo method)
    {
    }

    public virtual void RollbackTransaction(MethodInfo method)
    {
    }
}