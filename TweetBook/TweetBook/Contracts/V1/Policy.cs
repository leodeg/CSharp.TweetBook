namespace TweetBook.Contracts.V1
{
	public static class Policy
	{
		public const string TagViewer = "TagViewer";
		public const string MustWorkForCompany = "TagViewer";

		public static class Claims
		{
			public const string TagsView = "tags.view";
		}
	}
}
