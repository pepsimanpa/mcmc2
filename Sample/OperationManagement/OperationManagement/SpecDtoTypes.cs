using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Specification.Semantic.DTO
{
    public record IdentificationDTO
    {
        [XmlElement("ID")]
        public string ID;
        [XmlElement("Name")]
        public string Name;
    }

    [XmlRoot("VehicleSpec")]
    public class VehicleSpecDTO
    {
        [XmlElement("Identification")]
        public IdentificationDTO Identity;

        [XmlArray("Sensors")]
        [XmlArrayItem("SensorSpecRef")]
        public List<SpecRefDTO> SensorSpecRefs;

        [XmlArray("Platform")]
        [XmlArrayItem("PlatformSpecRef")]
        public List<SpecRefDTO> PlatformSpecRefs;

        [XmlArray("Payloads")]
        [XmlArrayItem("PayloadSpecRef")]
        public List<SpecRefDTO> PayloadSpecRefs;

        [XmlArray("Comms")]
        [XmlArrayItem("CommSpecRef")]
        public List<SpecRefDTO> CommSpecRefs;
    }

    public class SpecRefDTO
    {
        [XmlAttribute("id")]
        public string id;
        [XmlAttribute("name")]
        public string name;
        [XmlElement("SemanticPath")]
        public string SemanticPath;
        [XmlElement("BindingPath")]
        public string BindingPath;
    }

    [XmlRoot("SensorSpec")]
    public record SensorSpecDTO
    {
        [XmlElement("ConstantSpecs")]
        public ConstantSpecSetDTO ConstantSpecs;
        [XmlElement("MonitorSpecs")]
        public MonitorSpecSetDTO MonitorSpecs;
        [XmlElement("ControlSpecs")]
        public ControlSpecSetDTO ControlSpecs;
        [XmlElement("SensorProductSpecs")]
        public SensorProductSpecSetDTO SensorProductSpecs;
    }

    public record ConstantSpecSetDTO : SemanticSpecBaseDTO
    {
        [XmlElement("TextConstantSpec", typeof(TextConstantSpecDTO))]
        [XmlElement("QuantityConstantSpec", typeof(QuantityConstantSpecDTO))]
        public List<SemanticSpecBaseDTO> Specs;
    }

    public record MonitorSpecSetDTO : SemanticSpecBaseDTO
    {
        [XmlElement("QuantitySpec", typeof(QuantitySpecDTO))]
        [XmlElement("TextSpec", typeof(TextSpecDTO))]
        [XmlElement("ValueSetSpec", typeof(ValueSetSpecDTO))]
        [XmlElement("QuantityValueSetSpec", typeof(QuantityValueSetSpecDTO))]
        [XmlElement("HealthStatusSetSpec", typeof(HealthStatusSetSpecDTO))]
        [XmlElement("GroupSpec", typeof(GroupSpecDTO))]
        public List<SemanticSpecBaseDTO> Specs;
    }

    public record ControlSpecSetDTO : SemanticSpecBaseDTO
    {
        [XmlElement("ControlSpec", typeof(ControlSpecDTO))]
        [XmlElement("SetPointSpec", typeof(SetPointSpecDTO))]
        public List<SemanticSpecBaseDTO> Specs;
    }

    public record SensorProductSpecSetDTO : SemanticSpecBaseDTO
    {
        [XmlElement("ProductStreamSpec", typeof(ProductStreamSpecDTO))]
        public List<SemanticSpecBaseDTO> Specs;
    }

    [XmlRoot("PlatformSpec")]
    public record PlatformSpecDTO
    {
        [XmlElement("ConstantSpecs")]
        public ConstantSpecSetDTO ConstantSpecs;
        [XmlElement("MonitorSpecs")]
        public MonitorSpecSetDTO MonitorSpecs;
        [XmlElement("ControlSpecs")]
        public ControlSpecSetDTO ControlSpecs;
    }

    [XmlRoot("PayloadSpec")]
    public record PayloadSpecDTO
    {
        [XmlElement("ConstantSpecs")]
        public ConstantSpecSetDTO ConstantSpecs;
        [XmlElement("MonitorSpecs")]
        public MonitorSpecSetDTO MonitorSpecs;
        [XmlElement("ControlSpecs")]
        public ControlSpecSetDTO ControlSpecs;
    }

    [XmlRoot("CommSpec")]
    public record CommSpecDTO
    {
        [XmlElement("ConstantSpecs")]
        public ConstantSpecSetDTO ConstantSpecs;
        [XmlElement("MonitorSpecs")]
        public MonitorSpecSetDTO MonitorSpecs;
        [XmlElement("ControlSpecs")]
        public ControlSpecSetDTO ControlSpecs;
    }

    [XmlInclude(typeof(ConstantSpecSetDTO))]
    [XmlInclude(typeof(MonitorSpecSetDTO))]
    [XmlInclude(typeof(ControlSpecSetDTO))]
    [XmlInclude(typeof(SensorProductSpecSetDTO))]
    [XmlInclude(typeof(TextConstantSpecDTO))]
    [XmlInclude(typeof(TextSpecDTO))]
    [XmlInclude(typeof(ValueSetSpecDTO))]
    [XmlInclude(typeof(GroupSpecDTO))]
    [XmlInclude(typeof(QuantityConstantSpecDTO))]
    [XmlInclude(typeof(QuantitySpecDTO))]
    [XmlInclude(typeof(ControlSpecDTO))]
    [XmlInclude(typeof(ProductStreamSpecDTO))]
    public record SemanticSpecBaseDTO
    {
        [XmlAttribute("id")]
        public string id;
        [XmlAttribute("name")]
        public string name;
        [XmlAttribute("cdm")]
        public string cdm;

        public virtual List<SemanticSpecBaseDTO> GetChildren()
        {
            return null;
        }
    }
    
    public record TextConstantSpecDTO : SemanticSpecBaseDTO
    {
        [XmlElement("Value")]
        public string Value;
    }

    public record RangeDTO
    {
        [XmlAttribute("min")]
        public float min;
        [XmlAttribute("max")]
        public float max;
    }

    public record QuantityConstantSpecDTO : SemanticSpecBaseDTO
    {
        [XmlElement("Unit")]
        public string Unit;
        [XmlElement("Value")]
        public float Value;
    }

    public record QuantitySpecDTO : SemanticSpecBaseDTO
    {
        [XmlElement("Unit")]
        public string Unit;
        [XmlElement("Range")]
        public RangeDTO Range;
        [XmlElement("Resolution")]
        public float Resolution;
        [XmlElement("Format")]
        public string Format;
    }

    public record TextSpecDTO : SemanticSpecBaseDTO
    {
        [XmlElement("MaxLength")]
        public int MaxLength;
    }

    [XmlInclude(typeof(QuantityValueSetSpecDTO))]
    [XmlInclude(typeof(HealthStatusSetSpecDTO))]
    public record ValueSetSpecDTO : SemanticSpecBaseDTO
    {
        [XmlArray("Values")]
        [XmlArrayItem("Value")]
        public List<CdmRefAttrDTO> Values;
    }

    public record QuantityValueSetSpecDTO : ValueSetSpecDTO
    {
        [XmlElement("Unit")]
        public string Unit;
    }

    public record HealthStatusSetSpecDTO : ValueSetSpecDTO
    {
        [XmlElement("ComponentDomain")]
        public CdmRefAttrDTO ComponentDomain;
        [XmlElement("ComponentRole")]
        public CdmRefAttrDTO ComponentRole;
        [XmlElement("AffectedFunction")]
        public CdmRefAttrDTO AffectedFunction;
    }

    public record GroupSpecDTO : SemanticSpecBaseDTO
    {
        [XmlElement("QuantitySpec", typeof(QuantitySpecDTO))]
        [XmlElement("TextSpec", typeof(TextSpecDTO))]
        [XmlElement("ValueSetSpec", typeof(ValueSetSpecDTO))]
        [XmlElement("QuantityValueSetSpec", typeof(QuantityValueSetSpecDTO))]
        [XmlElement("HealthStatusSetSpec", typeof(HealthStatusSetSpecDTO))]
        public List<SemanticSpecBaseDTO> Specs;

        public override List<SemanticSpecBaseDTO> GetChildren()
        {
            return Specs;
        }
    }

    public record CdmRefAttrDTO
    {
        [XmlAttribute("cdm")]
        public string cdm;

        [XmlText]
        public string Text;
    }

    public record QuantityProfileDTO
    {
        [XmlElement("Unit")]
        public string Unit;
        [XmlElement("Range")]
        public RangeDTO Range;
        [XmlElement("Resolution")]
        public float Resolution;
        [XmlAttribute("cdmRef")]
        public string cdmRef;
    }

    public record ProfileCompositeDTO
    {
        [XmlElement("Profile", typeof(CdmRefAttrDTO))]
        public List<CdmRefAttrDTO> Profiles;

        [XmlElement("QuantityProfile", typeof(QuantityProfileDTO))]
        public List<QuantityProfileDTO> QuantityProfiles;
    }

    [XmlInclude(typeof(SetPointSpecDTO))]
    public record ControlSpecDTO : SemanticSpecBaseDTO
    {
        [XmlElement("Parameters")]
        public ProfileCompositeDTO Parameters;

        [XmlElement("Precondition")]
        public CdmRefAttrDTO Precondition;
    }

    public record SetPointSpecDTO : ControlSpecDTO
    {
        [XmlElement("Unit")]
        public string Unit;

        [XmlElement("Range")]
        public RangeDTO Range;

        [XmlElement("Resolution")]
        public float Resolution;

        [XmlElement("ControlMethod")]
        public string ControlMethod;
    }

    public record ProductStreamSpecDTO : SemanticSpecBaseDTO
    {
        [XmlElement("ProductKind")]
        public CdmRefAttrDTO ProductKind;

        [XmlElement("ProcessingState")]
        public CdmRefAttrDTO ProcessingState;
    }
}

