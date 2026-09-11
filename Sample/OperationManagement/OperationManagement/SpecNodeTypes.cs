using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageAdapter;
using Specification.Binding.DTO;
using Specification.Semantic.DTO;

namespace Specification.Node
{
    public class SpecNodeFactory
    {
        public static SpecNodeFactory Instance
        {
            get
            {
                if (_instance is null)
                    return _instance = new SpecNodeFactory();

                return _instance;
            }
        }

        public SpecNodeFactory()
        {
            // Constant Spec DTO Factories
            _factories.Add(typeof(ConstantSpecSetDTO), (dto, parentId) => new SimpleSpecNode(dto, parentId));
            _factories.Add(typeof(QuantityConstantSpecDTO), (dto, parentId) => new QuantityConstantSpecNode(dto, parentId));
            _factories.Add(typeof(TextConstantSpecDTO), (dto, parentId) => new TextConstantSpecNode(dto, parentId));

            // Monitor Spec DTO Factories
            _factories.Add(typeof(MonitorSpecSetDTO), (dto, parentId) => new SimpleSpecNode(dto, parentId));
            _factories.Add(typeof(QuantitySpecDTO), (dto, parentId) => new QuantitySpecNode(dto, parentId));
            _factories.Add(typeof(TextSpecDTO), (dto, parentId) => new TextSpecNode(dto, parentId));
            _factories.Add(typeof(ValueSetSpecDTO), (dto, parentId) => new ValueSetSpecNode(dto, parentId));
            _factories.Add(typeof(QuantityValueSetSpecDTO), (dto, parentId) => new QuantityValueSetSpecNode(dto, parentId));
            _factories.Add(typeof(HealthStatusSetSpecDTO), (dto, parentId) => new HealthStatusSetSpecNode(dto, parentId));
            _factories.Add(typeof(GroupSpecDTO), (dto, parentId) => new GroupSpecNode(dto, parentId));

            // Control Spec DTO Factories
            _factories.Add(typeof(ControlSpecSetDTO), (dto, parentId) => new SimpleSpecNode(dto, parentId));
            _factories.Add(typeof(ControlSpecDTO), (dto, parentId) => new ControlSpecNode(dto, parentId));
            _factories.Add(typeof(SetPointSpecDTO), (dto, parentId) => new SetPointSpecNode(dto, parentId));

            // SensorProduct Spec DTO Factories
            _factories.Add(typeof(SensorProductSpecSetDTO), (dto, parentId) => new SimpleSpecNode(dto, parentId));
            _factories.Add(typeof(ProductStreamSpecDTO), (dto, parentId) => new ProductStreamSpecNode(dto, parentId));
        }

        public SpecNode Create(SemanticSpecBaseDTO dto, string parentId)
        {
            Func<SemanticSpecBaseDTO, string, SpecNode> factory;
            if (_factories.TryGetValue(dto.GetType(), out factory) is false)
                return null;
            
            return factory(dto, parentId);
        }

        public bool Register<T>(Func<SemanticSpecBaseDTO, string, SpecNode> factory) where T : SemanticSpecBaseDTO
        {
            return _factories.TryAdd(typeof(T), factory);
        }

        private Dictionary<Type, Func<SemanticSpecBaseDTO, string, SpecNode>> _factories = new();
        private static SpecNodeFactory _instance = null;
    }

    public abstract class SpecNode
    {
        public string Id { get; init; }
        public string Cdm { get; init; }
        public string ParentId { get; init; }
        public string FullParentId { get; init; }
        public string FullId { get => $"{ParentId}.{Id}";}
        public int Level { get => FullId.Count(c => c == '.'); }

        public SpecNode(SemanticSpecBaseDTO dto, string fullParentId)
        {
            Id = dto.id;
            FullParentId = fullParentId;
            ParentId = fullParentId[(fullParentId.LastIndexOf('.') + 1)..];
            Cdm = dto.cdm;
        }
    }

