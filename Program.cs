using System;
using System.Collections.Generic;
using System.Linq;

namespace CSLight
{
    //Переменные именуются с маленькой буквы, 
    //приватные поля с символа _ и маленькой буквы (исключение - константы),
    //а всё остальное с большой буквы.
    class Program
    {
        static void Main()
        {
            List<BrokenCar> brokenCars = new List<BrokenCar>()
            {
                new BrokenCar(1, 0, 2),
                new BrokenCar(4, 0, 0),
                new BrokenCar(0, 0, 1),
                new BrokenCar(0, 2, 0)
            };

            CarService carService = new CarService(brokenCars, 10, 10, 10);
            carService.StartWork();
        }
    }

    class CarService
    {
        private int _surcharge = 20;

        private List<StackComponent> _stackComponents = new List<StackComponent>();
        private List<BrokenCar> _brokenCars = new List<BrokenCar>();

        public CarService(List<BrokenCar> brokenCars, int quantityComponentA, int quantityComponentB, int quantityComponentC)
        {
            _brokenCars = brokenCars;

            Money = 100;

            for (int i = 0; i < quantityComponentA; i++)
            {
                _stackComponents.Add(new StackComponent(new ComponentA(), quantityComponentA));
            }

            for (int i = 0; i < quantityComponentB; i++)
            {
                _stackComponents.Add(new StackComponent(new ComponentB(), quantityComponentB));
            }

            for (int i = 0; i < quantityComponentC; i++)
            {
                _stackComponents.Add(new StackComponent(new ComponentC(), quantityComponentC));
            }
        }

        public int Money { get; private set; }

        public void StartWork()
        {
            bool isWork = true;

            while (isWork)
            {                                
                Console.SetCursorPosition(0, 7);
                ShowCarsBreakdowns();
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"Денег у автосервиса - {Money}");
                Console.WriteLine("Напишите exit для выхода");
                Console.WriteLine("Напишите show, чтобы узнать сколько деталей на складе");

                string userInput = Console.ReadLine();

                if (userInput == "exit")
                {
                    isWork = false;
                }
                else if(userInput == "show")
                {
                    ShowComponents();
                }
                else
                {
                    ChooseCar();
                    DeleteRepairedCars();
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }        

        private void ChooseCar()
        {
            Console.Write("Выберите машину для починки: ");
            int enteredCarID;

            if(int.TryParse(Console.ReadLine(), out enteredCarID))
            {
                foreach(var car in _brokenCars)
                {
                    if(car.ID == enteredCarID)
                    {
                        RepairMenu(car);
                    }
                }
            }
        }

        private void RepairMenu(BrokenCar brokenCar)
        {
            Console.Clear();
            
            bool isRepair = true;

            while (isRepair)
            {
                Console.SetCursorPosition(0, 7);
                brokenCar.ShowBreakdowns();
                Console.SetCursorPosition(0, 0);

                Console.WriteLine("Выберите деталь для починки:\nДеталь А\tДеталь В\tДеталь С");
                string enteredNameComponent = Console.ReadLine();

                if (enteredNameComponent == "exit")
                {
                    isRepair = false;
                }
                else
                {
                    RepairComponent(enteredNameComponent, brokenCar);
                }
            }
        }

        private void RepairComponent(string nameComponent, BrokenCar brokenCar)
        {
            if (brokenCar.TryRepairComponent(nameComponent) == false)
            {
                Money -= _surcharge;
            }
        }        

        private void DeleteRepairedCars()
        {
            for(int i = 0; i < _brokenCars.Count; i++)
            {
                if (_brokenCars[i].IsFixed())
                {
                    Money += _brokenCars[i].GiveRepairCost;
                    _brokenCars.Remove(_brokenCars[i]);                    
                }
            }
        }

        private void ShowCarsBreakdowns()
        {
            foreach (var car in _brokenCars)
            {
                car.ShowInfo();
            }
        }

        private void ShowComponents()
        {
            Console.Clear();

            foreach (var component in _stackComponents)
            {
                component.ShowInfo();
            }

            Console.ReadKey();
        }
    }

    class BrokenCar
    {
        private static int _ids = 0;

        private List<StackComponent> _stackBrokenComponents = new List<StackComponent>();

        public BrokenCar(int quantityComponentA, int quantityComponentB, int quantityComponentC)
        {
            
            ID = ++_ids;

            for (int i = 0; i < quantityComponentA; i++)
            {
                StackComponent stackComponent = new StackComponent(new ComponentA(), quantityComponentA);
                _stackBrokenComponents.Add(stackComponent);
                GiveRepairCost += stackComponent.Price;
            }

            for (int i = 0; i < quantityComponentB; i++)
            {
                StackComponent stackComponent = new StackComponent(new ComponentB(), quantityComponentB);
                _stackBrokenComponents.Add(stackComponent);
                GiveRepairCost += stackComponent.Price;
            }

            for (int i = 0; i < quantityComponentC; i++)
            {
                StackComponent stackComponent = new StackComponent(new ComponentC(), quantityComponentC);
                _stackBrokenComponents.Add(stackComponent);
                GiveRepairCost += stackComponent.Price;
            }
        }

        public int ID { get; private set; }

        public int GiveRepairCost { get; private set; }

        public bool IsFixed()
        {
            if(_stackBrokenComponents.Count == 0)
            {
                return true;
            }
            return false;
        }

        public bool TryRepairComponent(string nameComponent)
        {
            foreach(var component in _stackBrokenComponents)
            {
                if(component.Name == nameComponent)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Успешно");
                    Console.ResetColor();

                    component.ReplaceComponent();
                    _stackBrokenComponents.Remove(component);

                    Console.ReadKey();
                    Console.Clear();

                    return true;
                }                
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Попытка заменить не ту деталь");
            Console.ResetColor();

            Console.ReadKey();
            Console.Clear();

            return false;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Машина - {ID} ");

            ShowBreakdowns();

            Console.WriteLine();
        }

        public void ShowBreakdowns()
        {
            foreach(var component in _stackBrokenComponents)
            {
                component.ShowInfo();
            }
        }
    }

    class Component
    {
        public int Price { get; protected set; }
        public string Name { get; protected set; }

        public void ShowInfo()
        {
            Console.WriteLine($"{Name}, стоимость - {Price}");
        }
    }

    class StackComponent : Component
    {
        private Component _component;
        private static int _quantityComponent;

        public StackComponent(Component component, int quantityComponent)
        {
            Name = component.Name;
            Price = component.Price;
            _component = component;            
            _quantityComponent = quantityComponent;
        }

        public void ReplaceComponent()
        {            
            --_quantityComponent;            
        }
    }

    class ComponentA : Component
    {
        public ComponentA()
        {
            Price = 10;
            Name = "Деталь А";
        }
    }

    class ComponentB : Component
    {
        public ComponentB()
        {
            Price = 20;
            Name = "Деталь В";
        }
    }

    class ComponentC : Component
    {
        public ComponentC()
        {
            Price = 30;
            Name = "Деталь С";
        }
    }
}