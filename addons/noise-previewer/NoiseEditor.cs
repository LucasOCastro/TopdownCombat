using Godot;

namespace NoisePreviewer
{
    [Tool]
    public class NoiseEditor : EditorProperty
    {
        private const int size = 256;

        public OpenSimplexNoise noise;

        private TextureRect _textureRect;
        private SpinBox _zoomField, _resolutionField;
        private SpinBox _xOffsetField, _yOffsetField;
        public override void _Ready()
        {
            if (noise == null) return;

            void connectToRefresh(SpinBox box) => box.Connect("value_changed", this, nameof(PreviewValueChanged));

            HBoxContainer container = new HBoxContainer();
            AddChild(container);
            SetBottomEditor(container);

            // _zoomField = new SpinBox() { Step = .1, Value=1, MinValue = .01, MaxValue = 6};
            // container.AddChild(new Label(){Text = "Zoom"});
            // container.AddChild(_zoomField);

            _resolutionField = new SpinBox() {Value = size, MinValue = 10, MaxValue = 300};
            container.AddChild(new Label(){Text = "Size"});
            container.AddChild(_resolutionField);

            _xOffsetField = new SpinBox() {MaxValue = 2000, AllowGreater = true, AllowLesser = true};
            _yOffsetField = new SpinBox() {MaxValue = 2000, AllowGreater = true, AllowLesser = true};
            container.AddChild(new Label(){Text = "Offset"});
            container.AddChild(_xOffsetField);
            container.AddChild(_yOffsetField);
            
            connectToRefresh(_resolutionField);
            // connectToRefresh(_zoomField);
            connectToRefresh(_xOffsetField);
            connectToRefresh(_yOffsetField);


            _textureRect = new TextureRect();
            // _textureRect.RectMinSize = new Vector2(size, size);
            _textureRect.RectSize = new Vector2(size, size);
            AddChild(_textureRect);
            SetBottomEditor(_textureRect);

            Refresh();
        }

        private void PreviewValueChanged(float value) => Refresh();

        public void Refresh()
        {
            if (noise == null) return;
            
            Vector2 offset = new Vector2((float)_xOffsetField.Value, (float)_yOffsetField.Value);
            // int imageSize = Mathf.RoundToInt(size * (float)_zoomField.Value);
            int imageSize = (int)_resolutionField.Value;
            Image image = noise.GetImage(imageSize, imageSize, offset);
            image.Resize(size, size);

            ImageTexture texture = new ImageTexture();
            texture.CreateFromImage(image);
            _textureRect.Texture = texture;
        }

        public override void UpdateProperty()
        {
            base.UpdateProperty();
            Refresh();
        }
    }
}
