namespace Percent
{
	public class CrossPromotionUIEventHandler : global::UnityEngine.MonoBehaviour
	{
		public global::Percent.CrossPromotion crossPromotion;

		public global::Percent.Event.ClickTracker clickTracker;

		public void onHide()
		{
			crossPromotion.hide();
		}

		public void onImageTypeClick()
		{
			global::Percent.Util.goMarket(global::Percent.PromotionData.crossPromotionData.storeUrl);
			int gameId = global::Percent.PromotionData.crossPromotionData.gameId;
			clickTracker.trigger(global::Percent.PromotionType.IMAGE, gameId);
		}

		public void onSlideTypeClick()
		{
			global::Percent.Util.goMarket(global::Percent.PromotionData.crossPromotionData.storeUrl);
			int gameId = global::Percent.PromotionData.crossPromotionData.gameId;
			clickTracker.trigger(global::Percent.PromotionType.SLIDE, gameId);
		}
	}
}
