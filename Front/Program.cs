using System;
using System.Linq;
using System.Collections.Generic;

namespace Front
{
    class Fort
    {
        private readonly int Id;

        private int Forces;


        public Fort(int id, int initialAmountOfForces)
        {
            Id = id;

            Forces = initialAmountOfForces;
        }

        public int GetForces()
        {
            return Forces;
        }

        public void IncreaseForce(int amount)
        {
            Forces += amount;
        }

        public void DecreaseForce(int amount)
        {
            Forces -= amount;
        }

        public void MoveForces()
        {
            Forces = 1;
        }
    }

    class Faction
    {
        private static readonly Random play = new Random();

        private readonly int Id;

        private List<Fort> Forts;

        public Faction(int id, List<Fort> initialForts)
        {
            Id = id;

            Forts = initialForts;
        }

        public List<Fort> GetForts()
        {
            return Forts;
        }

        public void AddFort(Fort fort)
        {
            Forts.Add(fort);
        }

        public void RemoveFort(Fort fort)
        {
            Forts.Remove(fort);
        }

        public int MakeChoice(Fort enemyFrontFort)
        {
            return play.Next(0, 3);
        }

        public Fort GetFrontFort()
        {
            return Forts.Last();
        }

        public void Move()
        {
            int forcesFromLastFort = 0;

            foreach (Fort fort in Forts)
            {
                int forcesToMove = forcesFromLastFort;

                forcesFromLastFort = fort.GetForces() - 1;

                fort.MoveForces();

                fort.IncreaseForce(forcesToMove);
            }

            Forts.Last().IncreaseForce(forcesFromLastFort);
        }

        public void Reinforcements()
        {
            int amountOfReinforcements = Forts.Count / 2;

            Forts.First().IncreaseForce(amountOfReinforcements);
        }
    }

    class Field
    {
        private static readonly Random diceRoll = new Random();

        private Faction RedFaction;
        private Faction BlueFaction;

        public Field(int initialAmountOfFortsPerFaction, int initialAmountOfForcesPerFort)
        {
            int totalAmountOfForts = initialAmountOfFortsPerFaction * 2;

            var redForts = new List<Fort>();
            var blueForts = new List<Fort>();

            for (int fortId = 0; fortId < initialAmountOfFortsPerFaction; fortId++)
            {
                redForts.Add(new Fort(fortId, initialAmountOfFortsPerFaction));
                blueForts.Add(new Fort(totalAmountOfForts - fortId - 1, initialAmountOfFortsPerFaction));
            }

            RedFaction = new Faction(0, redForts);
            BlueFaction = new Faction(1, blueForts);
        }

        public bool IsFactionWinner(int factionId)
        {
            if (factionId == 0)
            {
                return BlueFaction.GetForts().Count == 0;
            }
            else
            {
                return RedFaction.GetForts().Count == 0;
            }
        }

        public void RedFactionPlay()
        {
            var choice = RedFaction.MakeChoice(BlueFaction.GetFrontFort());

            MakePlay(0, choice);
        }

        public void BlueFactionPlay()
        {
            var choice = BlueFaction.MakeChoice(RedFaction.GetFrontFort());

            MakePlay(1, choice);
        }

        private void MakePlay(int factionId, int choice)
        {
            switch(choice)
            {
                case 0:
                    FactionAttack(factionId);
                    break;
                case 1:
                    FactionMove(factionId);
                    break;
                case 2:
                    FactionReinforcements(factionId);
                    break;
            }
        }

        private void FactionAttack(int factionId)
        {
            List<int> redDiceRolls = new List<int>();
            List<int> blueDiceRolls = new List<int>();

            for (int force = 0; force < RedFaction.GetFrontFort().GetForces(); force++)
            {
                var roll = diceRoll.Next(1, 7);

                redDiceRolls.Add(roll);
            }

            for (int force = 0; force < BlueFaction.GetFrontFort().GetForces(); force++)
            {
                var roll = diceRoll.Next(1, 7);

                blueDiceRolls.Add(roll);
            }

            redDiceRolls.Sort();
            blueDiceRolls.Sort();

            // TODO: Cap dice amounts to the minimum Count on Lists
            // TODO: Match dices and look winners
            // TODO: Make Forces diminish
            // TODO: Check for Fort change
        }

        private void FactionMove(int factionId)
        {
            if (factionId == 0)
            {
                RedFaction.Move();
            }
            else
            {
                BlueFaction.Move();
            }
        }

        private void FactionReinforcements(int factionId)
        {
            if(factionId == 0)
            {
                RedFaction.Reinforcements();
            }
            else
            {
                BlueFaction.Reinforcements();
            }
        }
    }

    class Program
    {
        private static readonly int initialAmountOfFortsPerFaction = 5;
        private static readonly int initialAmountOfForcesPerFort = 2;

        private static readonly Random rnd = new Random();

        static void Main(string[] args)
        {
            bool shouldGameContinue = true;
            int winningFaction = 0;

            Console.WriteLine("Welcome to the Front!");
            Console.WriteLine();
            Console.WriteLine("Press any key to start the Game.");
            Console.ReadKey();
            Console.WriteLine();

            Field field = new Field(initialAmountOfFortsPerFaction, initialAmountOfForcesPerFort);

            int firstFaction = rnd.Next(0, 2);
            var secondFaction = 1 - firstFaction;
            List<int> factionOrder = new List<int>() { firstFaction, secondFaction };

            while(shouldGameContinue)
            {
                foreach (int factionId in factionOrder)
                {
                    if (factionId == 0)
                    {
                        field.RedFactionPlay();
                    }
                    else
                    {
                        field.BlueFactionPlay();
                    }

                    if (field.IsFactionWinner(factionId))
                    {
                        shouldGameContinue = false;

                        winningFaction = factionId;
                    }
                }
            }
            
            if (winningFaction == 0)
            {
                Console.WriteLine("Red Faction is the Winner!");
            }
            else
            {
                Console.WriteLine("Blue Faction is the Winner!");
            }
        }
    }
}
