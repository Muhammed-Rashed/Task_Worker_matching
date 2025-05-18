using System;
using System.Collections.Generic;
using PersistenceLayer;
using Task_worker_matching.Memory_Layer;
namespace Task_worker_matching.ServiceLayer
{
    class Cache<T> : IDataService<T>
    {
        private IMemory<T> memory;
        private IRepositoryStrategy<T> repository;

        public Cache()
        {
            if (memory is IMemory<Question>)
            {
                memory = (IMemory<T>)new QuestionsList();
                repository = (IRepositoryStrategy<T>)new QuestionRepositoryStrategy();
            }
            else if (memory is IMemory<Offer>)
            {
                memory = (IMemory<T>)new WorkerOffersList();
                repository = (IRepositoryStrategy<T>)new WorkerOfferRepoStrategy();
            }
            else if (memory is IMemory<Request>)
            {
                memory = (IMemory<T>)new RequestList();
                repository = (IRepositoryStrategy<T>)new RequestRepoStrategy();
            }
            else if (memory is IMemory<RequestExecuted>)
            {
                memory = (IMemory<T>)new RequestExecutedList();
                repository = (IRepositoryStrategy<T>)new RequestExecutedRepositoryStrategy();
            }
            else if (memory is IMemory<Task>)
            {
                memory = (IMemory<T>)new TasksList();
                repository = (IRepositoryStrategy<T>)new TaskRepoStrategy();
            }
        }
        public bool add(T item)
        {
            return memory.AddItem(item) && repository.add_item(item);
        }

        public bool update(T new_item, T old_item)
        {
            return memory.Update(new_item, old_item) &&
                   repository.update_item(new_item, old_item);
        }

        public bool delete(T item)
        {
            return memory.DeleteItem(item) && repository.delete_item(item);
        }

        public List<T> get_data()
        {
            if (!memory.IsEmpty())
                return memory.Get_Data();

            List<T> data = repository.get_items(0);
            memory.Set_Data(data);
            return data;

        }
    }
}