namespace Specification.Binding.DTO
{
    public record BindingBaseDTO
    {
        [XmlAttribute("semantic_id")]
        public string semanticId;
    }

    [XmlRoot("SensorBinding")]
    public record SensorBindingDTO
    {
        [XmlAttribute("semantic_ref")]
        public string semanticRef;
        [XmlElement("Monitors")]
        public MonitorsDTO Monitors;
        [XmlElement("Controls")]
        public ControlsDTO Controls;
    }

    public record MonitorsDTO : BindingBaseDTO
    {

    }

    public record ControlsDTO : BindingBaseDTO
    {
        [XmlElement("ControlBindingDDS", typeof(ControlBindingDTO))]
        public List<ControlBindingDTO> Bindings;
    }

    public record ControlBindingDTO : BindingBaseDTO
    {
        [XmlElement("Channel")]
        public ChannelDTO Channel;

        [XmlElement("Field", typeof(FieldDTO))]
        [XmlElement("FixedField", typeof(FixedFieldDTO))]
        public List<FieldDTO> Fields;
    }

    public record ChannelDTO
    {
        [XmlAttribute("topicName")]
        public string topicName;
        [XmlAttribute("typeName")]
        public string typeName;
    }

    public record FieldDTO
    {
        [XmlAttribute("name")]
        public string name;
        [XmlAttribute("cdm")]
        public string cdm;
    }

    public record FixedFieldDTO : FieldDTO
    {
        [XmlAttribute("value")]
        public string value;
    }
}