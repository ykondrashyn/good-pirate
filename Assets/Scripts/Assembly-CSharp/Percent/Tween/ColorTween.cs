namespace Percent.Tween
{
	public class ColorTween : global::Percent.Tween.PercentTween
	{
		public global::UnityEngine.Color fromColor;

		public global::UnityEngine.Color toColor;

		public bool cascadeColor;

		public bool hideOnCompletion;

		private global::UnityEngine.UI.RawImage rawImage;

		private global::UnityEngine.UI.Image image;

		private global::UnityEngine.Color deltaColor;

		private bool isSwaped;

		private void Awake()
		{
			rawImage = GetComponent<global::UnityEngine.UI.RawImage>();
			if (rawImage == null)
			{
				image = GetComponent<global::UnityEngine.UI.Image>();
			}
			if (hideOnCompletion)
			{
				onEnd = (global::Percent.Tween.PercentTween.OnTweenEnd)global::System.Delegate.Combine(onEnd, new global::Percent.Tween.PercentTween.OnTweenEnd(deactivateSelf));
			}
			originalStartDelay = startDelay;
		}

		private void deactivateSelf()
		{
			resetToBegining();
			base.gameObject.SetActive(value: false);
		}

		protected override void onTweenStart()
		{
			deltaColor = fromColor - toColor;
		}

		protected override void onTween()
		{
			global::UnityEngine.Color color = fromColor - deltaColor * animCurve.Evaluate(getProgress());
			if ((bool)rawImage)
			{
				rawImage.color = color;
			}
			else
			{
				image.color = color;
			}
			if (!cascadeColor)
			{
				return;
			}
			foreach (global::UnityEngine.Transform item in base.transform)
			{
				global::UnityEngine.UI.RawImage component = item.GetComponent<global::UnityEngine.UI.RawImage>();
				if ((bool)component)
				{
					component.color = color;
				}
				global::UnityEngine.UI.Image component2 = item.GetComponent<global::UnityEngine.UI.Image>();
				if ((bool)component2)
				{
					component2.color = color;
				}
			}
		}

		protected override void onTweenEnd()
		{
			if ((bool)rawImage)
			{
				rawImage.color = toColor;
			}
			else
			{
				image.color = toColor;
			}
			if (cascadeColor)
			{
				foreach (global::UnityEngine.Transform item in base.transform)
				{
					global::UnityEngine.UI.RawImage component = item.GetComponent<global::UnityEngine.UI.RawImage>();
					if ((bool)component)
					{
						component.color = toColor;
					}
					global::UnityEngine.UI.Image component2 = item.GetComponent<global::UnityEngine.UI.Image>();
					if ((bool)component2)
					{
						component2.color = toColor;
					}
				}
			}
			if (isSwaped)
			{
				swapFromAndToColor();
				isSwaped = false;
			}
			startDelay = originalStartDelay;
		}

		internal override void resetToBegining()
		{
			if ((bool)rawImage)
			{
				rawImage.color = fromColor;
			}
			else
			{
				image.color = fromColor;
			}
		}

		internal override void toEndFrame()
		{
			if ((bool)rawImage)
			{
				rawImage.color = toColor;
			}
			else
			{
				image.color = toColor;
			}
		}

		internal override void playReverse()
		{
			if (!isSwaped)
			{
				swapFromAndToColor();
				isSwaped = true;
				play();
			}
		}

		private void swapFromAndToColor()
		{
			global::UnityEngine.Color color = fromColor;
			fromColor = toColor;
			toColor = color;
		}

		public void playTween()
		{
			play();
		}
	}
}
