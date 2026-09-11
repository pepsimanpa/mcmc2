
/*
WARNING: THIS FILE IS AUTO-GENERATED. DO NOT MODIFY.

This file was generated from OperationManagement.idl
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

namespace Common
{

    public class ControlParam :  IEquatable<ControlParam>
    {
        [Bound(255)]
        public string fieldName { get; set; } = string.Empty;
        [Bound(255)]
        public string fixedValue { get; set; } = string.Empty;

        public ControlParam()
        {
        }

        public ControlParam(string  fieldName, string  fixedValue)
        {
            this.fieldName = fieldName;
            this.fixedValue = fixedValue;
        }

        public ControlParam(ControlParam other)
        {
            if (other == null)
            {
                return;
            }

            this.fieldName = other.fieldName;
            this.fixedValue = other.fixedValue;

        }

        public override int GetHashCode()
        {
            var hash = new HashCode();

            hash.Add(this.fieldName);
            hash.Add(this.fixedValue);

            return hash.ToHashCode();
        }

        public bool Equals(ControlParam other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.fieldName.Equals(other.fieldName) && 
            this.fixedValue.Equals(other.fixedValue);
        }

        public override bool Equals(object obj) => this.Equals(obj as ControlParam);

        public override string ToString() => ControlParamSupport.Instance.ToString(this);
    }

    public class ControlSpec :  IEquatable<ControlSpec>
    {
        [Bound(40)]
        public string id { get; set; } = string.Empty;
        [Bound(100)]
        public string name { get; set; } = string.Empty;
        [Bound(100)]
        public string cdm { get; set; } = string.Empty;
        public int numsOfParams { get; set; }
        [Bound(100)]
        public ISequence<global::Common.ControlParam> @params { get; }

        public ControlSpec()
        {
            @params = new Rti.Types.Sequence<global::Common.ControlParam>();
        }

        public ControlSpec(string  id, string  name, string  cdm, int  numsOfParams, ISequence<global::Common.ControlParam>@params)
        {
            this.id = id;
            this.name = name;
            this.cdm = cdm;
            this.numsOfParams = numsOfParams;
            this.@params = @params;
        }

        public ControlSpec(ControlSpec other)
        {
            if (other == null)
            {
                return;
            }

            this.id = other.id;
            this.name = other.name;
            this.cdm = other.cdm;
            this.numsOfParams = other.numsOfParams;
            this.@params = new Rti.Types.Sequence<global::Common.ControlParam>(other.@params.Select(element => new global::Common.ControlParam(element)));

        }

        public override int GetHashCode()
        {
            var hash = new HashCode();

            hash.Add(this.id);
            hash.Add(this.name);
            hash.Add(this.cdm);
            hash.Add(this.numsOfParams);
            hash.Add(this.@params.Count);

            return hash.ToHashCode();
        }

        public bool Equals(ControlSpec other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.id.Equals(other.id) && 
            this.name.Equals(other.name) && 
            this.cdm.Equals(other.cdm) && 
            this.numsOfParams.Equals(other.numsOfParams) && 
            this.@params.SequenceEqual(other.@params);
        }

        public override bool Equals(object obj) => this.Equals(obj as ControlSpec);

        public override string ToString() => ControlSpecSupport.Instance.ToString(this);
    }

    public enum OperationState
    {
        Finished,
        Processing,
        Failed
    }
} // namespace Common
namespace Messages
{
    namespace OperationManagement
    {

        public class ControlSpecListRequest :  IEquatable<ControlSpecListRequest>
        {
            [Key]
            [Bound(40)]
            public string targetId { get; set; } = string.Empty;
            [Bound(40)]
            public string targetName { get; set; } = string.Empty;

            public ControlSpecListRequest()
            {
            }

            public ControlSpecListRequest(string  targetId, string  targetName)
            {
                this.targetId = targetId;
                this.targetName = targetName;
            }

            public ControlSpecListRequest(ControlSpecListRequest other)
            {
                if (other == null)
                {
                    return;
                }

                this.targetId = other.targetId;
                this.targetName = other.targetName;

            }

            public override int GetHashCode()
            {
                var hash = new HashCode();

                hash.Add(this.targetId);
                hash.Add(this.targetName);

                return hash.ToHashCode();
            }

            public bool Equals(ControlSpecListRequest other)
            {
                if (other == null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return this.targetId.Equals(other.targetId) && 
                this.targetName.Equals(other.targetName);
            }

            public override bool Equals(object obj) => this.Equals(obj as ControlSpecListRequest);

            public override string ToString() => ControlSpecListRequestSupport.Instance.ToString(this);
        }

        public class ControlSpecListReply :  IEquatable<ControlSpecListReply>
        {
            [Key]
            [Bound(40)]
            public string targetId { get; set; } = string.Empty;
            [Bound(40)]
            public string targetName { get; set; } = string.Empty;
            public int numsOfControlSpecs { get; set; }
            [Bound(100)]
            public ISequence<global::Common.ControlSpec> controlSpecList { get; }

            public ControlSpecListReply()
            {
                controlSpecList = new Rti.Types.Sequence<global::Common.ControlSpec>();
            }

            public ControlSpecListReply(string  targetId, string  targetName, int  numsOfControlSpecs, ISequence<global::Common.ControlSpec>controlSpecList)
            {
                this.targetId = targetId;
                this.targetName = targetName;
                this.numsOfControlSpecs = numsOfControlSpecs;
                this.controlSpecList = controlSpecList;
            }

            public ControlSpecListReply(ControlSpecListReply other)
            {
                if (other == null)
                {
                    return;
                }

                this.targetId = other.targetId;
                this.targetName = other.targetName;
                this.numsOfControlSpecs = other.numsOfControlSpecs;
                this.controlSpecList = new Rti.Types.Sequence<global::Common.ControlSpec>(other.controlSpecList.Select(element => new global::Common.ControlSpec(element)));

            }

            public override int GetHashCode()
            {
                var hash = new HashCode();

                hash.Add(this.targetId);
                hash.Add(this.targetName);
                hash.Add(this.numsOfControlSpecs);
                hash.Add(this.controlSpecList.Count);

                return hash.ToHashCode();
            }

            public bool Equals(ControlSpecListReply other)
            {
                if (other == null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return this.targetId.Equals(other.targetId) && 
                this.targetName.Equals(other.targetName) && 
                this.numsOfControlSpecs.Equals(other.numsOfControlSpecs) && 
                this.controlSpecList.SequenceEqual(other.controlSpecList);
            }

            public override bool Equals(object obj) => this.Equals(obj as ControlSpecListReply);

            public override string ToString() => ControlSpecListReplySupport.Instance.ToString(this);
        }

        public class ControlExecutionRequest :  IEquatable<ControlExecutionRequest>
        {
            [Key]
            [Bound(255)]
            public string targetId { get; set; } = string.Empty;
            public global::Common.ControlSpec controlSpec { get; set; }

            public ControlExecutionRequest()
            {
                controlSpec = new global::Common.ControlSpec();
            }

            public ControlExecutionRequest(string  targetId, global::Common.ControlSpec  controlSpec)
            {
                this.targetId = targetId;
                this.controlSpec = controlSpec;
            }

            public ControlExecutionRequest(ControlExecutionRequest other)
            {
                if (other == null)
                {
                    return;
                }

                this.targetId = other.targetId;
                this.controlSpec = new global::Common.ControlSpec(other.controlSpec);

            }

            public override int GetHashCode()
            {
                var hash = new HashCode();

                hash.Add(this.targetId);
                hash.Add(this.controlSpec);

                return hash.ToHashCode();
            }

            public bool Equals(ControlExecutionRequest other)
            {
                if (other == null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return this.targetId.Equals(other.targetId) && 
                this.controlSpec.Equals(other.controlSpec);
            }

            public override bool Equals(object obj) => this.Equals(obj as ControlExecutionRequest);

            public override string ToString() => ControlExecutionRequestSupport.Instance.ToString(this);
        }

        public class ControlExecutionReply :  IEquatable<ControlExecutionReply>
        {
            [Key]
            [Bound(255)]
            public string targetId { get; set; } = string.Empty;
            public global::Common.OperationState executionReport { get; set; }

            public ControlExecutionReply()
            {
                executionReport = (global::Common.OperationState) (0);
            }

            public ControlExecutionReply(string  targetId, global::Common.OperationState  executionReport)
            {
                this.targetId = targetId;
                this.executionReport = executionReport;
            }

            public ControlExecutionReply(ControlExecutionReply other)
            {
                if (other == null)
                {
                    return;
                }

                this.targetId = other.targetId;
                this.executionReport = other.executionReport;

            }

            public override int GetHashCode()
            {
                var hash = new HashCode();

                hash.Add(this.targetId);
                hash.Add(this.executionReport);

                return hash.ToHashCode();
            }

            public bool Equals(ControlExecutionReply other)
            {
                if (other == null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return this.targetId.Equals(other.targetId) && 
                this.executionReport.Equals(other.executionReport);
            }

            public override bool Equals(object obj) => this.Equals(obj as ControlExecutionReply);

            public override string ToString() => ControlExecutionReplySupport.Instance.ToString(this);
        }

        public class MonitorSpecListRequest :  IEquatable<MonitorSpecListRequest>
        {
            [Key]
            [Bound(255)]
            public string targetId { get; set; } = string.Empty;

            public MonitorSpecListRequest()
            {
            }

            public MonitorSpecListRequest(string  targetId)
            {
                this.targetId = targetId;
            }

            public MonitorSpecListRequest(MonitorSpecListRequest other)
            {
                if (other == null)
                {
                    return;
                }

                this.targetId = other.targetId;

            }

            public override int GetHashCode()
            {
                var hash = new HashCode();

                hash.Add(this.targetId);

                return hash.ToHashCode();
            }

            public bool Equals(MonitorSpecListRequest other)
            {
                if (other == null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return this.targetId.Equals(other.targetId);
            }

            public override bool Equals(object obj) => this.Equals(obj as MonitorSpecListRequest);

            public override string ToString() => MonitorSpecListRequestSupport.Instance.ToString(this);
        }

        public class MonitorSpecListReply :  IEquatable<MonitorSpecListReply>
        {
            [Key]
            [Bound(255)]
            public string targetId { get; set; } = string.Empty;

            public MonitorSpecListReply()
            {
            }

            public MonitorSpecListReply(string  targetId)
            {
                this.targetId = targetId;
            }

            public MonitorSpecListReply(MonitorSpecListReply other)
            {
                if (other == null)
                {
                    return;
                }

                this.targetId = other.targetId;

            }

            public override int GetHashCode()
            {
                var hash = new HashCode();

                hash.Add(this.targetId);

                return hash.ToHashCode();
            }

            public bool Equals(MonitorSpecListReply other)
            {
                if (other == null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return this.targetId.Equals(other.targetId);
            }

            public override bool Equals(object obj) => this.Equals(obj as MonitorSpecListReply);

            public override string ToString() => MonitorSpecListReplySupport.Instance.ToString(this);
        }

    } // namespace OperationManagement
} // namespace Messages
