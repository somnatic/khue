using System;
using LogicModule.Nodes.Helpers;
using LogicModule.ObjectModel;
using LogicModule.ObjectModel.TypeSystem;

namespace mail_thomaslinder_at.Logic.Nodes
{
    public class KHueNodeBrightness : LogicNodeBase
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ITypeService _typeService;

        public KHueNodeBrightness(INodeContext context) : base(context)
        {
            context.ThrowIfNull("context");

            _typeService = context.GetService<ITypeService>();

            Input = _typeService.CreateByte(PortTypes.Byte, "Brightness");
            LightId = _typeService.CreateInt(PortTypes.Integer, "Light ID");
            Username = _typeService.CreateString(PortTypes.String, "Hue user name");
            IpAddress = _typeService.CreateString(PortTypes.String, "Hue bridge IP address");
            ErrorMessage = _typeService.CreateString(PortTypes.String, "Error message");
        }


        [Input(DisplayOrder = 1, IsRequired = true)]
        public ByteValueObject Input { get; }

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

            //check for brightness = 0, if so: turn lamp off
            if (Input.Value == 0){
                var jsonData = $"{{\"on\": false}}";
            } else {
                // Coerce the data, according to the documentation values must be within (including) 1..254
                if (Input.Value < 1)
                {
                    Input.Value = 1;
                }
    
                if (Input.Value > 100)
                {
                    Input.Value = 100;
                }
                
                //Transform 1-100 to 1-254
                Input.Value = (int)(Input.Value * 2.54);
                
                var jsonData = $"{{\"on\": true,\"bri\":{Input.Value}}}";
            }

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
