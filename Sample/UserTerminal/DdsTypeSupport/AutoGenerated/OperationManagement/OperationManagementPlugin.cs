/*
WARNING: THIS FILE IS AUTO-GENERATED. DO NOT MODIFY.

This file was generated from OperationManagement.idl
using RTI Code Generator (rtiddsgen) version 4.3.0.
The rtiddsgen tool is part of the RTI Connext DDS distribution.
For more information, type 'rtiddsgen -help' at a command shell
or consult the Code Generator User's Manual.
*/

using System;
using System.Runtime.InteropServices;
using Omg.Types;
using Omg.Types.Dynamic;
using Rti.Types;
using Rti.Dds.Core;
using Rti.Types.Dynamic;
using Rti.Dds.NativeInterface.TypePlugin;

namespace Common
{

    namespace Implementation
    {

        public struct ControlParamUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::Common.ControlParam>
        {

            private NativeWstring fieldName;
            private NativeWstring fixedValue;

            public void Destroy(bool optionalsOnly)
            {
                if (optionalsOnly)
                {
                    return;
                }
                fieldName.Destroy();
                fixedValue.Destroy();
            }

            public void FromNative(global::Common.ControlParam sample, bool keysOnly = false)
            {

                sample.fieldName = fieldName.FromNative();
                sample.fixedValue = fixedValue.FromNative();
            }

            public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
            {
                fieldName.Initialize(size: ((int) 255), allocateMemory: allocateMemory);
                fixedValue.Initialize(size: ((int) 255), allocateMemory: allocateMemory);
            }

            public void ToNative(global::Common.ControlParam sample, bool keysOnly = false)
            {
                fieldName.ToNative(sample.fieldName, ((int) 255));
                fixedValue.ToNative(sample.fixedValue, ((int) 255));
            }
        }

        internal class ControlParamPlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::Common.ControlParam, ControlParamUnmanaged>
        {

            internal ControlParamPlugin() : base("global::Common.ControlParam", isKeyed: false, CreateDynamicType(isPublic: false))
            {
            }

            public static DynamicType CreateDynamicType(bool isPublic = true)
            {
                var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
                var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

                // ControlParam struct
                var ControlParamStructMembers = new StructMember[]
                {
                    new StructMember("fieldName", dtf.CreateWideString(((int) 255)), id: 0),
                    new StructMember("fixedValue", dtf.CreateWideString(((int) 255)), id: 1)
                };

                DynamicType result = tsf.CreateTypeWithAccessInfo<ControlParamUnmanaged>(
                    dtf.BuildStruct()
                    .WithExtensibility(ExtensibilityKind.Extensible)
                    .WithName("Common::ControlParam")
                    .AddMembers(ControlParamStructMembers));

                return result;
            }
        }
    }
    public class ControlParamSupport : Rti.Dds.Topics.TypeSupport<global::Common.ControlParam>
    {
        public ControlParamSupport() : base(
            new Implementation.ControlParamPlugin(),
            new Lazy<DynamicType>(() =>Implementation.ControlParamPlugin.CreateDynamicType(isPublic: true)))
        {
        }

        public static ControlParamSupport Instance { get; } =
        ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<ControlParamSupport, global::Common.ControlParam>();

    }

    namespace Implementation
    {

        public struct ControlSpecUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::Common.ControlSpec>
        {

            private NativeWstring id;
            private NativeWstring name;
            private NativeWstring cdm;
            private int numsOfParams;
            private NativeSeq @params;

            public void Destroy(bool optionalsOnly)
            {
                if (optionalsOnly)
                {
                    return;
                }
                id.Destroy();
                name.Destroy();
                cdm.Destroy();
                @params.Destroy<global::Common.ControlParam, global::Common.Implementation.ControlParamUnmanaged>(optionalsOnly);
            }

            public void FromNative(global::Common.ControlSpec sample, bool keysOnly = false)
            {

                sample.id = id.FromNative();
                sample.name = name.FromNative();
                sample.cdm = cdm.FromNative();
                sample.numsOfParams = numsOfParams;
                @params.FromNative<global::Common.ControlParam, global::Common.Implementation.ControlParamUnmanaged>(sample.@params);
            }

            public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
            {
                id.Initialize(size: ((int) 40), allocateMemory: allocateMemory);
                name.Initialize(size: ((int) 100), allocateMemory: allocateMemory);
                cdm.Initialize(size: ((int) 100), allocateMemory: allocateMemory);
                numsOfParams = (int) (0);
                @params.Initialize<global::Common.ControlParam , global::Common.Implementation.ControlParamUnmanaged >(max: ((int)100), absoluteMax: ((int)100), allocateMemory: allocateMemory);
            }

