/*
WARNING: THIS FILE IS AUTO-GENERATED. DO NOT MODIFY.

This file was generated from SensorProductContract.idl
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

namespace Types
{

    namespace Implementation
    {
        internal class SensorProductKindPlugin : Rti.Dds.NativeInterface.TypePlugin.EnumTypePlugin
        {
            public SensorProductKindPlugin() : base(CreateDynamicType(isPublic: false))
            {
            }

            internal static DynamicType CreateDynamicType(bool isPublic = true)
            {
                var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
                return dtf.BuildEnum()
                .WithName("Types::SensorProductKind")
                .AddMember(new EnumMember("SideScanSonarRaw", 0))
                .AddMember(new EnumMember("SideScanSonarProcessed", 1))
                .WithExtensibility(ExtensibilityKind.Extensible)
                .Create();
            }
        }
    }

    public class SensorProductKindSupport : Rti.Dds.Topics.TypeSupport<global::Types.SensorProductKind>
    {
        public SensorProductKindSupport() : base(
            new Implementation.SensorProductKindPlugin(),
            new Lazy<DynamicType>(() =>Implementation.SensorProductKindPlugin.CreateDynamicType(isPublic: true)))
        {
        }

        public static SensorProductKindSupport Instance { get; } =
        ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<SensorProductKindSupport, global::Types.SensorProductKind>();

    }

    namespace Implementation
    {
        internal class ResponseCodePlugin : Rti.Dds.NativeInterface.TypePlugin.EnumTypePlugin
        {
            public ResponseCodePlugin() : base(CreateDynamicType(isPublic: false))
            {
            }

            internal static DynamicType CreateDynamicType(bool isPublic = true)
            {
                var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
                return dtf.BuildEnum()
                .WithName("Types::ResponseCode")
                .AddMember(new EnumMember("OK", 0))
                .AddMember(new EnumMember("Failed", 1))
                .WithExtensibility(ExtensibilityKind.Extensible)
                .Create();
            }
        }
    }

    public class ResponseCodeSupport : Rti.Dds.Topics.TypeSupport<global::Types.ResponseCode>
    {
        public ResponseCodeSupport() : base(
            new Implementation.ResponseCodePlugin(),
            new Lazy<DynamicType>(() =>Implementation.ResponseCodePlugin.CreateDynamicType(isPublic: true)))
        {
        }

        public static ResponseCodeSupport Instance { get; } =
        ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<ResponseCodeSupport, global::Types.ResponseCode>();

    }

    namespace Implementation
    {

        public struct RegisterCommandTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::Types.RegisterCommandType>
        {

            private int messageId;
            private int producerId;
            private global::Types.SensorProductKind serviceSensorProduct;

            public void Destroy(bool optionalsOnly)
            {
            }

            public void FromNative(global::Types.RegisterCommandType sample, bool keysOnly = false)
            {

                sample.messageId = messageId;
                sample.producerId = producerId;
                if (keysOnly)
                {
                    return;
                }
                sample.serviceSensorProduct = serviceSensorProduct;
            }

            public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
            {
                messageId = (int) (0);
                producerId = (int) (0);
                serviceSensorProduct = (global::Types.SensorProductKind) (0);
            }

            public void ToNative(global::Types.RegisterCommandType sample, bool keysOnly = false)
            {
                messageId = sample.messageId;
                producerId = sample.producerId;
                if (keysOnly)
                {
                    return;
                }
                serviceSensorProduct = sample.serviceSensorProduct;
            }
        }

        internal class RegisterCommandTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::Types.RegisterCommandType, RegisterCommandTypeUnmanaged>
        {

            internal RegisterCommandTypePlugin() : base("global::Types.RegisterCommandType", isKeyed: true, CreateDynamicType(isPublic: false))
            {
            }

            public static DynamicType CreateDynamicType(bool isPublic = true)
            {
                var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
                var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

                // RegisterCommandType struct
                var RegisterCommandTypeStructMembers = new StructMember[]
                {
                    new StructMember("messageId", dtf.GetPrimitiveType<int>(), isKey: true, id: 0),
                    new StructMember("producerId", dtf.GetPrimitiveType<int>(), isKey: true, id: 1),
                    new StructMember("serviceSensorProduct", global::Types.SensorProductKindSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2)
                };

                DynamicType result = tsf.CreateTypeWithAccessInfo<RegisterCommandTypeUnmanaged>(
                    dtf.BuildStruct()
                    .WithExtensibility(ExtensibilityKind.Extensible)
                    .WithName("Types::RegisterCommandType")
                    .AddMembers(RegisterCommandTypeStructMembers));

                return result;
            }
        }
    }
    public class RegisterCommandTypeSupport : Rti.Dds.Topics.TypeSupport<global::Types.RegisterCommandType>
    {
        public RegisterCommandTypeSupport() : base(
            new Implementation.RegisterCommandTypePlugin(),
            new Lazy<DynamicType>(() =>Implementation.RegisterCommandTypePlugin.CreateDynamicType(isPublic: true)))
        {
        }

        public static RegisterCommandTypeSupport Instance { get; } =
        ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<RegisterCommandTypeSupport, global::Types.RegisterCommandType>();

    }

    namespace Implementation
    {

        public struct RegisterResponseTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::Types.RegisterResponseType>
        {

            private int messageId;
            private int producerId;
            private global::Types.ResponseCode responseCode;

            public void Destroy(bool optionalsOnly)
            {
            }

            public void FromNative(global::Types.RegisterResponseType sample, bool keysOnly = false)
            {

                sample.messageId = messageId;
                sample.producerId = producerId;
                if (keysOnly)
                {
                    return;
                }
                sample.responseCode = responseCode;
            }

            public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
            {
                messageId = (int) (0);
                producerId = (int) (0);
                responseCode = (global::Types.ResponseCode) (0);
            }

            public void ToNative(global::Types.RegisterResponseType sample, bool keysOnly = false)
            {
                messageId = sample.messageId;
                producerId = sample.producerId;
                if (keysOnly)
                {
                    return;
                }
                responseCode = sample.responseCode;
            }
        }

        internal class RegisterResponseTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::Types.RegisterResponseType, RegisterResponseTypeUnmanaged>
        {

            internal RegisterResponseTypePlugin() : base("global::Types.RegisterResponseType", isKeyed: true, CreateDynamicType(isPublic: false))
            {
            }

            public static DynamicType CreateDynamicType(bool isPublic = true)
            {
                var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
                var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

                // RegisterResponseType struct
                var RegisterResponseTypeStructMembers = new StructMember[]
                {
                    new StructMember("messageId", dtf.GetPrimitiveType<int>(), isKey: true, id: 0),
                    new StructMember("producerId", dtf.GetPrimitiveType<int>(), isKey: true, id: 1),
                    new StructMember("responseCode", global::Types.ResponseCodeSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2)
                };

                DynamicType result = tsf.CreateTypeWithAccessInfo<RegisterResponseTypeUnmanaged>(
                    dtf.BuildStruct()
                    .WithExtensibility(ExtensibilityKind.Extensible)
                    .WithName("Types::RegisterResponseType")
                    .AddMembers(RegisterResponseTypeStructMembers));

                return result;
            }
        }
    }
    public class RegisterResponseTypeSupport : Rti.Dds.Topics.TypeSupport<global::Types.RegisterResponseType>
    {
        public RegisterResponseTypeSupport() : base(
            new Implementation.RegisterResponseTypePlugin(),
            new Lazy<DynamicType>(() =>Implementation.RegisterResponseTypePlugin.CreateDynamicType(isPublic: true)))
        {
        }

        public static RegisterResponseTypeSupport Instance { get; } =
        ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<RegisterResponseTypeSupport, global::Types.RegisterResponseType>();

    }

} // namespace Types

