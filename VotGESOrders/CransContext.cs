using VotGESOrders.CranService;

namespace VotGESOrders
{
	public class CransContext {
		public static CransContext Single { get; set; }
		public CranServiceClient Client;
		protected CransContext() {

		}
		static CransContext() {
			Single = new CransContext();
		}
		public static void init() {
			Single.Client = new CranServiceClient();
		}


	}
}