    public abstract class SemanticSpecNode<TSemantic, TBinding> : SpecNode 
        where TSemantic : SemanticSpecBaseDTO
        where TBinding : BindingBaseDTO
    {
        public SemanticSpecNode(SemanticSpecBaseDTO dto, string parentId) : base(dto, parentId)
        {
            Semantic = dto as TSemantic;
        }

        public abstract TSemantic Semantic { get; init; }
        public abstract TBinding Binding { get; set; }
    }

    public class SimpleSpecNode : SpecNode
    {
        public SimpleSpecNode(SemanticSpecBaseDTO dto, string parentId) : base(dto, parentId)
        { }
    }

    public class TextConstantSpecNode : SemanticSpecNode<TextConstantSpecDTO, BindingBaseDTO>
    {
        public TextConstantSpecNode(SemanticSpecBaseDTO dto, string parentId) : base(dto, parentId)
        {

        }

        public override TextConstantSpecDTO Semantic { get; init; }
        public override BindingBaseDTO Binding { get; set; }
    }

    public class QuantityConstantSpecNode : SemanticSpecNode<QuantityConstantSpecDTO, BindingBaseDTO>
    {
        public QuantityConstantSpecNode(SemanticSpecBaseDTO dto, string parentId) : base(dto, parentId)
        {
            
        }

        public override QuantityConstantSpecDTO Semantic { get; init; }
        public override BindingBaseDTO Binding { get; set; }
    }

    public class MonitorSpecSetNode : SemanticSpecNode<MonitorSpecSetDTO, BindingBaseDTO>
    {
        public MonitorSpecSetNode(SemanticSpecBaseDTO dto, string parentId) : base(dto, parentId)
        {

        }

        public override MonitorSpecSetDTO Semantic { get; init; }
        public override BindingBaseDTO Binding { get; set; }
    }

    public class QuantitySpecNode : SemanticSpecNode<QuantitySpecDTO, BindingBaseDTO>
    {
        public QuantitySpecNode(SemanticSpecBaseDTO dto, string parentId) : base(dto, parentId)
        {

        }

        public override QuantitySpecDTO Semantic { get; init; }
        public override BindingBaseDTO Binding { get; set; }
    }

    public class TextSpecNode : SemanticSpecNode<TextSpecDTO, BindingBaseDTO>
    {
        public TextSpecNode(SemanticSpecBaseDTO dto, string parentId) : base(dto, parentId)
        {
            
        }

        public override TextSpecDTO Semantic { get; init; }
        public override BindingBaseDTO Binding { get; set; }
    }

    public class ValueSetSpecNode : SemanticSpecNode<ValueSetSpecDTO, BindingBaseDTO>
    {
        public ValueSetSpecNode(SemanticSpecBaseDTO dto, string parentId) : base(dto, parentId)
        {

        }

        public override ValueSetSpecDTO Semantic { get; init; }
        public override BindingBaseDTO Binding { get; set; }
    }

    public class QuantityValueSetSpecNode : SemanticSpecNode<QuantityValueSetSpecDTO, BindingBaseDTO>
    {
        public QuantityValueSetSpecNode(SemanticSpecBaseDTO dto, string parentId) : base(dto, parentId)
        {

        }

        public override QuantityValueSetSpecDTO Semantic { get; init; }
        public override BindingBaseDTO Binding { get; set; }
    }

    public class HealthStatusSetSpecNode : SemanticSpecNode<HealthStatusSetSpecDTO, BindingBaseDTO>
    {
        public HealthStatusSetSpecNode(SemanticSpecBaseDTO dto, string parentId) : base(dto, parentId)
        {
            
        }

        public override HealthStatusSetSpecDTO Semantic { get; init; }
        public override BindingBaseDTO Binding { get; set; }
    }

    public class GroupSpecNode : SemanticSpecNode<GroupSpecDTO, BindingBaseDTO>
    {
        public GroupSpecNode(SemanticSpecBaseDTO dto, string parentId) : base(dto, parentId)
        {
           
        }

        public override GroupSpecDTO Semantic { get; init; }
        public override BindingBaseDTO Binding { get; set; }
    }

