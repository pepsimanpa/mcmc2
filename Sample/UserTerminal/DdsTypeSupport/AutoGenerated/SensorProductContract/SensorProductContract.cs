
/*
WARNING: THIS FILE IS AUTO-GENERATED. DO NOT MODIFY.

This file was generated from SensorProductContract.idl
using RTI Code Generator (rtiddsgen) version 4.3.0.
The rtiddsgen tool is part of the RTI Connext DDS distribution.
For more information, type 'rtiddsgen -help' at a command shell
or consult the Code Generator User's Manual.
*/

using System;
using System.Reflection;
using System.Collections.Generic;
using Rti.Types;
using System.Linq;
using Omg.Types;

namespace Types
{
    public static class REGISTER_PRODUCER_CONTRACT_SERVICE
    {
        public const string Value = "RegisterProducerContract";
    }
    public static class COMMAND_REGISTER_PRODUCER_CONTRACT_TOPIC
    {
        public const string Value = "commandRegisterProducerContract";
    }
    public static class RESPONSE_REGISTER_PRODUCER_CONTRACT_TOPIC
    {
        public const string Value = "responseRegisterProducerContract";
    }

    public enum SensorProductKind
    {
        SideScanSonarRaw,
        SideScanSonarProcessed
    }

    public enum ResponseCode
    {
        OK,
        Failed
    }

    public class RegisterCommandType :  IEquatable<RegisterCommandType>
    {
        [Key]
        public int messageId { get; set; }
        [Key]
        public int producerId { get; set; }
        public global::Types.SensorProductKind serviceSensorProduct { get; set; }

        public RegisterCommandType()
        {
            serviceSensorProduct = (global::Types.SensorProductKind) (0);
        }

        public RegisterCommandType(int  messageId, int  producerId, global::Types.SensorProductKind  serviceSensorProduct)
        {
            this.messageId = messageId;
            this.producerId = producerId;
            this.serviceSensorProduct = serviceSensorProduct;
        }

        public RegisterCommandType(RegisterCommandType other)
        {
            if (other == null)
            {
                return;
            }

            this.messageId = other.messageId;
            this.producerId = other.producerId;
            this.serviceSensorProduct = other.serviceSensorProduct;

        }

        public override int GetHashCode()
        {
            var hash = new HashCode();

            hash.Add(this.messageId);
            hash.Add(this.producerId);
            hash.Add(this.serviceSensorProduct);

            return hash.ToHashCode();
        }

        public bool Equals(RegisterCommandType other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.messageId.Equals(other.messageId) && 
            this.producerId.Equals(other.producerId) && 
            this.serviceSensorProduct.Equals(other.serviceSensorProduct);
        }

        public override bool Equals(object obj) => this.Equals(obj as RegisterCommandType);

        public override string ToString() => RegisterCommandTypeSupport.Instance.ToString(this);
    }

    public class RegisterResponseType :  IEquatable<RegisterResponseType>
    {
        [Key]
        public int messageId { get; set; }
        [Key]
        public int producerId { get; set; }
        public global::Types.ResponseCode responseCode { get; set; }

        public RegisterResponseType()
        {
            responseCode = (global::Types.ResponseCode) (0);
        }

        public RegisterResponseType(int  messageId, int  producerId, global::Types.ResponseCode  responseCode)
        {
            this.messageId = messageId;
            this.producerId = producerId;
            this.responseCode = responseCode;
        }

        public RegisterResponseType(RegisterResponseType other)
        {
            if (other == null)
            {
                return;
            }

            this.messageId = other.messageId;
            this.producerId = other.producerId;
            this.responseCode = other.responseCode;

        }

        public override int GetHashCode()
        {
            var hash = new HashCode();

            hash.Add(this.messageId);
            hash.Add(this.producerId);
            hash.Add(this.responseCode);

            return hash.ToHashCode();
        }

        public bool Equals(RegisterResponseType other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.messageId.Equals(other.messageId) && 
            this.producerId.Equals(other.producerId) && 
            this.responseCode.Equals(other.responseCode);
        }

        public override bool Equals(object obj) => this.Equals(obj as RegisterResponseType);

        public override string ToString() => RegisterResponseTypeSupport.Instance.ToString(this);
    }

} // namespace Types