            public void ToNative(global::Common.ControlSpec sample, bool keysOnly = false)
            {
                id.ToNative(sample.id, ((int) 40));
                name.ToNative(sample.name, ((int) 100));
                cdm.ToNative(sample.cdm, ((int) 100));
                numsOfParams = sample.numsOfParams;
                @params.ToNative<global::Common.ControlParam, global::Common.Implementation.ControlParamUnmanaged>(sample.@params);
            }
        }

        internal class ControlSpecPlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::Common.ControlSpec, ControlSpecUnmanaged>
        {

            internal ControlSpecPlugin() : base("global::Common.ControlSpec", isKeyed: false, CreateDynamicType(isPublic: false))
            {
            }

            public static DynamicType CreateDynamicType(bool isPublic = true)
            {
                var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
                var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

                // ControlSpec struct
                var ControlSpecStructMembers = new StructMember[]
                {
                    new StructMember("id", dtf.CreateWideString(((int) 40)), id: 0),
                    new StructMember("name", dtf.CreateWideString(((int) 100)), id: 1),
                    new StructMember("cdm", dtf.CreateWideString(((int) 100)), id: 2),
                    new StructMember("numsOfParams", dtf.GetPrimitiveType<int>(), id: 3),
                    new StructMember("params", tsf.CreateSequenceWithAccessInfo(dtf, global::Common.ControlParamSupport.Instance.GetDynamicTypeInternal(isPublic), ((int)100)), id: 4)
                };

                DynamicType result = tsf.CreateTypeWithAccessInfo<ControlSpecUnmanaged>(
                    dtf.BuildStruct()
                    .WithExtensibility(ExtensibilityKind.Extensible)
                    .WithName("Common::ControlSpec")
                    .AddMembers(ControlSpecStructMembers));

                return result;
            }
        }
    }
    public class ControlSpecSupport : Rti.Dds.Topics.TypeSupport<global::Common.ControlSpec>
    {
        public ControlSpecSupport() : base(
            new Implementation.ControlSpecPlugin(),
            new Lazy<DynamicType>(() =>Implementation.ControlSpecPlugin.CreateDynamicType(isPublic: true)))
        {
        }

        public static ControlSpecSupport Instance { get; } =
        ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<ControlSpecSupport, global::Common.ControlSpec>();

    }

    namespace Implementation
    {
        internal class OperationStatePlugin : Rti.Dds.NativeInterface.TypePlugin.EnumTypePlugin
        {
            public OperationStatePlugin() : base(CreateDynamicType(isPublic: false))
            {
            }

            internal static DynamicType CreateDynamicType(bool isPublic = true)
            {
                var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
                return dtf.BuildEnum()
                .WithName("Common::OperationState")
                .AddMember(new EnumMember("Finished", 0))
                .AddMember(new EnumMember("Processing", 1))
                .AddMember(new EnumMember("Failed", 2))
                .WithExtensibility(ExtensibilityKind.Extensible)
                .Create();
            }
        }
    }

    public class OperationStateSupport : Rti.Dds.Topics.TypeSupport<global::Common.OperationState>
    {
        public OperationStateSupport() : base(
            new Implementation.OperationStatePlugin(),
            new Lazy<DynamicType>(() =>Implementation.OperationStatePlugin.CreateDynamicType(isPublic: true)))
        {
        }

        public static OperationStateSupport Instance { get; } =
        ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<OperationStateSupport, global::Common.OperationState>();

    }

} // namespace Common

namespace Messages
{

    namespace OperationManagement
    {

        namespace Implementation
        {

            public struct ControlSpecListRequestUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::Messages.OperationManagement.ControlSpecListRequest>
            {

                private NativeWstring targetId;
                private NativeWstring targetName;

                public void Destroy(bool optionalsOnly)
                {
                    if (optionalsOnly)
                    {
                        return;
                    }
                    targetId.Destroy();
                    targetName.Destroy();
                }

                public void FromNative(global::Messages.OperationManagement.ControlSpecListRequest sample, bool keysOnly = false)
                {

                    sample.targetId = targetId.FromNative();
                    if (keysOnly)
                    {
                        return;
                    }
                    sample.targetName = targetName.FromNative();
                }

