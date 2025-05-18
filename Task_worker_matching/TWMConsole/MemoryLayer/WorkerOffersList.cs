using System;
using System.Collections.Generic;
namespace Task_worker_matching.Memory_Layer;

class WorkerOffersList : IMemory<Offer>
{
    private List<Offer> offers = new List<Offer>();

    public bool AddItem(Offer item)
    {
        if (item != null)
        {
            offers.Add(item);
            return true;
        }
        return false;
    }

    public bool Update(Offer newItem, Offer oldItem)
    {
        int index = offers.IndexOf(oldItem);
        if (index != -1)
        {
            offers[index] = newItem;
            return true;
        }
        return false;
    }

    public bool DeleteItem(Offer item)
    {
        if(item != null)
        {
            return offers.Remove(item);
        }
        return false;
    }

    public List<Offer> Get_Data()
    {
        return offers;
    }

    public void set_data(List<Offer> newOffers)
    {
        offers = newOffers;
    }

    public bool IsEmpty()
    {
        return offers.Count == 0;
    }

    public Offer get_byID(int id)
    {
        foreach (var offer in offers)
        {
            if (offer.GetId() == id)
                return offer;
        }
        return null;
    }

    public void Set_Data(List<Offer> data)
    {
        offers = data;
    }
}