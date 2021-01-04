using System;
using LogicModule.Nodes.Helpers;
using LogicModule.ObjectModel;
using LogicModule.ObjectModel.TypeSystem;

namespace mail_thomaslinder_at.Logic.Nodes
{
    public class KHueNodeColorTemperature : LogicNodeBase
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ITypeService _typeService;

        public KHueNodeColorTemperature(INodeContext context) : base(context)
        {
            context.ThrowIfNull("context");

            _typeService = context.GetService<ITypeService>();

            Input = _typeService.CreateUShort(PortTypes.Word, "Input");
            LightId = _typeService.CreateInt(PortTypes.Integer, "Light ID");
            Username = _typeService.CreateString(PortTypes.String, "Hue user name");
            IpAddress = _typeService.CreateString(PortTypes.String, "Hue bridge IP address");
            ErrorMessage = _typeService.CreateString(PortTypes.String, "Error message");
        }

        [Input(DisplayOrder = 1, IsRequired = true)]
        public UShortValueObject Input { get; }

        [Input(DisplayOrder = 2, IsDefaultShown = false, IsInput = false)]
        public IntValueObject LightId { get; }

        [Input(DisplayOrder = 3, IsDefaultShown = false, IsInput = false)]
        public StringValueObject Username { get; }

        [Input(DisplayOrder = 4, IsDefaultShown = false, IsInput = false)]
        public StringValueObject IpAddress { get; }

        [Output(DisplayOrder = 1)]
        public StringValueObject ErrorMessage { get; }

        public override void Startup()
        {
        }

        public override void Execute()
        {
            ErrorMessage.Value = "";

            // Coerce the data, according to the documentation values must be within (including) 153 (6500K)..500 (2000K)
            if (Input.Value < 153)
            {
                Input.Value = 153;
            }

            if (Input.Value > 500)
            {
                Input.Value = 500;
            }
            
            var jsonData = $"{{\"ct\":{Input.Value}}}";

            try
            {
                KHueHttpClient.ExecuteCommand(jsonData, IpAddress.Value, Username.Value, LightId.Value);
            }
            catch (KHueException kex)
            {
                ErrorMessage.Value = kex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage.Value = $"Unexpected error: {ex.Message}";
            }

        }
    }
}