                public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
                {
                    targetId.Initialize(size: ((int) 40), allocateMemory: allocateMemory);
                    targetName.Initialize(size: ((int) 40), allocateMemory: allocateMemory);
                }

                public void ToNative(global::Messages.OperationManagement.ControlSpecListRequest sample, bool keysOnly = false)
                {
                    targetId.ToNative(sample.targetId, ((int) 40));
                    if (keysOnly)
                    {
                        return;
                    }
                    targetName.ToNative(sample.targetName, ((int) 40));
                }
            }

            internal class ControlSpecListRequestPlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::Messages.OperationManagement.ControlSpecListRequest, ControlSpecListRequestUnmanaged>
            {

                internal ControlSpecListRequestPlugin() : base("global::Messages.OperationManagement.ControlSpecListRequest", isKeyed: true, CreateDynamicType(isPublic: false))
                {
                }

                public static DynamicType CreateDynamicType(bool isPublic = true)
                {
                    var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
                    var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

                    // ControlSpecListRequest struct
                    var ControlSpecListRequestStructMembers = new StructMember[]
                    {
                        new StructMember("targetId", dtf.CreateWideString(((int) 40)), isKey: true, id: 0),
                        new StructMember("targetName", dtf.CreateWideString(((int) 40)), id: 1)
                    };

                    DynamicType result = tsf.CreateTypeWithAccessInfo<ControlSpecListRequestUnmanaged>(
                        dtf.BuildStruct()
                        .WithExtensibility(ExtensibilityKind.Extensible)
                        .WithName("Messages::OperationManagement::ControlSpecListRequest")
                        .AddMembers(ControlSpecListRequestStructMembers));

                    return result;
                }
            }
        }
        public class ControlSpecListRequestSupport : Rti.Dds.Topics.TypeSupport<global::Messages.OperationManagement.ControlSpecListRequest>
        {
            public ControlSpecListRequestSupport() : base(
                new Implementation.ControlSpecListRequestPlugin(),
                new Lazy<DynamicType>(() =>Implementation.ControlSpecListRequestPlugin.CreateDynamicType(isPublic: true)))
            {
            }

            public static ControlSpecListRequestSupport Instance { get; } =
            ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<ControlSpecListRequestSupport, global::Messages.OperationManagement.ControlSpecListRequest>();

        }

        namespace Implementation
        {

            public struct ControlSpecListReplyUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::Messages.OperationManagement.ControlSpecListReply>
            {

                private NativeWstring targetId;
                private NativeWstring targetName;
                private int numsOfControlSpecs;
                private NativeSeq controlSpecList;

                public void Destroy(bool optionalsOnly)
                {
                    if (optionalsOnly)
                    {
                        return;
                    }
                    targetId.Destroy();
                    targetName.Destroy();
                    controlSpecList.Destroy<global::Common.ControlSpec, global::Common.Implementation.ControlSpecUnmanaged>(optionalsOnly);
                }

                public void FromNative(global::Messages.OperationManagement.ControlSpecListReply sample, bool keysOnly = false)
                {

                    sample.targetId = targetId.FromNative();
                    if (keysOnly)
                    {
                        return;
                    }
                    sample.targetName = targetName.FromNative();
                    sample.numsOfControlSpecs = numsOfControlSpecs;
                    controlSpecList.FromNative<global::Common.ControlSpec, global::Common.Implementation.ControlSpecUnmanaged>(sample.controlSpecList);
                }

                public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
                {
                    targetId.Initialize(size: ((int) 40), allocateMemory: allocateMemory);
                    targetName.Initialize(size: ((int) 40), allocateMemory: allocateMemory);
                    numsOfControlSpecs = (int) (0);
                    controlSpecList.Initialize<global::Common.ControlSpec , global::Common.Implementation.ControlSpecUnmanaged >(max: ((int)100), absoluteMax: ((int)100), allocateMemory: allocateMemory);
                }

                public void ToNative(global::Messages.OperationManagement.ControlSpecListReply sample, bool keysOnly = false)
                {
                    targetId.ToNative(sample.targetId, ((int) 40));
                    if (keysOnly)
                    {
                        return;
                    }
                    targetName.ToNative(sample.targetName, ((int) 40));
                    numsOfControlSpecs = sample.numsOfControlSpecs;
                    controlSpecList.ToNative<global::Common.ControlSpec, global::Common.Implementation.ControlSpecUnmanaged>(sample.controlSpecList);
                }
            }

