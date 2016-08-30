using System;
using System.Collections.Generic;

namespace BackpackAlgorithm.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var backpack = new Backpack(100);

            var items = SampleItems();

            try
            {
                var packedItems = backpack.TryToPack(items, 3);

                ShowPackedItems(packedItems);
                ShowRemainingCapacity(backpack);
                Console.WriteLine();

                packedItems = backpack.TryToPack(AdditionalItems(), 2);

                ShowPackedItems(packedItems);
                ShowRemainingCapacity(backpack);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            Console.ReadKey();
        }

        static List<Item> SampleItems()
        {
            return new List<Item>
            {
                new Item(200, 50),
                new Item(90, 25),
                new Item(155, 40),
                new Item(115, 30)
            };
        }

        static List<Item> AdditionalItems()
        {
            return new List<Item>
            {
                new Item(100, 95),
                new Item(5, 1)
            };
        }

        static void ShowPackedItems(List<Item> packedItems)
        {
            Console.WriteLine("Packed items:");
            Console.WriteLine("----------------");
            Console.WriteLine("Value:  Weight:");

            foreach (var packedItem in packedItems)
            {
                Console.Write("{0,6}{1,9}", packedItem.Value, packedItem.Weight);
                Console.WriteLine();
            }
        }

        static void ShowRemainingCapacity(Backpack backpack)
        {
            Console.WriteLine("Remaining backpack's capacity: {0}", backpack.RemainingCapacity);
        }
    }
}
