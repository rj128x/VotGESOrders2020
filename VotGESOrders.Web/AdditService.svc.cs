using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Web;
using VotGESOrders.Web.Logging;
using VotGESOrders.Web.Models;

namespace VotGESOrders.Web
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "AdditService" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы AdditService.svc или AdditService.svc.cs в обозревателе решений и начните отладку.

    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AdditService 
    {
        [OperationContract]
        public void DoWork()
        {
        }

        [OperationContract]
        public User GetAuthenticatedUser()
        {
            string login = HttpContext.Current.User.Identity.Name.ToLower();
            User user = new User();
            user.loadFromDB(login);
            Logger.info(String.Format("Пользователь авторизовался в системе: {0}", user), Logger.LoggerSource.server);

            return user;
        }

        [OperationContract]
        public void info(string message)
        {
            Logger.info(String.Format("{0}", message), Logger.LoggerSource.client);
        }

        [OperationContract]
        public void error(string message)
        {
            Logger.error(String.Format("{0}", message), Logger.LoggerSource.client);
        }

        [OperationContract]
        public void debug(string message)
        {
            Logger.debug(String.Format("{0}", message), Logger.LoggerSource.client);
        }
    }
}
