using System;
using LogicModule.Nodes.Helpers;
using LogicModule.ObjectModel;
using LogicModule.ObjectModel.TypeSystem;

namespace mail_thomaslinder_at.Logic.Nodes
{
    public class KHueNodeRgb : LogicNodeBase
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ITypeService _typeService;

        public KHueNodeRgb(INodeContext context) : base(context)
        {
            context.ThrowIfNull("context");

            _typeService = context.GetService<ITypeService>();

            Red = _typeService.CreateByte(PortTypes.Byte, "Red");
            Green = _typeService.CreateByte(PortTypes.Byte, "Green");
            Blue = _typeService.CreateByte(PortTypes.Byte, "Blue");
            LightId = _typeService.CreateInt(PortTypes.Integer, "Light ID");
            Username = _typeService.CreateString(PortTypes.String, "Hue user name");
            IpAddress = _typeService.CreateString(PortTypes.String, "Hue bridge IP address");
            ErrorMessage = _typeService.CreateString(PortTypes.String, "Error message");
        }


        [Input(DisplayOrder = 1, IsRequired = true)]
        public ByteValueObject Red { get; }

        [Input(DisplayOrder = 2, IsRequired = true)]
        public ByteValueObject Green { get; }

        [Input(DisplayOrder = 3, IsRequired = true)]
        public ByteValueObject Blue { get; }

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

            // The hue lights won't accept RGB values, we will convert them to xy values in the CIE color space
            // The general guide on how this is done is taken from here: https://github.com/PhilipsHue/PhilipsHueSDK-iOS-OSX/commit/f41091cf671e13fe8c32fcced12604cd31cceaf3
            double red = ((double) Red) / 255.0;
            double green = ((double)Green) / 255.0;
            double blue = ((double)Blue) / 255.0;

            red = (red > 0.04045f) ? Math.Pow((red + 0.055f) / (1.0f + 0.055f), 2.4f) : (red / 12.92f);
            green = (green > 0.04045f) ? Math.Pow((green + 0.055f) / (1.0f + 0.055f), 2.4f) : (green / 12.92f);
            blue = (blue > 0.04045f) ? Math.Pow((blue + 0.055f) / (1.0f + 0.055f), 2.4f) : (blue / 12.92f);

            double xd = red * 0.649926f + green * 0.103455f + blue * 0.197109f;
            double yd = red * 0.234327f + green * 0.743075f + blue * 0.022598f;
            double zd = red * 0.0000000f + green * 0.053077f + blue * 1.035763f;

            float x = (float)(xd / (xd + yd + zd));
            float y = (float)(yd / (xd + yd + zd));

            
            var jsonData = $"{{\"xy\":[{x:0.####},{y:0.####}]}}";

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