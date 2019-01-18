using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ICities;
using ColossalFramework.UI;
using UnityEngine;

namespace BetterBudget
{
    class ServiceInfo
    {
        public String serviceName;
        public ItemClass.Service service;
        public ItemClass.SubService subService;
        public int budgetExpensePollIndex;

        public ServiceInfo(String serviceName, ItemClass.Service service, ItemClass.SubService subService, int budgetExpensePollIndex)
        {
            this.serviceName = serviceName;
            this.service = service;
            this.subService = subService;
            this.budgetExpensePollIndex = budgetExpensePollIndex;
        }

        public void initBudgetItem(BudgetItem budgetItem)
        {
            budgetItem.Init(service, subService, budgetExpensePollIndex);
        }

    }
}
