using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using TH_Sakuya.Scripts.Main;

namespace TH_Sakuya.Scripts.Nodes.Combat;

public partial class NTimeStopPointCounter : Control
{
	private Player? _player;
	private MegaRichTextLabel _label = null!;
	private Control _rotationLayers = null!;
	private Control _icon = null!;
	private ShaderMaterial _hsv = null!;
	private HoverTip _hoverTip;
	private int _displayedTsp;
	private int _displayedMax;

	public override void _Ready()
	{
		_label = GetNode<MegaRichTextLabel>("%CountLabel");
		_rotationLayers = GetNode<Control>("%RotationLayers");
		_icon = GetNode<Control>("Icon");
		_hsv = (ShaderMaterial)_icon.Material;
		_hoverTip = new HoverTip(new LocString("static_hover_tips", "TSP.title"), new LocString("static_hover_tips", "TSP.description"));
		Connect(SignalName.MouseEntered, Callable.From(OnHovered));
		Connect(SignalName.MouseExited, Callable.From(OnUnhovered));
		Visible = false;
	}

	public void Initialize(Player player)
	{
		_player = player;
		Visible = TimeStopPointSystem.IsEnabledFor(player);
	}

	private void OnHovered()
	{
		HoverTipAlignment alignment = HoverTip.GetHoverTipAlignment(this, threshold: 0.75f);
		NHoverTipSet.CreateAndShow(this, _hoverTip, alignment);
	}

	private void OnUnhovered()
	{
		NHoverTipSet.Remove(this);
	}

	public override void _Process(double delta)
	{
		if (_player == null || !TimeStopPointSystem.IsEnabledFor(_player))
		{
			Visible = false;
			return;
		}

		int tsp = TimeStopPointSystem.Get(_player);
		int max = TimeStopPointSystem.GetMax(_player);

		float rotSpeed = (tsp == 0) ? 5f : 30f;
		for (int i = 0; i < _rotationLayers.GetChildCount(); i++)
		{
			_rotationLayers.GetChild<Control>(i).RotationDegrees += (float)delta * rotSpeed * (i + 1);
		}

		if (_displayedTsp != tsp || _displayedMax != max)
		{
			_displayedTsp = tsp;
			_displayedMax = max;
			_label.AddThemeColorOverride(ThemeConstants.Label.FontColor, (tsp == 0) ? StsColors.red : StsColors.cream);
			_label.Text = $"[center]{tsp}/{max}[/center]";
		}

		Visible = true;
	}
}

