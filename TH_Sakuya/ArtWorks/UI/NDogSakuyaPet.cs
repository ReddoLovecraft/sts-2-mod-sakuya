using Godot;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using System;

namespace TH_Sakuya.ArtWorks.UI;

public partial class NDogSakuyaPet : Control
{
	private Vector2 _restPosition;
	private Tween? _tween;

	public override void _Ready()
	{
		_restPosition = Position;
	}

	public void Dash()
	{
		_tween?.Kill();
		Position = _restPosition;
		_tween = CreateTween();
		_tween.SetTrans(Tween.TransitionType.Cubic);
		_tween.SetEase(Tween.EaseType.Out);
		_tween.TweenProperty(this, "position", _restPosition + new Vector2(80f, 0f), 0.12);
		_tween.SetEase(Tween.EaseType.In);
		_tween.TweenProperty(this, "position", _restPosition, 0.18);
	}

	public static void TryDash()
	{
		NDogSakuyaPet? pet = NCombatRoom.Instance?.Ui?.GetNodeOrNull<NDogSakuyaPet>("DogSakuyaPet");
		pet?.Dash();
	}
}

