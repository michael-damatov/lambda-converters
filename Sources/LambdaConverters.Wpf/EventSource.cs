using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

namespace LambdaConverters
{
    [EventSource(Name = "LambdaConverters")]
    internal sealed class EventSource : System.Diagnostics.Tracing.EventSource
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "The class must be public due to an ETW restriction.")]
        public static class Keywords
        {
            public const EventKeywords Converters = (EventKeywords)0x4;
            public const EventKeywords Selectors = (EventKeywords)0x8;
            public const EventKeywords Rules = (EventKeywords)0x10;
        }

        [JetBrains.Annotations.NotNull]
        public static readonly EventSource Log = new EventSource();

        EventSource() { }

        /// <summary>
        /// The {0} is null, conversion result is a value according to the specified error strategy ({1}).
        /// </summary>
        [Conditional("TRACE")]
        [Event(
            7,
            Version = 0,
            Message = "The {0} is null, conversion result is a value according to the specified error strategy ({1}).",
            Level = EventLevel.Warning,
            Keywords = Keywords.Converters,
            Opcode = EventOpcode.Info
#if NET46 || NETCOREAPP
            , Channel = EventChannel.Operational
#endif
        )]
        public void MissingConvertFunction(
            string callback,
            string errorStrategy,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
            =>
                WriteEvent(
                    7,
                    callback,
                    errorStrategy,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);

        /// <summary>
        /// The {0} is null, back conversion result is a value according to the specified error strategy ({1}).
        /// </summary>
        [Conditional("TRACE")]
        [Event(
            8,
            Version = 0,
            Message = "The {0} is null, back conversion result is a value according to the specified error strategy ({1}).",
            Level = EventLevel.Warning,
            Keywords = Keywords.Converters,
            Opcode = EventOpcode.Info
#if NET46 || NETCOREAPP
            , Channel = EventChannel.Operational
#endif
        )]
        public void MissingConvertBackFunction(
            string callback,
            string errorStrategy,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
            =>
                WriteEvent(
                    8,
                    callback,
                    errorStrategy,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);

        /// <summary>
        /// The requested target type ({0}) is not assignable from the specified output type ({1}), conversion result is a value according to the specified error strategy ({2}).
        /// </summary>
        [Conditional("TRACE")]
        [Event(
            9,
            Version = 0,
            Message = "The requested target type ({0}) is not assignable from the specified output type ({1}), conversion result is a value according to the specified error strategy ({2}).",
            Level = EventLevel.Warning,
            Keywords = Keywords.Converters,
            Opcode = EventOpcode.Info
#if NET46 || NETCOREAPP
            , Channel = EventChannel.Operational
#endif
        )]
        public void NonAssignableTargetType(
            string targetType,
            string outputType,
            string errorStrategy,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
            =>
                WriteEvent(
                    9,
                    targetType,
                    outputType,
                    errorStrategy,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);

        /// <summary>
        /// The requested target type ({0}) is not assignable from the specified input type ({1}), back conversion result is a value according to the specified error strategy ({2}).
        /// </summary>
        [Conditional("TRACE")]
        [Event(
            10,
            Version = 0,
            Message = "The requested target type ({0}) is not assignable from the specified input type ({1}), back conversion result is a value according to the specified error strategy ({2}).",
            Level = EventLevel.Warning,
            Keywords = Keywords.Converters,
            Opcode = EventOpcode.Info
#if NET46 || NETCOREAPP
            , Channel = EventChannel.Operational
#endif
        )]
        public void NonAssignableTargetTypeForBackConversion(
            string targetType,
            string inputType,
            string errorStrategy,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
            =>
                WriteEvent(
                    10,
                    targetType,
                    inputType,
                    errorStrategy,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);

        /// <summary>
        /// The requested target type ({0}) at the position {1} is not assignable from the specified input type ({2}), back conversion result is a value according to the specified error strategy ({3}).
        /// </summary>
        [Conditional("TRACE")]
        [Event(
            11,
            Version = 0,
            Message = "The requested target type ({0}) at the position {1} is not assignable from the specified input type ({2}), back conversion result is a value according to the specified error strategy ({3}).",
            Level = EventLevel.Warning,
            Keywords = Keywords.Converters,
            Opcode = EventOpcode.Info
#if NET46 || NETCOREAPP
            , Channel = EventChannel.Operational
#endif
        )]
        public void NonAssignableTargetTypeAtPositionForBackConversion(
            string targetType,
            int position,
            string inputType,
            string errorStrategy,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
            =>
                WriteEvent(
                    11,
                    targetType,
                    position,
                    inputType,
                    errorStrategy,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);

        /// <summary>
        /// The provided values are null, conversion result is a value according to the specified error strategy ({0}).
        /// </summary>
        [Conditional("TRACE")]
        [Event(
            12,
            Version = 0,
            Message = "The provided values are null, conversion result is a value according to the specified error strategy ({0}).",
            Level = EventLevel.Warning,
            Keywords = Keywords.Converters,
            Opcode = EventOpcode.Info
#if NET46 || NETCOREAPP
            , Channel = EventChannel.Operational
#endif
        )]
        public void NullValues(
            string errorStrategy,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
            =>
                WriteEvent(
                    12,
                    errorStrategy,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);

        /// <summary>
        /// The target type is not requested.
        /// </summary>
        [Conditional("TRACE")]
        [Event(
            13,
            Version = 0,
            Message = "The target type is not requested.",
            Level = EventLevel.Informational,
            Keywords = Keywords.Converters,
            Opcode = EventOpcode.Info
#if NET46 || NETCOREAPP
            , Channel = EventChannel.Operational
#endif
        )]
        public void NonRequestedTargetType(
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
            =>
                WriteEvent(
                    13,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);

        /// <summary>
        /// The target type at the position {0} is not requested.
        /// </summary>
        [Conditional("TRACE")]
        [Event(
            14,
            Version = 0,
            Message = "The target type at the position {0} is not requested.",
            Level = EventLevel.Informational,
            Keywords = Keywords.Converters,
            Opcode = EventOpcode.Info
#if NET46 || NETCOREAPP
            , Channel = EventChannel.Operational
#endif
        )]
        public void NonRequestedTargetTypeAtPosition(
            int position,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
            =>
                WriteEvent(
                    14,
                    position,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);

        /// <summary>
        /// A conversion parameter ({0}) is provided, use the appropriate converter, conversion result is a value according to the specified error strategy ({1}).
        /// </summary>
        [Conditional("TRACE")]
        [Event(
            15,
            Version = 0,
            Message = "A conversion parameter ({0}) is provided, use the appropriate converter, conversion result is a value according to the specified error strategy ({1}).",
            Level = EventLevel.Warning,
            Keywords = Keywords.Converters,
            Opcode = EventOpcode.Info
#if NET46 || NETCOREAPP
            , Channel = EventChannel.Operational
#endif
        )]
        public void ParameterInParameterlessConverter(
            string objectType,
            string errorStrategy,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
            =>
                WriteEvent(
                    15,
                    objectType,
                    errorStrategy,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);

        /// <summary>
        /// A conversion parameter ({0}) is provided, use the appropriate converter, back conversion result is a value according to the specified error strategy ({1}).
        /// </summary>
        [Conditional("TRACE")]
        [Event(
            16,
            Version = 0,
            Message = "A conversion parameter ({0}) is provided, use the appropriate converter, back conversion result is a value according to the specified error strategy ({1}).",
            Level = EventLevel.Warning,
            Keywords = Keywords.Converters,
            Opcode = EventOpcode.Info
#if NET46 || NETCOREAPP
            , Channel = EventChannel.Operational
#endif
        )]
        public void ParameterInParameterlessConverterForBackConversion(
            string objectType,
            string errorStrategy,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
            =>
                WriteEvent(
                    16,
                    objectType,
                    errorStrategy,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);

        /// <summary>
        /// The value ({0}) cannot be cast to the specified input type ({1}), conversion result is a value according to the specified error strategy ({2}).
        /// </summary>
        [Conditional("TRACE")]
        [Event(
            17,
            Version = 0,
            Message = "The value ({0}) cannot be cast to the specified input type ({1}), conversion result is a value according to the specified error strategy ({2}).",
            Level = EventLevel.Warning,
            Keywords = Keywords.Converters,
            Opcode = EventOpcode.Info
#if NET46 || NETCOREAPP
            , Channel = EventChannel.Operational
#endif
        )]
        public void UnableToCastToInputType(
            string objectType,
            string inputType,
            string errorStrategy,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
            =>
                WriteEvent(
                    17,
                    objectType,
                    inputType,
                    errorStrategy,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);

        /// <summary>
        /// The value ({0}) at the position {1} cannot be cast to the specified input type ({2}), conversion result is a value according to the specified error strategy ({3}).
        /// </summary>
        [Conditional("TRACE")]
        [Event(
            18,
            Version = 0,
            Message = "The value ({0}) at the position {1} cannot be cast to the specified input type ({2}), conversion result is a value according to the specified error strategy ({3}).",
            Level = EventLevel.Warning,
            Keywords = Keywords.Converters,
            Opcode = EventOpcode.Info
#if NET46 || NETCOREAPP
            , Channel = EventChannel.Operational
#endif
        )]
        public void UnableToCastAtPositionToInputType(
            string objectType,
            int position,
            string inputType,
            string errorStrategy,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
            =>
                WriteEvent(
                    18,
                    objectType,
                    position,
                    inputType,
                    errorStrategy,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);

        /// <summary>
        /// The value ({0}) cannot be cast to the specified output type ({1}), back conversion result is a value according to the specified error strategy ({2}).
        /// </summary>
        [Conditional("TRACE")]
        [Event(
            19,
            Version = 0,
            Message = "The value ({0}) cannot be cast to the specified output type ({1}), back conversion result is a value according to the specified error strategy ({2}).",
            Level = EventLevel.Warning,
            Keywords = Keywords.Converters,
            Opcode = EventOpcode.Info
#if NET46 || NETCOREAPP
            , Channel = EventChannel.Operational
#endif
        )]
        public void UnableToCastToOutputType(
            string objectType,
            string outputType,
            string errorStrategy,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
            =>
                WriteEvent(
                    19,
                    objectType,
                    outputType,
                    errorStrategy,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);

        /// <summary>
        /// The parameter value ({0}) cannot be cast to the specified parameter type ({1}), conversion result is a value according to the specified error strategy ({2}).
        /// </summary>
        [Conditional("TRACE")]
        [Event(
            20,
            Version = 0,
            Message = "The parameter value ({0}) cannot be cast to the specified parameter type ({1}), conversion result is a value according to the specified error strategy ({2}).",
            Level = EventLevel.Warning,
            Keywords = Keywords.Converters,
            Opcode = EventOpcode.Info
#if NET46 || NETCOREAPP
            , Channel = EventChannel.Operational
#endif
        )]
        public void UnableToCastToParameterType(
            string objectType,
            string parameter,
            string errorStrategy,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
            =>
                WriteEvent(
                    20,
                    objectType,
                    parameter,
                    errorStrategy,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);

        /// <summary>
        /// The parameter value ({0}) cannot be cast to the specified parameter type ({1}), back conversion result is a value according to the specified error strategy ({2}).
        /// </summary>
        [Conditional("TRACE")]
        [Event(
            21,
            Version = 0,
            Message = "The parameter value ({0}) cannot be cast to the specified parameter type ({1}), back conversion result is a value according to the specified error strategy ({2}).",
            Level = EventLevel.Warning,
            Keywords = Keywords.Converters,
            Opcode = EventOpcode.Info
#if NET46 || NETCOREAPP
            , Channel = EventChannel.Operational
#endif
        )]
        public void UnableToCastToParameterTypeForBackConversion(
            string objectType,
            string parameter,
            string errorStrategy,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
            =>
                WriteEvent(
                    21,
                    objectType,
                    parameter,
                    errorStrategy,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);

        /// <summary>
        /// The {0} is null, conversion result is a value according to the specified error strategy ({1}).
        /// </summary>
        [Conditional("TRACE")]
        [Event(
            22,
            Version = 0,
            Message = "The {0} is null, conversion result is a value according to the specified error strategy ({1}).",
            Level = EventLevel.Warning,
            Keywords = Keywords.Selectors,
            Opcode = EventOpcode.Info
#if NET46 || NETCOREAPP
            , Channel = EventChannel.Operational
#endif
        )]
        public void MissingSelectTemplateFunction(
            string callback,
            string errorStrategy,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
            =>
                WriteEvent(
                    22,
                    callback,
                    errorStrategy,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);

        /// <summary>
        /// The value ({0}) cannot be cast to the specified input type ({1}), conversion result is a value according to the specified error strategy ({2}).
        /// </summary>
        [Conditional("TRACE")]
        [Event(
            23,
            Version = 0,
            Message = "The value ({0}) cannot be cast to the specified input type ({1}), conversion result is a value according to the specified error strategy ({2}).",
            Level = EventLevel.Warning,
            Keywords = Keywords.Selectors,
            Opcode = EventOpcode.Info
#if NET46 || NETCOREAPP
            , Channel = EventChannel.Operational
#endif
        )]
        public void UnableToCastToItemType(
            string itemType,
            string inputType,
            string errorStrategy,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
            =>
                WriteEvent(
                    23,
                    itemType,
                    inputType,
                    errorStrategy,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);

        /// <summary>
        /// The {0} is null, conversion result is a value according to the specified error strategy ({1}).
        /// </summary>
        [Conditional("TRACE")]
        [Event(
            24,
            Version = 0,
            Message = "The {0} is null, conversion result is a value according to the specified error strategy ({1}).",
            Level = EventLevel.Warning,
            Keywords = Keywords.Rules,
            Opcode = EventOpcode.Info
#if NET46 || NETCOREAPP
            , Channel = EventChannel.Operational
#endif
        )]
        public void MissingRuleFunction(
            string callback,
            string errorStrategy,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
            =>
                WriteEvent(
                    24,
                    callback,
                    errorStrategy,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);

        /// <summary>
        /// The value ({0}) cannot be cast to the specified input type ({1}), conversion result is a value according to the specified error strategy ({2}).
        /// </summary>
        [Conditional("TRACE")]
        [Event(
            25,
            Version = 0,
            Message = "The value ({0}) cannot be cast to the specified input type ({1}), conversion result is a value according to the specified error strategy ({2}).",
            Level = EventLevel.Warning,
            Keywords = Keywords.Rules,
            Opcode = EventOpcode.Info
#if NET46 || NETCOREAPP
            , Channel = EventChannel.Operational
#endif
        )]
        public void UnableToCastToRuleInputType(
            string itemType,
            string inputType,
            string errorStrategy,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
            =>
                WriteEvent(
                    25,
                    itemType,
                    inputType,
                    errorStrategy,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);
    }
}
