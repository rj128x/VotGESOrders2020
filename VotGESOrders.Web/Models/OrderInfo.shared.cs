using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Runtime.Serialization;

namespace VotGESOrders.Web.Models
{


    public class OrderInfo
    {
        [DataMember]
        public Dictionary<OrderTypeEnum, string> OrderTypes { get; set; }
        [DataMember]
        public Dictionary<OrderTypeEnum, string> OrderTypesShort { get; set; }
        [DataMember]
        public Dictionary<OrderStateEnum, string> OrderStates { get; set; }
        [DataMember]
        public NumberFormatInfo NFI { get; set; }

        public static OrderInfo Current { get; set; }

        static OrderInfo()
        {
            Current = new OrderInfo();
        }

        public OrderInfo()
        {

            OrderTypes = new Dictionary<OrderTypeEnum, string>();

            OrderTypesShort = new Dictionary<OrderTypeEnum, string>();
            OrderStates = new Dictionary<OrderStateEnum, string>();
            NFI = new NumberFormatInfo();
            NFI.NumberDecimalSeparator = "/";
            NFI.NumberDecimalDigits = 2;
            //NFI.NumberDecimalDigits = 3;			


            OrderTypes.Add(OrderTypeEnum.crash, "Аварийная");
            OrderTypes.Add(OrderTypeEnum.pl, "Плановая");
            OrderTypes.Add(OrderTypeEnum.npl, "Неплановая");
            OrderTypes.Add(OrderTypeEnum.no, "Неотложная");

            OrderTypesShort.Add(OrderTypeEnum.crash, "АВ");
            OrderTypesShort.Add(OrderTypeEnum.pl, "ПЛН");
            OrderTypesShort.Add(OrderTypeEnum.npl, "НПЛ");
            OrderTypesShort.Add(OrderTypeEnum.no, "НО");

            OrderStates.Add(OrderStateEnum.accepted, "Разрешена");
            OrderStates.Add(OrderStateEnum.banned, "Отклонена");
            OrderStates.Add(OrderStateEnum.closed, "Работы завершены");
            OrderStates.Add(OrderStateEnum.created, "Создана");
            OrderStates.Add(OrderStateEnum.extended, "Продлена");
            OrderStates.Add(OrderStateEnum.askExtended, "Заявка на продление");
            OrderStates.Add(OrderStateEnum.opened, "Открыта");
            OrderStates.Add(OrderStateEnum.canceled, "Снята");
            OrderStates.Add(OrderStateEnum.completed, "Закрыта");
            OrderStates.Add(OrderStateEnum.completedWithoutEnter, "Закрыта без ввода");
        }


    }
}