            internal class ControlSpecListReplyPlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::Messages.OperationManagement.ControlSpecListReply, ControlSpecListReplyUnmanaged>
            {

                internal ControlSpecListReplyPlugin() : base("global::Messages.OperationManagement.ControlSpecListReply", isKeyed: true, CreateDynamicType(isPublic: false))
                {
                }

                public static DynamicType CreateDynamicType(bool isPublic = true)
                {
                    var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
                    var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

                    // ControlSpecListReply struct
                    var ControlSpecListReplyStructMembers = new StructMember[]
                    {
                        new StructMember("targetId", dtf.CreateWideString(((int) 40)), isKey: true, id: 0),
                        new StructMember("targetName", dtf.CreateWideString(((int) 40)), id: 1),
                        new StructMember("numsOfControlSpecs", dtf.GetPrimitiveType<int>(), id: 2),
                        new StructMember("controlSpecList", tsf.CreateSequenceWithAccessInfo(dtf, global::Common.ControlSpecSupport.Instance.GetDynamicTypeInternal(isPublic), ((int)100)), id: 3)
                    };

                    DynamicType result = tsf.CreateTypeWithAccessInfo<ControlSpecListReplyUnmanaged>(
                        dtf.BuildStruct()
                        .WithExtensibility(ExtensibilityKind.Extensible)
                        .WithName("Messages::OperationManagement::ControlSpecListReply")
                        .AddMembers(ControlSpecListReplyStructMembers));

                    return result;
                }
            }
        }
        public class ControlSpecListReplySupport : Rti.Dds.Topics.TypeSupport<global::Messages.OperationManagement.ControlSpecListReply>
        {
            public ControlSpecListReplySupport() : base(
                new Implementation.ControlSpecListReplyPlugin(),
                new Lazy<DynamicType>(() =>Implementation.ControlSpecListReplyPlugin.CreateDynamicType(isPublic: true)))
            {
            }

            public static ControlSpecListReplySupport Instance { get; } =
            ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<ControlSpecListReplySupport, global::Messages.OperationManagement.ControlSpecListReply>();

        }

        namespace Implementation
        {

            public struct ControlExecutionRequestUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::Messages.OperationManagement.ControlExecutionRequest>
            {

                private NativeWstring targetId;
                private global::Common.Implementation.ControlSpecUnmanaged controlSpec;

                public void Destroy(bool optionalsOnly)
                {
                    if (optionalsOnly)
                    {
                        return;
                    }
                    targetId.Destroy();
                    controlSpec.Destroy(optionalsOnly);
                }

                public void FromNative(global::Messages.OperationManagement.ControlExecutionRequest sample, bool keysOnly = false)
                {

                    sample.targetId = targetId.FromNative();
                    if (keysOnly)
                    {
                        return;
                    }
                    controlSpec.FromNative(sample.controlSpec, keysOnly: keysOnly);
                }

                public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
                {
                    targetId.Initialize(size: ((int) 255), allocateMemory: allocateMemory);
                    controlSpec.Initialize(allocatePointers, allocateMemory);
                }

                public void ToNative(global::Messages.OperationManagement.ControlExecutionRequest sample, bool keysOnly = false)
                {
                    targetId.ToNative(sample.targetId, ((int) 255));
                    if (keysOnly)
                    {
                        return;
                    }
                    controlSpec.ToNative(sample.controlSpec, keysOnly: keysOnly);
                }
            }

            internal class ControlExecutionRequestPlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::Messages.OperationManagement.ControlExecutionRequest, ControlExecutionRequestUnmanaged>
            {

                internal ControlExecutionRequestPlugin() : base("global::Messages.OperationManagement.ControlExecutionRequest", isKeyed: true, CreateDynamicType(isPublic: false))
                {
                }

                public static DynamicType CreateDynamicType(bool isPublic = true)
                {
                    var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
                    var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

                    // ControlExecutionRequest struct
                    var ControlExecutionRequestStructMembers = new StructMember[]
                    {
                        new StructMember("targetId", dtf.CreateWideString(((int) 255)), isKey: true, id: 0),
                        new StructMember("controlSpec", global::Common.ControlSpecSupport.Instance.GetDynamicTypeInternal(isPublic), id: 1)
                    };

                    DynamicType result = tsf.CreateTypeWithAccessInfo<ControlExecutionRequestUnmanaged>(
                        dtf.BuildStruct()
                        .WithExtensibility(ExtensibilityKind.Extensible)
                        .WithName("Messages::OperationManagement::ControlExecutionRequest")
                        .AddMembers(ControlExecutionRequestStructMembers));

                    return result;
                }
            }
        }
        public class ControlExecutionRequestSupport : Rti.Dds.Topics.TypeSupport<global::Messages.OperationManagement.ControlExecutionRequest>
        {
            public ControlExecutionRequestSupport() : base(
                new Implementation.ControlExecutionRequestPlugin(),
                new Lazy<DynamicType>(() =>Implementation.ControlExecutionRequestPlugin.CreateDynamicType(isPublic: true)))
            {
            }

