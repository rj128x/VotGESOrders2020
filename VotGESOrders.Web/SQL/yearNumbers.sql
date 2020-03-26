use VotGESOrdersTest
update Orders set orderYearNumber=orderNumber-(select floor(max(o.orderNumber)) from Orders as o where year(o.orderDateCreate)=year(orders.orderDateCreate)-1)+0 where floor(orderNumber)=orderNumber
update Orders set orderYearNumber=orderNumber where year(orderdateCreate)=2011
update Orders set orderYearNumber=orderNumber-FLOOR(ordernumber)+(select floor(max(o.orderYearNumber))from orders as o where (floor(o.orderNumber)=floor(Orders.parentOrderNumber))) where floor(orderNumber)<>orderNumber
