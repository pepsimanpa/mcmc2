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
            .WithName("SensorProductKind")
            .AddMember(new EnumMember("SideScanSonarRaw", 0))
            .AddMember(new EnumMember("SideScanSonarProcessed", 1))
            .WithExtensibility(ExtensibilityKind.Extensible)
            .Create();
        }
    }
}

public class SensorProductKindSupport : Rti.Dds.Topics.TypeSupport<global::SensorProductKind>
{
    public SensorProductKindSupport() : base(
        new Implementation.SensorProductKindPlugin(),
        new Lazy<DynamicType>(() =>Implementation.SensorProductKindPlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static SensorProductKindSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<SensorProductKindSupport, global::SensorProductKind>();

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
            .WithName("ResponseCode")
            .AddMember(new EnumMember("OK", 0))
            .AddMember(new EnumMember("Failed", 1))
            .WithExtensibility(ExtensibilityKind.Extensible)
            .Create();
        }
    }
}

public class ResponseCodeSupport : Rti.Dds.Topics.TypeSupport<global::ResponseCode>
{
    public ResponseCodeSupport() : base(
        new Implementation.ResponseCodePlugin(),
        new Lazy<DynamicType>(() =>Implementation.ResponseCodePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static ResponseCodeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<ResponseCodeSupport, global::ResponseCode>();

}

namespace Implementation
{

    public struct ProducerContractRegisterCommandTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::ProducerContractRegisterCommandType>
    {

        private int messageId;
        private int producerId;
        private global::SensorProductKind serviceSensorProduct;

        public void Destroy(bool optionalsOnly)
        {
        }

        public void FromNative(global::ProducerContractRegisterCommandType sample, bool keysOnly = false)
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
            serviceSensorProduct = (global::SensorProductKind) (0);
        }

        public void ToNative(global::ProducerContractRegisterCommandType sample, bool keysOnly = false)
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

    internal class ProducerContractRegisterCommandTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::ProducerContractRegisterCommandType, ProducerContractRegisterCommandTypeUnmanaged>
    {

        internal ProducerContractRegisterCommandTypePlugin() : base("global::ProducerContractRegisterCommandType", isKeyed: true, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // ProducerContractRegisterCommandType struct
            var ProducerContractRegisterCommandTypeStructMembers = new StructMember[]
            {
                new StructMember("messageId", dtf.GetPrimitiveType<int>(), isKey: true, id: 0),
                new StructMember("producerId", dtf.GetPrimitiveType<int>(), isKey: true, id: 1),
                new StructMember("serviceSensorProduct", global::SensorProductKindSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<ProducerContractRegisterCommandTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("ProducerContractRegisterCommandType")
                .AddMembers(ProducerContractRegisterCommandTypeStructMembers));

            return result;
        }
    }
}
public class ProducerContractRegisterCommandTypeSupport : Rti.Dds.Topics.TypeSupport<global::ProducerContractRegisterCommandType>
{
    public ProducerContractRegisterCommandTypeSupport() : base(
        new Implementation.ProducerContractRegisterCommandTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.ProducerContractRegisterCommandTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static ProducerContractRegisterCommandTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<ProducerContractRegisterCommandTypeSupport, global::ProducerContractRegisterCommandType>();

}

namespace Implementation
{

    public struct ProducerContractRegisterResponseTypeUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::ProducerContractRegisterResponseType>
    {

        private int messageId;
        private int producerId;
        private global::ResponseCode responseCode;

        public void Destroy(bool optionalsOnly)
        {
        }

        public void FromNative(global::ProducerContractRegisterResponseType sample, bool keysOnly = false)
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
            responseCode = (global::ResponseCode) (0);
        }

        public void ToNative(global::ProducerContractRegisterResponseType sample, bool keysOnly = false)
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

    internal class ProducerContractRegisterResponseTypePlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::ProducerContractRegisterResponseType, ProducerContractRegisterResponseTypeUnmanaged>
    {

        internal ProducerContractRegisterResponseTypePlugin() : base("global::ProducerContractRegisterResponseType", isKeyed: true, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // ProducerContractRegisterResponseType struct
            var ProducerContractRegisterResponseTypeStructMembers = new StructMember[]
            {
                new StructMember("messageId", dtf.GetPrimitiveType<int>(), isKey: true, id: 0),
                new StructMember("producerId", dtf.GetPrimitiveType<int>(), isKey: true, id: 1),
                new StructMember("responseCode", global::ResponseCodeSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<ProducerContractRegisterResponseTypeUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("ProducerContractRegisterResponseType")
                .AddMembers(ProducerContractRegisterResponseTypeStructMembers));

            return result;
        }
    }
}
public class ProducerContractRegisterResponseTypeSupport : Rti.Dds.Topics.TypeSupport<global::ProducerContractRegisterResponseType>
{
    public ProducerContractRegisterResponseTypeSupport() : base(
        new Implementation.ProducerContractRegisterResponseTypePlugin(),
        new Lazy<DynamicType>(() =>Implementation.ProducerContractRegisterResponseTypePlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static ProducerContractRegisterResponseTypeSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<ProducerContractRegisterResponseTypeSupport, global::ProducerContractRegisterResponseType>();

}

