using System;
using System.Collections.Generic;
using Task_worker_matching.Memory_Layer;

namespace Task_worker_matching.ServiceLayer
{
    class WorkerOfferService : IDataService<Offer>
    {
        private Cache<Offer> cache = new(new WorkerOffersList(), new WorkerOfferRepoStrategy());
    
        public bool isValidItem(Offer item)
        {
            return item.GetMessage() != "" && item.GetFee() > 0;
        }
    
        public bool add(Offer item)
        {
            if (isValidItem(item))
                return cache.add(item);
        
            return false;
        }
    
        public bool update(Offer new_item, Offer old_item)
        {
            if(isValidItem(new_item))
                return cache.update(new_item, old_item);
        
            return false;
        }
    
        public bool delete(Offer item)
        {
            if(isValidItem(item))
                return cache.delete(item);
        
            return false;
        }
    
        public List<Offer> get_data()
        {
            return cache.get_data();
        }
    }
}
