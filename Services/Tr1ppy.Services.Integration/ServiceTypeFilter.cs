using System.Data;
using System.Buffers;
using System.Numerics;
using System.Collections;
using System.ComponentModel;
using System.Xml.Serialization;
using System.ComponentModel.Design;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;

namespace Tr1ppy.Services.Integration;

public static class ServiceTypeFilter
{
    private static readonly HashSet<Type> ExcludedNonGenericInterfaces =
    [
        typeof(IDisposable),
        typeof(IAsyncDisposable),
        typeof(IComparable),
        typeof(IStructuralComparable),
        typeof(IStructuralEquatable),
        typeof(ICloneable),
        typeof(IEnumerable),
        typeof(ICollection),
        typeof(IList),
        typeof(IDictionary),
        typeof(IEnumerator),
        typeof(IFormattable),
        typeof(IConvertible),
        typeof(ISpanFormattable),
        typeof(ISerializable),
        typeof(IXmlSerializable),
        typeof(IObjectReference),
        typeof(INotifyPropertyChanged),
        typeof(INotifyPropertyChanging),
        typeof(ICustomTypeDescriptor),
        typeof(ITypedList),
        typeof(IBindingList),
        typeof(ICancelAddNew),
        typeof(IChangeTracking),
        typeof(IComponent),
        typeof(IContainer),
        typeof(IExtenderProvider),
        typeof(ISite),
        typeof(ISupportInitialize),
        typeof(ISupportInitializeNotification),
        typeof(IDesigner),
        typeof(IDesignerHost),
        typeof(IReferenceService),
        typeof(ITypeResolutionService),
        typeof(IDataReader),
        typeof(IDataRecord),
        typeof(IDbConnection),
        typeof(IDbCommand),
        typeof(IDbDataParameter),
        typeof(IDataAdapter),
        typeof(IDbTransaction),
        typeof(IDataParameter),
        typeof(IDataParameterCollection),
        typeof(System.Data.Common.DbProviderFactory),
        typeof(IAsyncResult),
        typeof(IJsonOnDeserialized),
        typeof(IJsonOnDeserializing),
        typeof(IJsonOnSerialized),
        typeof(IJsonOnSerializing),
        typeof(IThreadPoolWorkItem),
        typeof(ITimer),
        typeof(TaskFactory),
    ];

    private static readonly HashSet<Type> ExcludedGenericInterfaceDefinitions =
    [
        typeof(IComparable<>),
        typeof(IEquatable<>),
        typeof(IComparer<>),
        typeof(IEqualityComparer<>),
        typeof(IEnumerable<>),
        typeof(ICollection<>),
        typeof(IList<>),
        typeof(IDictionary<,>),
        typeof(IReadOnlyCollection<>),
        typeof(IReadOnlyList<>),
        typeof(IReadOnlyDictionary<,>),
        typeof(IEnumerator<>),
        typeof(ISet<>),
        typeof(IProducerConsumerCollection<>),
        typeof(IQueryable<>),
        typeof(IObservable<>),
        typeof(IObserver<>),
        typeof(IBufferWriter<>),
        typeof(IMemoryOwner<>),
        typeof(IAdditionOperators<,,>),
        typeof(IBitwiseOperators<,,>),
        typeof(IComparisonOperators<,,>),
        typeof(IDecrementOperators<>),
        typeof(IDivisionOperators<,,>),
        typeof(IEqualityOperators<,,>),
        typeof(IIncrementOperators<>),
        typeof(ILogarithmicFunctions<>),
        typeof(IMinMaxValue<>),
        typeof(IModulusOperators<,,>),
        typeof(IMultiplicativeIdentity<,>),
        typeof(IMultiplyOperators<,,>),
        typeof(INumber<>),
        typeof(INumberBase<>),
        typeof(IPowerFunctions<>),
        typeof(IRootFunctions<>),
        typeof(ISignedNumber<>),
        typeof(ISubtractionOperators<,,>),
        typeof(ITrigonometricFunctions<>),
        typeof(IUnaryPlusOperators<,>),
        typeof(IUnaryNegationOperators<,>),
        typeof(IBinaryNumber<>),
        typeof(IFloatingPoint<>),
        typeof(IFloatingPointIeee754<>),
        typeof(IParsable<>),
        typeof(IUnsignedNumber<>),
        typeof(Action<>),
        typeof(Func<>),
        typeof(Func<,>),
        typeof(Func<,,>),
        typeof(Func<,,,>),
        typeof(Func<,,,,>),
        typeof(ValueTask<>),
        typeof(Task<>),
        typeof(IAsyncStateMachine),
        typeof(IAsyncEnumerable<>),
        typeof(IAsyncEnumerator<>),
        typeof(ICriticalNotifyCompletion),
        typeof(INotifyCompletion),
    ];

    public static bool IsExcluded(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (!type.IsInterface)
            return false;
        
        if (ExcludedNonGenericInterfaces.Contains(type))
            return true;
        
        if (type.IsGenericType)
        {
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            if (ExcludedGenericInterfaceDefinitions.Contains(genericTypeDefinition))
                return true;
            
        }

        return false;
    }

    public static IEnumerable<Type> GetAbstractionsToRegister(Type serviceType)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        var typesToRegister = new HashSet<Type>();

        if (ServiceAttributeResolver.TryGetAbstractionFromAttribute(serviceType, out var abstractions))
        {
            foreach (var abstraction in abstractions)
                typesToRegister.Add(abstraction);
        }

        Type? currentTypeInHierarchy = serviceType;
        while (currentTypeInHierarchy != null && currentTypeInHierarchy != typeof(object))
        {
            if (!currentTypeInHierarchy.IsAbstract)
            {
                currentTypeInHierarchy = currentTypeInHierarchy.BaseType;
                continue;
            }

            if (!IsExcluded(currentTypeInHierarchy)) 
                typesToRegister.Add(currentTypeInHierarchy);

            var implementedInterfaces = currentTypeInHierarchy.GetInterfaces();
            foreach (var interfaceType in implementedInterfaces)
            {
                if (!IsExcluded(interfaceType))
                    typesToRegister.Add(interfaceType);
            }

            currentTypeInHierarchy = currentTypeInHierarchy.BaseType;
        }

        return typesToRegister.Distinct();
    }
}
