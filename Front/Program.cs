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

        public int GetId()
        {
            return Id;
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

        public void DecreaseForceToOne()
        {
            Forces = 1;
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

        public readonly List<int> Choices = new List<int>() { 0, 0, 0 };

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

        public void RemoveFrontFort()
        {
            Forts.Remove(Forts.Last());
        }

        public int MakeChoice(Fort enemyFrontFort)
        {
            int choice = play.Next(0, 3);

            Choices[choice]++;

            return choice;
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

        public Faction RedFaction;
        public Faction BlueFaction;

        public Field(int initialAmountOfFortsPerFaction, int initialAmountOfForcesPerFort)
        {
            int totalAmountOfForts = initialAmountOfFortsPerFaction * 2;

            var redForts = new List<Fort>();
            var blueForts = new List<Fort>();

            for (int fortId = 0; fortId < initialAmountOfFortsPerFaction; fortId++)
            {
                redForts.Add(new Fort(fortId, initialAmountOfForcesPerFort));
                blueForts.Add(new Fort(totalAmountOfForts - fortId - 1, initialAmountOfForcesPerFort));
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
                    if(FactionCanAttack(factionId))
                    {
                        FactionAttack(factionId);
                    }
                    else
                    {
                        FactionReinforcements(factionId);
                    }
                    break;
                case 1:
                    FactionMove(factionId);
                    break;
                case 2:
                    FactionReinforcements(factionId);
                    break;
            }
        }
        
        private bool FactionCanAttack(int factionId)
        {
            if(factionId == 0)
            {
                return (RedFaction.GetFrontFort().GetForces() > 1);
            }
            else
            {
                return (BlueFaction.GetFrontFort().GetForces() > 1);
            }
        }

        private void FactionAttack(int factionId)
        {
            List<int> redDiceRolls = new List<int>();
            List<int> blueDiceRolls = new List<int>();

            // The attacking Faction loses one force
            int redAttackerPenalty = (1 - factionId);
            int blueAttackerPenalty = factionId;

            int redForcesLost = 0;
            int blueForcesLost = 0;

            for (int force = 0; force < (RedFaction.GetFrontFort().GetForces() - redAttackerPenalty); force++)
            {
                var roll = diceRoll.Next(1, 7);

                redDiceRolls.Add(roll);
            }

            for (int force = 0; force < (BlueFaction.GetFrontFort().GetForces() - blueAttackerPenalty); force++)
            {
                var roll = diceRoll.Next(1, 7);

                blueDiceRolls.Add(roll);
            }

            redDiceRolls.Sort();
            blueDiceRolls.Sort();

            int additionalRedForces = 0;
            int additionalBlueForces = 0;

            if (redDiceRolls.Count() > blueDiceRolls.Count())
            {
                additionalRedForces = redDiceRolls.Count() - blueDiceRolls.Count();
            }
            else if (blueDiceRolls.Count() > redDiceRolls.Count())
            {
                additionalBlueForces = blueDiceRolls.Count() - redDiceRolls.Count();
            }

            int lesserForceAmount = Math.Min(redDiceRolls.Count(), blueDiceRolls.Count());

            for (int force = 0; force < lesserForceAmount; force++)
            {
                if (redDiceRolls[force + additionalRedForces] > blueDiceRolls[force + additionalBlueForces])
                {
                    blueForcesLost++;
                }
                else if (redDiceRolls[force + additionalRedForces] < blueDiceRolls[force + additionalBlueForces])
                {
                    redForcesLost++;
                }
            }

            RedFaction.GetFrontFort().DecreaseForce(redForcesLost);
            BlueFaction.GetFrontFort().DecreaseForce(blueForcesLost);

            if(factionId == 0)
            {
                if (BlueFaction.GetFrontFort().GetForces() <= 0)
                {
                    int redForcesToFront = RedFaction.GetFrontFort().GetForces() - 1;
                    int blueFrontFortId = BlueFaction.GetFrontFort().GetId();

                    BlueFaction.RemoveFrontFort();

                    Fort newRedFort = new Fort(blueFrontFortId, redForcesToFront - 1);

                    RedFaction.GetFrontFort().DecreaseForceToOne();

                    RedFaction.AddFort(newRedFort);
                }
            }
            else
            {
                if (RedFaction.GetFrontFort().GetForces() <= 0)
                {
                    int blueForcesToFront = BlueFaction.GetFrontFort().GetForces() - 1;
                    int redFrontFortId = RedFaction.GetFrontFort().GetId();

                    RedFaction.RemoveFrontFort();

                    Fort newBlueFort = new Fort(redFrontFortId, blueForcesToFront - 1);

                    BlueFaction.GetFrontFort().DecreaseForceToOne();

                    BlueFaction.AddFort(newBlueFort);
                }
            }
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
            int turns = 0;
            bool shouldGameContinue = true;
            int winningFaction = 0;

            Console.WriteLine("Welcome to the Front!");
            Console.WriteLine();
            Console.WriteLine("Press any key to start the Game.");
            Console.ReadKey();
            Console.WriteLine();
            Console.WriteLine("...");
            Console.WriteLine();

            Field field = new Field(initialAmountOfFortsPerFaction, initialAmountOfForcesPerFort);

            int firstFaction = rnd.Next(0, 2);
            int secondFaction = 1 - firstFaction;
            List<int> factionOrder = new List<int>() { firstFaction, secondFaction };

            while(shouldGameContinue)
            {
                foreach (int factionId in factionOrder)
                {
                    if(shouldGameContinue)
                    {
                        if (factionId == 0)
                        {
                            field.RedFactionPlay();
                        }
                        else
                        {
                            field.BlueFactionPlay();
                        }
                    }

                    if (field.IsFactionWinner(factionId))
                    {
                        shouldGameContinue = false;

                        winningFaction = factionId;
                    }
                }

                turns++;
            }
            
            if (winningFaction == 0)
            {
                Console.WriteLine("Red Faction is the Winner with {0} Forces on the Front!", field.RedFaction.GetFrontFort().GetForces());
            }
            else
            {
                Console.WriteLine("Blue Faction is the Winner with {0} Forces on the Front!", field.BlueFaction.GetFrontFort().GetForces());
            }

            Console.WriteLine();

            Console.WriteLine("- The game took {0} turns to end.", turns);
            Console.WriteLine();
            Console.WriteLine("- Red performed:");
            Console.WriteLine("  - {0} Attacks on the Front.", field.RedFaction.Choices[0]);
            Console.WriteLine("  - {0} Forces Movements.", field.RedFaction.Choices[1]);
            Console.WriteLine("  - {0} Reinforcement Calls.", field.RedFaction.Choices[2]);
            Console.WriteLine();
            Console.WriteLine("- Blue performed:");
            Console.WriteLine("  - {0} Attacks on the Front.", field.BlueFaction.Choices[0]);
            Console.WriteLine("  - {0} Forces Movements.", field.BlueFaction.Choices[1]);
            Console.WriteLine("  - {0} Reinforcement Calls.", field.BlueFaction.Choices[2]);

            Console.WriteLine();
            Console.WriteLine("Press any key to close the Game.");
            Console.ReadKey();
        }
    }
}