            public static ControlExecutionRequestSupport Instance { get; } =
            ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<ControlExecutionRequestSupport, global::Messages.OperationManagement.ControlExecutionRequest>();

        }

        namespace Implementation
        {

            public struct ControlExecutionReplyUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::Messages.OperationManagement.ControlExecutionReply>
            {

                private NativeWstring targetId;
                private global::Common.OperationState executionReport;

                public void Destroy(bool optionalsOnly)
                {
                    if (optionalsOnly)
                    {
                        return;
                    }
                    targetId.Destroy();
                }

                public void FromNative(global::Messages.OperationManagement.ControlExecutionReply sample, bool keysOnly = false)
                {

                    sample.targetId = targetId.FromNative();
                    if (keysOnly)
                    {
                        return;
                    }
                    sample.executionReport = executionReport;
                }

                public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
                {
                    targetId.Initialize(size: ((int) 255), allocateMemory: allocateMemory);
                    executionReport = (global::Common.OperationState) (0);
                }

                public void ToNative(global::Messages.OperationManagement.ControlExecutionReply sample, bool keysOnly = false)
                {
                    targetId.ToNative(sample.targetId, ((int) 255));
                    if (keysOnly)
                    {
                        return;
                    }
                    executionReport = sample.executionReport;
                }
            }

            internal class ControlExecutionReplyPlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::Messages.OperationManagement.ControlExecutionReply, ControlExecutionReplyUnmanaged>
            {

                internal ControlExecutionReplyPlugin() : base("global::Messages.OperationManagement.ControlExecutionReply", isKeyed: true, CreateDynamicType(isPublic: false))
                {
                }

                public static DynamicType CreateDynamicType(bool isPublic = true)
                {
                    var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
                    var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

                    // ControlExecutionReply struct
                    var ControlExecutionReplyStructMembers = new StructMember[]
                    {
                        new StructMember("targetId", dtf.CreateWideString(((int) 255)), isKey: true, id: 0),
                        new StructMember("executionReport", global::Common.OperationStateSupport.Instance.GetDynamicTypeInternal(isPublic), id: 1)
                    };

                    DynamicType result = tsf.CreateTypeWithAccessInfo<ControlExecutionReplyUnmanaged>(
                        dtf.BuildStruct()
                        .WithExtensibility(ExtensibilityKind.Extensible)
                        .WithName("Messages::OperationManagement::ControlExecutionReply")
                        .AddMembers(ControlExecutionReplyStructMembers));

                    return result;
                }
            }
        }
        public class ControlExecutionReplySupport : Rti.Dds.Topics.TypeSupport<global::Messages.OperationManagement.ControlExecutionReply>
        {
            public ControlExecutionReplySupport() : base(
                new Implementation.ControlExecutionReplyPlugin(),
                new Lazy<DynamicType>(() =>Implementation.ControlExecutionReplyPlugin.CreateDynamicType(isPublic: true)))
            {
            }

            public static ControlExecutionReplySupport Instance { get; } =
            ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<ControlExecutionReplySupport, global::Messages.OperationManagement.ControlExecutionReply>();

        }

        namespace Implementation
        {

            public struct MonitorSpecListRequestUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::Messages.OperationManagement.MonitorSpecListRequest>
            {

                private NativeWstring targetId;

                public void Destroy(bool optionalsOnly)
                {
                    if (optionalsOnly)
                    {
                        return;
                    }
                    targetId.Destroy();
                }

                public void FromNative(global::Messages.OperationManagement.MonitorSpecListRequest sample, bool keysOnly = false)
                {

                    sample.targetId = targetId.FromNative();
                }

                public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
                {
                    targetId.Initialize(size: ((int) 255), allocateMemory: allocateMemory);
                }

