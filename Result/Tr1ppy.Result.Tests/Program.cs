// See https://aka.ms/new-console-template for more information
using Tr1ppy.Result.Types;
using Tr1ppy.Result.Types.Markers;

Console.WriteLine("Hello, World!");

//var resultFaultedStatic = Test.TestingFaultStatic();
//Console.WriteLine(resultFaultedStatic.IsSuccess);

//var resultSuccesedStatic = Test.TestingSuccessStatic();
//Console.WriteLine(resultSuccesedStatic.IsSuccess);

//var resultFaultedOperator = Test.TestFaultOperator();
//Console.WriteLine(resultFaultedOperator.IsSuccess);

//var resultSuccesedOperator = Test.TestSuccessOperator();
//Console.WriteLine(resultSuccesedOperator.IsSuccess);


//var a = Test.TestingFaultStatic();
//Console.WriteLine(a.GetFaultOrDefault());

public class ExtendedResult<TFault> : Result<TFault>
{
}
static class Test
{
    //public static ExtendedResult<string> TestingFaultStatic()
    //{
    //    return "string";
    //}

    //public static Result<string> TestingSuccessStatic()
    //{
    //    return Result.Success(322);
    //}

    //public static Result<string> TestFaultOperator()
    //{
    //    return "anotherString";
    //}

    //public static Result<string, int> TestSuccessOperator()
    //{
    //    return 322;
    //}
}