    public class ControlSpecSetNode : SemanticSpecNode<ControlSpecSetDTO, BindingBaseDTO>
    {
        public ControlSpecSetNode(SemanticSpecBaseDTO dto, string parentId) : base(dto, parentId)
        {
           
        }

        public override ControlSpecSetDTO Semantic { get; init; }
        public override BindingBaseDTO Binding { get; set; }
    }

    public class ControlSpecNode : SemanticSpecNode<ControlSpecDTO, ControlBindingDTO>
    {
        public ControlSpecNode(SemanticSpecBaseDTO semantic, string parentId) : base(semantic, parentId)
        { }

        public ControlSpecNode(SemanticSpecBaseDTO semantic, ControlBindingDTO binding, string parentId)
            : base(semantic, parentId)
        {
            _binding = binding;
        }

        public bool Invoke(Dictionary<string, object> param, TransportAdapterRegistry messageAdapterRegistry)
        {
            if (_binding is null)
                return false;

            var messageAdapter = messageAdapterRegistry.GetAdapter(_binding.Channel.topicName);
            var sample = messageAdapter.CreateSample();

            foreach(var field in _binding.Fields)
            {
                if (!param.TryGetValue(field.name, out _) && field is not FixedFieldDTO)
                {
                    Console.WriteLine($"[warning] Cannot find {field.name} in params.");
                    continue;
                }

                if (field is FixedFieldDTO fixedField)
                    messageAdapter.SetField(sample, field.name, fixedField.value);
                else
                    messageAdapter.SetField(sample, field.name, param[field.name]);
            }

            messageAdapter.Write(sample);
            return true;
        }

        public override string ToString()
        {
            return "";
        }

        public override ControlSpecDTO Semantic { get; init; }
        public override ControlBindingDTO Binding 
        {
            get => _binding;
            set
            {
                _binding = value;
            }
        }

        private ControlBindingDTO _binding;
    }

    public class SetPointSpecNode : SemanticSpecNode<SetPointSpecDTO, ControlBindingDTO>
    {
        public SetPointSpecNode(SemanticSpecBaseDTO dto, string parentId) : base(dto, parentId)
        {
            
        }

        public SetPointSpecNode(SemanticSpecBaseDTO semantic, ControlBindingDTO binding, string parentId)
            : base(semantic, parentId)
        {
            _binding = binding;
        }

        public bool Invoke(Dictionary<string, object> param, TransportAdapterRegistry messageAdapterRegistry)
        {
            if (_binding is null)
                return false;

            var messageAdapter = messageAdapterRegistry.GetAdapter(_binding.Channel.topicName);
            var sample = messageAdapter.CreateSample();

            foreach (var field in _binding.Fields)
            {
                if (field is FixedFieldDTO)
                    messageAdapter.SetField(sample, field.name, (field as FixedFieldDTO).value);
                else
                    messageAdapter.SetField(sample, field.name, param[field.name]);
            }

            messageAdapter.Write(sample);
            return true;
        }

        public override SetPointSpecDTO Semantic { get; init; }
        public override ControlBindingDTO Binding
        {
            get => _binding;
            set
            {
                _binding = value;
            }
        }

        private ControlBindingDTO _binding;
    }

    public class SensorProductSpecSetNode : SemanticSpecNode<SensorProductSpecSetDTO, BindingBaseDTO>
    {
        public SensorProductSpecSetNode(SemanticSpecBaseDTO dto, string parentId) : base(dto, parentId)
        {

        }

        public override SensorProductSpecSetDTO Semantic { get; init; }
        public override BindingBaseDTO Binding { get; set; }
    }

    public class ProductStreamSpecNode : SemanticSpecNode<ProductStreamSpecDTO, BindingBaseDTO>
    {
        public ProductStreamSpecNode(SemanticSpecBaseDTO dto, string parentId) : base(dto, parentId)
        {
            
        }

        public override ProductStreamSpecDTO Semantic { get; init; }
        public override BindingBaseDTO Binding { get; set; }
    }
}