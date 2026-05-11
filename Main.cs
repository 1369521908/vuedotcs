using Godot;
using vuedotcs.api;
using vuedotcs.reactive;

public partial class Main : Node2D
{
	static Ref val = Vue.Ref("");
	static Ref val2 = Vue.Ref("22");
	static Ref comp = Vue.Computed(Callable.From<StringName>(() => $"{val.Value}{val2.Value}45"));
	static Ref color = Vue.Ref(new Color(1,1,1,1));
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var LineEdit2 = GetNode<LineEdit>("LineEdit2");
		var LineEdit = GetNode<LineEdit>("LineEdit");
		var TextEdit = GetNode<TextEdit>("TextEdit");
		var ColorPicker2 = GetNode<ColorPicker>("ColorPicker2");
		var ColorPicker = GetNode<ColorPicker>("ColorPicker");
		var Label = GetNode<Label>("Label");
		var LabelComputed = GetNode<Label>("LabelComputed");
		
		GD.PrintRich($"val:{val.Value}");
		GD.PrintRich($"val1:{val2.Value}");
		Vue.Model(LineEdit2, "text", val2);
		Vue.Model(new Node[]{LineEdit, TextEdit}, "text", val);
		Vue.Model(new Node[]{ColorPicker2, ColorPicker}, "color", color);
		Vue.Hyper(Label, Callable.From(() => Label.LabelSettings.FontColor = (Color) color.Value));
		Vue.Bind(Label, "text", val);
		Vue.Bind(LabelComputed, "text", comp);
		Vue.Bind(new Node[]{Label, LabelComputed, LineEdit, LineEdit2, TextEdit}, 
			"theme_override_colors/font_color", color);
	}

	// Called every frame. 'delta' is the elapsed time since the previous fra	me.
	public override void _Process(double delta)
	{
	}

	public void _on_button_pressed()
	{
		foreach (var node in GetChildren())
		{
			node.QueueFree();
		}

		val = null;
		val2 = null;
		comp = null;
		color = null;
	}
	
}