                public void ToNative(global::Messages.OperationManagement.MonitorSpecListRequest sample, bool keysOnly = false)
                {
                    targetId.ToNative(sample.targetId, ((int) 255));
                }
            }

            internal class MonitorSpecListRequestPlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::Messages.OperationManagement.MonitorSpecListRequest, MonitorSpecListRequestUnmanaged>
            {

                internal MonitorSpecListRequestPlugin() : base("global::Messages.OperationManagement.MonitorSpecListRequest", isKeyed: true, CreateDynamicType(isPublic: false))
                {
                }

                public static DynamicType CreateDynamicType(bool isPublic = true)
                {
                    var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
                    var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

                    // MonitorSpecListRequest struct
                    var MonitorSpecListRequestStructMembers = new StructMember[]
                    {
                        new StructMember("targetId", dtf.CreateWideString(((int) 255)), isKey: true, id: 0)
                    };

                    DynamicType result = tsf.CreateTypeWithAccessInfo<MonitorSpecListRequestUnmanaged>(
                        dtf.BuildStruct()
                        .WithExtensibility(ExtensibilityKind.Extensible)
                        .WithName("Messages::OperationManagement::MonitorSpecListRequest")
                        .AddMembers(MonitorSpecListRequestStructMembers));

                    return result;
                }
            }
        }
        public class MonitorSpecListRequestSupport : Rti.Dds.Topics.TypeSupport<global::Messages.OperationManagement.MonitorSpecListRequest>
        {
            public MonitorSpecListRequestSupport() : base(
                new Implementation.MonitorSpecListRequestPlugin(),
                new Lazy<DynamicType>(() =>Implementation.MonitorSpecListRequestPlugin.CreateDynamicType(isPublic: true)))
            {
            }

            public static MonitorSpecListRequestSupport Instance { get; } =
            ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<MonitorSpecListRequestSupport, global::Messages.OperationManagement.MonitorSpecListRequest>();

        }

        namespace Implementation
        {

            public struct MonitorSpecListReplyUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::Messages.OperationManagement.MonitorSpecListReply>
            {

                private NativeWstring targetId;

                public void Destroy(bool optionalsOnly)
                {
                    if (optionalsOnly)
                    {
                        return;
                    }
                    targetId.Destroy();
                }

                public void FromNative(global::Messages.OperationManagement.MonitorSpecListReply sample, bool keysOnly = false)
                {

                    sample.targetId = targetId.FromNative();
                }

                public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
                {
                    targetId.Initialize(size: ((int) 255), allocateMemory: allocateMemory);
                }

                public void ToNative(global::Messages.OperationManagement.MonitorSpecListReply sample, bool keysOnly = false)
                {
                    targetId.ToNative(sample.targetId, ((int) 255));
                }
            }

            internal class MonitorSpecListReplyPlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::Messages.OperationManagement.MonitorSpecListReply, MonitorSpecListReplyUnmanaged>
            {

                internal MonitorSpecListReplyPlugin() : base("global::Messages.OperationManagement.MonitorSpecListReply", isKeyed: true, CreateDynamicType(isPublic: false))
                {
                }

                public static DynamicType CreateDynamicType(bool isPublic = true)
                {
                    var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
                    var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

                    // MonitorSpecListReply struct
                    var MonitorSpecListReplyStructMembers = new StructMember[]
                    {
                        new StructMember("targetId", dtf.CreateWideString(((int) 255)), isKey: true, id: 0)
                    };

                    DynamicType result = tsf.CreateTypeWithAccessInfo<MonitorSpecListReplyUnmanaged>(
                        dtf.BuildStruct()
                        .WithExtensibility(ExtensibilityKind.Extensible)
                        .WithName("Messages::OperationManagement::MonitorSpecListReply")
                        .AddMembers(MonitorSpecListReplyStructMembers));

                    return result;
                }
            }
        }
        public class MonitorSpecListReplySupport : Rti.Dds.Topics.TypeSupport<global::Messages.OperationManagement.MonitorSpecListReply>
        {
            public MonitorSpecListReplySupport() : base(
                new Implementation.MonitorSpecListReplyPlugin(),
                new Lazy<DynamicType>(() =>Implementation.MonitorSpecListReplyPlugin.CreateDynamicType(isPublic: true)))
            {
            }

            public static MonitorSpecListReplySupport Instance { get; } =
            ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<MonitorSpecListReplySupport, global::Messages.OperationManagement.MonitorSpecListReply>();

        }

    } // namespace OperationManagement

} // namespace Messages

