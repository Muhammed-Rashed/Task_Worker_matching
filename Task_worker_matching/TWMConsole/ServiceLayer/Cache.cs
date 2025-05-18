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

        public Cache(IMemory<T> memory, IRepositoryStrategy<T> repository)
        {
            this.memory = memory;
            this.repository = repository;
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

        public bool add_answer(Answer ans, int question_id)
        {
            if (typeof(T) == typeof(Question))
            {
                var questionRepo = repository as QuestionRepositoryStrategy;
                if (questionRepo.add_answer(ans, question_id))
                {
                    var questionMemory = memory as QuestionsList;
                    return questionMemory.add_answer(ans, question_id);
                }
            }
            return false;
        }
    }
}