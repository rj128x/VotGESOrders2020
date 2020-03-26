using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using VotGESOrders.Web.Models;

namespace VotGESOrders.Web {
	[ServiceContract(Namespace = "")]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class CranService {
		[OperationContract]
		public void DoWork() {
			// Добавьте здесь реализацию операции
			return;
		}

		[OperationContract]
		public CranFilter getCranTasks(CranFilter Filter) {
			return CranTaskInfo.LoadCranTasks(Filter);
		}

		[OperationContract]
		public ReturnMessage CreateCranTask(CranTaskInfo task) {
			return CranTaskInfo.CreateCranTask(task);
		}


		[OperationContract]
		public ReturnMessage CommentCranTask(CranTaskInfo task,string comment) {
			return CranTaskInfo.AddComment(task, comment);
		}

		[OperationContract]
		public ReturnMessage CancelCranTask(CranTaskInfo task) {
			return CranTaskInfo.CreateCranTask(task);
		}

		[OperationContract]
		public ReturnMessage FinishCranTask(CranTaskInfo task) {
			return CranTaskInfo.CreateCranTask(task);
		}

    [OperationContract]
    public DateTime getLastUpdate() {
      return CranTaskInfo.LastUpdate;
    }


    // Добавьте здесь дополнительные операции и отметьте их атрибутом [OperationContract]
  }
}
